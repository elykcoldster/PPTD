using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkMan : NetworkManager {

	public static NetworkMan instance;

	public List<NetworkPlayer> connectedPlayers;
	public GameObject healthBar;

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

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		Debug.Log("OnClientDisconnect");

		base.OnClientDisconnect(conn);
	}

	public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
		Debug.Log ("OnServerRemovePlayer");
		base.OnServerRemovePlayer (conn, player);
	}
}
