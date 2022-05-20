using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushLine : MonoBehaviour
{
    private PhotonView _photonView;
    private LineRenderer _lineRenderer;

    private Transform _referenceTransform;

    //private Vector2 _lastPosition;
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
            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    _photonView.RPC("SetStart", RpcTarget.All, mousePosition);
            //}
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //if (mousePosition != _lastPosition)
                //{
                //    _lastPosition = mousePosition;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20, (1 << LayerMask.NameToLayer("Drawable") | 1 << LayerMask.NameToLayer("Default"))))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default")) 
                        return;

                    Vector3 d = hit.point - ray.direction * 0.01f;
                    Vector3 localPoint = hit.transform.InverseTransformPoint(d);
                    _photonView.RPC("AddAPoint", RpcTarget.All, localPoint);
                    Debug.Log(localPoint);
                }
                //}
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
    private void Init(string parentName)
    {
        GameObject parent = GameObject.Find(parentName);
        transform.SetParent(parent.transform);
        _referenceTransform = parent.transform;
    }

    [PunRPC]
    private void SetStart(Vector3 localPoint)
    {
        Vector3 worldPoint = _referenceTransform.TransformPoint(localPoint);
        _lineRenderer.SetPosition(0, worldPoint);
        _lineRenderer.SetPosition(1, worldPoint);
    }

    [PunRPC]
    private void AddAPoint(Vector3 localPoint)
    {
        Vector3 worldPoint = _referenceTransform.TransformPoint(localPoint);
        _lineRenderer.positionCount++;
        int positionIndex = _lineRenderer.positionCount - 1;
        _lineRenderer.SetPosition(positionIndex, worldPoint);
    }

    [PunRPC]
    private void FinishStroke()
    {
        _drawing = false;
        _lineRenderer = null;
    }
}
