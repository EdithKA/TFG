using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP!

/**
 * @brief Manages the player stats (health and sanity), updates the UI, and triggers world corruption effects.
 *        Designed for a survival horror game where the world becomes more unstable as sanity decreases.
 */
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    public int sanity = 100;                                 /// Current sanity value (0-100).
    public int health = 100;                                 /// Current health value (0-100).
    public float decreaseInterval = 1.5f;                     /// Time in seconds between sanity decreases.
    float lastSanityDecreaseTime;                     /// Timestamp of last sanity decrease.
    public bool hasPhone = false;                             /// Whether the the player has obtained the phone.

    [Header("World Corruption")]
    public string corruptibleTag;                             /// Tag for objects that can be corrupted.
    public float moveRange = 0.5f;                            /// Maximum random movement offset during corruption.
    public float rotationRange = 45f;                         /// Maximum random rotation angle during corruption.
    public int sanityThreshold = 50;                          /// Sanity value at which corruption starts (50% threshold).

    List<GameObject> corruptibleObjects = new List<GameObject>(); /// List of objects that can be corrupted.
    Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); /// Stores original positions.
    Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>(); /// Stores original rotations.

    [Header("Health UI")]
    public Image heartIcon;                                   /// UI image for the heart icon.
    public Sprite[] heartSprites;                             /// Sprites for different health states.
    public List<GameObject> healthBars;                       /// List of health bar UI segments.

    [Header("Sanity UI")]
    public Image sanityIcon;                                  /// UI image for the brain icon.
    public Sprite[] sanitySprites;                            /// Sprites for different sanity states.
    public List<GameObject> sanityBars;                       /// List of sanity bar UI segments.

    [Header("References")]
    public GameManager gameManager;

    [Header("Camera Effects")]
    public Volume postProcessVolume; // URP Volume
    Vignette vignette;

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

    void Update()
    {
        if (health <= 0)
            gameManager.LoadSceneByName("GameOver");
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

        if(sanity > 90)
            RestoreHealth();

        if (sanity >= sanityThreshold)
        {
            ResetCorruptedObjects();
        }

        UpdateUI();
        UpdateBars();
        UpdateVignetteEffect();
    }

   

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

    void UpdateUI()
    {
        SetHeartIcon();
        SetBrainIcon();
    }

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

    void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    void UpdateBar(int statValue, List<GameObject> bars)
    {
        int activeBarsCount = Mathf.CeilToInt(statValue / 25f);

        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].SetActive(i < activeBarsCount);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            gameManager.LoadSceneByName("GameOver");
        }
    }

    void UpdateVignetteEffect()
    {
        if (vignette == null) return;

        float healthPercent = Mathf.Clamp01(health / 100f);
        vignette.color.value = Color.Lerp(Color.red, Color.black, healthPercent);
        vignette.intensity.value = Mathf.Lerp(0.9f, 0.3f, healthPercent);

        float vignetteSmoothness = Mathf.Lerp(0.1f, 0.5f, healthPercent);
        vignette.smoothness.Override(vignetteSmoothness);
    }

    void RestoreHealth()
    {
        if (health < 100)
            health += 1;
    }
}
