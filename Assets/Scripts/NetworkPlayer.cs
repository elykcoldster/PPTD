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
	[SyncVar]
	public float fireRate = 4f;
	[SyncVar]
	public float damage = 5f;
	[SyncVar]
	public bool dead;
	public GameObject projectile;

	Animator anim;
	CharacterController cc;
	NavMeshAgent nma;
	Rigidbody rb;
	NetworkMan netManager;
	HealthBar healthBar;

	int groundLayer;
	float animForward;
	float updateInterval;
	float shootInterval;

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

		SpawnPoint spawnPoint = SpawnManager.instance.GetSpawnPointByID (playerId);
		transform.position = spawnPoint.transform.position;

		GetComponent<NavMeshAgent> ().enabled = true;

		if (playerId == 0) {
			tag = "Predator";
			gameObject.AddComponent<Predator> ();
		} else {
			tag = "Prey";
			gameObject.AddComponent<Prey> ();
		}
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
		if (dead) {
			return;
		}
		if (isLocalPlayer && !stunned) {
			if (Input.GetMouseButton (0)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayer)) {
					nma.SetDestination (hit.point);
				}
			}

			if (Input.GetMouseButton (1)) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayer)) {
					nma.ResetPath ();
					Vector3 targetDir = hit.point - transform.position;
					float step = nma.angularSpeed * Mathf.Deg2Rad * Time.deltaTime;
					Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0f);

					transform.rotation = Quaternion.LookRotation (newDir);
					CmdFire (targetDir);
				}
			}
		} else {
			transform.position = Vector3.Lerp (transform.position, realPosition, 12f * Time.deltaTime);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 12f * Time.deltaTime);
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

		shootInterval += Time.deltaTime;
		if (shootInterval >= 1 / fireRate) {
			shootInterval = 1 / fireRate;
		}
	}

	void OnDestroy() {
		NetworkMan.instance.DeregisterNetworkPlayer (this);
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

	public void Dead() {
		if (dead) {
			return;
		}
		if (isLocalPlayer) {
			CmdDead ();
		} else {
			RpcDead ();
		}
	}

	public int PlayerId() {
		return this.playerId;
	}

	public void SetHealthBar(HealthBar hb) {
		this.healthBar = hb;
	}

	public HealthBar HealthBar() {
		return this.healthBar;
	}

	public void ChangeFireRate(float fr) {
		if (isLocalPlayer) {
			CmdChangeFireRate (fr);
		} else {
			
		}
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

	[Command]
	void CmdDead() {
		this.dead = true;
		GetComponent<Collider> ().enabled = false;

		nma.ResetPath ();
		GetComponent<NetworkAnimator> ().SetTrigger ("dead");
	}

	[Command]
	void CmdFire(Vector3 dir) {
		if (shootInterval < 1 / fireRate) {
			return;
		} else {
			shootInterval = 0f;
		}
		GameObject p = (GameObject)Instantiate (projectile);
		p.transform.position = transform.position + Vector3.up + dir.normalized * (nma.radius + 0.1f);
		p.transform.LookAt (p.transform.position + dir);
		p.GetComponent<Projectile> ().SetDamage (damage);

		NetworkServer.Spawn (p);
	}

	[Command]
	void CmdChangeFireRate (float fr) {
		this.fireRate = fr;
	}

	[ClientRpc]
	void RpcStun() {
		stunned = true;

		nma.ResetPath ();
		GetComponent<NetworkAnimator> ().SetTrigger ("stunned");

		StartCoroutine (StunForSeconds (STUN_TIME));
	}

	[ClientRpc]
	void RpcDead() {
		this.dead = true;
		GetComponent<Collider> ().enabled = false;

		nma.ResetPath ();
		GetComponent<NetworkAnimator> ().SetTrigger ("dead");
	}

	[ClientRpc]
	void RpcChangeFireRate(float fr) {
		this.fireRate = fr;
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
