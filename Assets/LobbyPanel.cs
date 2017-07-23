using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour {

	private MainMenuUI menuUI;
	private NetworkMan netManager;
	private NetPlayer netPlayer;

	[SerializeField]
	protected Button startButton;

	// Use this for initialization
	void Start () {
		menuUI = MainMenuUI.Instance;
		netManager = NetworkMan.instance;
	}
	
	public void OnBackClicked() {
		Back ();
	}

	public void OnStartClicked() {
		if (/*netManager.AllPlayersReady () && */netManager.HasEnoughPredators ()) {
			netManager.ProgressToGameScene ();
		}
	}

	private void Back() {
		netManager.Disconnect ();
		menuUI.ShowDefaultPanel ();
	}
}
