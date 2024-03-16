using UnityEngine;

namespace CustomSRP.Runtime
{
    [System.Serializable]
    public class SSAOSettings {
        
        public Texture2D normalMapTexture;

        public float randomSize;
        public float gSampleRad;
        public float gIntensity;
        public float gScale;
        public float gBias;

    }
}