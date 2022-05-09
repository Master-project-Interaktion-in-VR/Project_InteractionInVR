using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushLine : MonoBehaviour
{
    private PhotonView _photonView;
    private LineRenderer _lineRenderer;

    private Vector2 _lastPosition;
    private bool _drawing;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _lineRenderer = GetComponent<LineRenderer>();
        _drawing = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (_drawing)
            Draw();
    }

    private void Draw()
    {
        if (_photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _photonView.RPC("SetStart", RpcTarget.All, mousePosition);
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (mousePosition != _lastPosition)
                {
                    //AddAPoint(mousePosition);
                    _photonView.RPC("AddAPoint", RpcTarget.All, mousePosition);
                    _lastPosition = mousePosition;
                }
            }
            else
            {
                _photonView.RPC("FinishStroke", RpcTarget.All);
            }
        }
        else
        {

        }
    }


    [PunRPC]
    private void SetStart(Vector2 position)
    {
        _lineRenderer.SetPosition(0, position);
        _lineRenderer.SetPosition(1, position);
    }

    [PunRPC]
    private void AddAPoint(Vector2 point)
    {
        _lineRenderer.positionCount++;
        int positionIndex = _lineRenderer.positionCount - 1;
        _lineRenderer.SetPosition(positionIndex, point);
    }

    [PunRPC]
    private void FinishStroke()
    {
        _drawing = false;
        _lineRenderer = null;
    }
}
