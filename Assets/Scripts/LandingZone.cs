using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LandingZone : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider c) {
		if (!isServer) {
			return;
		}
		if (c.GetComponent<Prey> () != null && Global.instance.podPieces >= 2) {
			Global.instance.SetVictory (c.GetComponent<NetworkPlayer> ());
		}
	}
}
