using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LightsPanel : MonoBehaviour
{
    [SerializeField]
    private Sprite greenLightSprite;

    [SerializeField]
    private Sprite redLightSprite;

    [Space(10)]

    [SerializeField]
    private GUIConstants.Light[] lights;

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
        lights.Where(l => l.light == light).First().image.sprite = redLightSprite;
    }

    public void SetGreen(GUIConstants.IndicatorLight light)
    {
        lights.Where(l => l.light == light).First().image.sprite = greenLightSprite;
    }
}
