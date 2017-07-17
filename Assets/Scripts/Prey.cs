using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour {

	public static float preyFireRate = 2f;

	// Use this for initialization
	void Start () {
		GetComponent<NetworkPlayer>().ChangeFireRate (preyFireRate);
	}

	// Update is called once per frame
	void Update () {

	}
}
