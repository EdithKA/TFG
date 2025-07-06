using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/**
 * @brief Manages the player's health and sanity, UI, and world corruption effects.
 */
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    public int sanity = 100; ///< Current sanity.
    public int health = 100; ///< Current health.
    public float decreaseInterval = 1.5f; ///< Sanity decrease interval.
    float lastSanityDecreaseTime; ///< Last time sanity was decreased.
    public bool hasPhone = false; ///< Indicates if the player has the phone.

    [Header("World Corruption")]
    public string corruptibleTag; ///< Tag for corruptible objects.
    public float moveRange = 0.5f; ///< Maximum random movement of corruptible objects.
    public float rotationRange = 45f; ///< Maximum random rotation of corruptible objects.
    public int sanityThreshold = 50; ///< Sanity threshold to start corruption.

    List<GameObject> corruptibleObjects = new List<GameObject>(); ///< List of corruptible objects.
    Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); ///< Original positions of corruptible objects.
    Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>(); ///< Original rotations of corruptible objects.

    [Header("Health UI")]
    public Image heartIcon; ///< Health icon in the UI.
    public Sprite[] heartSprites; ///< Sprites for different health states.
    public List<GameObject> healthBars; ///< Health bars in the UI.

    [Header("Sanity UI")]
    public Image sanityIcon; ///< Sanity icon in the UI.
    public Sprite[] sanitySprites; ///< Sprites for different sanity states.
    public List<GameObject> sanityBars; ///< Sanity bars in the UI.

    [Header("References")]
    public GameManager gameManager;

    [Header("Camera Effects")]
    public Volume postProcessVolume;
    Vignette vignette;

    /**
     * @brief Initializes references, corruptible objects, and initial stats.
     */
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

    /**
     * @brief Handles sanity reduction, world corruption, stat restoration, and UI/effects update.
     */
    void Update()
    {
        // If health reaches 0 --> GAME OVER
        if (health <= 0)
            gameManager.ChangeScene("GameOver");
        // If the player has the phone, sanity starts to decrease.
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

        // If sanity is high, restore health little by little.
        if (sanity > 90)
            RestoreHealth();

        // If sanity is above the threshold, restore corrupt objects.
        if (sanity >= sanityThreshold)
        {
            ResetCorruptedObjects();
        }

        // Update icons, bars, and visual effects.
        UpdateUI();
        UpdateBars();
        UpdateVignetteEffect();
    }

    /**
     * @brief Restores the original state of corruptible objects.
     */
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

    /**
     * @brief Updates the health and sanity icons in the UI.
     */
    void UpdateUI()
    {
        SetHeartIcon();
        SetBrainIcon();
    }

    /**
     * @brief Changes the heart sprite according to the amount of health.
     */
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

    /**
     * @brief Changes the brain sprite according to the amount of sanity.
     */
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

    /**
     * @brief Updates the health and sanity bars in the UI.
     */
    void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    /**
     * @brief Activates or deactivates the bars according to the stat value.
     * @param statValue The value of the stat.
     * @param bars The list of bar GameObjects.
     */
    void UpdateBar(int statValue, List<GameObject> bars)
    {
        int activeBarsCount = Mathf.CeilToInt(statValue / 25f);

        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].SetActive(i < activeBarsCount);
        }
    }

    /**
     * @brief Subtracts health from the player.
     * @param amount The amount of health to subtract.
     */
    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    /**
     * @brief Updates the vignette effect according to health (less health = more red and closed).
     */
    void UpdateVignetteEffect()
    {
        if (vignette == null) return;

        float healthPercent = Mathf.Clamp01(health / 100f);
        vignette.color.value = Color.Lerp(Color.red, Color.black, healthPercent);
        vignette.intensity.value = Mathf.Lerp(0.9f, 0.3f, healthPercent);

        float vignetteSmoothness = Mathf.Lerp(0.1f, 0.5f, healthPercent);
        vignette.smoothness.Override(vignetteSmoothness);
    }

    /**
     * @brief Gradually restores health.
     */
    void RestoreHealth()
    {
        if (health < 100)
            health += 1;
    }
}
