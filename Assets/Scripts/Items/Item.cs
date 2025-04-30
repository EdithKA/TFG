using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Defines inventory items as ScriptableObjects for easy asset creation
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Configuration")]
    public string itemName;         /// Name displayed in inventory UI
    public Sprite icon;             /// 2D representation in inventory slots
    public string type;
    public GameObject itemPrefab;   /// 3D prefab for in-world representation
    public string collectedText;

}
