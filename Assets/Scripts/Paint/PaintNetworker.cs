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

public class PaintNetworker : MonoBehaviour
{

    [SerializeField]
    private float delay;


    private PaintManager _paintManager;
    private PhotonView _photonView;

    private Vector2 _previousPoint;
    private float _startTime;


    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _paintManager = GetComponent<PaintManager>();
        _paintManager.OnInitialized += OnPaintManagerInitialized;
    }

    private void OnPaintManagerInitialized(PaintManager paintManager)
    {
        //_paintManager = paintManager;
        //_paintManager.PaintObject.OnPaintDataHandler += OnPaint;
        _paintManager.PaintObject.OnMouseHandler += OnMouseDown;
        _paintManager.PaintObject.OnMouseUpHandler += OnMouseUp;
    }

    private void OnMouseDown(BasePaintObject sender, Vector2 uv, Vector2 paintPosition, float pressure)
    {
        if (Time.time - _startTime < delay)
        {
            //Debug.Log("not now");
            return;
        }
        //Debug.Log("send rpc");
        if (Vector2.Distance(_previousPoint, Vector2.zero) != 0) // previous point is still empty
            _photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, paintPosition);
        _previousPoint = paintPosition;
        _startTime = Time.time;
    }

    private void OnMouseUp(BasePaintObject sender, bool inBounds)
    {
        // end line, don't know the current draw pos
        //_photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, paintPosition);
        _previousPoint = Vector2.zero;
    }

    [PunRPC]
    private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }
}
