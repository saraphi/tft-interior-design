using System.Collections.Generic;
using UnityEngine;

public abstract class Model : MonoBehaviour
{
    [SerializeField] protected List<GameObject> children;
    [SerializeField] private BoxCollider objectCollider;

    [Header("Material Templates")]
    [SerializeField] protected Material opaqueMaterial;
    [SerializeField] protected Material transparentMaterial;
    [SerializeField] protected Material fresnelMaterial;

    [Header("Color Profiles")]
    [SerializeField] protected List<ColorProfile> colorProfiles;

    protected Dictionary<string, ModelElement> elementsByID = new();
    protected Dictionary<string, ColorProfile> colorProfilesByName = new();
    protected string currentProfile = null;

    protected void Awake()
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

        if (objectCollider == null) objectCollider = GetComponent<BoxCollider>();
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

    public BoxCollider GetCollider() => objectCollider;
    public ColorProfile GetFurnitureColorProfile() => colorProfilesByName[currentProfile];
    public List<ColorProfile> GetColorProfiles() => colorProfiles;
}