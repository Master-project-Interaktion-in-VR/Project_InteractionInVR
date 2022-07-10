using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image image;
    public Sprite newSprite;


    /* private void Update()
     {
         if (Input.GetKeyDown(KeyCode.DownArrow))
         {
             UIActive = !UIActive;
             Inventory.SetActive(UIActive);
         }
         if (UIActive)
         {
             Inventory.transform.position = Anchor.transform.position;
             Inventory.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);
         }
     }*/

    public void ImageChange()
    {
        image.sprite = newSprite;
    }
}