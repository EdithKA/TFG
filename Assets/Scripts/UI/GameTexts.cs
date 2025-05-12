using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameTexts")]
public class GameTexts : ScriptableObject
{
    [Header("Interaction Messages")]
    public string needMobileMessage = "Parece que necesito recoger algo antes";
    public string interactMessage = "Pulsa [E] para interactuar";
    public string collectMessage = "Pulsa [E] para recoger";
    public string collectedMessage = "¡Objeto recogido!";
    public string needObjectMessage = "Necesitas un objeto para colocar aquí";
    public string wrongObjectMessage = "Parece que esto no va aqui";
    public string objectAlreadyPlaced = "Ya hay un objeto aquí";

    [Header("VideoClub Messages")]
    public string placedCorrectlyMessage = "¡Objeto colocado correctamente!";
    public string dvdCorrectMessage = "¡DVD correcto! Iniciando juego...";
    public string dvdError = "DVD incorrecto";
    public string dvdMissing = "Necesitas un DVD";


}
