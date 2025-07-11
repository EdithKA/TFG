using UnityEngine;

/**
 * @brief Game texts asset for interaction and thought messages.
 */
[CreateAssetMenu(menuName = "Game/GameTexts")]
public class GameTexts : ScriptableObject
{
    [Header("Interaction Messages")]
    public string interactMessage = "";         ///< Interaction prompt
    public string placeObjectMessage = "";      ///< Place object prompt
    public string collectMessage = "";          ///< Collect prompt
    public string collectedMessage = "";        ///< Collected object message
    public string inspectMessage = "";          ///< Inspect prompt
    public string objectAdded = "";             ///< Added to inventory
    public string objectRemoved = "";           ///< Removed from inventory

    [Header("Thoughts")]
    public string startThought = "";            ///< Initial thought
    public string needMobileMessage = "";       ///< Need mobile
    public string objectAlreadyPlaced = "";     ///< Already placed
    public string needObjectMessage = "";       ///< Need object
    public string placedCorrectlyMessage = "";  ///< Correctly placed
    public string wrongObjectMessage = "";      ///< Wrong object

    public string dvdCorrectMessage = "";       ///< Correct DVD
    public string dvdError = "";                ///< DVD error
    public string dvdMissing = "";              ///< DVD missing
    public string rewardAppear = "";            ///< Reward appears

    public string needSomething = "";           ///< Need something
    public string canOpen = "";                 ///< Can open
    public string closeDoor = "";               ///< Close door
    public string rewardCollected = "";         ///< Reward collected
    public string photoCollected = "";          ///< Photo collected
    public string needPieces = "";              ///< Need more pieces
}
