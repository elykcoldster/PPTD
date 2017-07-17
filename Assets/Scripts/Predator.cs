using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour {

	public static float predatorFireRate = 10f;

	// Use this for initialization
	void Start () {
		GetComponent<NetworkPlayer>().ChangeFireRate (predatorFireRate);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
