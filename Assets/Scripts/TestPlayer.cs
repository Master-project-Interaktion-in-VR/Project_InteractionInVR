using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // JUST A DEMO

    [SerializeField]
    private float movementSpeed;

    private PhotonView _photonView;

    private int _health;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (_photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _photonView.RPC("SetHealth", RpcTarget.All, Random.Range(1, 15));
            }
            if (Input.GetKey(KeyCode.W))
            {
                gameObject.transform.position = gameObject.transform.position + gameObject.transform.forward * Time.deltaTime * movementSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                gameObject.transform.position = gameObject.transform.position - gameObject.transform.forward * Time.deltaTime * movementSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.position = gameObject.transform.position + gameObject.transform.right * Time.deltaTime * movementSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.position = gameObject.transform.position - gameObject.transform.right * Time.deltaTime * movementSpeed;
            }
        }
    }

    [PunRPC]
    public void SetHealth(int health)
    {
        _health = health;
        Debug.Log("Set Health to " + health);
    }
}
