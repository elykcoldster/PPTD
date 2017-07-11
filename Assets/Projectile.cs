using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {

	public float speed = 10f;

	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
		Destroy (gameObject, 1f);
	}
	
	public void SetSpeed(float s) {
		this.speed = s;
	}
}
