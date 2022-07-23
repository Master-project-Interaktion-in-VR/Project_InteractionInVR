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
        if (AssemblySceneManager.IsRunningOnGlasses())
        { 
            // find runtime MRTK hands and track them
            if (handedness == Handedness.Left)
                _hand = GameObject.Find("LeftHandAnchor").transform;
            else
                _hand = GameObject.Find("RightHandAnchor").transform;
        }
    }

    private void Update()
    {
        if (AssemblySceneManager.IsRunningOnGlasses())
        {
            transform.position = _hand.position;
            transform.rotation = _hand.rotation;
        }
    }
}
