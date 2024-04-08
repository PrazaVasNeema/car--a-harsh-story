using System;
using System.Collections;
using System.Collections.Generic;
using DopeRP.CPU;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "DopeRP/PostFX/FXAA")]
public partial class FX_FXAA : FX_Feature
{

    
    public override void SetupUniforms()
    {

    }

    public override void Render(int sourceRT, int targetRT, PostFXSettings generalFXSettings)
    {
        
        RAPI.Draw(sourceRT,targetRT, PostFXStack.Pass.FXAA, generalFXSettings.Material);

    }
    
}