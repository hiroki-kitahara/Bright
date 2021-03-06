﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Bright
{
	/// <summary>
	/// .
	/// </summary>
	[NetworkSettings(channel = 1, sendInterval=0.0f)]
	public class SyncPosition : NetworkBehaviour
	{
		[SerializeField]
		private bool autoProvideToServer;

		[SerializeField]
		private float lerpRate = 1.0f;

		[SyncVar]
		private Vector3 syncPos;

		[ClientCallback]
		void Update ()
		{
			if(this.isLocalPlayer)
			{
				TransmitPosition();
			}
			else
			{
				this.transform.position = Vector3.Lerp(this.transform.position, this.syncPos, lerpRate);
			}
		}

		[Command]
		public void CmdProvidePositionToServer(Vector3 position)
		{
			this.syncPos = position;
		}

		[Client]
		void TransmitPosition()
		{
			if(!this.autoProvideToServer)
			{
				return;
			}

			CmdProvidePositionToServer(this.transform.position);
		}
	}
}
