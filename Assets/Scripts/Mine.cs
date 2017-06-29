using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision c) {
		PlayerController pc = c.gameObject.GetComponent<PlayerController> ();
		Health health = c.gameObject.GetComponent<Health> ();

		if (health != null) {
			health.TakeDamage (10);
			pc.CmdStun (2f);
		}
	}
}
