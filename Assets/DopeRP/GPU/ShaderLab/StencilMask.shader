Shader "DopeRP/Shaders/StencilMask"
{
	Properties
	{
		[InRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
	}

	SubShader
	{
		
		Tags { "RenderType"="Opaque" "LightMode"="gfg"}
		
		Pass {
			Blend Zero One
			ZWrite off
			
			Stencil
			{
				ref [_StencilID]
				Comp Always
				Pass Replace
			}
		}

	}
}