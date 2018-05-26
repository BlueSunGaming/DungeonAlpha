Shader "Projector/Additive Tinted" { 

    Properties { 
        _Color ("Main Color", Color) = (1,0.5,0.5,1) 
        _ShadowTex ("Cookie", 2D) = "" {  } 
        _FalloffTex ("FallOff", 2D) = "" {  } 

    } 
    
    Subshader { 
        Pass { 
            ZWrite off 
            Fog { Color (1, 1, 1) } 
            ColorMask RGB 
            Blend One One 
            SetTexture [_ShadowTex] { 
                constantColor [_Color]
                combine texture * constant, ONE - texture 

            } 
            SetTexture [_FalloffTex] { 
                constantColor (0,0,0,0) 
                combine previous lerp (texture) constant 

            } 

        } 

    } 

}