using TMPro;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text textPro;

    public static DebugCanvas Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void AddNewLine(string line)
    {
        textPro.text += line;
    }

    public void ClearText() => textPro.text = "";
}
