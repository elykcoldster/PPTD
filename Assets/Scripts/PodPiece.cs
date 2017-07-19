using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PodPiece : NetworkBehaviour {

	public float rotationSpeed = 60f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (transform.position, Vector3.up, Time.deltaTime * rotationSpeed);
	}

	void OnTriggerEnter(Collider c) {
		if (c.GetComponent<Prey> () != null) {
			Global.instance.IncrementPodPieces ();
			Destroy (gameObject);
		}
	}
}
