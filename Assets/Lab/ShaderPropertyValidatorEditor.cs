// ShaderPropertyValidatorEditor.cs
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShaderPropertyValidator))]
public class ShaderPropertyValidatorEditor : Editor
{
    private string propertyName = "";
    private int propertyID;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShaderPropertyValidator validator = (ShaderPropertyValidator)target;

        EditorGUILayout.LabelField("Available Properties", EditorStyles.boldLabel);
        foreach (var property in validator.availableProperties)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"ID: {property.propertyID}, Name: {property.propertyName}");
            if (GUILayout.Button("Remove"))
            {
                validator.RemoveAvailableProperty(property.propertyID);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Needed Properties", EditorStyles.boldLabel);
        foreach (var property in validator.neededProperties)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"ID: {property.propertyID}, Name: {property.propertyName}");
            if (GUILayout.Button("Remove"))
            {
                validator.RemoveNeededProperty(property.propertyID);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Add New Property", EditorStyles.boldLabel);
        propertyID = EditorGUILayout.IntField("Property ID", propertyID);
        propertyName = EditorGUILayout.TextField("Property Name", propertyName);

        if (GUILayout.Button("Add Available Property"))
        {
            validator.AddAvailableProperty(propertyID, propertyName);
        }

        if (GUILayout.Button("Add Needed Property"))
        {
            validator.AddNeededProperty(propertyID, propertyName);
        }

        if (GUILayout.Button("Validate Properties"))
        {
            validator.ValidateProperties();
        }


        EditorGUILayout.LabelField("Save/Load Properties", EditorStyles.boldLabel);
        if (GUILayout.Button("Save Properties"))
        {
            string path = EditorUtility.SaveFilePanel("Save Properties", "", "properties.dat", "dat");
            if (path.Length != 0)
            {
                validator.SaveProperties(path);
            }
        }

        if (GUILayout.Button("Load Properties"))
        {
            string path = EditorUtility.OpenFilePanel("Load Properties", "", "dat");
            if (path.Length != 0)
            {
                validator.LoadProperties(path);
            }
        }
    }
}