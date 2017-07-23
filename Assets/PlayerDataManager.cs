using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exale.Utilities;

public class PlayerDataManager : Singleton<PlayerDataManager> {
	[NonSerialized]
	private DataStore data;
	/*[NonSerialized]
	private IDataSaver saver;*/

	public int selectedRole {
		get {
			return data.selectedRole;
		}
		set {
			data.selectedRole = value;
		}
	}

	void Awake() {
		DontDestroyOnLoad (gameObject);

		data = new DataStore ();
	}
}
