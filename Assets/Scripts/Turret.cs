using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Turret : NetworkBehaviour {

	public GameObject projectile;
	public Vector3 startRotation;
	public float fireRate = 1f;
	public float projectileSpeed = 20f;
	public float damage = 10f;

	[SyncVar]
	Vector3 realRotation;

	Transform target;
	float updateInterval, shootInterval;

	void Start() {
		transform.rotation = Quaternion.Euler (startRotation);
	}

	void OnStartClient() {
		realRotation = startRotation;
	}

	void Update() {
		ShootAtTarget ();
	}

	void OnTriggerStay(Collider c) {
		if (c.gameObject.layer == LayerMask.NameToLayer ("Player") && c.tag == "Prey" && target == null) {
			target = c.transform;
		}
	}

	void OnTriggerExit(Collider c) {
		target = null;
	}

	void ShootAtTarget() {
		if (!isServer) {
			transform.rotation = Quaternion.Lerp (transform.rotation,
				Quaternion.Euler(realRotation),
				12f * Time.deltaTime
			);
			return;
		}

		updateInterval += Time.deltaTime;
		if (updateInterval > 0.11f) {
			updateInterval = 0f;
			if (isServer) {
				Sync ();
			} else {
				CmdSync ();
			}
		}

		if (target == null) {
			transform.rotation = Quaternion.Lerp (
				transform.rotation,
				Quaternion.Euler(startRotation),
				12f * Time.deltaTime
			);
			return;
		}

		transform.rotation = Quaternion.Lerp (
			transform.rotation,
			Quaternion.LookRotation (target.position + Vector3.up - transform.position),
			12f * Time.deltaTime
		);

		shootInterval += Time.deltaTime;
		if (shootInterval > 1 / fireRate) {
			GameObject p = (GameObject)Instantiate (projectile);
			p.transform.position = transform.position + 2f * transform.forward;
			p.transform.rotation = transform.rotation;
			p.GetComponent<Projectile> ().SetSpeed (this.projectileSpeed);
			p.GetComponent<Projectile> ().SetDamage (this.damage);

			NetworkServer.Spawn (p);

			shootInterval = 0f;
		}
	}

	void Sync() {
		realRotation = transform.rotation.eulerAngles;
	}

	[Command]
	void CmdSync() {
		realRotation = transform.rotation.eulerAngles;
	}
}
