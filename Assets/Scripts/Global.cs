using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Global : NetworkBehaviour {

	public static Global instance;

	[SyncVar]
	public int podPieces;
	[SyncVar]
	public bool victory;

	void Awake() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	public void SetVictory(NetworkPlayer player) {
		this.victory = true;
		Debug.Log (this.victory);

		player.Victory ();
	}

	public void IncrementPodPieces() {
		this.podPieces++;
	}
}
