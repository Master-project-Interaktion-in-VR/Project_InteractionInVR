using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsController : MonoBehaviour
{
    public static Queue<GameObject> boundCollisions;
    static BuildManager manager;
    static List<GameObject> toRemove;

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

            boundCollisions.Clear();
        }
    }

    IEnumerator repositionObjects()
    {
        yield return new WaitForSeconds(0.1f);
        if (toRemove.Count > 0)
        {
            foreach (GameObject obj in toRemove)
            {
                Vector3 pos = GameObject.Find("Table/SpawnPoint").transform.position;
                pos.y += 0.5f;
                obj.GetComponent<Rigidbody>().useGravity = false;
                //obj.GetComponent<Rigidbody>().isKinematic = true;

                GameObject firstChild = null;
                foreach (Transform child in obj.transform)
                {
                    if (child.tag == "InitialObject")
                    {
                        firstChild = child.gameObject;
                        break;
                    }
                }
                firstChild.transform.parent = null;
                obj.transform.parent = firstChild.transform;

                firstChild.transform.localPosition = pos;

                obj.transform.parent = null;
                firstChild.transform.parent = obj.transform;

                obj.GetComponent<Rigidbody>().useGravity = true;
                //obj.GetComponent<Rigidbody>().isKinematic = false;

                Debug.Log("respawn " + obj.name + " to position " + pos);
            }
            toRemove.Clear();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "InitialObject")
        {
            Debug.Log("BoundsController: " + other.name + " left the bounds");
            boundCollisions.Enqueue(other.gameObject);
        }
    }
}
