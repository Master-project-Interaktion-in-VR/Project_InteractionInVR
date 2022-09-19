using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save items from occasionally falling through or being pushed through ground by the player.
/// The floor has a custom mesh which is why it cannot be made thicker.
/// </summary>
public class FloorSaver : MonoBehaviour
{
    private float _lastY;

    private void Update()
    {
        bool hitSomething = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5, 1 << LayerMask.NameToLayer("Drawable"));
        if (!hitSomething)
        {
            // item is below the floor
            //Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitUp, 20, 1 << LayerMask.NameToLayer("Drawable"));
            //transform.position = hitUp.point + new Vector3(0, 0.1f, 0);
            transform.position = new Vector3(transform.position.x, _lastY + 0.03f, transform.position.z);
        }
        else
        {
            // item is above the floor, save last y coordinate
            _lastY = hit.point.y;
        }
    }
}
