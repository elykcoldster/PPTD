using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Exale.Map;

public class GameManager : NetworkBehaviour {

	public static GameManager Instance;

	public static List<PlayerManager> players = new List<PlayerManager> ();

	void Awake() {
		Instance = this;
	}

	public static void AddPlayer(PlayerManager player) {
		if (players.IndexOf (player) == -1) {
			players.Add (player);
			player.MoveToSpawnLocation (SpawnManager.Instance.GetSpawnPointTransformByIndex (player.playerNumber));
		}
	}
}
