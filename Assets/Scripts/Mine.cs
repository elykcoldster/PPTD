using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mine : NetworkBehaviour {

	public int damage = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision c) {
		if (c.gameObject.layer != LayerMask.NameToLayer ("Player")) {
			return;
		}

		NetworkPlayer pc = c.gameObject.GetComponent<NetworkPlayer> ();
		Health health = c.gameObject.GetComponent<Health> ();

		if (health != null) {
			health.TakeDamage (damage, true);
		}

		gameObject.SetActive (false);
	}
}
