using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGame : MonoBehaviour {

	[SerializeField]
	protected InputField matchName;

	private MainMenuUI menuUI;
	private NetworkMan netManager;

	// Use this for initialization
	void Start () {
		menuUI = MainMenuUI.Instance;
		netManager = NetworkMan.instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnBackClicked() {
		menuUI.ShowDefaultPanel ();
	}

	public void OnCreateClicked() {
		if (string.IsNullOrEmpty (matchName.text)) {
			return;
		}

		StartMatchMakingGame ();
	}

	private void StartMatchMakingGame() {
		netManager.StartMatchMakingGame (matchName.text, (success, matchInfo) =>
			{
				if (!success) {
				
				} else {
					menuUI.ShowLobbyPanel();
				}
			});
	}
}
