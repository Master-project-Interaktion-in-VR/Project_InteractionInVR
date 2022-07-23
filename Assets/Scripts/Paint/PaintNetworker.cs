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
        if (!Application.isMobilePlatform)
            return;
        _paintManager.PaintObject.OnMouseHandler += OnMouseDownP;
        _paintManager.PaintObject.OnMouseUpHandler += OnMouseUpP;
    }


    private void OnMouseDownP(BasePaintObject sender, Vector2 uv, Vector2 paintPosition, float pressure)
    {
        if (Time.time - _startTime < delay)
        {
            //Debug.Log("not now");
            return;
        }
        if (Vector2.Distance(_previousPoint, Vector2.zero) != 0) // previous point is not empty
            _photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, paintPosition);
        _previousPoint = paintPosition;
        _startTime = Time.time;
    }

    private void OnMouseUpP(BasePaintObject sender, bool inBounds)
    {
        // end line, don't know the current draw pos
        _photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, _previousPoint);
        _previousPoint = Vector2.zero;
    }

    [PunRPC]
    private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }
}
