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

/// <summary>
/// This class is the Photon manager for the PC UI Paint.
/// </summary>
public class PCUIPaintNetworker : MonoBehaviour
{

		[SerializeField]
		private Vector2 screenTextureSize;


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



		/// <summary>
		/// DrawLineRpc is called on all clients and draws a line from lineStartPosition to lineEndPosition on
		/// the PaintObject
		/// </summary>
		/// <param name="lineStartPosition">The start position of the line</param>
		/// <param name="lineEndPosition">The start position of the line</param>
		[PunRPC]
		private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
		{
				_paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
		}

		/// <summary>
		/// We take the position of the mouse postion and scale it to the size of the screen texture. Then we draw a
		/// line from the last point to the current point
		/// </summary>
		/// <param name="position">The position of the mouse on the screen.</param>
		[PunRPC]
		private void DrawPointRpc(Vector2 position)
		{
				Vector2 scaledPosition = new Vector2(position.x * screenTextureSize.x, position.y * screenTextureSize.y);
				Debug.Log("DrawPointRpc" + scaledPosition);

				if (Vector2.Distance(lastPoint, Vector2.zero) == 0)
				{
						lastPoint = scaledPosition;
				}

				_paintManager.PaintObject.DrawLine(lastPoint, scaledPosition);
				// _paintManager.PaintObject.DrawPoint(scaledPosition, 30);

				lastPoint = scaledPosition;
		}

		/// <summary>
		/// When the mouse is released, the last point is set to zero
		/// </summary>
		[PunRPC]
		private void MouseUpRpc()
		{
				lastPoint = Vector2.zero;
		}

		/// <summary>
		/// This function is called by the PC Player when the user clicks the "Reset" button. It clears the
		/// texture and renders the new texture
		/// </summary>
		[PunRPC]
		private void ResetDrawingRpc()
		{
				Debug.Log("ResetDrawingRpc");
				//_paintManager.PaintObject.DoDispose();
				_paintManager.PaintObject.ClearTexture();
				_paintManager.Render();
		}
}
