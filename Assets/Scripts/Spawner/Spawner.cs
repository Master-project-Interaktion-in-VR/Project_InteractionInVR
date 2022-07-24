using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> toSpawn;

    [SerializeField]
    private Transform saveSpawnPoint;

    [SerializeField]
    private GameObject antennaOriginsParent;

    [SerializeField]
    private GameObject originPrefab;


    private bool _testRoomSpawnDone;

    private void OnEnable()
    {
        if (EnvironmentGameSceneManager.IsRunningOnGlasses() && !EnvironmentGameSceneManager.RUNNING_IN_TEST_ROOM) // if in test room, spawn must be in Update
        {
            // spawn on VR platform
            // only spawn items once, VR player must be owner in order to synchronize position (grabbing) and destroy it
            List<Vector3> spawnPoints = GetSpawnPoints();
            SpawnAll(spawnPoints);
        }
    }

    /// <summary>
    /// Only for spawning in test room because then we have to wait with the Photon calls until we have joined the room.
    /// </summary>
    private void Update()
    {
        if (!EnvironmentGameSceneManager.RUNNING_IN_TEST_ROOM || !PhotonNetwork.InRoom || _testRoomSpawnDone)
            return;
        _testRoomSpawnDone = true;
        List<Vector3> spawnPoints = GetSpawnPoints();
        SpawnAll(spawnPoints);
    }

    private List<Vector3> GetSpawnPoints()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child.position);
        }
        return spawnPoints;
    }

    private void SpawnAll(List<Vector3> spawnPoints)
    {
        List<GameObject> temp = new List<GameObject>(toSpawn);
        SpawnItem(temp[0], saveSpawnPoint.position);
        temp.Remove(temp[0]);
        bool succ = spawnPoints.Remove(saveSpawnPoint.position);
        //Debug.Log("deleted: " + succ);

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
        //spawned.GetComponent<NetworkHelper>().SetParent(antennaParent.transform); can't set parent because of XRI grabbing
        //spawned.GetComponent<Item>().SetOrigin(); // set origin, after parent (and therefore absolute position) was changed

        // origin collider
        Physics.Raycast(position, Vector3.down, out RaycastHit hit, 5, 1 << LayerMask.NameToLayer("Drawable"));
        GameObject origin = PhotonNetwork.Instantiate("EnvironmentAntennaPieces/" + originPrefab.name, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);
        origin.GetComponent<NetworkHelper>().SetParent(antennaOriginsParent.transform);
    }
}
