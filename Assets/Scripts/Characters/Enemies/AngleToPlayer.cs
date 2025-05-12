using UnityEngine;

/**
 * @brief This script calculates the angle between this object and the player,
 *        and flips the sprite depending on where the player is.
 *        It's useful for 2D enemies or NPCs that need to "look" at the player.
 */
public class AngleToPlayer : MonoBehaviour
{
    // Reference to the player's transform (we find it automatically in Start)
    private Transform playerTransform;

    // We'll use this to store the player's position but with our own Y (so we ignore vertical difference)
    private Vector3 flattenedPlayerPosition;

    // Direction vector from this object to the player (on the XZ plane)
    private Vector3 directionToPlayer;

    // The signed angle (in degrees) between where we're facing and where the player is
    private float signedAngleToPlayer;

    // This will store which of the 8 directions (for animation) we're currently facing
    public int lastIndex; // Keeps the last valid direction index

    // Reference to the SpriteRenderer so we can flip the sprite visually
    private SpriteRenderer spriteRenderer;

    /**
     * @brief Get references to the player and the sprite renderer.
     */
    void Start()
    {
        // Find the player in the scene (assumes there's only one PlayerController)
        playerTransform = FindObjectOfType<PlayerController>().transform;

        // Get the SpriteRenderer from children (so it works even if the sprite is a child)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /**
     * @brief Every frame, calculate the direction to the player and update the sprite.
     */
    void Update()
    {
        // Ignore Y axis so the calculation is always "flat" (good for 2D or top-down)
        flattenedPlayerPosition = new Vector3(
            playerTransform.position.x,
            transform.position.y,
            playerTransform.position.z
        );

        // Get the direction vector from us to the player
        directionToPlayer = flattenedPlayerPosition - transform.position;

        // Calculate the signed angle between our forward and the direction to the player
        // Positive = player is to our right, Negative = player is to our left
        signedAngleToPlayer = Vector3.SignedAngle(directionToPlayer, transform.forward, Vector3.up);

        // Flip the sprite if the player is to our right (so the enemy looks at the player)
        Vector3 scale = Vector3.one;
        if (signedAngleToPlayer > 0)
            scale.x = -1f; // Mirror the sprite
        spriteRenderer.transform.localScale = scale;

        // Get the direction index (for 8-way animation, if needed)
        lastIndex = GetDirectionIndex(signedAngleToPlayer, lastIndex);
    }

    /**
     * @brief Convert the angle to an index (0-7) for 8 possible directions.
     *        This is useful if you want to play different animations depending on where the player is.
     * @param angle The signed angle between us and the player, in degrees.
     * @param previousIndex The last valid direction index.
     * @return An integer from 0 to 7 representing the direction.
     */
    int GetDirectionIndex(float angle, int previousIndex)
    {
        // 0 = Front, 1 = Front-Left, 2 = Left, 3 = Back-Left, 4 = Back,
        // 5 = Back-Right, 6 = Right, 7 = Front-Right

        if (angle > -22.5f && angle < 22.5f) return 0; // Front
        if (angle >= 22.5f && angle < 67.5f) return 7; // Front-Right
        if (angle >= 67.5f && angle < 112.5f) return 6; // Right
        if (angle >= 112.5f && angle < 157.5f) return 5; // Back-Right
        if (angle <= -157.5f || angle >= 157.5f) return 4; // Back
        if (angle >= -157.5f && angle < -112.5f) return 3; // Back-Left
        if (angle >= -112.5f && angle < -67.5f) return 2; // Left
        if (angle >= -67.5f && angle <= -22.5f) return 1; // Front-Left

        // If for some reason the angle doesn't fit, keep the previous direction
        return previousIndex;
    }

    /**
     * @brief Draws debug lines in the Scene view so I can see the direction calculations.
     */
    private void OnDrawGizmosSelected()
    {
        // Draw a green line pointing from this object to the player
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, flattenedPlayerPosition);

        // Draw a blue ray showing our forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}
