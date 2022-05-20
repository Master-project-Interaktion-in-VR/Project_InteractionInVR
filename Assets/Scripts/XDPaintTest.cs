using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDPaint;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.PaintObject.Base;

public class XDPaintTest : MonoBehaviour
{

    public PaintManager _pm;
    public PaintManager _screen;
    //private Renderer _plane;

    //private Texture2D _screenTex;
    private Vector2 _previousPoint;

    void Start()
    {
        //_screen = GameObject.Find("Screen").GetComponent<Renderer>();
        //_plane = GameObject.Find("Plane").GetComponent<Renderer>();
        //_screenTex = (Texture2D)_screen.material.mainTexture;
        //_screen.material.mainTexture = _screenTex;
        _pm.OnInitialized += OnPaintManagerInitialized;
        //_pm.ToolsManager.CurrentTool.OnPaint;

    }

    private void OnPaintManagerInitialized(PaintManager paintManager)
    {
        _pm = paintManager;
        _pm.PaintObject.OnPaintDataHandler += OnPaint;
        _pm.PaintObject.OnDrawLineHandler += Abc;
    }

    private void Abc(BasePaintObject sender, Vector2 lineStartPosition, Vector2 lineEndPosition, float lineStartPressure, float lineEndPressure)
    {
        _screen.PaintObject.DrawLine(lineStartPosition, lineEndPosition);
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
        if (_pm.Initialized)
        {
            SendBytes();
        }
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
