using UnityEngine;

public class ReScanCanvas : MonoBehaviour
{
    void Update()
    {
        if (gameObject.activeInHierarchy) GameManager.Instance.SetEffectMeshHideMesh(false);
    }

    public void OnStartScanner()
    {
        SoundManager.Instance.PlayEnterClip();
        GameManager.Instance.ScanScene();
        GameManager.Instance.CloseCurrentCanvas();
    }

    public void OnCancelScanner()
    {
        SoundManager.Instance.PlayEnterClip();  
        if (gameObject.activeInHierarchy) GameManager.Instance.CloseCurrentCanvas();
    }
}
