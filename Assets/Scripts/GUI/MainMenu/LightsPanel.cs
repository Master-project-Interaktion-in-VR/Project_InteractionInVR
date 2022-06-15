using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightsPanel : MonoBehaviour
{
    [SerializeField]
    private Sprite greenLightSprite;

    [SerializeField]
    private Sprite redLightSprite;

    [Space(10)]

    [SerializeField]
    private Image coreLight;

    [SerializeField]
    private Image photonLight;

    [SerializeField]
    private Image airLockLight;

    [SerializeField]
    private Image oxygenLight;

    [SerializeField]
    private Image protectorsLight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRed(GUIConstants.IndicatorLight light)
    {
        switch (light)
        {
            case GUIConstants.IndicatorLight.CORE:
                coreLight.sprite = redLightSprite;
                break;
            case GUIConstants.IndicatorLight.PHOTON:
                photonLight.sprite = redLightSprite;
                break;
            case GUIConstants.IndicatorLight.AIR_LOCK:
                airLockLight.sprite = redLightSprite;
                break;
            case GUIConstants.IndicatorLight.OXYGEN:
                oxygenLight.sprite = redLightSprite;
                break;
            case GUIConstants.IndicatorLight.PROTECTORS:
                protectorsLight.sprite = redLightSprite;
                break;
        }
    }

    public void SetGreen(GUIConstants.IndicatorLight light)
    {
        switch (light)
        {
            case GUIConstants.IndicatorLight.CORE:
                coreLight.sprite = greenLightSprite;
                break;
            case GUIConstants.IndicatorLight.PHOTON:
                photonLight.sprite = greenLightSprite;
                break;
            case GUIConstants.IndicatorLight.AIR_LOCK:
                airLockLight.sprite = greenLightSprite;
                break;
            case GUIConstants.IndicatorLight.OXYGEN:
                oxygenLight.sprite = greenLightSprite;
                break;
            case GUIConstants.IndicatorLight.PROTECTORS:
                protectorsLight.sprite = greenLightSprite;
                break;
        }
    }
}
