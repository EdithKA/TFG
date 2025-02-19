using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int inventorySize = 20;

    public bool AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            return true;
        }
        else
        {
            Debug.Log("Inventory is full");
            return false;
        }
    }

    
}



