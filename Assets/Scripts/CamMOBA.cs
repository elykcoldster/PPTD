using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMOBA : MonoBehaviour {

	[SerializeField] Transform target;
	public Vector3 cameraOffset = new Vector3(4f, 6f, 4f);
	public float cameraSpeed = 3f;

	void Update() {
	
	}
	
	void LateUpdate () {
		if (target) {
			transform.position = target.position + cameraOffset;
			DetectWalls ();
		}
	}

	void DetectWalls() {
		int wallLayerMask = 1 << LayerMask.NameToLayer ("Wall");
		Vector3 castDir = target.position - transform.position;
		RaycastHit[] hits = Physics.RaycastAll (transform.position, castDir, castDir.magnitude, wallLayerMask);

		foreach (RaycastHit hit in hits) {
			hit.transform.GetComponent<Renderer> ().enabled = false;
		}
	}

	public void SetTarget(Transform t) {
		this.target = t;
	}
}
