using UnityEngine;

public abstract class RayInteractor : MonoBehaviour
{
    [SerializeField] public float maxRayLength = 2f;
    [SerializeField] protected float smoothTime = 0.1f;
    [SerializeField] protected float rotationDegrees = 0.9f;

    protected Vector3 velocity = Vector3.zero;
    protected Rigidbody rb;
    protected Vector3 lastValidPosition;
    protected Quaternion lastValidRotation;
    protected bool hasValidPosition = false;
    protected bool wouldCollide = false;

    public abstract bool Move();
    protected abstract Quaternion GetAddiotionalRotation();
}