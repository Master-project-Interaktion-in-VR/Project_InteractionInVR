using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It's a class that checks if two objects collide with each other and if they do, it adds them to a list 
/// it also holds a list of the correct assembled objects
/// </summary>
public class CollisionManager : MonoBehaviour
{
    // It's a list of tuples that holds the correct assembled objects. 
    public static List<(string, string)> correct_AssembledBuildPoints;

    private void Start()
    {
        correct_AssembledBuildPoints = new List<(string, string)>();

        correct_AssembledBuildPoints.Add(("halterungsstange_BuildCollider2", "sch�ssel_BuildCollider1")); 
        correct_AssembledBuildPoints.Add(("sch�ssel_BuildCollider1", "halterungsstange_BuildCollider2")); 

        correct_AssembledBuildPoints.Add(("bodenteil_BuildCollider1", "mittelstange_BuildCollider1")); 
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider1", "bodenteil_BuildCollider1")); 

        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider2", "seitenteil_unten_BuildCollider1")); 
        correct_AssembledBuildPoints.Add(("seitenteil_unten_BuildCollider1", "mittelstange_BuildCollider2")); 

        correct_AssembledBuildPoints.Add(("halterungsstange_BuildCollider1", "mittelstange_BuildCollider3")); 
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider3", "halterungsstange_BuildCollider1")); 

        correct_AssembledBuildPoints.Add(("seitenteil_oben_BuildCollider1", "mittelstange_BuildCollider4")); 
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider4", "seitenteil_oben_BuildCollider1")); 
    }

    /// <summary>
    /// When a collision occurs, add the collision to a list of collisions
    /// </summary>
    /// <param name="Collider">The collider that was hit.</param>
    private void OnTriggerEnter(Collider col)
    {
        if (!Application.isMobilePlatform)
            return;

        GameObject colObject = col.gameObject;
        // only add the collision if it is a build point
        if (colObject.tag == "BuildObject")
        {
            BuildManager.CollisionEvent collision = new BuildManager.CollisionEvent();

            // the object who called the collision
            collision.object1 = gameObject; 
            // the object it collided with
            collision.object2 = colObject; 

            // add this collision to the list
            BuildManager.collisions.Enqueue(collision);

            //Debug.Log(this.name + " collided with " + colObject.name + " on point: " + collision.position.ToString());
        }
    }
}
