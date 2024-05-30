// ShaderPropertyValidator.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ShaderProperty
{
    public int propertyID;
    public string propertyName;

    public ShaderProperty(int id, string name)
    {
        propertyID = id;
        propertyName = name;
    }
}

public class ShaderPropertyValidator : MonoBehaviour
{
    [HideInInspector]
    public List<ShaderProperty> availableProperties = new List<ShaderProperty>();
    public List<ShaderProperty> neededProperties = new List<ShaderProperty>();

    public void AddAvailableProperty(int propertyID, string propertyName)
    {
        ShaderProperty property = new ShaderProperty(propertyID, propertyName);
        if (!availableProperties.Exists(p => p.propertyID == propertyID))
        {
            availableProperties.Add(property);
            Debug.Log($"Added available property: {propertyName} (ID: {propertyID})");
        }
    }

    public void DetermineSettings()
    {
        int noiseScaleID = Shader.PropertyToID("SProps_SSAO_NoiseScale");
        int radiusID = Shader.PropertyToID("SProps_SSAO_Radius");

        AddAvailableProperty(noiseScaleID, "SProps_SSAO_NoiseScale");
        AddAvailableProperty(radiusID, "SProps_SSAO_Radius");

        Shader.SetGlobalVector(noiseScaleID, new Vector4(0.5f, 0.5f, 0f, 0f));
    }

    public void SetupNeededProperties()
    {
        neededProperties.Clear();
        neededProperties.Add(new ShaderProperty(Shader.PropertyToID("SProps_SSAO_NoiseScale"), "SProps_SSAO_NoiseScale"));
        neededProperties.Add(new ShaderProperty(Shader.PropertyToID("SProps_SSAO_Intensity"), "SProps_SSAO_Intensity"));
    }

    public void SimulateValidation()
    {
        DetermineSettings();
        SetupNeededProperties();
        ValidateProperties();
    }

    public void ValidateProperties()
    {
        List<ShaderProperty> missingProperties = new List<ShaderProperty>();

        foreach (var property in neededProperties)
        {
            if (!availableProperties.Exists(p => p.propertyID == property.propertyID))
            {
                missingProperties.Add(property);
            }
        }

        if (missingProperties.Count > 0)
        {
            string missingProps = string.Join(", ", missingProperties.ConvertAll(p => p.propertyName));
            Debug.LogError($"Missing shader properties: {missingProps}");
        }
        else
        {
            Debug.Log("All required shader properties are available.");
        }
    }
}
