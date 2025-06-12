using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FurnitureSelectorCanvas furnitureSelector;
    [SerializeField] private LayerMask collisionMask;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        furnitureSelector.gameObject.SetActive(false);
    }

    void Update()
    {
        if (ControllerManager.Instance.OnMenu())
            if (!furnitureSelector.gameObject.activeInHierarchy)
            {
                PositionFurnitureSelectorCanvas();
                furnitureSelector.gameObject.SetActive(true);
                SoundManager.Instance.PlayEnterClip();
            }
            else
            {
                furnitureSelector.gameObject.SetActive(false);
                SoundManager.Instance.PlayExitClip();
            }
    }

    private void PositionFurnitureSelectorCanvas()
    {
        Vector3 eyeLevelPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        eyeLevelPos.y = Camera.main.transform.position.y;

        Quaternion rotation = Quaternion.LookRotation(eyeLevelPos - Camera.main.transform.position);

        Vector3 finalPos = eyeLevelPos;
        float radius = 0.5f;
        int attempts = 10;
        float step = 0.2f;

        while (Physics.CheckSphere(finalPos, radius, collisionMask) && attempts > 0)
        {
            finalPos += Camera.main.transform.right * step;
            attempts--;
        }

        furnitureSelector.transform.position = finalPos;
        furnitureSelector.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}