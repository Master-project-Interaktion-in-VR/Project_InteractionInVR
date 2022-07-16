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



    [PunRPC]
    private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }

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

    [PunRPC]
    private void MouseUpRpc()
    {
        lastPoint = Vector2.zero;
    }

    [PunRPC]
    private void ResetDrawingRpc()
    {
        Debug.Log("ResetDrawingRpc");
        //_paintManager.PaintObject.DoDispose();
        _paintManager.PaintObject.ClearTexture();
        _paintManager.Render();
    }
}
