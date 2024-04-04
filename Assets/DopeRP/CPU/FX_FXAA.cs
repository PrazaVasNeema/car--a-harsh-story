using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_FXAA : FX_Feature
{
    public override bool SetupUniforms()
    {
        return true;
    }

    public override bool Render(int sourceRT, int targetRT)
    {
        return true;
    }
    
    
}
