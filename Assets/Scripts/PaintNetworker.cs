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
        _paintManager.PaintObject.OnMouseUpHandler += OnEndLine;
    }

    private void OnMouseDown(BasePaintObject sender, Vector2 uv, Vector2 paintPosition, float pressure)
    {
        if (Time.time - _startTime < delay)
        {
            Debug.Log("not now");
            return;
        }
        Debug.Log("send rpc");
        if (Vector2.Distance(_previousPoint, Vector2.zero) != 0) // previous point is still empty
            _photonView.RPC("DrawLineRpc", RpcTarget.Others, _previousPoint, paintPosition);
        _previousPoint = paintPosition;
        _startTime = Time.time;
    }

    private void OnEndLine(BasePaintObject sender, bool inBounds)
    {
        _previousPoint = Vector2.zero;
    }

    private void OnLineDrawn(BasePaintObject sender, Vector2 lineStartPosition, Vector2 lineEndPosition, float lineStartPressure, float lineEndPressure)
    {
        //_screen.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }

    [PunRPC]
    private void DrawLineRpc(Vector2 lineStartPosition, Vector2 lineEndPosition)
    {
        _paintManager.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
    }

    private void OnPaint(BasePaintObject sender, Vector2 paintPosition, float brushSize, float pressure, Color brushColor, PaintTool tool)
    {
        ////_screen.ToolsManager.CurrentTool.OnPaint(sender, paintPosition, pressure);
        //if (_previousPoint != null)
        //    _screen.PaintObject.DrawLine(_previousPoint, paintPosition);
        //_previousPoint = paintPosition;
    }

    private void FixedUpdate()
    {
        //if (_paintManager.Initialized)
        //{
        //    SendBytes();
        //}
    }

    //public byte[] GetRenderTexturePixels(RenderTexture tex)
    //{
    //    RenderTexture.active = tex;
    //    Texture2D tempTex = new Texture2D(tex.width, tex.height);
    //    tempTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
    //    tempTex.Apply();
    //    //return tempTex.GetPixels();
    //    byte[] bytes = tempTex.GetRawTextureData();
    //    Destroy(tempTex);
    //    return bytes;
    //}

    private void SendBytes()
    {
        //Texture2D resTex = _pm.GetResultTexture();
        //RenderTexture resTex = _pm.GetPaintTexture();
        //byte[] bytes = GetRenderTexturePixels(resTex);



        //byte[] bytes = resTex.GetRawTextureData();

        //Texture2D newTex = new Texture2D(resTex.width, resTex.height, TextureFormat.ARGB32, false);
        //newTex.LoadRawTextureData(bytes);
        //newTex.Apply();
        //_screen.material.mainTexture = newTex;

        //RenderTexture tex = _pm.GetPaintTexture();
        //RenderTexture.active = tex;
        //Texture2D tempTex = new Texture2D(tex.width, tex.height);
        //tempTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        //tempTex.Apply();
        //_screen.material.mainTexture = tempTex;


        //byte[] bytes = tex.GetRawTextureData();
        //Debug.Log(bytes.Length);
        //Texture2D screen = (Texture2D)_screen.material.mainTexture;
        //screen.LoadRawTextureData(bytes);
        //screen.Apply();

        //Color[] img = GetRenderTexturePixels(tex);


        //byte[] bytes = tex.EncodeToPNG();
        //bytes = bytes.Compress();
        //Debug.Log(bytes.Length);
        //if (bytes.Length <= 427)
        //    return;
        //Texture2D tex2d = (Texture2D)_screen.material.mainTexture;
        //bytes = bytes.Decompress();
        //tex2d.LoadRawTextureData(bytes);
        //tex2d.Apply();


        //Texture2D receivedTex = new Texture2D(tex.width, tex.height);
        //receivedTex.SetPixels(img);
        //receivedTex.Apply();
        //_screen.material.mainTexture = receivedTex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
