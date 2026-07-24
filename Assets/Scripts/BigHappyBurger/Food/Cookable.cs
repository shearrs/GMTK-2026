using UnityEngine;

public class Cookable : MonoBehaviour
{
    [SerializeField]
    private float cookTime = 0.0f;

    public float CookTime => cookTime;
}
