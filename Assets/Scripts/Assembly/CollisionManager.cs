using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static List<(string, string)> correct_AssembledBuildPoints;

    private void Start()
    {
        correct_AssembledBuildPoints = new List<(string, string)>();

        correct_AssembledBuildPoints.Add(("halterungsstange_BuildCollider2", "schüssel_BuildCollider1")); //
        correct_AssembledBuildPoints.Add(("schüssel_BuildCollider1", "halterungsstange_BuildCollider2")); //

        correct_AssembledBuildPoints.Add(("bodenteil_BuildCollider1", "mittelstange_BuildCollider1")); //
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider1", "bodenteil_BuildCollider1")); // 

        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider2", "seitenteil_unten_BuildCollider1")); //
        correct_AssembledBuildPoints.Add(("seitenteil_unten_BuildCollider1", "mittelstange_BuildCollider2")); //

        correct_AssembledBuildPoints.Add(("halterungsstange_BuildCollider1", "mittelstange_BuildCollider3")); //
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider3", "halterungsstange_BuildCollider1")); //

        correct_AssembledBuildPoints.Add(("seitenteil_oben_BuildCollider1", "mittelstange_BuildCollider4")); //
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider4", "seitenteil_oben_BuildCollider1")); //
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!Application.isMobilePlatform)
            return;

        GameObject colObject = col.gameObject;
        if (colObject.tag == "BuildObject")
        {
            BuildManager.CollisionEvent collision = new BuildManager.CollisionEvent();

            //collision.position = col.GetContact(0).point;
            collision.object1 = gameObject; // the object who called the collision
            collision.object2 = colObject; // the object it collided with

            // add this collision to the list
            BuildManager.collisions.Enqueue(collision);

            Debug.Log(this.name + " collided with " + colObject.name + " on point: " + collision.position.ToString());
        }
    }
}
