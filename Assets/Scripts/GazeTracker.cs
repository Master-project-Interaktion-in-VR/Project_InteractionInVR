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
            int dir;
            if (Mathf.Abs((transform.rotation.eulerAngles.y + distance) % 360 - targetY) < 1) // ignore floating point error
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }
            Debug.Log(distance * dir);

            float fracComplete = (Time.time - _startTime) / slerpSeconds;
            Vector3 targetRotation = transform.rotation.eulerAngles;
            targetRotation.y = targetRotation.y + distance * dir;

            Vector3 nextRotation = Vector3.Slerp(transform.rotation.eulerAngles, targetRotation, fracComplete);
            if (nextRotation.y > 360)
                nextRotation.y -= 360;
            if (nextRotation.y < 0)
                nextRotation.y += 360;

            nextRotation.x = 0;
            nextRotation.z = 0;
            Debug.Log(nextRotation);
            transform.rotation = Quaternion.Euler(nextRotation);

            //Vector3 tempRotation = transform.rotation.eulerAngles;
            //if (tempRotation.y < 0)
            //{
            //    tempRotation.y += length;
            //    Debug.Log("here: " + tempRotation.y);
            //}
            //else if (tempRotation.y > length)
            //{
            //    tempRotation.y -= length;
            //    Debug.Log("here: " + tempRotation.y);
            //}

            //transform.rotation = Quaternion.Euler(tempRotation);
        }

        //float currentRotationY = transform.rotation.eulerAngles.y;
        //float targetRotationY = centerEyeAnchor.rotation.eulerAngles.y;
        //if (targetRotationY < 0)
        //    targetRotationY = 360 + targetRotationY;
        //float length = 360;

        //float distance = Mathf.Abs(currentRotationY - targetRotationY);
        //if (distance > length * 0.5f)
        //{
        //    distance = length - distance; // shortest distance
        //}
        ////print(distance);

        //if (distance > toleranceDegrees && !_slerping)
        //{
        //    _startTime = Time.time;
        //    _slerping = true;
        //}
        //if (distance < 10 && _slerping)
        //{
        //    _slerping = false;
        //}

        //if (_slerping)
        //{
        //    int dir;
        //    if (currentRotationY + distance == targetRotationY)
        //    {
        //        dir = 1;
        //    }
        //    else
        //    {
        //        dir = -1;
        //    }

        //    Vector3 to = transform.rotation.eulerAngles;
        //    to.y = currentRotationY + (distance * dir);
        //    float fracComplete = (Time.time - _startTime) / slerpSeconds;
        //    transform.rotation = Quaternion.Euler(Vector3.Slerp(transform.rotation.eulerAngles, to, fracComplete));
        //    Debug.Log("from: " + transform.rotation.eulerAngles + " to: " + to + " actual: " + transform.rotation.eulerAngles);

        //    Vector3 tempRotation = transform.rotation.eulerAngles;

        //    if (tempRotation.y < 0)
        //    {
        //        tempRotation.y += length;
        //        Debug.Log("here: " + tempRotation.y);
        //    }
        //    else if (tempRotation.y > length)
        //    {
        //        tempRotation.y -= length;
        //        Debug.Log("here: " + tempRotation.y);
        //    }

        //    transform.rotation = Quaternion.Euler(tempRotation);
        //}
    }
}
