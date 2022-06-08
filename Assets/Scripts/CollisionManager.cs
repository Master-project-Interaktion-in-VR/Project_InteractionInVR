using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        GameObject colObject = col.gameObject;
        if (colObject.tag == "BuildObject")
        {
            BuildManager.CollisionEvent collision = new BuildManager.CollisionEvent();

            collision.position = col.GetContact(0).point;
            collision.object1 = gameObject; // the object who called the collision
            collision.object2 = colObject; // the object it collided with

            // add this collision to the list
            BuildManager.collisions.Enqueue(collision);
            
            Debug.Log(this.name + " collided with " + colObject.name + " on point: " + collision.position.ToString());
        }
    }

}
