using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class Calibration : MonoBehaviour
{

    private float _floorHeight;

    void Start()
    {
        Debug.Log("Calibration");

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void OnConfirmFloor()
    {
        Debug.Log(transform.position);
        _floorHeight = transform.position.y;
    }
}
