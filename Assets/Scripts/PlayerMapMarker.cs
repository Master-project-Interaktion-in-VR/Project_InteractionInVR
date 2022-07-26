using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapMarker : MonoBehaviour
{
    [SerializeField]
    private Transform vrHead;

    private void LateUpdate()
    {
        transform.position = vrHead.position;
        Vector3 currentRotation = Vector3.zero;
        currentRotation.y = vrHead.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
