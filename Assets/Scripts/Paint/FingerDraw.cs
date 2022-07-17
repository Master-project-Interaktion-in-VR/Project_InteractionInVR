using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDPaint;

public class FingerDraw : MonoBehaviour
{
    [SerializeField]
    private Vector2 sourceTextureDimensions;

    [SerializeField]
    private float networkSendDelay;


    private PaintManager _paintManager;
    private Vector2 _previousPoint;
    private bool _paintManagerReady;


    private PhotonView _photonView;
    private float _startTime;


    void Awake()
    {
        _paintManager = GetComponent<PaintManager>();
        _paintManager.OnInitialized += OnPaintManagerInitialized;


        _photonView = GetComponent<PhotonView>();
    }

    private void OnPaintManagerInitialized(PaintManager paintManager)
    {
        _paintManagerReady = true;
    }


    public void OnFingerDrawing(Vector2 paintUv)
    {
        if (!_paintManagerReady)
            return;
        Vector2 paintPosition = new Vector2(paintUv.x * sourceTextureDimensions.x, (1 - paintUv.y) * sourceTextureDimensions.y);

        if (Vector2.Distance(_previousPoint, Vector2.zero) != 0) // previous point is not empty
        {
            DrawLine(_previousPoint, paintPosition);

            if (Time.time - _startTime > networkSendDelay)
            {
                _photonView.RPC("DrawLine", RpcTarget.Others, _previousPoint, paintPosition);
            }
        }
        _previousPoint = paintPosition;
    }

    public void OnFingerUp(Vector2 paintUv)
    {
        //if (!_paintManagerReady)
        //    return;
        //Vector2 paintPosition = new Vector2(paintUv.x * sourceTextureDimensions.x, (1 - paintUv.y) * sourceTextureDimensions.y);
        //DrawLine(_previousPoint, paintPosition);
        _previousPoint = Vector2.zero;
    }

    [PunRPC]
    private void DrawLine(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }
}
