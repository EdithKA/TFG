using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Manages player stats (health and sanity), updates UI, and handles world corruption effects.
 */
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    int sanity; /// Current sanity value (0-100).
    int health; /// Current health value (0-100).
    float decreaseInterval = 1.5f; /// Time interval between sanity decreases.


    [Header("World Corruption")]
    public string corruptibleTag; /// Tag for objects that can be corrupted.
    public float moveRange = 0.5f; /// Maximum random movement offset during corruption.
    public float rotationSpeed = 50f; /// Speed of rotation during corruption (not used in this script).
    public int sanityThreshold; /// Sanity value at which corruption starts.
    public float rotationRange = 45f; /// Maximum random rotation angle during corruption.

    private List<GameObject> corruptibleObjects = new List<GameObject>(); /// List of objects that can be corrupted.
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); /// Original positions of corruptible objects.
    private int lastSanityTrigger; /// Last sanity value at which corruption was triggered.

    [Header("Health")]
    public Image heartIcon; /// UI image for the heart icon.
    public Sprite[] heartStats; /// Sprites for different heart states.
    public List<GameObject> healthBars; /// List of health bar UI segments.

    [Header("Sanity")]
    public Image sanityIcon; /// UI image for the brain icon.
    public Sprite[] brainStats; /// Sprites for different brain states.
    public List<GameObject> sanityBars; /// List of sanity bar UI segments.

    /**
     * @brief Initializes stats, finds corruptible objects, and starts the sanity decrease coroutine.
     */
    private void Start()
    {
        GameObject[] corruptObjects = GameObject.FindGameObjectsWithTag(corruptibleTag);
        corruptibleObjects.AddRange(corruptObjects);

        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
            }
        }

        sanity = 100;
        health = 100;
        lastSanityTrigger = sanity;

        StartCoroutine(DecreaseSanity());
    }

    /**
     * @brief Updates UI and bar displays every frame.
     */
    private void Update()
    {
        UpdateUI();
        SetHealth();
        SetSanity();
        UpdateBars();
    }

    /**
     * @brief Updates the text UI for sanity and health.
     */
    void UpdateUI()
    {
        
        SetHealth();
        SetSanity();
    }

    /**
     * @brief Sets the heart icon sprite based on current health value.
     */
    void SetHealth()
    {
        switch (health)
        {
            case <= 0:
                heartIcon.sprite = heartStats[0];
                break;
            case <= 25:
                heartIcon.sprite = heartStats[1];
                break;
            case > 25 and <= 50:
                heartIcon.sprite = heartStats[2];
                break;
            case > 50 and <= 75:
                heartIcon.sprite = heartStats[3];
                break;
            case > 75:
                heartIcon.sprite = heartStats[4];
                break;
        }
    }

    /**
     * @brief Sets the brain icon sprite based on current sanity value.
     */
    void SetSanity()
    {
        switch (sanity)
        {
            case <= 0:
                sanityIcon.sprite = brainStats[0];
                break;
            case <= 25:
                sanityIcon.sprite = brainStats[1];
                break;
            case > 25 and <= 50:
                sanityIcon.sprite = brainStats[2];
                break;
            case > 50 and <= 75:
                sanityIcon.sprite = brainStats[3];
                break;
            case > 75:
                sanityIcon.sprite = brainStats[4];
                break;
        }
    }

    /**
     * @brief Coroutine that decreases sanity over time and triggers corruption if needed.
     */
    IEnumerator DecreaseSanity()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);

            if (sanity > 0)
            {
                sanity--;
                if (sanity < sanityThreshold)
                {
                    StartCoroutine(CorruptObjects());
                }
            }
            else if (health > 0)
            {
                health--;
            }
        }
    }

    /**
     * @brief Updates the health and sanity bar UI elements.
     */
    void UpdateBars()
    {
        UpdateBar(health, healthBars);
        UpdateBar(sanity, sanityBars);
    }

    /**
     * @brief Activates or deactivates bar segments based on the stat value.
     * @param statValue The value of the stat (health or sanity).
     * @param bars The list of bar segment GameObjects.
     */
    void UpdateBar(int statValue, List<GameObject> bars)
    {
        int activeBarsCount = Mathf.CeilToInt(statValue / 25f);

        for (int i = 0; i < bars.Count; i++)
        {
            if (i < activeBarsCount)
            {
                bars[i].SetActive(true);
            }
            else
            {
                bars[i].SetActive(false);
            }
        }
    }

    /**
     * @brief Coroutine that corrupts objects by moving and rotating them randomly, then resets them.
     */
    IEnumerator CorruptObjects()
    {
        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange)
                );
                obj.transform.position += randomOffset;

                float randomXRotation = Random.Range(-rotationRange, rotationRange);
                float randomYRotation = Random.Range(-rotationRange, rotationRange);
                float randomZRotation = Random.Range(-rotationRange, rotationRange);
                obj.transform.Rotate(new Vector3(randomXRotation, randomYRotation, randomZRotation));
            }
        }

        yield return new WaitForSeconds(2f);

        ResetObjects();
    }

    /**
     * @brief Resets all corruptible objects to their original positions and rotations.
     */
    private void ResetObjects()
    {

        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null && originalPositions.ContainsKey(obj))
            {
                obj.transform.position = originalPositions[obj];
                obj.transform.rotation = Quaternion.identity;
            }
        }
    }
}
