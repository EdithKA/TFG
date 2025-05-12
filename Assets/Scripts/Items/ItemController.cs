using UnityEngine;

public class ItemController : MonoBehaviour
{
    [Header("Configuración")]
    public Item itemData;
    public bool isHeld = false;

    [Header("Referencias")]
    private InventoryManager inventoryManager;
    private UITextController uiTextController;
    private PlayerController playerController;
    private bool playerInRange;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        uiTextController = FindObjectOfType<UITextController>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isHeld)
        {

            if(!inventoryManager.HasItem("Mobile") && itemData.name != "Mobile")
                uiTextController.ShowThought(uiTextController.gameTexts.needMobileMessage);
            else
                PickUp();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isHeld)
        {
            playerInRange = true;
            uiTextController.ShowInteraction(uiTextController.gameTexts.collectMessage, Color.yellow);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiTextController.ClearMessages();
        }
    }

    private void PickUp()
    {
        inventoryManager.AddItem(itemData);
        uiTextController.ShowThought(uiTextController.gameTexts.collectedMessage);

        if (itemData.itemName == "Mobile")
        {
            isHeld = true;
            transform.SetParent(playerController.leftHand);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Destroy(gameObject);
        }

        // Limpia el mensaje de interacción al recoger
        uiTextController.ClearMessages();
    }

}
