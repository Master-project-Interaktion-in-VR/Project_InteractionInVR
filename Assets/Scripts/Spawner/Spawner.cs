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

    [SerializeField]
    private GameObject originPrefab;


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
        SpawnItem(temp[0], saveSpawnPoint.position);
        temp.Remove(temp[0]);
        bool succ = spawnPoints.Remove(saveSpawnPoint.position);
        Debug.Log("deleted: " + succ);

        foreach (GameObject item in temp)
        {
            int random = Random.Range(0, spawnPoints.Count);
            SpawnItem(item, spawnPoints[random]);
            spawnPoints.Remove(spawnPoints[random]);
        }
    }

    private void SpawnItem(GameObject item, Vector3 position)
    {
        GameObject spawned = Instantiate(item, position, Quaternion.identity);
        spawned.transform.SetParent(antennaParent.transform);

        // origin
        Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5, 1 << LayerMask.NameToLayer("Drawable"));
        GameObject origin = Instantiate(originPrefab, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);
        origin.transform.SetParent(antennaParent.transform);
    }
}
