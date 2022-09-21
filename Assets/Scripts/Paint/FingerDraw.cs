using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDPaint;

/// <summary>
/// Enable drawing with the index finger on the drawing screen.
/// </summary>
public class FingerDraw : MonoBehaviour
{
    [SerializeField]
    private Vector2 sourceTextureDimensions;

    [SerializeField]
    private float networkSendDelay;

    [SerializeField]
    private Material screenOnMaterial;


    private PaintManager _paintManager;
    private Vector2 _previousPoint;
    private bool _paintManagerReady;


    private PhotonView _photonView;
    private float _startTime;
    private CustomHandInteractionPanZoom _customHandInteractionPanZoom;


    private void Awake()
    {
        _paintManager = GetComponent<PaintManager>();
        _paintManager.OnInitialized += OnPaintManagerInitialized;


        _photonView = GetComponent<PhotonView>();
        _customHandInteractionPanZoom = GetComponent<CustomHandInteractionPanZoom>();
        _customHandInteractionPanZoom.Enabled = false;
    }

    /// <summary>
    /// Callback for paint manager (XDPaint plugin).
    /// </summary>
    private void OnPaintManagerInitialized(PaintManager paintManager)
    {
        _paintManagerReady = true;
    }

    /// <summary>
    /// Assembly was successful. Enable drawing pad.
    /// </summary>
    public void OnAssemblySuccess(bool success)
    {
        _photonView.RPC("OnAssemblySuccessPcRpc", RpcTarget.Others, success);
        OnAssemblySuccessVrRpc(true);
    }

    /// <summary>
    /// Assembly was skipped. Enable drawing pad.
    /// </summary>
    public void OnAssemblySuccessPcShortcut(bool success)
    {
        _photonView.RPC("OnAssemblySuccessVrRpc", RpcTarget.Others, success);
        OnAssemblySuccessPcRpc(true);
    }

    /// <summary>
    /// RPC for enabling drawing on VR.
    /// </summary>
    [PunRPC]
    private void OnAssemblySuccessVrRpc(bool success)
    {
        // is on VR
        _customHandInteractionPanZoom.Enabled = true;
        GetComponent<MeshRenderer>().material = screenOnMaterial;
        _paintManager.enabled = true;
    }

    /// <summary>
    /// RPC for enabling drawing on PC.
    /// </summary>
    [PunRPC]
    private void OnAssemblySuccessPcRpc(bool success)
    {
        // is on PC
        GetComponent<MeshRenderer>().material = screenOnMaterial;
        _paintManager.enabled = true;
    }

    /// <summary>
    /// Callback for drawing.
    /// </summary>
    /// <param name="paintUv">The UV coordinates on the texture drawn on</param>
    public void OnFingerDrawing(Vector2 paintUv)
    {
        // only draw if paint manager is ready
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

    /// <summary>
    /// Callback for finger was lifted off the drawing pad.
    /// </summary>
    public void OnFingerUp(Vector2 paintUv)
    {
        _previousPoint = Vector2.zero;
    }

    /// <summary>
    /// RPC for drawing a line with XDPaint plugin.
    /// </summary>
    [PunRPC]
    private void DrawLine(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }

    /// <summary>
    /// Clear the drawing screen.
    /// </summary>
    public void ResetDrawing()
    {
        _photonView.RPC("ResetDrawingRpc", RpcTarget.All);
    }

    /// <summary>
    /// RPC for clearing the drawing screen.
    /// </summary>
    [PunRPC]
    private void ResetDrawingRpc()
    {
        Debug.Log("ResetDrawingRpc");
        _paintManager.PaintObject.ClearTexture();
        _paintManager.Render();
    }
}
