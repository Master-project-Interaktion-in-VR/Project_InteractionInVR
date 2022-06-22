using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVisuals : MonoBehaviour
{

    [SerializeField]
    private Color normal;

    [SerializeField]
    private Color hover;

    [SerializeField]
    private Color pressed;

    [SerializeField]
    private Color disabled;


    private List<Selectable> _disabledButtons;

    // Start is called before the first frame update
    void Start()
    {
        _disabledButtons = new List<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void DisableButton(Selectable button)
    {
        button.interactable = false;
        button.gameObject.GetComponent<Image>().color = disabled;
        button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = disabled;
        Color tempColor = button.transform.parent.GetComponent<Image>().color;
        tempColor.a = 0.3f;
        button.transform.parent.GetComponent<Image>().color = tempColor;
        _disabledButtons.Add(button);
    }
    public void EnableButton(Selectable button)
    {
        button.interactable = true;
        button.gameObject.GetComponent<Image>().color = normal;
        button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = normal;
        Color tempColor = button.transform.parent.GetComponent<Image>().color;
        tempColor.a = 1f;
        button.transform.parent.GetComponent<Image>().color = tempColor;
        _disabledButtons.Remove(button);
    }

    public void OnButtonHovered(Selectable button)
    {
        if (_disabledButtons.Contains(button))
            return;
        //Debug.Log("HOVER");
        button.gameObject.GetComponent<Image>().color = hover;
        button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = hover;
    }
    public void OnButtonUnhovered(Selectable button)
    {
        if (_disabledButtons.Contains(button))
            return;
        //Debug.Log("UNHOVER");
        button.gameObject.GetComponent<Image>().color = normal;
        button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = normal;
    }


    public void OnButtonDown(Selectable button)
    {
        if (_disabledButtons.Contains(button))
            return;
        //Debug.Log("PRESSED");
        button.gameObject.GetComponent<Image>().color = pressed;
        button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = pressed;
    }

    public void OnButtonUp(Selectable button)
    {
        if (_disabledButtons.Contains(button))
            return;
        //Debug.Log("UP");
        button.gameObject.GetComponent<Image>().color = normal;
        button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = normal;
    }

    //public void OnSliderDown(Image sliderImage)
    //{
    //    sliderImage.color = pressed;
    //}

    //public void OnSliderUp(Image sliderImage)
    //{
    //    sliderImage.color = normal;
    //}
}
