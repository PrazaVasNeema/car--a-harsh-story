using UnityEngine;

[CreateAssetMenu(menuName = "DopeRP/Post FX Settings")]
public class PostFXSettings : ScriptableObject
{
    [SerializeField]
    Shader shader = default;
    
    [System.Serializable]
    public struct ToneMappingSettings {

        public enum Mode { None = -1, ACES, Neutral, Reinhard }

        public Mode mode;
    }

    [SerializeField]
    ToneMappingSettings toneMapping = default;

    public ToneMappingSettings ToneMapping => toneMapping;

    [System.NonSerialized]
    Material material;

    public Material Material {
        get {
            if (material == null && shader != null) {
                material = new Material(shader);
                material.hideFlags = HideFlags.HideAndDontSave;
            }
            return material;
        }
    }
}