using UnityEngine;

/// <summary>
/// Set up endscene for PC or VR.
/// </summary>
public class Endscene : MonoBehaviour
{
    [SerializeField]
    private GameObject rig;

    [SerializeField]
    private GameObject pcCamera;

    private void Start()
    {
        if (!Application.isMobilePlatform)
        {
            rig.SetActive(false);
            pcCamera.SetActive(true);
        }
    }
}
