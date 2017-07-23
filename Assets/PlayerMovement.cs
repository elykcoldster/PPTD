using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	private Rigidbody rb;

	public Rigidbody Rigidbody {
		get {
			return rb;
		}
	}

	void Awake() {
		LoadRigidBody ();
	}

	private void LoadRigidBody() {
		if (rb != null) {
			return;
		}

		rb = GetComponent<Rigidbody> ();
	}
}
