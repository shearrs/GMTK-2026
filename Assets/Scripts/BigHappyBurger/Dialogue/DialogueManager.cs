using UnityEngine;
using UnityEngine.UIElements;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private DialogueData  data;

    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer _renderer, VisualElement _rootElement)
    {
        _rootElement.Q<Button>("ButtonCompliment").clicked += OnClickCompliment;
        _rootElement.Q<Button>("ButtonWeather"   ).clicked += OnClickWeather;
        _rootElement.Q<Button>("ButtonUpdate"    ).clicked += OnClickUpdate;
        _rootElement.Q<Button>("ButtonPlans"     ).clicked += OnClickPlans;
        
        data.dialogueText = "Something else.";
    }

    void OnClickCompliment()
    {
        UnityEngine.Debug.Log("OnClickCompliment");
    }
    void OnClickWeather()
    {
        UnityEngine.Debug.Log("OnClickWeather");
    }
    void OnClickUpdate()
    {
        UnityEngine.Debug.Log("OnClickUpdate");
    }
    void OnClickPlans()
    {
        UnityEngine.Debug.Log("OnClickPlans");
    }
}
