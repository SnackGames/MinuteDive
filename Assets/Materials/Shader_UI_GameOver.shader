Shader "Hidden/Shader_UI_GameOver"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (1, 0, 0, 1)
        _ColorMiddle ("Middle Color", Color) = (0, 1, 0, 1)
        _ColorBottom ("Bottom Color", Color) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _ColorTop;
            fixed4 _ColorMiddle;
            fixed4 _ColorBottom;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the gradient factor based on the UV coordinate
                float gradientFactor = i.uv.y;

                // Interpolate between the colors for a tri-gradient
                fixed4 color;
                if (gradientFactor < 0.5)
                {
                    float t = gradientFactor * 2.0;
                    color = lerp(_ColorBottom, _ColorMiddle, t);
                }
                else
                {
                    float t = (gradientFactor - 0.5) * 2.0;
                    color = lerp(_ColorMiddle, _ColorTop, t);
                }

                return color;
            }
            ENDCG
        }
    }
}
