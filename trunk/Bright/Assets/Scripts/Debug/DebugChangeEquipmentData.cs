using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

namespace Bright
{
	/// <summary>
	/// デバッグで装備品データを切り替えるコンポーネント.
	/// </summary>
	public class DebugChangeEquipmentData : MonoBehaviour
	{
		[SerializeField]
		private DebugInputAction inputAction;

		[SerializeField]
		private EquipmentData data;

		void Update()
		{
			this.inputAction.Proccess(() =>
			{
				PlayerStatus.Instance.InventoryEquipment.ChangeEquipment(this.data.Copy);
			});
		}
	}
}
