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
    // Список доступных свойств
    private List<ShaderProperty> availableProperties = new List<ShaderProperty>();

    // Список необходимых свойств
    private List<ShaderProperty> neededProperties = new List<ShaderProperty>();

    // Добавление доступного свойства
    public void AddAvailableProperty(int propertyID, string propertyName)
    {
        ShaderProperty property = new ShaderProperty(propertyID, propertyName);
        if (!availableProperties.Exists(p => p.propertyID == propertyID))
        {
            availableProperties.Add(property);
        }
    }

    // Метод для симуляции валидации с добавлением списков и вызовом валидации
    public void SimulateValidation()
    {
        // Очистка списков для демонстрации
        availableProperties.Clear();
        neededProperties.Clear();

        // Добавление доступных свойств
        AddAvailableProperty(Shader.PropertyToID("SProps_SSAO_NoiseScale"), "SProps_SSAO_NoiseScale");
        AddAvailableProperty(Shader.PropertyToID("SProps_SSAO_Radius"), "SProps_SSAO_Radius");

        // Добавление необходимых свойств
        neededProperties.Add(new ShaderProperty(Shader.PropertyToID("SProps_SSAO_NoiseScale"), "SProps_SSAO_NoiseScale"));
        neededProperties.Add(new ShaderProperty(Shader.PropertyToID("SProps_SSAO_Intensity"), "SProps_SSAO_Intensity"));

        // Вызов метода валидации
        ValidateProperties();
    }

    // Проверка необходимых свойств
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
