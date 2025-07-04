using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameTexts")]
public class GameTexts : ScriptableObject
{
    [Header("Interaction Messages")]
    public string interactMessage = "";
    public string placeObjectMessage = "";
    public string collectMessage = "";
    public string collectedMessage = "";
    public string inspectMessage = "";
    public string objectAdded = "";
    public string objectRemoved = "";



    [Header("Thoughts")]
    public string needMobileMessage = "";
    public string objectAlreadyPlaced = "";
    public string needObjectMessage = "";
    public string placedCorrectlyMessage = "";
    public string wrongObjectMessage = "";

    public string dvdCorrectMessage = "";
    public string dvdError = "";
    public string dvdMissing = "";
    public string rewardAppear = "";

    public string needSomething = "";
    public string canOpen = "";
    public string closeDoor = "";
    public string rewardCollected = "";
    public string photoCollected = "";
    public string needPieces = "";

   


}
