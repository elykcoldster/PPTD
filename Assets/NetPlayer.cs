using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPlayer : NetworkBehaviour {

	public event Action<NetPlayer> syncVarsChanged;
	public event Action<NetPlayer> becameReady;
	public event Action gameDetailsReady;

	[SerializeField]
	protected GameObject playerPrefab;
	[SerializeField]
	protected GameObject lobbyPrefab;

	[SyncVar (hook = "OnMyName")]
	private string playerName = "";
	[SyncVar (hook = "OnMyRole")]
	private int playerRole = -1;
	[SyncVar (hook = "OnReadyChanged")]
	private bool ready = false;
	[SyncVar (hook = "OnHasInitialized")]
	private bool initialized = false;
	[SyncVar]
	private int playerId;

	public static NetPlayer LocalPlayer {
		get;
		private set;
	}

	public PlayerManager player {
		get;
		set;
	}

	public string PlayerName {
		get {
			return playerName;
		}
	}

	public int PlayerRole {
		get {
			return playerRole;
		}
	}

	public int PlayerId {
		get {
			return playerId;
		}
	}

	public bool Ready {
		get {
			return ready;
		}
	}

	private NetworkMan netManager;

	public LobbyPlayer lobbyObject {
		get;
		private set;
	}

	public void SetPlayerId(int id) {
		playerId = id;
	}

	public void ClearReady() {
		ready = false;
	}

	private void CreateLobbyObject() {
		lobbyObject = Instantiate (lobbyPrefab).GetComponent<LobbyPlayer> ();
		lobbyObject.Init (this);
	}

	public override void OnNetworkDestroy() {
		base.OnNetworkDestroy ();

		if (lobbyObject != null) {
			Destroy (lobbyObject.gameObject);
		}
	}

	protected virtual void OnDestroy() {
		if (lobbyObject != null) {
			Debug.Log ("Remove lobby object.");
			Destroy (lobbyObject.gameObject);
		}
	}

	[Client]
	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer ();
		UpdatePlayerSelections ();

		LocalPlayer = this;
	}

	[Client]
	public override void OnStartClient() {
		DontDestroyOnLoad (this);

		if (netManager == null) {
			netManager = NetworkMan.instance;
		}

		base.OnStartClient ();

		netManager.RegisterNetworkPlayer (this);
	}

	[Client]
	public void OnEnterLobbyScene () {
		if (initialized && lobbyObject == null) {
			CreateLobbyObject ();
		}
	}

	[Client]
	public void OnEnterGameScene() {
		if (hasAuthority) {
			CmdClientReadyInScene ();
		}
	}

	[Client]
	private void UpdatePlayerSelections() {
		CmdSetInitialValues (playerName);
	}

	[Command]
	private void CmdSetInitialValues(string newName) {
		playerName = newName;
		initialized = true;
	}

	[Command]
	private void CmdClientReadyInScene() {
		Debug.Log ("Client Ready in Scene.");
		GameObject playerObject = Instantiate (playerPrefab);
		NetworkServer.SpawnWithClientAuthority (playerObject, connectionToClient);

		player = playerObject.GetComponent<PlayerManager> ();
		player.SetPlayerId (playerId);
	}

	[Command]
	public void CmdNameChanged(string name) {
		Debug.Log ("CmdNameChanged");
		playerName = name;
	}

	[Command]
	public void CmdChangeRole(int role) {
		Debug.Log ("Role Changed to " + role.ToString());
		playerRole = role;
	}

	[Command]
	public void CmdSetReady() {
		if (playerRole == -1) {
			return;
		}

		ready = !ready;

		if (becameReady != null) {
			becameReady (this);
		}
	}

	[ClientRpc]
	public void RpcSetGameSettings() {
		if (gameDetailsReady != null && isLocalPlayer) {
			gameDetailsReady ();
		}
	}

	[ClientRpc]
	public void RpcPrepareForLoad() {
		if (isLocalPlayer) {
			
		}
	}

	private void OnHasInitialized(bool value) {
		if (!initialized && value) {
			initialized = value;
			CreateLobbyObject ();

			if (isServer) {
				RpcSetGameSettings ();
			}
		}
	}

	private void OnMyName(string newName) {
		playerName = newName;

		if (syncVarsChanged != null) {
			syncVarsChanged (this);
		}
	}


	private void OnMyRole(int newRole) {
		if (newRole != -1) {
			playerRole = newRole;

			if (newRole == 0) {
				lobbyObject.SelectPredatorButton ();
			} else if (newRole == 1) {
				lobbyObject.SelectPreyButton ();
			}

			if (syncVarsChanged != null) {
				syncVarsChanged (this);
			}
		}
	}

	private void OnReadyChanged(bool value) {
		ready = value;
		if (lobbyObject != null) {
			lobbyObject.SetReadyButtonColor (ready);
		}

		if (syncVarsChanged != null) {
			syncVarsChanged (this);
		}
	}
}
