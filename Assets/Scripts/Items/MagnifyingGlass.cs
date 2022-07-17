using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlass : MonoBehaviour
{
    [SerializeField]
    private Transform lensCamera;

    private Transform _vrCamera;
    private bool _isBackwards;

    private void Awake()
    {
        _vrCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        Vector3 forward = Vector3.forward;
        if (_isBackwards)
            forward = Vector3.back;

        Vector3 lensForward = lensCamera.TransformDirection(forward);
        Vector3 vrForward = _vrCamera.TransformDirection(Vector3.forward);

        Debug.DrawRay(_vrCamera.position, vrForward);
        Debug.DrawRay(lensCamera.position, lensForward);

        float facing = Vector3.Dot(vrForward.normalized, lensForward.normalized);

        Debug.Log(facing);

        if (!_isBackwards && facing < 0)
        {
            // was turned backwards
            lensCamera.Rotate(new Vector3(180, 0, 180), Space.Self);
            _isBackwards = true;
        }
        else if (_isBackwards && facing > 0)
        {
            lensCamera.Rotate(new Vector3(-180, 0, -180), Space.Self);
            _isBackwards = false;
        }
    }
}
