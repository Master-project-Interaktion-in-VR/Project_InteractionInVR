using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseTracker : MonoBehaviour
{
    [Serializable]
    private enum Handedness { Left, Right }

    [SerializeField]
    private Handedness handedness;

    private Transform _hand;

    private void Awake()
    {
#if !UNITY_EDITOR || !ABC
        if (!Application.isMobilePlatform) // only track hands with VR glasses
            return;
#endif

        if (handedness == Handedness.Left)
            _hand = GameObject.Find("LeftHandAnchor").transform;
        else
            _hand = GameObject.Find("RightHandAnchor").transform;
    }

    private void Update()
    {
#if !UNITY_EDITOR || !ABC
        if (!Application.isMobilePlatform)
            return;
#endif
        transform.position = _hand.position;
        transform.rotation = _hand.rotation;
    }
}
