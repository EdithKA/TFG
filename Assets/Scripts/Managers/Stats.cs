using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP!

/**
 * @brief Manages soundPlayer stats (health and sanity), updates the UI, and triggers world corruption effects.
 *        Designed for a survival horror game where the world becomes more unstable as sanity decreases.
 */
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    public int sanity = 100;                                 /// Current sanity value (0-100).
    public int health = 100;                                 /// Current health value (0-100).
    public float decreaseInterval = 1.5f;                     /// Time in seconds between sanity decreases.
    private float lastSanityDecreaseTime;                     /// Timestamp of last sanity decrease.
    public bool hasPhone = false;                             /// Whether the soundPlayer has obtained the phone.

    [Header("World Corruption")]
    public string corruptibleTag;                             /// Tag for objects that can be corrupted.
    public float moveRange = 0.5f;                            /// Maximum random movement offset during corruption.
    public float rotationRange = 45f;                         /// Maximum random rotation angle during corruption.
    public int sanityThreshold = 50;                          /// Sanity value at which corruption starts (50% threshold).

    private List<GameObject> corruptibleObjects = new List<GameObject>(); /// List of objects that can be corrupted.
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); /// Stores original positions.
    private Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>(); /// Stores original rotations.
    private int lastSanityValue = 100;                        /// Stores last sanity value to detect changes.

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
    private Vignette vignette;

    /// <summary>
    /// Initializes stats, finds corruptible objects, and stores their original transforms.
    /// </summary>
    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        // Find all objects in the scene that can be corrupted and store their original positions/rotations
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
        lastSanityValue = sanity;
        lastSanityDecreaseTime = Time.time;

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet<Vignette>(out vignette);
        }
    }

    /// <summary>
    /// Updates the UI, decreases sanity over time (if phone obtained), and applies corruption effects.
    /// </summary>
    private void Update()
    {
        // Only decrease sanity if the soundPlayer has obtained the phone
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

        // If sanity has changed and is below threshold, apply corruption effect
        if (sanity < lastSanityValue && sanity < sanityThreshold)
        {
            ApplyCorruptionStep();
        }
        lastSanityValue = sanity;

        // Reset objects if sanity is restored above threshold
        if (sanity >= sanityThreshold)
        {
            ResetCorruptedObjects();
        }

        UpdateUI();
        UpdateBars();
        UpdateVignetteEffect();
    }

    /// <summary>
    /// Applies a single step of corruption effect (objects "jump" to new position).
    /// </summary>
    private void ApplyCorruptionStep()
    {
        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null && originalPositions.ContainsKey(obj))
            {
                // Calculate new random offset (intensity based on sanity)
                float intensity = 1 - (sanity / (float)sanityThreshold);
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange) * intensity,
                    Random.Range(-moveRange, moveRange) * intensity,
                    Random.Range(-moveRange, moveRange) * intensity
                );
                obj.transform.position = originalPositions[obj] + randomOffset;

                // Apply random rotation
                float randomX = Random.Range(-rotationRange, rotationRange) * intensity;
                float randomY = Random.Range(-rotationRange, rotationRange) * intensity;
                float randomZ = Random.Range(-rotationRange, rotationRange) * intensity;
                obj.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);
            }
        }
    }

    /// <summary>
    /// Resets all corruptible objects to their original positions and rotations.
    /// </summary>
    private void ResetCorruptedObjects()
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

    /// <summary>
    /// Updates the heart and brain icons based on current health and sanity.
    /// </summary>
    private void UpdateUI()
    {
        SetHeartIcon();
        SetBrainIcon();
    }

    /// <summary>
    /// Sets the heart icon sprite based on current health value.
    /// </summary>
    private void SetHeartIcon()
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

    /// <summary>
    /// Sets the brain icon sprite based on current sanity value.
    /// </summary>
    private void SetBrainIcon()
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

    /// <summary>
    /// Updates the health and sanity bar UI elements.
    /// </summary>
    private void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    /// <summary>
    /// Activates or deactivates bar segments based on the stat value.
    /// </summary>
    /// <param name="statValue">The value of the stat (health or sanity).</param>
    /// <param name="bars">The list of bar segment GameObjects.</param>
    private void UpdateBar(int statValue, List<GameObject> bars)
    {
        int activeBarsCount = Mathf.CeilToInt(statValue / 25f);

        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].SetActive(i < activeBarsCount);
        }
    }

    /// <summary>
    /// Applies damage to the soundPlayer and triggers game over if health reaches zero.
    /// </summary>
    /// <param name="amount">Amount of damage to take.</param>
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            gameManager.LoadSceneByName("GameOver");
        }
    }

    /// <summary>
    /// Updates the vignette color and intensity based on the soundPlayer's health.
    /// </summary>
    private void UpdateVignetteEffect()
    {
        if (vignette == null) return;

        float healthPercent = Mathf.Clamp01(health / 100f);
        vignette.color.value = Color.Lerp(Color.red, Color.black, healthPercent);
        vignette.intensity.value = Mathf.Lerp(0.9f, 0.3f, healthPercent);

        float vignetteSmoothness = Mathf.Lerp(0.1f, 0.5f, healthPercent);
        vignette.smoothness.Override(vignetteSmoothness);
    }
}
