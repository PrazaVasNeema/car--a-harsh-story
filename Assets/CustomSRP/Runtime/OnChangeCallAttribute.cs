using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomSRP.Runtime
{

    public class OnChangedCallAttribute : PropertyAttribute
    {
        public string methodName;

        public OnChangedCallAttribute(string methodNameNoArguments)
        {
            // Debug.Log("Test2");
            methodName = methodNameNoArguments;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
    public class OnChangedCallAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log("Target Object Type: " + property.serializedObject.targetObject.GetType().FullName);
// Debug.Log(property.serializedObject.FindProperty("ssaoSettings"));
                OnChangedCallAttribute at = attribute as OnChangedCallAttribute;

                SerializedProperty targetProperty;
                Debug.Log("Test2");

                // if (!string.IsNullOrEmpty(at.propertyName))
                //     targetProperty = property.serializedObject.FindProperty(at.propertyName);
                // else
                // {
                //     targetProperty = property;
                //     Debug.Log("Test4");
                //
                // }

                Debug.Log("Test3");

                // Debug.Log(at.methodName);
                Debug.Log("Target Object Type: " + property.serializedObject.targetObject.GetType().FullName);

// Debug.Log(property.serializedObject.targetObject.GetType().GetMethods());
// Debug.Log(property.serializedObject.targetObject.GetType().GetMethod(at.methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods()
                    .Where(m => m.Name == at.methodName).First();

                if (method != null && method.GetParameters().Count() == 0) // Only instantiate methods with 0 parameters
                    method.Invoke(property.serializedObject.targetObject, null);
                
            }
        }
    }
#endif

}

