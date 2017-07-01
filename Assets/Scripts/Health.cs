using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour {

	public const int maxHealth = 100;

	[SyncVar]
	public int currentHealth = maxHealth;

	public void TakeDamage(int amount, bool stun)
	{
		if (!isServer)
		{
			return;
		}

		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Debug.Log("Dead!");
		}

		if (stun) {
			GetComponent<PlayerController> ().Stun ();
		}
	}
}