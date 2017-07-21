using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : MonoBehaviour {

	private MainMenuUI menuUI;
	private NetworkMan netManager;

	// Use this for initialization
	void Start () {
		menuUI = MainMenuUI.Instance;
		netManager = NetworkMan.instance;
	}
	
	public void OnBackClicked() {
		Back ();
	}

	private void Back() {
		netManager.Disconnect ();
		menuUI.ShowDefaultPanel ();
	}
}
