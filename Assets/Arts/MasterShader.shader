Shader "Custom/MasterSlotGame"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Sprite Sheet Texture", 2D) = "white" {}
		[NoScaleOffset] _SlotBackTexture("Slot Back Texture", 2D) = "white" {}
		[NoScaleOffset] _WindowTexture("Slot Window Texture", 2D) = "white" {}
		_VisibleUnder("Max Y Position For Rendering Slot Pieces", Float) = 2.376
		_VisibleAbove("Min Y Position For Rendering Slot Pieces", Float) = -2.466

		[HideInInspector] _SlotMachineEnabled("Is Slot Window Enabled", Float) = 0
		[HideInInspector] _ImageIndex("Image Index", int) = 14
		[HideInInspector] _BlurSize("Blur Amount", Float) = 0.5
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			UNITY_INSTANCING_BUFFER_START(Properties)
				UNITY_DEFINE_INSTANCED_PROP(int, _ImageIndex)
				UNITY_DEFINE_INSTANCED_PROP(float, _BlurSize)
				UNITY_DEFINE_INSTANCED_PROP(float, _SlotMachineEnabled)
			UNITY_INSTANCING_BUFFER_END(Properties)


			v2f vert(appdata_t IN)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _SlotBackTexture;
			sampler2D _WindowTexture;

			float _VisibleUnder;
			float _VisibleAbove;

			fixed4 frag(v2f IN) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);

				if(UNITY_ACCESS_INSTANCED_PROP(Properties, _SlotMachineEnabled) > 0) // Rendering SlotWindow And SlotBack
				{
					fixed4 c = tex2D(_SlotBackTexture, IN.texcoord);
					fixed4 cWindow = tex2D(_WindowTexture, IN.texcoord);

					return lerp(c,cWindow,cWindow.a);
				}
				else
				{
					if(IN.worldPos.y > _VisibleUnder || IN.worldPos.y < _VisibleAbove) //Masking For Slot Pieces
					{
						return 0;
					}
					else 
					{
						//tileCount is actualy how many sprites in one axis on our spritesheet ... So 3x3 = 9 total sprites in our spriteSheet
						float tileCount = 3.0; 
						//calcCoord is the calculated UV coordinates according to tileCount and ImageIndex
						//basically it divides 0 to 1 uv coordinate space to tileCount. in our example 1 divided by 3 is 0.333..
						//If we want to get 4th image simply 5 divided by 3 is 1.6666 .. whole number is 1 which is y coordinate multiplier
						//Mod 3 of 5 is 2 which is x coordinate. So our uv is starting from (2 * 0.333, 1 * 0.333) to (IN.texcoord(UV) / tileCount, IN.texcoord(UV) / tileCount)
						float2 calcCoord = float2(fmod(UNITY_ACCESS_INSTANCED_PROP(Properties, _ImageIndex) , tileCount) / tileCount 
							, 1 - (floor(UNITY_ACCESS_INSTANCED_PROP(Properties, _ImageIndex)/ tileCount) + 1) * 1 / tileCount ) + IN.texcoord / tileCount;

						if( UNITY_ACCESS_INSTANCED_PROP(Properties, _BlurSize) > 0) // Rendering Motion Blurred Slot Piece
						{
							float blurSize = UNITY_ACCESS_INSTANCED_PROP(Properties, _BlurSize) * 0.008;

							fixed4 sum = fixed4(0.0, 0.0, 0.0, 0.0);
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y - 4.0 * blurSize)) * 0.05;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y - 3.0 * blurSize)) * 0.09;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y - 2.0 * blurSize)) * 0.12;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y - blurSize)) * 0.15;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y)) * 0.16;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y + blurSize)) * 0.15;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y + 2.0 * blurSize)) * 0.12;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y + 3.0 * blurSize)) * 0.09;
							sum += tex2D(_MainTex, half2(calcCoord.x, calcCoord.y + 4.0 * blurSize)) * 0.05;
							
							sum.rgb *= sum.a;
							return sum;
						}
						else // Rendering Slot Piece
						{
							fixed4 c =  tex2D(_MainTex, calcCoord);
							c.rgb *= c.a;
							return c;
						}
					}
				}
			}
			ENDCG
		}
	}
}