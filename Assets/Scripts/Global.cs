using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Global : NetworkBehaviour {

	public static Global instance;

	public GameObject gameOverScreen;

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
		RpcSetVictory ();
		player.Victory ();
	}

	public void IncrementPodPieces() {
		this.podPieces++;
	}

	public void SetGameOverScreen(NetworkPlayer player) {
		string str = "";
		if (player.GetComponent<Prey> () && victory) {
			str = "Victory!";

		} else if (player.GetComponent<Predator> () && victory) {
			str = "Game Over";
		}

		Text gotext = gameOverScreen.GetComponentInChildren<Text> ();
		gotext.text = str;
		Color textcolor = gotext.color;
		textcolor.a = 1f;
		gotext.color = textcolor;
	}

	[Command]
	void CmdSetVictory() {
		Debug.Log ("Cmd");
		this.victory = true;
	}

	[ClientRpc]
	void RpcSetVictory() {
		Debug.Log ("Rpc");
		this.victory = true;
	}
}
