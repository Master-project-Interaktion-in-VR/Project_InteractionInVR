using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBrush : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject brushLine;


    private PhotonView _photonView;

    

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            int me = PhotonNetwork.LocalPlayer.ActorNumber;
            _photonView.RPC("SetName", RpcTarget.All, me);
        }
    }

    private void Update()
    {
        Draw();
    }


    private void Draw()
    {
        if (_photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                CreateBrush();
            }
        }
        else
        {

        }
        
    }

    private void CreateBrush()
    {
        //GameObject brushInstance = Instantiate(brush);
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 5);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, 1 << LayerMask.NameToLayer("Drawable")))
        {
            GameObject drawingOn = hit.transform.gameObject;
            float width = drawingOn.GetComponent<MeshRenderer>().bounds.size.x * drawingOn.transform.localScale.x;
            float height = drawingOn.GetComponent<MeshRenderer>().bounds.size.y * drawingOn.transform.localScale.y;
            //_photonView.RPC("SetReferenceTransform", RpcTarget.All, hit.transform.position); // only send to mobile, if there are more than 2 players
            SetReferenceTransform(hit.transform.position);

            GameObject line = PhotonNetwork.Instantiate(brushLine.name, new Vector3(0, 0, 0), Quaternion.identity);
            line.GetComponent<PhotonView>().RPC("Init", RpcTarget.All, name);
            Vector3 d = hit.point - ray.direction * 0.01f;
            Vector3 localPosition = hit.transform.InverseTransformPoint(d);
            line.GetComponent<PhotonView>().RPC("SetStart", RpcTarget.All, localPosition);
        }

        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z + 10));
        

        // TODO: REF TRANSFORM IS SET IF WE ARE DRAWING; BUT HOW DO WE SET IT IF WE ARE NOT DRAWING??????????????
    }

    [PunRPC]
    private void SetReferenceTransform(Vector3 position)
    {
        transform.position = position;
    }

    [PunRPC]
    private void SetName(int i)
    {
        name = "Player Brush " + i;
    }


}