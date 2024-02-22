using UnityEngine;

namespace CustomSRP.Runtime
{
    [System.Serializable]
    public class ShadowSettings {

        [Min(0f)]
        public float maxDistance = 100f;
    
        public enum TextureSize {
            _256 = 256, _512 = 512, _1024 = 1024,
            _2048 = 2048, _4096 = 4096, _8192 = 8192
        }
        
        public enum FilterMode {
            PCF2x2, PCF3x3, PCF5x5, PCF7x7
        }
    
        [System.Serializable]
        public struct Directional {

            public TextureSize atlasSize;
            public FilterMode filter;
        }

        public Directional directional = new Directional {
            atlasSize = TextureSize._1024,
            filter = FilterMode.PCF2x2,
        };
    }
}