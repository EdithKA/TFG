using UnityEngine;

public class PS2Interactuable : MonoBehaviour, IInteractuable
{
    public Light led;
    [SerializeField] private string requiredItemName = "Crash"; // Nombre exacto del �tem requerido

    private void Start()
    {
        led = GetComponentInChildren<Light>();
        led.color = Color.red;
    }

    public void Interact(GameObject objectOnHand = null)
    {
        Debug.Log("Intentando interactuar con PS2...");

        if (objectOnHand != null)
        {
            // Verifica si el objeto tiene el componente ItemController
            ItemController itemController = objectOnHand.GetComponent<ItemController>();
            if (itemController != null && itemController.itemData != null)
            {
                string objectName = itemController.itemData.itemName;
                Debug.Log("Objeto en mano: " + objectName);

                if (objectName == requiredItemName)
                {
                    led.color = Color.green;
                    Debug.Log("�DVD correcto insertado!");
                    // Aqu� puedes a�adir l�gica para iniciar el minijuego
                }
                else
                {
                    Debug.Log("DVD incorrecto. Se requiere: " + requiredItemName);
                }
            }
            else
            {
                Debug.LogError("El objeto en mano no tiene ItemController o itemData no est� asignado");
            }
        }
        else
        {
            Debug.Log("Necesitas un DVD para interactuar con la PS2");
        }
    }
}
