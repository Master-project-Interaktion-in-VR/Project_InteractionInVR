using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{

    public int selectedItem = 0;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        int previousSelectedItem = selectedItem;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedItem >= transform.childCount - 1)
                selectedItem = 0;
            else
                selectedItem++;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (selectedItem <= 0)
                selectedItem = transform.childCount - 1;
            else
                selectedItem--;
        }

        if(previousSelectedItem != selectedItem)
        {
            SelectItem();
        }
    }

    public void SelectItem()
    {
        int i = 0;
        foreach(Transform item in transform)
        {
            if (i == selectedItem)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
            i++;
        }
    }
}
