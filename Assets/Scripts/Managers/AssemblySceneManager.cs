using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class AssemblySceneManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> deactivateObjects;


    // TODO deactivate objects for PC
    private void Awake()
    {
#if !UNITY_EDITOR || !ABC
        if (!Application.isMobilePlatform)
        {
            foreach (GameObject obj in deactivateObjects)
            {
                obj.SetActive(false);
            }
            GameObject.Find("AvatarVrRigForMrtk").transform.Find("Head").GetComponent<TrackedPoseDriver>().enabled = false;
        }
#endif
        //if (Application.isMobilePlatform)
        //    StartCoroutine(Cor());
    }

    private void Update()
    {
    }

    private IEnumerator Cor()
    {
        yield return new WaitForSeconds(15);
        PhotonNetwork.Instantiate("mittelstange", GameObject.Find("AntennaPieces").transform.position, Quaternion.identity);
        StartCoroutine(Cor());
    }
}
