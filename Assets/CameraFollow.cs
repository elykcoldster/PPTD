using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exale.Utilities;

public class CameraFollow : MonoBehaviour {

	private static Vector3 cameraUnit = new Vector3 (-1f, 1.5f, -1f);

	[SerializeField]
	private float zoom = 5f;
	[SerializeField]
	private float zoomSpeed = 200f;
	[SerializeField]
	private float minZoom = 2f;
	[SerializeField]
	private float maxZoom = 6f;

	private Transform followTransform = null;

	// Use this for initialization
	void Start () {
		LoadPlayerToFollow ();
	}
	
	// Update is called once per frame
	void Update () {
		FollowPlayer ();
		Zoom ();
	}

	private void LoadPlayerToFollow() {
		if (followTransform != null) {
			return;
		}

		List <PlayerManager> playerList = GameManager.players;

		for (int i = 0; i < playerList.Count; i++) {
			PlayerManager pm = playerList [i];
			if (pm != null && pm.hasAuthority) {
				followTransform = pm.transform;
			}
		}
	}

	private void FollowPlayer() {
		LoadPlayerToFollow ();

		if (followTransform == null) {
			return;
		}

		Vector3 pos = followTransform.position;
		Vector3 targetPos = pos + zoom * cameraUnit;

		transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * 12f);
	}

	private void Zoom() {
		float mw = Input.GetAxis ("Mouse ScrollWheel");

		zoom -= mw * Time.deltaTime * zoomSpeed;
		zoom = Mathf.Clamp (zoom, minZoom, maxZoom);
	}
}
