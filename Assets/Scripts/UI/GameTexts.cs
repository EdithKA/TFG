using UnityEngine;

/**
 * @brief This class allows you to create an asset with the different texts present in the game.
 */
[CreateAssetMenu(menuName = "Game/GameTexts")]
public class GameTexts : ScriptableObject
{
    [Header("Interaction Messages")]
    public string interactMessage = ""; ///< Message for interaction.
    public string placeObjectMessage = ""; ///< Message for placing an object.
    public string collectMessage = ""; ///< Message for collecting.
    public string collectedMessage = ""; ///< Message for collected object.
    public string inspectMessage = ""; ///< Message for inspecting.
    public string objectAdded = ""; ///< Message for object added to inventory.
    public string objectRemoved = ""; ///< Message for object removed from inventory.

    [Header("Thoughts")]
    public string needMobileMessage = ""; ///< Message when mobile is needed.
    public string objectAlreadyPlaced = ""; ///< Message when object is already placed.
    public string needObjectMessage = ""; ///< Message when an object is needed.
    public string placedCorrectlyMessage = ""; ///< Message when object is placed correctly.
    public string wrongObjectMessage = ""; ///< Message when wrong object is placed.

    public string dvdCorrectMessage = ""; ///< Message when DVD is correct.
    public string dvdError = ""; ///< Message for DVD error.
    public string dvdMissing = ""; ///< Message when DVD is missing.
    public string rewardAppear = ""; ///< Message when reward appears.

    public string needSomething = ""; ///< Message when something is needed.
    public string canOpen = ""; ///< Message when something can be opened.
    public string closeDoor = ""; ///< Message for closing the door.
    public string rewardCollected = ""; ///< Message for reward collected.
    public string photoCollected = ""; ///< Message for photo collected.
    public string needPieces = ""; ///< Message when more pieces are needed.
}
