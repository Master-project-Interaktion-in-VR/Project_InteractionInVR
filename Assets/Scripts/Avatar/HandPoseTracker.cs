using System;
using UnityEngine;

/// <summary>
/// Make placeholder hands track MRTK hand objects' positions and rotations.
/// MRTK hand objects are instantiated at runtime and thus cannot be used as a reference.
/// </summary>
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
