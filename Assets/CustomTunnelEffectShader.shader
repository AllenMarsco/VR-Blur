Shader "Custom/TunnelEffectShader_BuiltIn"
{
Properties
{
_ColorInner ("Inner Color", Color) = (0,0,0,1)
_ColorOuter ("Outer Color", Color) = (1,1,1,1)
_MinRadius ("Min Radius", Float) = 0.2
_MaxRadius ("Max Radius", Float) = 0.8
_Alpha ("Alpha Strength", Float) = 1.0
_Direction ("Direction", Vector) = (0,0,1,0) 
_PlaneDistance ("Plane Distance", Float) = 1.0
}
SubShader
{
Tags { "Queue"="Overlay" "RenderType"="Transparent" }
LOD 100

    Pass
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        CGPROGRAM
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float3 worldPos : TEXCOORD1;
        };

        float4 _ColorInner;
        float4 _ColorOuter;
        float _MinRadius;
        float _MaxRadius;
        float _Alpha;
        float4 _Direction;
        float _PlaneDistance;

        
        v2f vert (appdata_t v)
        {
            v2f o;

           
            v.vertex.z += _PlaneDistance;

            
            o.vertex = UnityObjectToClipPos(v.vertex);

            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            o.uv = v.uv;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
           
            float3 dirToPixel = normalize(i.worldPos - float3(0.0, 0.0, 0.0));
            float maskDir = dot(dirToPixel, normalize(_Direction.xyz)); 

            float2 uv = i.uv * 2.0 - 1.0;
            float dist = length(uv);

           
            float mask = smoothstep(_MinRadius, _MaxRadius, dist);

          
            float4 color = lerp(_ColorInner, _ColorOuter, mask);

          
            color.a *= _Alpha;

            return color;
        }
        ENDCG
    }
}
}