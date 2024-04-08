using System;
using System.Collections;
using System.Collections.Generic;
using DopeRP.CPU;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "DopeRP/PostFX/Vignette")]
public partial class FX_Vignette : FX_Feature
{
    [Range(0,4)]
    public float m_vignette_power = 1;
    [Range(0,4)]
    public float m_vignette_offset = 1;
    
    
    public override void SetupUniforms()
    {
        var vignetteSettings = new Vector2(m_vignette_offset, m_vignette_power);
        RAPI.Buffer.SetGlobalVector(SProps.PostFX.VignetteSettings, vignetteSettings);
    }

    public override void Render(int sourceRT, int targetRT, PostFXSettings generalFXSettings)
    {
        
       RAPI.Draw(sourceRT,targetRT, PostFXStack.Pass.Vignette, generalFXSettings.Material);

    }
    
}
