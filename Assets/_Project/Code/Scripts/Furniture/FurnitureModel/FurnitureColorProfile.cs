using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FurnitureColorProfile
{
    public string profileName;
    public Gradient colorIdentifier;
    public List<FurnitureColorEntry> entries = new();
}

[Serializable]
public class FurnitureColorEntry
{
    public string elementID;
    public Color color;
}