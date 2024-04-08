using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DopeRP/Post FX Settings")]
public class PostFXSettings : ScriptableObject
{
    public bool m_FXAA_ON;
    public bool m_vignette_on;
    [Range(0,4)]
    public float m_vignette_power = 1;
    [Range(0,4)]
    public float m_vignette_offset = 1;

    public List<FX_Feature> currentFXFeaturesList;
    
    [SerializeField]
    Shader shader = default;

    [Serializable]
    public struct ColorAdjustmentsSettings
    {
        public float postExposure;

        [Range(-100f, 100f)]
        public float contrast;

        [ColorUsage(false, true)]
        public Color colorFilter;

        [Range(-180f, 180f)]
        public float hueShift;

        [Range(-100f, 100f)]
        public float saturation;
    }

    [SerializeField]
    ColorAdjustmentsSettings colorAdjustments = new ColorAdjustmentsSettings {
        colorFilter = Color.white
    };

    public ColorAdjustmentsSettings ColorAdjustments => colorAdjustments;
    
    [System.Serializable]
    public struct ToneMappingSettings {

        public enum Mode { None = 0, ACES = 1, Neutral = 2, Reinhard = 3 }

        public Mode mode;
    }

    [SerializeField]
    ToneMappingSettings toneMapping = default;

    public ToneMappingSettings ToneMapping => toneMapping;
    
    [Serializable]
    public struct WhiteBalanceSettings {

        [Range(-100f, 100f)]
        public float temperature, tint;
    }

    [SerializeField]
    WhiteBalanceSettings whiteBalance = default;

    public WhiteBalanceSettings WhiteBalance => whiteBalance;
    
    
    [Serializable]
    public struct SplitToningSettings {

        [ColorUsage(false)]
        public Color shadows, highlights;

        [Range(-100f, 100f)]
        public float balance;
    }

    [SerializeField]
    SplitToningSettings splitToning = new SplitToningSettings {
        shadows = Color.gray,
        highlights = Color.gray
    };

    public SplitToningSettings SplitToning => splitToning;
    
    
    [Serializable]
    public struct ChannelMixerSettings {

        public Vector3 red, green, blue;
    }
	
    [SerializeField]
    ChannelMixerSettings channelMixer = new ChannelMixerSettings {
        red = Vector3.right,
        green = Vector3.up,
        blue = Vector3.forward
    };

    public ChannelMixerSettings ChannelMixer => channelMixer;
    
    
    [Serializable]
    public struct ShadowsMidtonesHighlightsSettings {

        [ColorUsage(false, true)]
        public Color shadows, midtones, highlights;

        [Range(0f, 2f)]
        public float shadowsStart, shadowsEnd, highlightsStart, highLightsEnd;
    }

    [SerializeField]
    ShadowsMidtonesHighlightsSettings
        shadowsMidtonesHighlights = new ShadowsMidtonesHighlightsSettings {
            shadows = Color.white,
            midtones = Color.white,
            highlights = Color.white,
            shadowsEnd = 0.3f,
            highlightsStart = 0.55f,
            highLightsEnd = 1f
        };

    public ShadowsMidtonesHighlightsSettings ShadowsMidtonesHighlights =>
        shadowsMidtonesHighlights;
    

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