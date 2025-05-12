using UnityEngine;

/**
 * @brief Defines inventory items as ScriptableObjects for easy asset creation and management.
 *        Used for both 2D UI representations and 3D in-game objects.
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Configuration")]
    public string itemName;         /// Name displayed in inventory UI.
    public Sprite icon;             /// 2D representation in inventory slots.
    public string type;             /// Category or type for gameplay logic (e.g., "piece", "DVD", etc.).
    public GameObject itemPrefab;   /// 3D prefab for in-world representation.
}
