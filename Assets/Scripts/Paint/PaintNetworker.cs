using Photon.Pun;
using UnityEngine;
using XDPaint;
using XDPaint.Core.PaintObject.Base;

/// <summary>
/// Handle networking of painting.
/// Do not call RPC every frame.
/// </summary>
public class PaintNetworker : MonoBehaviour
{
    [SerializeField]
    private float delay;


    private PaintManager _paintManager;
    private PhotonView _photonView;

    private Vector2 _previousPoint;
    private float _startTime;


    private void Awake()
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

    /// <summary>
    /// Callback for mouse drawing.
    /// Do not use name OnMouseDown because it interfers with a Unity intern 
    /// function and causes weird effects.
    /// </summary>
    private void OnMouseDownP(BasePaintObject sender, Vector2 uv, Vector2 paintPosition, float pressure)
    {
        if (Time.time - _startTime < delay)
            return;

        if (Vector2.Distance(_previousPoint, Vector2.zero) != 0) // previous point is not empty
            _photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, paintPosition);
        _previousPoint = paintPosition;
        _startTime = Time.time;
    }

    /// <summary>
    /// Callback for mouse drawing. Mouse button up.
    /// Do not use name OnMouseUp because it interfers with a Unity intern 
    /// function and causes weird effects.
    /// </summary>
    private void OnMouseUpP(BasePaintObject sender, bool inBounds)
    {
        // end line, don't know the current draw pos
        _photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, _previousPoint);
        _previousPoint = Vector2.zero;
    }

    /// <summary>
    /// RPC for drawing the same line for the other player.
    /// </summary>
    [PunRPC]
    private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }
}
