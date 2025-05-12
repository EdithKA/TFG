using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameTexts")]
public class GameTexts : ScriptableObject
{
    [Header("Interaction Messages")]
    public string interactMessage = "Pulsa [E] para interactuar";
    public string collectMessage = "Pulsa [E] para recoger";
    public string collectedMessage = "¡Objeto recogido!";
    public string wrongObjectMessage = "Parece que esto no va aqui";

    [Header("VideoClub Messages")]
    


    [Header("Thoughts")]
    public string needMobileMessage = "Parece que necesito recoger algo antes.";
    public string objectAlreadyPlaced = "Ya hay un objeto aquí.";
    public string needObjectMessage = "Tengo que colocar algo aquí.";
    public string placedCorrectlyMessage = "Definitivamente esto va aquí.";
    public string dvdCorrectMessage = "¡DVD correcto! Iniciando juego...";
    public string dvdError = "DVD incorrecto";
    public string dvdMissing = "Necesitas un DVD";




}
