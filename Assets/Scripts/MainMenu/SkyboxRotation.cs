using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    [SerializeField]
    private float xSpeed;

    [SerializeField]
    private float ySpeed;

    void Update()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = rotation.x + Time.deltaTime * xSpeed;
        rotation.y = rotation.y + Time.deltaTime * ySpeed;
        transform.rotation = Quaternion.identity * Quaternion.Euler(rotation);
    }
}
