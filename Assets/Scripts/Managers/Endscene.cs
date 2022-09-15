using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endscene : MonoBehaviour
{
    [SerializeField]
    private GameObject rig;

    [SerializeField]
    private GameObject pcCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isMobilePlatform)
        {
            rig.SetActive(false);
            pcCamera.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
