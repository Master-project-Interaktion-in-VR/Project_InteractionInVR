using System.Collections;
using System.Collections.Generic;
using OVR;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BuildManager;

public class PhotonSceneLoader : MonoBehaviourPun
{
    [SerializeField]
    private AssemblySuccessUnityEvent asue = new AssemblySuccessUnityEvent();

    [SerializeField] 
    private FadeScreen fadeScreen;


    private Scene scene;
    PhotonView _photonView;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        _photonView = PhotonView.Get(this);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                if (scene.name == "StartScene")
                {
                    GameObject.Find("Launcher").GetComponent<Launcher>().StopVideoPcShortcut();
                }
                if (scene.name == "EnvironmentGameScene")
                {
                    Debug.Log("EnvironmentScene skipped");
                    //SceneManager.LoadScene("AssemblyScene");
                    StartCoroutine(LoadAssemblyScene());
                }
                else if (scene.name == "AssemblyScene")
                {
                    if (GameObject.Find("Puzzle") == null)
                    {
                        Debug.Log("Skip assembly");
                        asue.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("Skip puzzle");
                        StartCoroutine(LoadEndScene());
                    }
                }
            }
        }
    }

    [PunRPC]
    public void LoadScene(string GameScene_name)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(GameScene_name);
        }
    }

    private IEnumerator LoadAssemblyScene()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        
        _photonView.RPC("LoadScene", RpcTarget.All, "AssemblyScene");
    }
    
    private IEnumerator LoadEndScene()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        
        _photonView.RPC("LoadScene", RpcTarget.All, "EndScene");
    }
}
