using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class LobbyServerList : MonoBehaviour {

	[SerializeField]
	protected RectTransform serverList;
	[SerializeField]
	protected GameObject serverEntryPrefab;

	private NetworkMan netManager;
	private MainMenuUI menuUI;

	[SerializeField]
	private int pageSize = 6;

	protected int currentPage = 0;

	[SerializeField]
	private float autoRefreshTime = 10f;
	private float nextRefreshTime;

	void Update() {
		if (nextRefreshTime <= Time.time) {
			Debug.Log ("Refresh Request.");
			RequestPage (currentPage);
			nextRefreshTime = Time.time + autoRefreshTime;
		}
	}

	void OnEnable() {
		if (netManager == null) {
			netManager = NetworkMan.instance;
		}

		if (menuUI == null) {
			menuUI = MainMenuUI.Instance;
		}
	}

	public void OnBackClicked() {
		netManager.Disconnect ();
		menuUI.ShowDefaultPanel ();
	}

	public void OnGuiMatchList(bool flag, string extraInfo, List<MatchInfoSnapshot> response) {
		if (response == null) {
			return;
		}

		if (response.Count == 0) {
			if (currentPage == 0) {
				
			}

			return;
		}

		foreach (Transform t in serverList) {
			Destroy (t.gameObject);
		}

		for (int i = 0; i < response.Count; i++) {
			GameObject o = Instantiate (serverEntryPrefab);

			o.GetComponent<LobbyServerEntry> ().Populate (response [i], new Color (1f, 1f, 1f, 1f));

			o.transform.SetParent (serverList, false);
		}
	}

	private void RequestPage(int page) {
		if (netManager != null && netManager.matchMaker != null) {
			netManager.matchMaker.ListMatches (page, pageSize, string.Empty, false, 0, 0, OnGuiMatchList);
		}
	}
}
