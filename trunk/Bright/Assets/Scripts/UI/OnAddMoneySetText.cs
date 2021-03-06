﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

namespace Bright
{
	/// <summary>
	/// OnAddMoneyイベント時にテキストを設定するコンポーネント.
	/// </summary>
	public class OnAddMoneySetText : MonoBehaviour, IReceiveAddMoney
	{
		[SerializeField]
		private Text target;

		[SerializeField]
		private string key;

		public void OnAddMoney(int total, int addValue)
		{
			this.target.text = StringAsset.Format(this.key, total);
		}
	}
}
