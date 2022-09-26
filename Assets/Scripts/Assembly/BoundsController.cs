using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It checks if an object has left the bounds of the table, and if so, it respawns the object to the spawn point
/// </summary>
public class BoundsController : MonoBehaviour
{
    private static BuildManager manager;
    
    private static Queue<GameObject> boundCollisions;

    private static List<GameObject> toRemove;

    private void Start()
    {
        boundCollisions = new Queue<GameObject>();
        manager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
    }

    private void Update()
    {
        if (boundCollisions.Count > 0)
        {
            toRemove = new List<GameObject>();
            /* Checking if the object that left the bounds is a child of the holding body. If so, it
            adds the holding body to the list of objects to be respawned. */
            foreach (GameObject obj in boundCollisions)
            {
                Transform parent = obj.transform.parent;
                if (parent.name == "AntennaPieces(Clone)")
                {
                    manager.Respawn_object(obj.name);
                }
                // if object is child of holdingBody
                else
                {
                    // check if parent is already in list
                    if (!toRemove.Contains(parent.gameObject))
                    {
                        toRemove.Add(parent.gameObject);
                    }
                }
            }
            StartCoroutine(repositionObjects());
            // Clearing the queue of objects that have left the bounds.
            boundCollisions.Clear();
        }
    }

    /// <summary>
    /// respawns the objects that have left the bounds on the table
    /// </summary>
    private IEnumerator repositionObjects()
    {
        yield return new WaitForSeconds(0.1f);
        if (toRemove.Count > 0)
        {
            foreach (GameObject obj in toRemove)
            {
                Vector3 pos = GameObject.Find("Table/SpawnPoint").transform.position;
                pos.y += 0.5f;
                // turn off gravity for reposisioning
                obj.GetComponent<Rigidbody>().useGravity = false;

                GameObject firstChild = null;
                foreach (Transform child in obj.transform)
                {
                    if (child.tag == "InitialObject")
                    {
                        firstChild = child.gameObject;
                        break;
                    }
                }
                // make child the parent
                firstChild.transform.parent = null;
                obj.transform.parent = firstChild.transform;

                // reposition object
                firstChild.transform.localPosition = pos;

                // make parent the parent again
                obj.transform.parent = null;
                firstChild.transform.parent = obj.transform;

                // turn gravity back on
                obj.GetComponent<Rigidbody>().useGravity = true;

                //Debug.Log("respawn " + obj.name + " to position " + pos);
            }
            toRemove.Clear();
        }
    }

    /// <summary>
    /// When an object leaves the bounds, add it to the queue
    /// </summary>
    /// <param name="Collider">The collider of the object that collided with the bounds.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "InitialObject")
        {
            Debug.Log("BoundsController: " + other.name + " left the bounds");
            boundCollisions.Enqueue(other.gameObject);
        }
    }
}
