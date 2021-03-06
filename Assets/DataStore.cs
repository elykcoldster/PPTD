using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class DataStore : ISerializationCallbackReceiver {

	private static readonly string DefaultName = "Player";

	public int selectedRole;
	public int selectedDecoration;
	public int currency;
	public bool[] unlockedTanks;
	public string playerName;

	public DataStore() {
		playerName = DefaultName;
	}

	public void OnBeforeSerialize()
	{
		// LevelDataSerialize();
	}

	/// <summary>
	/// Deserialization implementation from ISerializationCallbackReceiver
	/// </summary>
	public void OnAfterDeserialize()
	{
		// LevelDataDeserialize();
	}
}
