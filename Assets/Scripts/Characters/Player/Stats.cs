using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Manages player stats (health and sanity), updates the UI, and triggers world corruption effects.
 *        Designed for a survival horror game where the world becomes more unstable as sanity decreases.
 */
public class Stats : MonoBehaviour
{
    [Header("Player Stats")]
    private int sanity;                                 /// Current sanity value (0-100).
    private int health;                                 /// Current health value (0-100).
    [SerializeField] private float decreaseInterval = 1.5f; /// Time in seconds between sanity decreases.

    [Header("World Corruption")]
    public string corruptibleTag;                       /// Tag for objects that can be corrupted.
    public float moveRange = 0.5f;                      /// Maximum random movement offset during corruption.
    public float rotationRange = 45f;                   /// Maximum random rotation angle during corruption.
    public int sanityThreshold = 75;                    /// Sanity value at which corruption starts.

    private List<GameObject> corruptibleObjects = new List<GameObject>(); /// List of objects that can be corrupted.
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); /// Stores original positions.

    [Header("Health UI")]
    public Image heartIcon;                             /// UI image for the heart icon.
    public Sprite[] heartSprites;                       /// Sprites for different health states.
    public List<GameObject> healthBars;                 /// List of health bar UI segments.

    [Header("Sanity UI")]
    public Image sanityIcon;                            /// UI image for the brain icon.
    public Sprite[] brainSprites;                       /// Sprites for different sanity states.
    public List<GameObject> sanityBars;                 /// List of sanity bar UI segments.

    /// <summary>
    /// Initializes stats, finds corruptible objects, and starts the sanity decrease coroutine.
    /// </summary>
    private void Start()
    {
        // Find all objects in the scene that can be corrupted and store their original positions
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(corruptibleTag);
        corruptibleObjects.AddRange(foundObjects);

        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
            }
        }

        sanity = 100;
        health = 100;

        StartCoroutine(DecreaseSanityOverTime());
    }

    /// <summary>
    /// Updates the UI and bar displays every frame.
    /// </summary>
    private void Update()
    {
        UpdateUI();
        UpdateBars();
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
            sanityIcon.sprite = brainSprites[0];
        else if (sanity <= 25)
            sanityIcon.sprite = brainSprites[1];
        else if (sanity <= 50)
            sanityIcon.sprite = brainSprites[2];
        else if (sanity <= 75)
            sanityIcon.sprite = brainSprites[3];
        else
            sanityIcon.sprite = brainSprites[4];
    }

    /// <summary>
    /// Coroutine that decreases sanity over time and triggers corruption if needed.
    /// </summary>
    private IEnumerator DecreaseSanityOverTime()
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
    /// Coroutine that corrupts objects by moving and rotating them randomly, then resets them.
    /// </summary>
    private IEnumerator CorruptObjects()
    {
        foreach (GameObject obj in corruptibleObjects)
        {
            if (obj != null)
            {
                // Move the object to a random nearby position
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange)
                );
                obj.transform.position += randomOffset;

                // Apply a random rotation
                float randomX = Random.Range(-rotationRange, rotationRange);
                float randomY = Random.Range(-rotationRange, rotationRange);
                float randomZ = Random.Range(-rotationRange, rotationRange);
                obj.transform.Rotate(new Vector3(randomX, randomY, randomZ));
            }
        }

        // Wait a moment before resetting
        yield return new WaitForSeconds(2f);

        ResetCorruptedObjects();
    }

    /// <summary>
    /// Resets all corruptible objects to their original positions and rotations.
    /// </summary>
    private void ResetCorruptedObjects()
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
