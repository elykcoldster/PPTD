using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {

	public float speed = 10f;
	[SyncVar]
	public float damage;

	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
		Destroy (gameObject, 1f);
	}
	
	public void SetSpeed(float s) {
		this.speed = s;
	}

	public void SetDamage(float damage) {
		this.damage = damage;
	}

	protected virtual void OnCollisionEnter(Collision c) {
		if (c.gameObject.layer == LayerMask.NameToLayer ("Player")) {
			c.transform.GetComponent<Health> ().TakeDamage ((int)damage, false);
		}
		Destroy (gameObject);
	}
}
