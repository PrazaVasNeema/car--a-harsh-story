using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShaderPropertyValidator))]
public class ShaderPropertyValidatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Отрисовка стандартного инспектора
        DrawDefaultInspector();

        // Получение ссылки на целевой объект
        ShaderPropertyValidator validator = (ShaderPropertyValidator)target;

        // Добавление кнопки "Validate Properties"
        if (GUILayout.Button("Validate Properties"))
        {
            // Вызов метода валидации
            validator.ValidateProperties();
        }

        // Добавление кнопки "Simulate Validation"
        // if (GUILayout.Button("Simulate Validation"))
        // {
        //     // Вызов метода симуляции валидации
        //     validator.SimulateValidation();
        // }
    }
}
