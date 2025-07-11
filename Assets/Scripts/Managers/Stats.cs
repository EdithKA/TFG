using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/**
 * @brief Player stats: sanity, health, world corruption, UI, and effects.
 */
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    public int sanity = 100;              ///< Current sanity
    public int health = 100;              ///< Current health
    public float decreaseInterval = 1.5f; ///< Sanity decrease interval
    float lastSanityDecreaseTime;         ///< Last sanity decrease
    public bool hasPhone = false;         ///< Player has phone

    [Header("World Corruption")]
    public string corruptibleTag;         ///< Tag for corruptibles
    public float moveRange = 0.5f;        ///< Max move offset
    public float rotationRange = 45f;     ///< Max rotation offset
    public int sanityThreshold = 50;      ///< Corrupt if sanity < threshold
    public float corruptionSpeed = 1f;    ///< Corruption speed

    List<GameObject> corruptibleObjects = new List<GameObject>(); ///< Corruptibles
    Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); ///< Original positions
    Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>(); ///< Original rotations

    [Header("Health UI")]
    public Image heartIcon;               ///< Heart icon
    public Sprite[] heartSprites;         ///< Heart sprites
    public List<GameObject> healthBars;   ///< Health bars

    [Header("Sanity UI")]
    public Image sanityIcon;              ///< Sanity icon
    public Sprite[] sanitySprites;        ///< Sanity sprites
    public List<GameObject> sanityBars;   ///< Sanity bars

    [Header("References")]
    public GameManager gameManager;       ///< Game manager

    [Header("Camera Effects")]
    public Volume postProcessVolume;      ///< Post-process volume
    Vignette vignette;                    ///< Vignette effect

    /**
     * @brief Get refs, setup stats and corruptibles.
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
     * @brief Handle sanity, corruption, UI, and effects.
     */
    void Update()
    {
        if (health <= 0)
            gameManager.ChangeScene("GameOver");

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

        if (sanity > 90)
            RestoreHealth();

        if (sanity < sanityThreshold)
        {
            CorruptObjects();
        }
        else
        {
            ResetCorruptedObjects();
        }

        UpdateUI();
        UpdateBars();
        UpdateVignetteEffect();
    }

    /**
     * @brief Corrupt objects if sanity is low.
     */
    void CorruptObjects()
    {
        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null && originalPositions.ContainsKey(obj) && originalRotations.ContainsKey(obj))
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange)
                );
                Vector3 corruptedPosition = originalPositions[obj] + randomOffset;

                Quaternion randomRotation = Quaternion.Euler(
                    originalRotations[obj].eulerAngles.x + Random.Range(-rotationRange, rotationRange),
                    originalRotations[obj].eulerAngles.y + Random.Range(-rotationRange, rotationRange),
                    originalRotations[obj].eulerAngles.z + Random.Range(-rotationRange, rotationRange)
                );

                obj.transform.position = Vector3.Lerp(
                    obj.transform.position,
                    corruptedPosition,
                    Time.deltaTime * corruptionSpeed
                );
                obj.transform.rotation = Quaternion.Lerp(
                    obj.transform.rotation,
                    randomRotation,
                    Time.deltaTime * corruptionSpeed
                );
            }
        }
    }

    /**
     * @brief Reset objects to original state.
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
     * @brief Update health and sanity icons.
     */
    void UpdateUI()
    {
        SetHeartIcon();
        SetBrainIcon();
    }

    /**
     * @brief Set heart icon by health.
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
     * @brief Set brain icon by sanity.
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
     * @brief Update health and sanity bars.
     */
    void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    /**
     * @brief Set bars active by stat value.
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
     * @brief Subtract health.
     */
    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    /**
     * @brief Update vignette by health.
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
     * @brief Restore health slowly.
     */
    void RestoreHealth()
    {
        if (health < 100)
            health += 1;
    }
}
