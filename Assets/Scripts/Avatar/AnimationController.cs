using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AnimationController : MonoBehaviour
{
    private InputDevice _head;


    private Animator _animator;

    private Vector3 _previousHeadPosition;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void TryInitialize()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach (InputDevice device in allDevices)
        {
            if (device.name.Contains("Quest"))
            {
                _head = device;
            }
        }
    }

    private void Update()
    {
        if (!_head.isValid)
        {
            // devices are not instantly available
            TryInitialize();
            return;
        }

        _head.TryGetFeatureValue(CommonUsages.centerEyePosition, out Vector3 headPosition);
        Quaternion localCoordinateSystem = transform.rotation; // use the rotation of the avatar

        Vector3 walkingDirection = _previousHeadPosition - headPosition;
        Vector3 lookingDirection = localCoordinateSystem * Vector3.forward;

        float angle = Vector3.Angle(walkingDirection, lookingDirection);

        if (walkingDirection.magnitude > 0.01)
        {
            if (angle < 90)
            {
                // forwards
                _animator.SetBool("isWalking", true);
                _animator.SetFloat("animationSpeed", 1);
            }
            else if (angle > 90)
            {
                // backwards
                _animator.SetBool("isWalking", true);
                _animator.SetFloat("animationSpeed", -1);
            }
        }
        else
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("animationSpeed", 0);
        }

        _previousHeadPosition = headPosition;
    }
}
