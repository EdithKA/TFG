using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; //nombre del objeto
    public Sprite icon; //icono del objeto en la ventana de inventario
    public GameObject itemPrefab;

}
public class Object : MonoBehaviour
{
    public Transform PlayerHand;
    bool isPlayerInTrigger = false;
    public bool isHeld = false;
    public Item itemData;
    public UITextController instructions;
    public InventoryManager inventory;
    PlayerMove playerMove;


    private void Start()
    {
        playerMove = FindAnyObjectByType<PlayerMove>();
        instructions = GetComponent<UITextController>();
        inventory = FindAnyObjectByType<InventoryManager>();
    }
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !isHeld)
        {
            if(itemData.name == "Mokia")
            {
                playerMove.LActive = true;
            }
            Destroy(instructions.instructionText);
            Destroy(instructions);
            PickUp();
        }
        if(isHeld)
        {
            Destroy(instructions);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = false;
        }
    }

    void PickUp()
    {
        

        Debug.Log("You picked up a " + itemData.itemName);
        inventory.AddItem(itemData);

        if(itemData.itemName == "Mokia")
        {
            isHeld = true;


            transform.position = PlayerHand.position;
            transform.rotation = PlayerHand.rotation;

            transform.SetParent(PlayerHand);
        }
        else
        {
            Destroy(gameObject);
        }
    }



}
