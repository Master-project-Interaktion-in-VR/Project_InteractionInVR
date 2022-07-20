using Photon.Pun;
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
        if (!PhotonNetwork.IsMasterClient) // only spawn items once, no matter on which client
            return;

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
        GameObject spawned = PhotonNetwork.Instantiate("EnvironmentAntennaPieces/" + item.name, position, Quaternion.identity);
        spawned.GetComponent<NetworkHelper>().SetParent(antennaParent.transform);

        // origin
        Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5, 1 << LayerMask.NameToLayer("Drawable"));
        GameObject origin = PhotonNetwork.Instantiate(originPrefab.name, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);
        origin.GetComponent<NetworkHelper>().SetParent(antennaParent.transform);
    }
}
