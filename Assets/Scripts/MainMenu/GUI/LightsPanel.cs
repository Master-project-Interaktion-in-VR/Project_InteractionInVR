using UnityEngine;
using System.Linq;

/// <summary>
/// VR menu lights panel with lights that can turn red or green.
/// </summary>
public class LightsPanel : MonoBehaviour
{
    [SerializeField]
    private Sprite greenLightSprite;

    [SerializeField]
    private Sprite redLightSprite;

    [Space(10)]

    [SerializeField]
    private GUIConstants.Light[] lights;


    public void SetRed(GUIConstants.IndicatorLight light)
    {
        lights.Where(l => l.light == light).First().image.sprite = redLightSprite;
    }

    public void SetGreen(GUIConstants.IndicatorLight light)
    {
        lights.Where(l => l.light == light).First().image.sprite = greenLightSprite;
    }
}
