using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// List manager for menu lists
/// This class is specially designed for button lists of rooms.
/// </summary>
public class ListManager : MonoBehaviour
{
    [SerializeField]
    private GameObject listItem;

    private List<GameObject> _listItems;
    private List<string> _listItemNames;

    private void Awake()
    {
        _listItems = new List<GameObject>();
        _listItemNames = new List<string>();
    }

    /// <summary>
    /// Refresh the list potentially adding new items to the list. 
    /// In this case rooms.
    /// </summary>
    public Dictionary<string, Button> Refresh(List<string> newRooms)
    {
        Dictionary<string, Button> addedItems = new Dictionary<string, Button>();
        //List<string> temp = new List<string>(_listItemNames);

        foreach(string roomName in newRooms)
        {
            if (!_listItemNames.Contains(roomName))
            {
                addedItems[roomName] = AddItem(roomName);
            }
            //else
            //{
            //    temp.Remove(roomName);
            //}
        }
        //// remove unavailable rooms
        //temp.ForEach(name => RemoveItem(name));

        return addedItems;
    }

    /// <summary>
    /// Remove an item from the list and destroy its button gameobject.
    /// In this case a room.
    /// </summary>
    public void RemoveItem(string roomName)
    {
        if (!_listItemNames.Contains(roomName))
            return;
        _listItemNames.Remove(roomName);
        GameObject toBeDestroyed = _listItems.Find(gameObject => gameObject.name.Equals(roomName));
        _listItems.Remove(toBeDestroyed);
        Destroy(toBeDestroyed);
    }

    /// <summary>
    /// Add a specific item to the list and create a button for it.
    /// </summary>
    /// <param name="separator">Whether a separator should be added</param>
    /// <returns>the button created</returns>
    public Button AddItem(string roomName, bool separator=true)
    {
        GameObject newListItem = Instantiate(listItem, transform);
        newListItem.transform.SetAsFirstSibling(); // move to the top of the container
        newListItem.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = roomName;
        if (!separator)
            newListItem.transform.GetChild(newListItem.transform.childCount - 1).gameObject.SetActive(false);

        newListItem.name = roomName;

        _listItems.Insert(0, newListItem);
        _listItemNames.Add(roomName);
        return newListItem.GetComponentInChildren<Button>();
    }
}
