﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

namespace Bright
{
	/// <summary>
	/// .
	/// </summary>
	public class CreatePrefab : MonoBehaviour
	{
		[SerializeField]
		private GameObject prefab;

		void Start()
		{
			var instance = Instantiate(prefab);
			ExecuteEvents.Execute<IReceiveCreatePrefabExtension>(this.gameObject, null, (handler, eventData) => handler.OnCreatePrefabExtension(instance));
		}

		public void ChangePrefab(GameObject prefab)
		{
			this.prefab = prefab;
		}
	}
}