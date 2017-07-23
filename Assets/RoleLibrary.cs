using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exale.Utilities;

namespace Exale.Data {

	[Serializable]
	public struct RoleTypeDefinition {
		public string id;

		public string name;

		public string description;

		public float speed;
		public float health;

		public float armor;
		public float attack;

		public GameObject classPrefab;
	}

	public class RoleLibrary : PersistentSingleton<RoleLibrary> {

		[SerializeField]
		private RoleTypeDefinition[] roleDefinitions;

		protected override void Awake() {
			base.Awake ();
		}

		public RoleTypeDefinition GetRoleDataForIndex(int index) {
			return roleDefinitions [index];
		}

		public int GetNumberOfDefinitions() {
			return roleDefinitions.Length;
		}
	}
}