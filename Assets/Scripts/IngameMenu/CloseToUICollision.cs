using System;
using UnityEngine;

/// <summary>
/// Invoke events for finger sensitive menu buttons.
/// Colliders must be at the tip of the fingers.
/// </summary>
public class CloseToUICollision : MonoBehaviour
{
    public static event Action<GameObject> OnEnterUIArea = delegate { };
    public static event Action<GameObject> OnExitUIArea = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("InteractiveUI"))
        {
            OnEnterUIArea?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("InteractiveUI"))
            OnExitUIArea?.Invoke(other.gameObject);
    }
}
