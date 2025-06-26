
using TMPro;
using UnityEngine;

public class DecorationAxisCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;

    void Start()
    {
        tMP_Text.text = "Y Axis";
    }

    public void SetAxis(Decoration.DecorationRotationAxis rotationAxis)
    {
        tMP_Text.text = rotationAxis.ToString() + " Axis";
    }
}