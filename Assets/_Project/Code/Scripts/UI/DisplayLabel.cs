using UnityEngine;
using Meta.XR.MRUtilityKit;
using TMPro;

public class DisplayLabel : MonoBehaviour
{
    public Transform rayStartPoint;
    public float rayLength = Mathf.Infinity;
    public MRUKAnchor.SceneLabels labelFlags;
    public TextMeshPro debugText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        bool hasHit = room.Raycast(ray, rayLength, new LabelFilter(labelFlags), out RaycastHit hit, out MRUKAnchor anchor);

        if (hasHit)
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;

            string label = anchor.Label.ToString(); 

            if (debugText != null)
            {
                debugText.transform.position = hitPoint;
                debugText.transform.rotation = Quaternion.LookRotation(-hitNormal);
                debugText.text = "ANCHOR : " + label;
            }
        }
    }
}
