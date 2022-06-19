using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListManager : MonoBehaviour
{
    [SerializeField]
    private GameObject listItem;

    [SerializeField]
    private float refreshInterval;

    private List<GameObject> _listItems;
    private List<string> _listItemNames;

    private void Awake()
    {
        _listItems = new List<GameObject>();
        _listItemNames = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //AddItem("Create Room");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Dictionary<string, Button> Refresh(List<string> roomNames)
    {
        Dictionary<string, Button> addedItems = new Dictionary<string, Button>();
        List<string> temp = new List<string>(_listItemNames);

        foreach(string roomName in roomNames)
        {
            if (!_listItemNames.Contains(roomName))
            {
                addedItems[roomName] = AddItem(roomName);
            }
            else
            {
                temp.Remove(roomName);
            }
        }
        // remove unavailable rooms
        temp.ForEach(name => RemoveItem(name));

        return addedItems;
    }

    public void RemoveItem(string roomName)
    {
        _listItemNames.Remove(roomName);
        GameObject toBeDestroyed = _listItems.Find(gameObject => gameObject.name.Equals(roomName));
        _listItems.Remove(toBeDestroyed);
        Destroy(toBeDestroyed);
    }

    public Button AddItem(string roomName)
    {
        GameObject newListItem = Instantiate(listItem, transform);
        newListItem.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = roomName;


        newListItem.name = roomName;

        // disable separator of last list item
        //if (_listItems.Count > 0)
        //{
        //    GameObject secondLastItem = _listItems[_listItems.Count - 1];
        //    secondLastItem.transform.GetChild(secondLastItem.transform.childCount - 1).gameObject.SetActive(true);
        //}
        //newListItem.transform.GetChild(newListItem.transform.childCount - 1).gameObject.SetActive(false);

        _listItems.Insert(0, newListItem);
        _listItemNames.Add(roomName);
        return newListItem.GetComponentInChildren<Button>();
    }
}
