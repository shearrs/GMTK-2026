using UnityEngine;

public class Cookable : MonoBehaviour
{
    [SerializeField]
    private int cookTime = 0;

    public int CookTime => cookTime;
}
