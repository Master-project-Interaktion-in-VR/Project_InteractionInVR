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
        transform.rotation = vrHead.rotation; // CONTINUE
    }
}
