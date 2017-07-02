using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChatBox : NetworkBehaviour {

	Text chat;
	SyncListString chatMessages;

	// Use this for initialization
	void Start () {
		chat = GetComponent<Text> ();
		chatMessages = new SyncListString ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PushChat(string txt) {
		chatMessages.Add (txt);
	}
}
