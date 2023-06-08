// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VertexColor_Recolor"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Brightness("_Brightness", Range( 0 , 1)) = 0
		_oppacity_Min("_oppacity_Min", Range( 0 , 0.99)) = 0
		_oppacity_Max("_oppacity_Max", Range( 0.01 , 1)) = 0
		_Hue("_Hue", Range( -1 , 1)) = 0
		_Saturation("_Saturation", Range( -1 , 1)) = 0
		_Value("_Value", Range( -1 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		Blend One OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _oppacity_Min;
		uniform float _oppacity_Max;
		uniform float _Hue;
		uniform float _Saturation;
		uniform float _Value;
		uniform float _Brightness;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode3 = tex2D( _MainTex, uv_MainTex );
			float4 break33 = tex2DNode3;
			float smoothstepResult36 = smoothstep( 0.0 , 1.0 , break33.r);
			float smoothstepResult37 = smoothstep( 0.0 , 1.0 , break33.g);
			float smoothstepResult38 = smoothstep( 0.0 , 1.0 , break33.b);
			float smoothstepResult40 = smoothstep( 0.0 , 3.0 , ( smoothstepResult36 + smoothstepResult37 + smoothstepResult38 ));
			float smoothstepResult41 = smoothstep( 0.0 , 1.0 , break33.a);
			float temp_output_42_0 = ( smoothstepResult40 * smoothstepResult41 );
			float smoothstepResult43 = smoothstep( _oppacity_Min , _oppacity_Max , temp_output_42_0);
			float smoothstepResult31 = smoothstep( _oppacity_Min , _oppacity_Max , tex2DNode3.a);
			float4 appendResult46 = (float4(tex2DNode3.rgb , ( ( temp_output_42_0 * smoothstepResult43 ) * ( tex2DNode3.a * smoothstepResult31 ) )));
			float3 hsvTorgb49 = RGBToHSV( appendResult46.xyz );
			float lerpResult58 = lerp( hsvTorgb49.x , ( hsvTorgb49.x + _Hue ) , abs( _Hue ));
			float lerpResult62 = lerp( hsvTorgb49.y , ( hsvTorgb49.y + _Saturation ) , abs( _Saturation ));
			float lerpResult65 = lerp( hsvTorgb49.z , ( hsvTorgb49.z + _Value ) , abs( hsvTorgb49.z ));
			float3 hsvTorgb50 = HSVToRGB( float3(lerpResult58,lerpResult62,lerpResult65) );
			float4 temp_output_5_0 = ( i.vertexColor * float4( hsvTorgb50 , 0.0 ) );
			float4 lerpResult20 = lerp( temp_output_5_0 , ( ( (0.0 + (_Brightness - 0.0) * (16384.0 - 0.0) / (1.0 - 0.0)) + 1.0 ) * temp_output_5_0 ) , temp_output_5_0.a);
			o.Emission = lerpResult20.rgb;
			o.Alpha = temp_output_5_0.a;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18707
324;673;1386;831;-308.1431;353.2858;1.31956;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;2;-4226.325,31.3802;Inherit;True;Property;_MainTex;_MainTex;1;0;Create;True;0;0;False;0;False;None;9de91662ea24adc4cb1be140bbd44e78;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-3909.471,133.2103;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-3677.326,33.3802;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;33;-2613.373,103.798;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SmoothstepOpNode;37;-2391.174,245.6977;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;38;-2391.174,373.6978;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;36;-2392.474,101.198;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-2200.474,101.198;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;40;-2056.474,101.198;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;41;-2386.374,495.6978;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2530.874,953.756;Inherit;False;Property;_oppacity_Max;_oppacity_Max;4;0;Create;True;0;0;False;0;False;0;1;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2530.874,873.756;Inherit;False;Property;_oppacity_Min;_oppacity_Min;3;0;Create;True;0;0;False;0;False;0;0;0;0.99;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1864.474,101.198;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;31;-1979.216,905.968;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;43;-1718.266,184.0577;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1729.391,816.9284;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1555,101.3288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1396.298,104.718;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-1244.302,26.9785;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1157.713,436.7065;Inherit;False;Property;_Saturation;_Saturation;6;0;Create;True;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;49;-1076.713,96.70644;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;51;-1161.999,294.7064;Inherit;False;Property;_Hue;_Hue;5;0;Create;True;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1162.713,603.7064;Inherit;False;Property;_Value;_Value;7;0;Create;True;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;59;-850.3036,298.7029;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-847.3036,203.7029;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-848.9703,369.0363;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;64;-851.9703,632.0363;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-851.9703,537.0363;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;61;-848.9703,464.0363;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;65;-595.9704,576.0363;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;62;-595.9704,334.0363;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;58;-605.3036,121.7029;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;50;-355.713,124.7064;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexColorNode;1;-285.9717,-134.2415;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;149.0283,-132.2415;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;487.6511,-313.7588;Inherit;False;Property;_Brightness;_Brightness;2;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;10;793.6511,-309.7588;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;16384;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;12;322.3416,-0.06338501;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WireNode;27;282.1598,-378.1847;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;48;1392.483,75.76593;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;991.1827,-248.3019;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;1186.028,-155.2415;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;28;314.1598,-394.1847;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;47;1412.942,40.42752;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;23;1500.21,115.8257;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;24;1503.604,103.9484;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;20;1438.393,-429.6986;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1624.174,-488.4446;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;VertexColor_Recolor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;3;1;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;2;2;0
WireConnection;3;0;2;0
WireConnection;3;1;25;0
WireConnection;33;0;3;0
WireConnection;37;0;33;1
WireConnection;38;0;33;2
WireConnection;36;0;33;0
WireConnection;39;0;36;0
WireConnection;39;1;37;0
WireConnection;39;2;38;0
WireConnection;40;0;39;0
WireConnection;41;0;33;3
WireConnection;42;0;40;0
WireConnection;42;1;41;0
WireConnection;31;0;3;4
WireConnection;31;1;29;0
WireConnection;31;2;30;0
WireConnection;43;0;42;0
WireConnection;43;1;29;0
WireConnection;43;2;30;0
WireConnection;32;0;3;4
WireConnection;32;1;31;0
WireConnection;44;0;42;0
WireConnection;44;1;43;0
WireConnection;45;0;44;0
WireConnection;45;1;32;0
WireConnection;46;0;3;0
WireConnection;46;3;45;0
WireConnection;49;0;46;0
WireConnection;59;0;51;0
WireConnection;57;0;49;1
WireConnection;57;1;51;0
WireConnection;60;0;49;2
WireConnection;60;1;52;0
WireConnection;64;0;49;3
WireConnection;63;0;49;3
WireConnection;63;1;53;0
WireConnection;61;0;52;0
WireConnection;65;0;49;3
WireConnection;65;1;63;0
WireConnection;65;2;64;0
WireConnection;62;0;49;2
WireConnection;62;1;60;0
WireConnection;62;2;61;0
WireConnection;58;0;49;1
WireConnection;58;1;57;0
WireConnection;58;2;59;0
WireConnection;50;0;58;0
WireConnection;50;1;62;0
WireConnection;50;2;65;0
WireConnection;5;0;1;0
WireConnection;5;1;50;0
WireConnection;10;0;9;0
WireConnection;12;0;5;0
WireConnection;27;0;5;0
WireConnection;48;0;12;3
WireConnection;26;0;10;0
WireConnection;11;0;26;0
WireConnection;11;1;5;0
WireConnection;28;0;27;0
WireConnection;47;0;48;0
WireConnection;23;0;12;3
WireConnection;24;0;23;0
WireConnection;20;0;28;0
WireConnection;20;1;11;0
WireConnection;20;2;47;0
WireConnection;0;2;20;0
WireConnection;0;9;24;0
ASEEND*/
//CHKSM=2F53A29E6B3A2CE80A70FDD0BFFF68FB2C9473BF