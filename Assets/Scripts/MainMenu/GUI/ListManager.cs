using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListManager : MonoBehaviour
{
    [SerializeField]
    private GameObject listItem;

    private List<GameObject> _listItems;

    private void Awake()
    {
        _listItems = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AddItem("Create Room");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(string text)
    {
        GameObject newListItem = Instantiate(listItem, transform);
        newListItem.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
        // disable separator of last list item
        if (_listItems.Count > 0)
        {
            GameObject secondLastItem = _listItems[_listItems.Count - 1];
            secondLastItem.transform.GetChild(secondLastItem.transform.childCount - 1).gameObject.SetActive(true);
        }
        newListItem.transform.GetChild(newListItem.transform.childCount - 1).gameObject.SetActive(false);

        _listItems.Add(newListItem);
    }
}
