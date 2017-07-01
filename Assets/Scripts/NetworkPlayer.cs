using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent (typeof(Animator))]
public class NetworkPlayer : NetworkBehaviour {

	public const float STUN_TIME = 1.133f;

	[SyncVar]
	public bool stunned;
	[SyncVar]
	Vector3 realPosition = Vector3.zero;
	[SyncVar]
	Quaternion realRotation;
	[SyncVar]
	float syncFwd;

	[SyncVar]
	public int playerId;

	Animator anim;
	CharacterController cc;
	NavMeshAgent nma;
	Rigidbody rb;
	NetworkMan netManager;

	int groundLayer;
	float animForward;
	float updateInterval;

	public override void OnStartLocalPlayer() {
		CamMOBA cm = FindObjectOfType<CamMOBA> () as CamMOBA;
		cm.SetTarget (transform);
	}

	[Client]
	public override void OnStartClient() {
		DontDestroyOnLoad (this);
		if (netManager == null) {
			netManager = NetworkMan.instance;
		}

		base.OnStartClient ();
		Debug.Log ("Client Network Player Start");

		netManager.RegisterNetworkPlayer (this);
	}

	public override void OnStopAuthority() {
		base.OnStopAuthority ();
		Debug.Log ("Client Network Player Destroyed.");
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

	/**
	 * Public Functions
	 **/

	public void Stun() {
		if (isLocalPlayer) {
			CmdStun ();
		} else {
			RpcStun ();
		}
	}

	public int PlayerId() {
		return this.playerId;
	}

	/**
	 * Commands
	 **/
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

	[Server]
	public void SetPlayerId(int playerId) {
		this.playerId = playerId;
	}
}
