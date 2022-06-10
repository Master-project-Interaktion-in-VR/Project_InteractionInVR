using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVisuals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonHovered(Button button)
    {
        Debug.Log("HOVER");
        button.gameObject.GetComponent<Image>().color = button.colors.highlightedColor;
    }
    public void OnButtonUnhovered(Button button)
    {
        Debug.Log("UNHOVER");
        button.gameObject.GetComponent<Image>().color = button.colors.normalColor;
    }
}
