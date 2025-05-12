using UnityEngine;

public abstract class FurnitureGizmoInteractor : MonoBehaviour
{

    [SerializeField] protected Furniture furniture;

    public virtual void ActivateGizmo()
    {
        gameObject.SetActive(true);
    }

    public virtual void DeactivateGizmo()
    {
        gameObject.SetActive(false);
    }
}