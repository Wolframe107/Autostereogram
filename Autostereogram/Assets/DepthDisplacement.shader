Shader "Custom/DepthDisplacement"
{
    Properties
    {
        _Strip ("Previous Strip", 2D) = "white" {} 
        _CameraView ("Camera input", 2D) = "white" {}
        _StripWidth ("Strip Width", Float) = 1.0   
        _ScreenWidth ("Screen Width", Float) = 1920
        _DepthFactor ("Depth Factor", Float) = 1.0   
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
            sampler2D _CameraView;
            sampler2D _CameraDepthTexture;
            float _DepthFactor;
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
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {       
                float2 uv = i.uv;
                float2 offsetUV = uv;
                offsetUV.x = uv.x + _OffsetX;

                // Adjust UV coordinates to sample only the part of the depth texture corresponding to the current strip
                float stripStart = _OffsetX;
                float stripEnd = stripStart + _StripWidth / _ScreenWidth;
                float2 depth_uv = i.uv;
                depth_uv.x = lerp(stripStart, stripEnd, depth_uv.x);

                float depth = tex2D(_CameraDepthTexture, depth_uv).r;
                
                float displacement = depth * _DepthFactor;

                uv.x -= displacement;
                uv.x = frac(uv.x);

                fixed4 col = tex2D(_Strip, uv);
                
                // Debugging modes
                //col = tex2D(_CameraView, i.uv);
                //col = tex2D(_CameraDepthTexture, depth_uv);

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
