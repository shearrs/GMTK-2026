using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string dialogueText    = "Default disgruntled driver";

    public string introduction    = "Default welcome";

    public string replyCompliment = "Default compliment";
    public string replyUpdate     = "Default update";
    public string replyWeather    = "Default weather";
    public string replyPlans      = "Default plans";
}
