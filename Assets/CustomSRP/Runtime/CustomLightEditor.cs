using UnityEngine;
using UnityEditor;

namespace CustomSRP.Runtime
{


    [CanEditMultipleObjects]
    [CustomEditorForRenderPipeline(typeof(Light), typeof(CustomRenderPipelineAsset))]
    public class CustomLightEditor : LightEditor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            if (
                !settings.lightType.hasMultipleDifferentValues &&
                (LightType)settings.lightType.enumValueIndex == LightType.Spot
            )
            {
                settings.DrawInnerAndOuterSpotAngle();
                settings.ApplyModifiedProperties();
            }
        }
    }

    
}