Shader "Hidden/Shader_CenterGradient"
{
    Properties
    {
        _ColorCenter ("Center Color", Color) = (1, 1, 1, 1)
        _ColorBorder ("Border Color", Color) = (0, 0, 0, 1)
        _Transparency ("Transparency", float) = 1.0
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "Sprite"="Default" }
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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            fixed4 _ColorCenter;
            fixed4 _ColorBorder;
            float _Transparency;
            sampler2D _MainTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Calculate the distance of the UV from the center (0.5, 0.5)
                float2 center = float2(0.5, 0.5);
                float distanceFromCenter = length(i.uv - center);

                // Blend between the center color and border color based on distance
                fixed4 color = lerp(_ColorCenter, _ColorBorder, distanceFromCenter * 2.0);

                // Apply transparency
                color.a *= _Transparency;

                // Combine with the sampled texture color
                return color * texColor * i.color; // Multiply by vertex color for sprites
            }
            ENDCG
        }
    }
}
