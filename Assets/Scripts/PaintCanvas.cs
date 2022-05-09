using UnityEngine;

public class PaintCanvas : MonoBehaviour
{
    public static Texture2D Texture { get; private set; }

    public static byte[] GetAllTextureData()
    {
        return Texture.GetRawTextureData();
    }

    private void Start()
    {
        var texture = GetComponent<Renderer>().material.mainTexture;
        var scale = (Screen.height / 2.0) / Camera.main.orthographicSize;
        gameObject.transform.localScale = new Vector3((float)(texture.width / scale), (float)(texture.height / scale), gameObject.transform.localScale.z);

        PrepareTemporaryTexture();
    }

    private void PrepareTemporaryTexture()
    {
        Texture = (Texture2D)Instantiate(GetComponent<Renderer>().material.mainTexture);
        GetComponent<Renderer>().material.mainTexture = Texture;
    }

    internal static void SetAllTextureData(byte[] textureData)
    {
        Texture.LoadRawTextureData(textureData);
        Texture.Apply();
    }
}