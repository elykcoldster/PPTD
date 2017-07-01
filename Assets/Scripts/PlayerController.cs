using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent (typeof(Animator))]
public class PlayerController : NetworkBehaviour {

	public const float STUN_TIME = 1.133f;

	[SyncVar]
	public bool stunned;
	[SyncVar]
	Vector3 realPosition = Vector3.zero;
	[SyncVar]
	Quaternion realRotation;
	[SyncVar]
	float syncFwd;

	Animator anim;
	CharacterController cc;
	NavMeshAgent nma;
	Rigidbody rb;

	int groundLayer;
	float animForward;
	float updateInterval;

	public override void OnStartLocalPlayer() {
		CamMOBA cm = FindObjectOfType<CamMOBA> () as CamMOBA;
		cm.SetTarget (transform);
	}

	void Start() {
		anim = GetComponent<Animator> ();
		cc = GetComponent<CharacterController> ();
		nma = GetComponent<NavMeshAgent> ();
		rb = GetComponent<Rigidbody> ();

		groundLayer = 1 << LayerMask.NameToLayer ("Ground");
	}

	void Update() {
		MouseInput ();
		Animate ();
		Sync ();

		if (isLocalPlayer && stunned) {
			nma.ResetPath ();
		}
	}

	void MouseInput() {
		if (isLocalPlayer && !stunned) {
			if (Input.GetMouseButton (0)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayer)) {
					nma.SetDestination (hit.point);
				}
			}
		} else {
			transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
		}
	}

	void Animate() {
		if (isLocalPlayer) {
			animForward = nma.velocity.magnitude / nma.speed;
			anim.SetFloat ("forward", animForward);
		} else {
			float forward = anim.GetFloat ("forward");
			forward = Mathf.Lerp (forward, syncFwd, 0.1f);
			anim.SetFloat ("forward", forward);
		}
	}

	void Sync() {
		if (isLocalPlayer) {
			updateInterval += Time.deltaTime;

			if (updateInterval > 0.11f) {
				updateInterval = 0f;
				CmdSync (transform.position, transform.rotation);
				CmdSyncAnim (animForward);
			}
		}
	}

	public void Stun() {
		if (isLocalPlayer) {
			CmdStun ();
		} else {
			RpcStun ();
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

	[Command]
	void CmdRecover() {
		stunned = false;
	}

	[Command]
	void CmdStun() {
		stunned = true;

		nma.ResetPath ();
		GetComponent<NetworkAnimator> ().SetTrigger ("stunned");

		StartCoroutine (StunForSeconds (STUN_TIME));
	}

	[ClientRpc]
	void RpcStun() {
		stunned = true;

		nma.ResetPath ();
		GetComponent<NetworkAnimator> ().SetTrigger ("stunned");

		StartCoroutine (StunForSeconds (STUN_TIME));
	}

	IEnumerator StunForSeconds(float time) {
		yield return new WaitForSeconds(time);
		CmdRecover ();
	}
}
