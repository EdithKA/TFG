using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Iventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; //nombre del objeto
    public Sprite icon; //icono del objeto en la ventana de inventario

}
public class PickUpObject : MonoBehaviour
{
    public Transform PlayerHand;
    bool isPlayerInTrigger = false;
    private bool isHeld = false;
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !isHeld)
        {
            PickUp(gameObject.name);
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

    void PickUp(string item)
    {
        Debug.Log("You picked up a " + item);

        if(item == "mokia")
        {
            isHeld = true;


            transform.position = PlayerHand.position;
            transform.rotation = PlayerHand.rotation;

            transform.SetParent(PlayerHand);
        }
    }



}
