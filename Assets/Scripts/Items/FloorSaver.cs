using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSaver : MonoBehaviour
{
    private float _lastY;


    void Update()
    {
        bool hitSomething = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2, 1 << LayerMask.NameToLayer("Drawable"));
        if (!hitSomething)
        {
            //Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitUp, 20, LayerMask.NameToLayer("Drawable"));
            //transform.position = hitUp.point + new Vector3(0, 0.1f, 0);
            transform.position = new Vector3(transform.position.x, _lastY + 0.03f, transform.position.z);
        }
        else
        {
            _lastY = hit.point.y;
        }
    }
}
