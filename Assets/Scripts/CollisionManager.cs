using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static List<(string, string)> correct_AssembledBuildPoints;

    private void Start()
    {
        correct_AssembledBuildPoints = new List<(string, string)>();

        correct_AssembledBuildPoints.Add(("halterungsstange_2_BuildCollider2", "schüssel_2_BuildCollider1"));
        correct_AssembledBuildPoints.Add(("schüssel_2_BuildCollider1", "halterungsstange_2_BuildCollider2"));

        correct_AssembledBuildPoints.Add(("bodenteil_BuildCollider1", "mittelstange_BuildCollider1"));
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider1", "bodenteil_BuildCollider1"));

        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider2", "seitenteil_BuildCollider1"));
        correct_AssembledBuildPoints.Add(("seitenteil_BuildCollider1", "mittelstange_BuildCollider2"));

        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider5", "zwischenhalterung_BuildCollider1"));
        correct_AssembledBuildPoints.Add(("zwischenhalterung_BuildCollider1", "mittelstange_BuildCollider5"));

        correct_AssembledBuildPoints.Add(("halterungsstange_1_BuildCollider2", "schüssel_1_BuildCollider1"));
        correct_AssembledBuildPoints.Add(("schüssel_1_BuildCollider1", "halterungsstange_1_BuildCollider2"));

        correct_AssembledBuildPoints.Add(("halterungsstange_2_BuildCollider1", "mittelstange_BuildCollider3"));
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider3", "halterungsstange_2_BuildCollider1"));

        correct_AssembledBuildPoints.Add(("halterungsstange_1_BuildCollider1", "mittelstange_BuildCollider4"));
        correct_AssembledBuildPoints.Add(("mittelstange_BuildCollider4", "halterungsstange_1_BuildCollider1"));
    }

    private void OnTriggerEnter(Collider col)
    {
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
