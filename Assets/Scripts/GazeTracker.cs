using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTracker : MonoBehaviour
{
    [SerializeField]
    private Transform centerEyeAnchor;

    [SerializeField]
    private float slerpSeconds;

    [SerializeField]
    private int toleranceDegrees;

    private float _startTime;
    private bool _slerping;
    private float _diff;
    private Vector3 _from;
    private Vector3 _targetRotation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float targetY = centerEyeAnchor.rotation.eulerAngles.y;
        if (targetY < 0)
            targetY += 360;

        float distance = Mathf.Abs(targetY - transform.rotation.eulerAngles.y);
        if (distance > 360 * 0.5f)
        {
            distance = 360 - distance; // shortest distance
        }


        if (distance > toleranceDegrees && !_slerping)
        {
            _startTime = Time.time;
            _slerping = true;
        }
        if (distance < 10 && _slerping)
        {
            _slerping = false;
        }
        if (_slerping)
        {

            float fracComplete = (Time.time - _startTime) / slerpSeconds;
            float nextY = Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetY, fracComplete);

            Vector3 nextRotation = Vector3.zero;
            nextRotation.y = nextY;
            transform.rotation = Quaternion.Euler(nextRotation);
        }
    }
}
