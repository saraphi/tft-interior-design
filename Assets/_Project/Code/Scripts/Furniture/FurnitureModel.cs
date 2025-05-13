using UnityEngine;

public class FurnitureModel : MonoBehaviour
{
    [SerializeField] private string modelName;

    public string GetName() => modelName;
}