using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class LobbyServerEntry : MonoBehaviour {

	//UI elements
	[SerializeField]
	protected Text ServerInfoText;
	[SerializeField]
	protected Text ModeText;
	[SerializeField]
	protected Text SlotInfo;
	[SerializeField]
	protected Button JoinButton;

	//The network manager
	protected NetworkMan netManager;

	//Sets up the UI
	public void Populate(MatchInfoSnapshot match, Color c)
	{
		string name = match.name;

		string[] split = name.Split(new char [1]{ '|' }, StringSplitOptions.RemoveEmptyEntries);

		ServerInfoText.text = name;
		// ModeText.text = split[0];

		// SlotInfo.text = string.Format("{0}/{1}", match.currentSize, match.maxSize);

		NetworkID networkId = match.networkId;

		JoinButton.onClick.RemoveAllListeners();
		JoinButton.onClick.AddListener(() => JoinMatch(networkId));

		JoinButton.interactable = match.currentSize < match.maxSize;

		GetComponent<Image>().color = c;
	}

	//Load the network manager on enable
	protected virtual void OnEnable()
	{
		if (netManager == null)
		{
			netManager = NetworkMan.instance;
		}
	}

	//Fired when player clicks join
	private void JoinMatch(NetworkID networkId)
	{
		MainMenuUI menuUi = MainMenuUI.Instance;

		// menuUi.ShowConnectingModal(true);

		netManager.JoinMatchmakingGame(networkId, (success, matchInfo) =>
			{
				//Failure flow
				if (!success)
				{
					// menuUi.ShowInfoPopup("Failed to join game.", null);
				}
				//Success flow
				else
				{
					// menuUi.HideInfoPopup();
					// menuUi.ShowInfoPopup("Entering lobby...");
					netManager.gameModeUpdated += menuUi.ShowLobbyPanelForConnection;
				}
			});
	}
}
