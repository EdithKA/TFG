using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameTexts")]
public class GameTexts : ScriptableObject
{
    [Header("Interaction Messages")]
    public string interactMessage = "";
    public string collectMessage = "";
    public string collectedMessage = "";

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

   


}
