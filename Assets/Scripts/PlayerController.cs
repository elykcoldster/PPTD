using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent (typeof(Animator))]
public class PlayerController : NetworkBehaviour {

	[SyncVar]
	Vector3 realPosition = Vector3.zero;
	[SyncVar]
	Quaternion realRotation;
	[SyncVar]
	float syncFwd;

	Animator anim;
	CharacterController cc;
	NavMeshAgent nma;

	int groundLayer;
	float updateInterval;

	public override void OnStartLocalPlayer() {
		CamMOBA cm = FindObjectOfType<CamMOBA> () as CamMOBA;
		cm.SetTarget (transform);
	}

	void Start() {
		anim = GetComponent<Animator> ();
		cc = GetComponent<CharacterController> ();
		nma = GetComponent<NavMeshAgent> ();

		groundLayer = 1 << LayerMask.NameToLayer ("Ground");
	}

	void Update() {
		MouseInput ();
		Animate ();
	}

	void MouseInput() {
		if (isLocalPlayer) {
			if (Input.GetMouseButton (0)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayer)) {
					nma.SetDestination (hit.point);
				}
			}

			updateInterval += Time.deltaTime;
			if (updateInterval > 0.11f) {
				updateInterval = 0f;
				CmdSync (transform.position, transform.rotation);
			}
		} else {
			transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
		}
	}

	void Animate() {
		if (isLocalPlayer) {
			float forward = nma.velocity.magnitude / nma.speed;
			anim.SetFloat ("forward", forward);

			CmdSyncAnim (forward);
		} else {
			float forward = anim.GetFloat ("forward");
			forward = Mathf.Lerp (forward, syncFwd, 0.1f);
			anim.SetFloat ("forward", forward);
		}
	}

	[Command]
	void CmdSync(Vector3 position, Quaternion rotation) {
		realPosition = position;
		realRotation = rotation;
	}

	[Command]
	void CmdSyncAnim(float fwd) {
		syncFwd = fwd;
	}
}
