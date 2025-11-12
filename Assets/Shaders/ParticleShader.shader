Shader "CellularSeance/Particle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Style ("Particle Style", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Style;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from center for circular particles
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // Different particle styles
                float alpha = 1.0;

                // Style 0: Circle
                if (_Style == 0)
                {
                    alpha = 1.0 - smoothstep(0.4, 0.5, dist);
                }
                // Style 1: Hard circle
                else if (_Style == 1)
                {
                    alpha = dist < 0.5 ? 1.0 : 0.0;
                }
                // Style 2: Glow
                else if (_Style == 2)
                {
                    alpha = 1.0 - (dist * 2.0);
                }
                // Style 3: Square
                else if (_Style == 3)
                {
                    alpha = 1.0;
                }

                fixed4 col = _Color;
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}
