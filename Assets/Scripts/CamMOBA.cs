using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMOBA : MonoBehaviour {

	[SerializeField] Transform target;
	public Vector3 cameraOffset = new Vector3(4f, 6f, 4f);
	public float cameraSpeed = 3f;

	// Use this for initialization
	void Start () {
		
	}
	
	void LateUpdate () {
		if (target) {
			transform.position = target.position + cameraOffset;
		}
	}

	public void SetTarget(Transform t) {
		this.target = t;
	}
}
