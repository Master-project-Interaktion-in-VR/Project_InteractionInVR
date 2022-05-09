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
        //if (_photonView.IsMine)
        //{

        //}
        //var data = PaintCanvas.GetAllTextureData();
        //var zippeddata = data.Compress();

        //RpcSendFullTexture(zippeddata);
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
        GameObject brushLine = PhotonNetwork.Instantiate("BrushLine", new Vector3(0, 0, 0), Quaternion.identity);
        brushLine.transform.SetParent(transform);
        //_lineRenderer = brushLine.GetComponent<LineRenderer>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z + 10));
        brushLine.GetComponent<PhotonView>().RPC("SetStart", RpcTarget.All, mousePosition);
    }






    public override void OnPlayerEnteredRoom(Player other)
    {
        // player entered room while game is already running
        if (PhotonNetwork.IsMasterClient)
        {
            var data = PaintCanvas.GetAllTextureData();
            var zippeddata = data.Compress();
            _photonView.RPC("RpcSendFullTexture", other, zippeddata);
        }
    }

    [PunRPC]
    private void RpcSendFullTexture(byte[] textureData)
    {
        PaintCanvas.SetAllTextureData(textureData.Decompress());
    }

    private void Update()
    {
        Draw();
        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        var pallet = hit.collider.GetComponent<PaintCanvas>();
        //        if (pallet != null)
        //        {
        //            Debug.Log(hit.textureCoord);
        //            Debug.Log(hit.point);

        //            Renderer rend = hit.transform.GetComponent<Renderer>();
        //            MeshCollider meshCollider = hit.collider as MeshCollider;

        //            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        //                return;

        //            Texture2D tex = rend.material.mainTexture as Texture2D;
        //            Vector2 pixelUV = hit.textureCoord;
        //            pixelUV.x *= tex.width;
        //            pixelUV.y *= tex.height;

        //            _photonView.RPC("RpcBrushAreaWithColor", RpcTarget.All, pixelUV);
        //            BrushAreaWithColor(pixelUV, new Color(255, 0, 0), 5);
        //        }
        //    }
        //}
    }

    [PunRPC]
    private void RpcBrushAreaWithColor(Vector2 pixelUV)
    {
        BrushAreaWithColor(pixelUV, new Color(255, 0, 0), 5);
    }

    private void BrushAreaWithColor(Vector2 pixelUV, Color color, int size)
    {
        for (int x = -size; x < size; x++)
        {
            for (int y = -size; y < size; y++)
            {
                PaintCanvas.Texture.SetPixel((int)pixelUV.x + x, (int)pixelUV.y + y, color);
            }
        }

        PaintCanvas.Texture.Apply();
    }
}