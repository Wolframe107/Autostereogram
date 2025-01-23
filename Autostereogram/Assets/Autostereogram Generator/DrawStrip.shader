Shader "Custom/DrawStrip"
{
    Properties
    {

        _Strip ("Previous Strip", 2D) = "white" {} 
        _StripWidth ("Strip Width", Float) = 1.0   
        _ScreenWidth ("Screen Width", Float) = 1920
        _OffsetX ("Horizontal Offset", Float) = 0.0  
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
            float _StripWidth;
            float _ScreenWidth;
            float _OffsetX;

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

                // Early exit for fragments outside the aspect ratio area
                if (i.uv.x > aspectRatioFactor || i.uv.x < 0.0 || i.uv.y < 0.0 || i.uv.y > 1.0) 
                {
                    return fixed4(0, 0, 0, 0);
                }

                // Adjust the UV only for sampling
                float2 adjustedUV = i.uv;
                adjustedUV.x = clamp(adjustedUV.x * aspectRatioFactor, 0.0, 1.0);

                // Sample the strip texture
                fixed4 col = tex2D(_Strip, adjustedUV);

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
