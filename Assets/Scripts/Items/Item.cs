using UnityEngine;

/**
 * @brief Inventory item ScriptableObject for easy asset management.
 *        Used for UI (2D) and world (3D) objects.
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Config")]
    public string itemID;             ///< Inventory name
    public string displayName;        ///< Shown name
    public Sprite icon;               ///< 2D icon for UI
    public string type;               ///< Item type/category
    public GameObject itemPrefab;     ///< 3D prefab
    public Vector3 equipPositionOffset;///< Offset when equipped
    public Vector3 equipRotationOffset;///< Rotation when equipped
}
