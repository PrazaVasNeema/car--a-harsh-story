using UnityEngine;

namespace DopeRP.CPU
{
    
    [System.Serializable]
    public class SSRSettings {
        
        
        
        
        [Header("(Just so unity don't create a new material each render call)")]
        public Material SSRMaterial;
        
    }
    
}