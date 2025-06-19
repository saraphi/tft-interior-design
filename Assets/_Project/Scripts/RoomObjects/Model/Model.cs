using UnityEngine;
using System.Collections.Generic;

public class Model : MonoBehaviour
{
    [SerializeField] private List<GameObject> children;
    [SerializeField] private BoxCollider boxCollider;

    [Header("Material Templates")]
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material fresnelMaterial;

    [Header("Color Profiles")]
    [SerializeField] private List<ColorProfile> colorProfiles;

    private Dictionary<string, ModelElement> elementsByID = new();
    private Dictionary<string, ColorProfile> colorProfilesByName = new();
    private string currentProfile = null;

    void Awake()
    {
        foreach (var child in children)
        {
            ModelElement element = child.GetComponent<ModelElement>();
            if (element == null) return;
            element.SetMaterialsTemplate(opaqueMaterial, transparentMaterial, fresnelMaterial);
            element.SetOpaque();
            elementsByID.Add(element.GetElementID(), element);
        }

        foreach (var profile in colorProfiles)
            colorProfilesByName.Add(profile.profileName, profile);

        if (colorProfiles.Count != 0) currentProfile = colorProfiles[0].profileName;

        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
    }

    public void ApplyColorProfile(string profileName)
    {
        if (profileName == null) currentProfile = colorProfiles[0].profileName;
        else currentProfile = profileName;

        if (!colorProfilesByName.TryGetValue(profileName, out var profile)) return;

        foreach (var entry in profile.entries)
        {
            if (elementsByID.TryGetValue(entry.elementID, out var element))
                element.SetColor(entry.color);
        }
    }

    public void SetModelMaterialsTransparency(bool isTransparent)
    {
        foreach (var element in elementsByID.Values)
        {
            if (isTransparent) element.SetTransparent();
            else element.SetOpaque();
        }
    }

    public void SetFresnelHighlight()
    {
        foreach (var element in elementsByID.Values) element.SetFresnel();
    }

    public void SetChildrenCollidersEnabled(bool enabled)
    {
        foreach (var element in elementsByID.Values) element.SetColliderEnabled(enabled);
    }

    public BoxCollider GetCollider() => boxCollider;
    public ColorProfile GetFurnitureColorProfile() => colorProfilesByName[currentProfile];
    public List<ColorProfile> GetColorProfiles() => colorProfiles;
}