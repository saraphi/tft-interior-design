using UnityEngine;
using Meta.XR.MRUtilityKit;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject furniturePrefab;

    public void AddFurniture(string type)
    {
        if (PlacementManager.Instance.IsUsingFurniture()) return;

        GameObject newFurniture = Instantiate(furniturePrefab, Vector3.zero, Quaternion.identity);
        
        Furniture furnitureComponent = newFurniture.GetComponent<Furniture>();
        furnitureComponent.SetSceneLabel(GetSceneLabelFromType(type));
        
        SoundManager.Instance.PlayPressSound();
    }

    private MRUKAnchor.SceneLabels GetSceneLabelFromType(string type)
    {
        return type switch
        {
            "floor" => MRUKAnchor.SceneLabels.FLOOR,
            "wall" => MRUKAnchor.SceneLabels.WALL_FACE,
            "ceiling" => MRUKAnchor.SceneLabels.CEILING,
            _ => MRUKAnchor.SceneLabels.OTHER
        };
    }
}