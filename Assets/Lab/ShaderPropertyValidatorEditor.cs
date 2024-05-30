// ShaderPropertyValidatorEditor.cs
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShaderPropertyValidator))]
public class ShaderPropertyValidatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShaderPropertyValidator validator = (ShaderPropertyValidator)target;

        EditorGUILayout.LabelField("Available Properties", EditorStyles.boldLabel);
        foreach (var property in validator.availableProperties)
        {
            EditorGUILayout.LabelField($"ID: {property.propertyID}, Name: {property.propertyName}");
        }

        EditorGUILayout.LabelField("Needed Properties", EditorStyles.boldLabel);
        foreach (var property in validator.neededProperties)
        {
            EditorGUILayout.LabelField($"ID: {property.propertyID}, Name: {property.propertyName}");
        }

        if (GUILayout.Button("Validate Properties"))
        {
            validator.ValidateProperties();
        }
        
    }
}