Shader "Custom/DepthDisplacement"
{
    Properties
    {

        _Strip ("Previous Strip", 2D) = "white" {} 
        _CameraView ("Camera input", 2D) = "white" {}
        _StripWidth ("Strip Width", Float) = 1.0   
        _ScreenWidth ("Screen Width", Float) = 1920
        _ScreenHeight ("Screen Height", Float) = 1080
        _DepthFactor ("Depth Factor", Float) = 1.0   
        _OffsetX ("Horizontal Offset", Float) = 0.0  
        _numStrips ("Number of Strips", Float) = 0.0
        _CurrentStrip ("Current Strip", Float) = 0.0 
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Strip;
            sampler2D _CameraView;
            sampler2D _CameraDepthTexture;
            float _DepthFactor;
            float _StripWidth;
            float _ScreenWidth;
            float _ScreenHeight;
            float _OffsetX;
            float _numStrips;
            float _CurrentStrip;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.uv.x -= _OffsetX;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {   
                float aspectRatioFactor = _ScreenWidth / _StripWidth;
                float2 depth_uv = i.uv;
                depth_uv.x += _CurrentStrip * 0.5;
                //depth_uv.x += 0.5;

                float depth = tex2D(_CameraDepthTexture, depth_uv).r;
                
                // Early exit for fragments outside the aspect ratio area
                if (i.uv.x > aspectRatioFactor || i.uv.x < 0.0 || i.uv.y < 0.0 || i.uv.y > 1.0) 
                {
                    return fixed4(0, 0, 0, 0); // Transparent outside bounds
                }

                // Adjust the UV only for sampling
                float2 adjustedUV = i.uv;
                adjustedUV.x = clamp(adjustedUV.x * aspectRatioFactor, 0.0, 1.0);
                
                // Offset UV for depth displacement
                adjustedUV.x -= depth * _DepthFactor;

                // Sample the strip texture
                fixed4 col = tex2D(_Strip, adjustedUV);
                
                // Debugging modes
                //col = tex2D(_CameraView, i.uv);
                //col = tex2D(_CameraDepthTexture, i.uv);

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
