using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Exale.Utilities;

public class MainMenuUI : Singleton<MainMenuUI> {

	[SerializeField]
	protected CanvasGroup defaultPanel;
	[SerializeField]
	protected CanvasGroup createGamePanel;
	[SerializeField]
	protected CanvasGroup lobbyPanel;
	[SerializeField]
	protected CanvasGroup serverListPanel;
	[SerializeField]
	protected LobbyPlayerList lobbyPlayerList;

	private CanvasGroup currentPanel;

	private Action waitTask;

	private bool readyToFireTask;

	public LobbyPlayerList playerList {
		get {
			return this.lobbyPlayerList;
		}
	}

	public LobbyPanel lobbyPanelObject {
		get {
			return this.lobbyPanel.GetComponent<LobbyPanel> ();
		}
	}

	public void ShowPanel(CanvasGroup newPanel) {
		if (currentPanel != null) {
			currentPanel.gameObject.SetActive (false);
		}

		currentPanel = newPanel;
		if (currentPanel != null) {
			currentPanel.gameObject.SetActive (true);
		}
	}

	public void OnCreateGameClicked() {
		DoIfNetworkReady (GoToCreateGamePanel);
	}

	public void OnFindGameClicked() {
		DoIfNetworkReady (GoToFindGamePanel);
	}

	public void ShowDefaultPanel() {
		ShowPanel (defaultPanel);
	}

	public void ShowLobbyPanel() {
		ShowPanel (lobbyPanel);
	}

	public void ShowServerListPanel() {
		ShowPanel (serverListPanel);
	}

	public void ShowLobbyPanelForConnection() {
		ShowPanel (lobbyPanel);
		NetworkMan.instance.gameModeUpdated -= ShowLobbyPanelForConnection;
	}

	public void DoIfNetworkReady(Action task) {
		if (task == null) {
			throw new ArgumentNullException ("task");
		}

		NetworkMan netManager = NetworkMan.instance;

		if (netManager.isNetworkActive) {
			waitTask = task;

			readyToFireTask = false;
		} else {
			task ();
		}
	}

	private void GoToCreateGamePanel() {
		ShowPanel (createGamePanel);
	}

	private void GoToFindGamePanel() {
		ShowServerListPanel ();
		NetworkMan.instance.StartMatchmakingClient ();
	}
}
