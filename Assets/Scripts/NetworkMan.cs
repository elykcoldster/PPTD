using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public enum SceneChangeMode {
	None,
	Game,
	Menu
}

public enum NetworkState {
	Inactive,
	Pregame,
	Connecting,
	InLobby,
	InGame
}

public enum NetworkGameType {
	Matchmaking,
	Direct,
	Singleplayer
}

public class NetworkMan : NetworkManager {

	public static NetworkMan instance;

	[SerializeField]
	protected int multiplayerMaxPlayers = 2;

	[SerializeField]
	protected NetPlayer networkPlayerPrefab;

	public List<NetworkPlayer> connectedPlayers;

	public event Action<bool, MatchInfo> matchCreated;

	private Action<bool, MatchInfo> nextMatchCreatedCallback;

	public NetworkState state {
		get;
		protected set;
	}

	public NetworkGameType gameType {
		get;
		protected set;
	}

	protected virtual void Awake() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;

			connectedPlayers = new List<NetworkPlayer> ();
		}
	}

	void UpdatePlayerIDs() {
		for (int i = 0; i < connectedPlayers.Count; i++) {
			connectedPlayers [i].SetPlayerId (i);
		}
	}

	public void RegisterNetworkPlayer(NetworkPlayer newPlayer) {
		Debug.Log ("Player Connected.");

		connectedPlayers.Add (newPlayer);

		if (NetworkServer.active) {
			UpdatePlayerIDs ();
		}
	}

	public void DeregisterNetworkPlayer(NetworkPlayer removedPlayer) {
		Debug.Log ("Removed Player ID: " + removedPlayer.playerId.ToString());

		int index = connectedPlayers.IndexOf (removedPlayer);

		if (index >= 0) {
			connectedPlayers.RemoveAt (index);
		}

		UpdatePlayerIDs ();
	}

	public void Host() {
		StartHost ();
	}

	public void Exit() {
		StopHost ();
	}

	public void LANClient() {
		StartClient ();
	}

	public void MatchMaker() {
		StartMatchMaker ();
	}

	public void Disconnect() {
		StopMatchmakingGame ();
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		Debug.Log ("Server Add Player");

		NetPlayer newPlayer = Instantiate<NetPlayer> (networkPlayerPrefab);
		DontDestroyOnLoad (newPlayer);
		NetworkServer.AddPlayerForConnection (conn, newPlayer.gameObject, playerControllerId);
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		Debug.Log("OnClientDisconnect");

		base.OnClientDisconnect(conn);
	}

	public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
		Debug.Log ("OnServerRemovePlayer");
		base.OnServerRemovePlayer (conn, player);
	}

	public override void OnStartHost() {
		base.OnStartHost ();
		Debug.Log ("Host start.");
	}

	public override void OnStartServer() {
		base.OnStartServer ();
		Debug.Log ("Server Start.");
	}

	void OnStartClient() {
		
	}

	public override void OnStopHost () {
		base.OnStopHost ();
	}

	public override void OnStopClient() {
		base.OnStopClient ();
	}

	public override void OnStopServer() {
		base.OnStopServer ();
		Debug.Log ("Server Stopped.");
	}

	public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
		base.OnMatchCreate (success, extendedInfo, matchInfo);

		if (success) {
			state = NetworkState.InLobby;
			Debug.Log ("Match Create Successful.");
		} else {
			state = NetworkState.Inactive;
		}

		if (nextMatchCreatedCallback != null) {
			nextMatchCreatedCallback (success, matchInfo);
			nextMatchCreatedCallback = null;
		}

		if (matchCreated != null) {
			matchCreated (success, matchInfo);
		}
	}

	public void StartMatchMakingGame(string gameName, Action<bool, MatchInfo> onCreate) {
		if (state != NetworkState.Inactive) {
			throw new InvalidOperationException ("Network current active. Disconnect first.");
		}

		state = NetworkState.Connecting;
		gameType = NetworkGameType.Matchmaking;

		StartMatchMaker ();
		nextMatchCreatedCallback = onCreate;

		matchMaker.CreateMatch (gameName, (uint)multiplayerMaxPlayers, true, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchCreate);
	}

	private void StopMatchmakingGame() {
		switch (state)
		{
		case NetworkState.Pregame:
			if (NetworkServer.active)
			{
				Debug.LogError("Server should never be in this state.");
			}
			else
			{
				StopMatchMaker();
			}
			break;

		case NetworkState.Connecting:
			if (NetworkServer.active)
			{
				StopMatchMaker();
				StopHost();
				matchInfo = null;
			}
			else
			{
				StopMatchMaker();
				StopClient();
				matchInfo = null;
			}
			break;

		case NetworkState.InLobby:
		case NetworkState.InGame:
			if (NetworkServer.active)
			{
				if (matchMaker != null && matchInfo != null)
				{
					matchMaker.DestroyMatch(matchInfo.networkId, 0, (success, info) =>
						{
							if (!success)
							{
								Debug.LogErrorFormat("Failed to terminate matchmaking game. {0}", info);
							}
							StopMatchMaker();
							StopHost();

							matchInfo = null;
						});
				}
				else
				{
					Debug.LogWarning("No matchmaker or matchInfo despite being a server in matchmaking state.");

					StopMatchMaker();
					StopHost();
					matchInfo = null;
				}
			}
			else
			{
				if (matchMaker != null && matchInfo != null)
				{
					matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, (success, info) =>
						{
							if (!success)
							{
								Debug.LogErrorFormat("Failed to disconnect from matchmaking game. {0}", info);
							}
							StopMatchMaker();
							StopClient();
							matchInfo = null;
						});
				}
				else
				{
					Debug.LogWarning("No matchmaker or matchInfo despite being a client in matchmaking state.");

					StopMatchMaker();
					StopClient();
					matchInfo = null;
				}
			}
			break;
		}

		state = NetworkState.Inactive;
	}
}
