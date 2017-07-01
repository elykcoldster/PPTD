using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mine : MonoBehaviour {

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

		PlayerController pc = c.gameObject.GetComponent<PlayerController> ();
		Health health = c.gameObject.GetComponent<Health> ();

		if (health != null) {
			health.TakeDamage (10, true);
		}

		Destroy (gameObject);
	}
}
