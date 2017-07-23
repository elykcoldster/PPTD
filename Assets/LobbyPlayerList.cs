using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerList : MonoBehaviour {

	public static LobbyPlayerList Instance = null;

	[SerializeField]
	protected RectTransform playerListContent;

	private NetworkMan netManager;

	void Awake() {
		Instance = this;
	}

	void Start () {
		netManager = NetworkMan.instance;

		if (netManager != null) {
			
		}
	}
	
	//Add lobby player to UI
	public void AddPlayer(LobbyPlayer player)
	{
		Debug.Log("Add player to list");
		player.transform.SetParent(playerListContent, false);
	}

	//Log player joining for tracing
	protected virtual void PlayerJoined(NetPlayer player)
	{
		Debug.LogFormat("Player joined {0}", player.name);
	}

	//Log player leaving for tracing
	protected virtual void PlayerLeft(NetPlayer player)
	{
		Debug.LogFormat("Player left {0}", player.name);
	}

	//When players are all ready progress
	protected virtual void PlayersReadied()
	{
		netManager.ProgressToGameScene();
	}
}
