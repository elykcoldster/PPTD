using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	public float maxLength = 0.75f;
	public Vector2 girth = new Vector2(0.1f, 0.1f);

	Health playerHealth;
	Transform target;

	float length;

	// Use this for initialization
	void Start () {
		playerHealth = GetComponentInParent<Health> ();
		length = maxLength * playerHealth.currentHealth / Health.maxHealth;
	}

	void Update() {
		length = maxLength * playerHealth.currentHealth / Health.maxHealth;

		transform.localRotation = Quaternion.Euler (-transform.parent.rotation.eulerAngles + 45f * Vector3.up);
		transform.localScale = new Vector3 (length, girth.x, girth.y);
	}
}
