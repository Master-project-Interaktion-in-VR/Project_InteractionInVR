using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDPaint;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.PaintObject.Base;
using XDPaint.Tools.Raycast;

public class PCUIPaintNetworker : MonoBehaviour
{

		[SerializeField]
		private float delay;


		private PaintManager _paintManager;
		private PhotonView _photonView;

		private Vector2 _previousPoint;
		private float _startTime;

		private Vector2 lastPoint;


		void Awake()
		{
				_photonView = GetComponent<PhotonView>();
				_paintManager = GetComponent<PaintManager>();
				// _paintManager.OnInitialized += OnPaintManagerInitialized;
		}

		// private void OnPaintManagerInitialized(PaintManager paintManager)
		// {
		//     //_paintManager = paintManager;
		//     //_paintManager.PaintObject.OnPaintDataHandler += OnPaint;
		//     // _paintManager.PaintObject.OnMouseHandler += OnMouseDown;
		//     // _paintManager.PaintObject.OnMouseUpHandler += OnMouseUp;
		// }



		[PunRPC]
		private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
		{
				_paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
		}

		[PunRPC]
		private void DrawPointRpc(Vector2 position)
		{
				Vector2 scaledPosition = new Vector2(position.x * 2048, position.y * 1024);
				Debug.Log("DrawPointRpc" + scaledPosition);

				if (lastPoint == null)
				{
						lastPoint = scaledPosition;
				}

				_paintManager.PaintObject.DrawLine(lastPoint, scaledPosition);

				lastPoint = scaledPosition;
		}
}
