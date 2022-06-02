using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    GameObject cube;
    GameObject capsule;
    GameObject sphere;
    GameObject cylinder;

    // Start is called before the first frame update
    void Start()
    {
        cube = GameObject.Find("Cube_build");
        capsule = GameObject.Find("Capsule_build");
        sphere = GameObject.Find("Sphere_build");
        cylinder = GameObject.Find("Cylinder_build");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //void OnCollisionEnter(Collision other)
    //{
    //    // Print how many points are colliding with this transform
    //    Debug.Log("Points colliding: " + other.contacts.Length);

    //    // Print the normal of the first point in the collision.
    //    Debug.Log("Normal of the first point: " + other.contacts[0].normal);

    //    // Draw a different colored ray for every normal in the collision
    //    foreach (var item in other.contacts)
    //    {
    //        Debug.DrawRay(item.point, item.normal * 100, Color.red, 10f);
    //    }
    //}


    void OnCollisionEnter(Collision col)
    {
        GameObject colObject = col.gameObject;
        if (colObject.tag == "BuildObject")
        {
            Vector3 colNormal = col.GetContact(0).normal;
            Vector3 colPosition = col.GetContact(0).point;
            Debug.Log("collided with " + colObject.name + " on point: " + col.GetContact(0));
        }
    }
}
