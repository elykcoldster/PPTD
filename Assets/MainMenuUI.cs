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

	private CanvasGroup currentPanel;

	private Action waitTask;

	private bool readyToFireTask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void GoToCreateGamePanel() {
		ShowPanel (createGamePanel);
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

	public void ShowDefaultPanel() {
		ShowPanel (defaultPanel);
	}

	public void ShowLobbyPanel() {
		ShowPanel (lobbyPanel);
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
}
