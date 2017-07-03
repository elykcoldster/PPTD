using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

	public static SpawnManager instance;

	public SpawnPoint[] spawnPoints;

	void Awake() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (this.gameObject);

			spawnPoints = GameObject.FindObjectsOfType<SpawnPoint> ();
		}
	}
	
	public SpawnPoint GetSpawnPointByID(int id) {
		return spawnPoints[id];
	}
}
