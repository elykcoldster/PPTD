﻿using UnityEngine;
using System.Collections;

namespace Exale.Utilities
{
	/// <summary>
	/// Singleton class
	/// </summary>
	/// <typeparam name="T">Type of the singleton</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T instance;

		/// <summary>
		/// The static reference to the instance
		/// </summary>
		public static T Instance
		{
			get
			{
				return instance;
			}
			protected set
			{
				instance = value;
			}
		}

		/// <summary>
		/// Gets whether an instance of this singleton exists
		/// </summary>
		public static bool InstanceExists { get { return instance != null; } }

		/// <summary>
		/// Awake method to associate singleton with instance
		/// </summary>
		protected virtual void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = (T)this;
			}
		}

		/// <summary>
		/// OnDestroy method to clear singleton association
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}
	}
}
