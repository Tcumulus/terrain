Shader "Unlit/Contours"
{
    Properties{
        _HueShift("HueShift", Float) = 0
        _HueScale("HueScale", Float) = 0.1

        _LineFillRatio("LineFillRatio", Range(-0.001, 1)) = 0.3
        _LineScale("LineScale", Float) = 1

        _Brightness("Brightness", Range(-0.001, 2)) = 0.7
        _Contrast("Contrast", Range(-0.001, 1.5)) = 1
    }
        SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _HueShift;
            float _HueScale;
            float _LineFillRatio;
            float _LineScale;
            float _Brightness;
            float _Contrast;

            half4 hsv_to_rgb(float3 HSV)
            {
                half4 RGB = HSV.z;

                        float var_h = HSV.x * 6;
                        float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
                        float var_1 = HSV.z * (1.0 - HSV.y);
                        float var_2 = HSV.z * (1.0 - HSV.y * (var_h - var_i));
                        float var_3 = HSV.z * (1.0 - HSV.y * (1 - (var_h - var_i)));
                        if (var_i == 0) { RGB = half4(HSV.z, var_3, var_1,1); }
                        else if (var_i == 1) { RGB = half4(var_2, HSV.z, var_1,1); }
                        else if (var_i == 2) { RGB = half4(var_1, HSV.z, var_3,1); }
                        else if (var_i == 3) { RGB = half4(var_1, var_2, HSV.z,1); }
                        else if (var_i == 4) { RGB = half4(var_3, var_1, HSV.z,1); }
                        else { RGB = half4(HSV.z, var_1, var_2, 1); }

                return (RGB);
            }


            struct v2f {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;

                //fixed4 color : COLOR;
            };



            v2f vert(appdata_base v)
            {
                v2f o;

                o.localPos = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                half4 col = hsv_to_rgb(float3((i.localPos.y * _HueScale + 100000 + _HueShift) % 1.0, 1, 1));
                half4 lines = round((i.localPos.y * _LineScale + 100000) % 1.0 - _LineFillRatio + 0.5) * half4(1, 1, 1, 1);
                half4 normals = ((i.worldNormal.z) * _Contrast + _Brightness) * half4(1,1,1,1);

                return lines * col * normals;
            }

            ENDCG
        }
    }
}