using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> toSpawn;

    [SerializeField]
    private Transform saveSpawnPoint;

    [SerializeField]
    private GameObject antennaParent;


    private void Awake()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child.position);
        }

        SpawnAll(spawnPoints);
    }

    private void SpawnAll(List<Vector3> spawnPoints)
    {
        List<GameObject> temp = new List<GameObject>(toSpawn);
        GameObject spawned = Instantiate(temp[0], saveSpawnPoint.position, Quaternion.identity);
        spawned.transform.SetParent(antennaParent.transform);
        temp.Remove(temp[0]);
        bool succ = spawnPoints.Remove(saveSpawnPoint.position);
        Debug.Log("deleted: " + succ);

        foreach (GameObject item in temp)
        {
            int random = Random.Range(0, spawnPoints.Count);
            spawned = Instantiate(item, spawnPoints[random], Quaternion.identity);
            spawned.transform.SetParent(antennaParent.transform);
            spawnPoints.Remove(spawnPoints[random]);
        }
    }
}
