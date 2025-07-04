using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Gestiona la salud y cordura del jugador, la UI y los efectos de corrupción del mundo.
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    public int sanity = 100; // Cordura actual.
    public int health = 100; // Salud actual.
    public float decreaseInterval = 1.5f; // Intervalo de decrecimiento de la cordura.
    float lastSanityDecreaseTime; //última vez que se redujo la cordura.
    public bool hasPhone = false; // Indica si el jugador tiene el teléfono.

    [Header("World Corruption")]
    public string corruptibleTag; // Tag de objetos corruptibles.
    public float moveRange = 0.5f; // Máximo desplazamiento aleatorio de los objetos corruptibles.
    public float rotationRange = 45f; // Máxima rotación aleatoria de los objetos corruptibles.
    public int sanityThreshold = 50; // Umbral de cordura para iniciar la corrupción.

    List<GameObject> corruptibleObjects = new List<GameObject>(); // Lista de objetos corruptibles.
    Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); // Posiciones originales de los objetos corruptibles.
    Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>(); // Rotaciones originales de los objetos corruptibles.

    [Header("Health UI")]
    public Image heartIcon; // Icono de la salud en la UI.
    public Sprite[] heartSprites; // Sprites para los distintos estados de salud.
    public List<GameObject> healthBars; // Barras de salud en la UI.

    [Header("Sanity UI")]
    public Image sanityIcon; // Icono de la cordura en la UI.
    public Sprite[] sanitySprites; // Sprites para los distintos estados de la cordura.
    public List<GameObject> sanityBars; // Barras de cordura en la UI.

    [Header("References")]
    public GameManager gameManager;

    [Header("Camera Effects")]
    public Volume postProcessVolume;
    Vignette vignette; 


    // Ínicialización de referencias, objetos corruptibles y estadísticas iniciales.
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(corruptibleTag);
        corruptibleObjects.AddRange(foundObjects);

        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
                originalRotations[obj] = obj.transform.rotation;
            }
        }

        sanity = 100;
        health = 100;
        lastSanityDecreaseTime = Time.time;

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet<Vignette>(out vignette);
        }
    }

    // Reducción de cordura, corrupción del mundo, restauración de estadísticas y actualización de UI/efectos.
    void Update()
    {
        // Si la salud llega a 0 --> GAME OVER
        if (health <= 0)
            gameManager.LoadSceneByName("GameOver");
        //Si el jugador tiene el movil, la cordura comienza a bajar.
        if (hasPhone)
        {
            if (Time.time - lastSanityDecreaseTime > decreaseInterval)
            {
                lastSanityDecreaseTime = Time.time;
                if (sanity > 0)
                {
                    sanity--;
                }
                else if (health > 0)
                {
                    health--;
                }
            }
        }

        // Si la cordura es alta, restaurar salud poco a poco.
        if (sanity > 90)
            RestoreHealth();

        // Si la cordura está por encima del umbral, restaurar objetos corruptos.
        if (sanity >= sanityThreshold)
        {
            ResetCorruptedObjects();
        }

        // Actualizar iconos, barras y efectos visuales.
        UpdateUI();
        UpdateBars();
        UpdateVignetteEffect();
    }

    // Restaura el estado original de los objetos corruptibles.
    void ResetCorruptedObjects()
    {
        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null && originalPositions.ContainsKey(obj) && originalRotations.ContainsKey(obj))
            {
                obj.transform.position = originalPositions[obj];
                obj.transform.rotation = originalRotations[obj];
            }
        }
    }

    // Actualiza los iconos de salud y cordura en la UI.
    void UpdateUI()
    {
        SetHeartIcon();
        SetBrainIcon();
    }

    // Cambia el sprite del corazón según la cantidad de salud.
    void SetHeartIcon()
    {
        if (health <= 0)
            heartIcon.sprite = heartSprites[0];
        else if (health <= 25)
            heartIcon.sprite = heartSprites[1];
        else if (health <= 50)
            heartIcon.sprite = heartSprites[2];
        else if (health <= 75)
            heartIcon.sprite = heartSprites[3];
        else
            heartIcon.sprite = heartSprites[4];
    }
    
    // Cambia el sprite del cerebro según la cantidad de cordura.
    void SetBrainIcon()
    {
        if (sanity <= 0)
            sanityIcon.sprite = sanitySprites[0];
        else if (sanity <= 25)
            sanityIcon.sprite = sanitySprites[1];
        else if (sanity <= 50)
            sanityIcon.sprite = sanitySprites[2];
        else if (sanity <= 75)
            sanityIcon.sprite = sanitySprites[3];
        else
            sanityIcon.sprite = sanitySprites[4];
    }

    // Actualiza las barras de salud y cordura en la UI.
    void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    // Activa o desactiva las barras según el valor del stat.
    void UpdateBar(int statValue, List<GameObject> bars)
    {
        int activeBarsCount = Mathf.CeilToInt(statValue / 25f);

        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].SetActive(i < activeBarsCount);
        }
    }

    // Resta salud al jugador.
    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    // Actualiza el efecto de viñeta según la salud (menos salud = más rojo y cerrado).
    void UpdateVignetteEffect()
    {
        if (vignette == null) return;

        float healthPercent = Mathf.Clamp01(health / 100f);
        vignette.color.value = Color.Lerp(Color.red, Color.black, healthPercent);
        vignette.intensity.value = Mathf.Lerp(0.9f, 0.3f, healthPercent);

        float vignetteSmoothness = Mathf.Lerp(0.1f, 0.5f, healthPercent);
        vignette.smoothness.Override(vignetteSmoothness);
    }

    //  Restaura la salud poco a poco.
    void RestoreHealth()
    {
        if (health < 100)
            health += 1;
    }
}
