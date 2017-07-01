using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkMan : NetworkManager {

	public static NetworkMan instance;

	public List<NetworkPlayer> connectedPlayers;

	protected virtual void Awake() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;

			connectedPlayers = new List<NetworkPlayer> ();
		}
	}

	public void RegisterNetworkPlayer(NetworkPlayer newPlayer) {
		Debug.Log ("Player joined");

		connectedPlayers.Add (newPlayer);

		if (NetworkServer.active) {
			UpdatePlayerIDs ();
		}
	}

	void UpdatePlayerIDs() {
		for (int i = 0; i < connectedPlayers.Count; i++) {
			connectedPlayers [i].SetPlayerId (i);
		}
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
