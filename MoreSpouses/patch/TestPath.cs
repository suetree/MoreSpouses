using HarmonyLib;
using SueMoreSpouses.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SueMoreSpouses.patch
{
	//attacker_infantry
	//[HarmonyPatch(typeof(Mission), "GetFormationSpawnFrame")]
	class TestPath
	{

		public static void Prefix(ref WorldFrame __result, ref Mission __instance, BattleSideEnum side, FormationClass formationClass, bool isReinforcement, int customAgentCount = -1, float offsetAwayFrom = 0f, bool isSpawningWithHorses = true)
		{
			int _usedSpawnPathIndex = (int)ReflectUtils.ReflectField("_usedSpawnPathIndex", __instance);
			//int _spawnPoints = (int)ReflectUtils.ReflectField("_spawnPoints", __instance);
			int _agentCount = (int)ReflectUtils.ReflectField("_agentCount", __instance);
			Path _usedSpawnPath = (Path)ReflectUtils.ReflectField("_usedSpawnPath", __instance);
			float _randomMiddlePointAddition = (float)ReflectUtils.ReflectField("_randomMiddlePointAddition", __instance);
			MatrixFrame[] _spawnPoints = (MatrixFrame[])ReflectUtils.ReflectField("_spawnPoints", __instance);
			int num;
			if (customAgentCount != -1)
			{
				num = customAgentCount;
			}
			else
			{
				num = _agentCount;
			}
			if (_usedSpawnPathIndex < 0)
			{
				int num2 = 0;
				int num3;
				do
				{
					num3 = ((num2 <= 20) ? (MBRandom.RandomInt(3) + 1) : ((num2 - 20) % 3 + 1));
					_usedSpawnPath = __instance.Scene.GetPathWithName("spawn_path_0" + num3);
					num2++;
				}
				while (_usedSpawnPath == null && num2 < 24);
				_usedSpawnPathIndex = num3;
				_randomMiddlePointAddition = -0.13f + MBRandom.RandomFloat * 0.26f;
			}
			else
			{
				_usedSpawnPath = __instance.Scene.GetPathWithName("spawn_path_0" + _usedSpawnPathIndex);
			}
			if (_usedSpawnPath != null)
			{
				_usedSpawnPath.GetPoints(_spawnPoints);
				float totalLength = _usedSpawnPath.GetTotalLength();
				float num4 = 800f / totalLength;
				float num5 = 0.5f + _randomMiddlePointAddition;
				float num6 = -1f;
				if (num > 0)
				{
					num6 = 0.04f + 0.08f * (float)Math.Pow((double)(num + (isReinforcement ? 25 : 0)), 0.40000000596046448);
					num6 *= num4;
				}
				num6 = MBMath.ClampFloat(num6, 0.15f, 1f);
				float num7 = 0f;
				if (formationClass >= FormationClass.Ranged)
				{
					Formation formation = ((side == BattleSideEnum.Attacker) ? __instance.Teams.Attacker : __instance.Teams.Defender).GetFormation(FormationClass.Infantry);
					num7 += formation.Depth;
				}
				if (formationClass >= FormationClass.Cavalry)
				{
					Formation formation = ((side == BattleSideEnum.Attacker) ? __instance.Teams.Attacker : __instance.Teams.Defender).GetFormation(FormationClass.Ranged);
					num7 += formation.Depth;
				}
				if (!isSpawningWithHorses && formationClass >= FormationClass.HorseArcher)
				{
					Formation formation = ((side == BattleSideEnum.Attacker) ? __instance.Teams.Attacker : __instance.Teams.Defender).GetFormation(FormationClass.Cavalry);
					num7 += formation.Depth;
				}
				float num8 = Math.Max(0f, num5 - 0.44f * num6);
				float num9 = Math.Min(1f, num5 + 0.44f * num6);
				MatrixFrame frameForDistance = _usedSpawnPath.GetFrameForDistance(totalLength * num8 - offsetAwayFrom - num7);
				MatrixFrame frameForDistance2 = _usedSpawnPath.GetFrameForDistance(totalLength * num9 + offsetAwayFrom + num7);
				MatrixFrame matrixFrame = (side == BattleSideEnum.Attacker) ? frameForDistance2 : frameForDistance;
				Vec2 v = ((side == BattleSideEnum.Defender) ? frameForDistance2 : frameForDistance).origin.AsVec2 - matrixFrame.origin.AsVec2;
				v.Normalize();
				Mat3 identity = Mat3.Identity;
				identity.RotateAboutUp(v.RotationInRadians);
				WorldPosition worldPosition = new WorldPosition(__instance.Scene, UIntPtr.Zero, matrixFrame.origin, false);
				if (formationClass == FormationClass.NumberOfRegularFormations)
				{
					worldPosition.SetVec2(worldPosition.AsVec2 - v * 12f);
				}
				if (isSpawningWithHorses && (formationClass == FormationClass.Cavalry || formationClass == FormationClass.HorseArcher))
				{
					WorldPosition worldPosition2 = worldPosition;
					float num10 = ((side == BattleSideEnum.Attacker) ? __instance.Teams.Attacker : __instance.Teams.Defender).GetFormation(FormationClass.Cavalry).Width * 0.5f;
					float num11 = Math.Max(((side == BattleSideEnum.Attacker) ? __instance.Teams.Attacker : __instance.Teams.Defender).GetFormation(FormationClass.Infantry).Width, ((side == BattleSideEnum.Attacker) ? __instance.Teams.Attacker : __instance.Teams.Defender).GetFormation(FormationClass.Ranged).Width) * 0.5f;
					worldPosition2.SetVec2(worldPosition.AsVec2 + ((formationClass == FormationClass.Cavalry) ? v.LeftVec() : v.RightVec()) * (num10 + num11));
					if (worldPosition2.GetNavMesh() != UIntPtr.Zero)
					{
						worldPosition.SetVec2(worldPosition2.AsVec2);
					}
				}
				__result = new WorldFrame(identity, worldPosition);
			}
			if (__instance.IsFieldBattle)
			{
				GameEntity gameEntity = __instance.Scene.FindEntityWithTag("defender_" + ((formationClass != FormationClass.NumberOfRegularFormations) ? formationClass.GetName().ToLower() : "infantry"));
				GameEntity gameEntity2 = __instance.Scene.FindEntityWithTag("attacker_" + ((formationClass != FormationClass.NumberOfRegularFormations) ? formationClass.GetName().ToLower() : "infantry"));
				if (gameEntity != null && gameEntity2 != null)
				{
					Vec3 globalPosition = gameEntity.GlobalPosition;
					Vec3 v2 = gameEntity2.GlobalPosition - globalPosition;
					Vec3 arg_4E7_0 = globalPosition;
					float num12 = v2.Normalize();
					float num13 = 1000f / num12;
					float num14 = 0.08f + 0.08f * (float)Math.Pow((double)(num + (isReinforcement ? 25 : 0)), 0.30000001192092896);
					num14 *= num13;
					float num15 = Math.Max(0f, 0.5f - 0.5f * num14);
					float num16 = Math.Min(1f, 0.5f + 0.5f * num14);
					Vec3 vec = arg_4E7_0 + v2 * (num12 * num15);
					Vec3 vec2 = arg_4E7_0 + v2 * (num12 * num16);
					Vec3 vec3 = (side == BattleSideEnum.Attacker) ? vec2 : vec;
					Vec3 vec4 = ((side == BattleSideEnum.Defender) ? vec2 : vec) - vec3;
					vec4.Normalize();
					Mat3 identity2 = Mat3.Identity;
					identity2.RotateAboutUp(vec4.AsVec2.RotationInRadians);
					WorldPosition origin = new WorldPosition(__instance.Scene, UIntPtr.Zero, vec3, false);
					__result = new WorldFrame(identity2, origin);
				}
				Vec2 v3 = Vec2.Invalid;
				ICollection<Vec2> value = __instance.Boundaries.First<KeyValuePair<string, ICollection<Vec2>>>().Value;
				if (value.Count > 0)
				{
					v3 = value.Aggregate((a,b) => a + b);
					v3 *= 1f / (float)value.Count;
				}
				WorldPosition origin2 = new WorldPosition(__instance.Scene, UIntPtr.Zero, v3.ToVec3(0f), false);
				__result = new WorldFrame(Mat3.Identity, origin2);
			}
			else
			{
				SiegeMissionController expr_609 = __instance.GetMissionBehaviour<SiegeMissionController>();
				if (expr_609 != null)
				{
					bool arg_614_0 = expr_609.IsSallyOut;
				}
				string text = side.ToString().ToLower() + "_" + ((formationClass != FormationClass.NumberOfRegularFormations) ? formationClass.GetName().ToLower() : "infantry");
				if (isReinforcement)
				{
					text += "_reinforcement";
				}
				GameEntity gameEntity3 = __instance.Scene.FindEntityWithTag(text);
				if (gameEntity3 != null)
				{
					MatrixFrame globalFrame = gameEntity3.GetGlobalFrame();
					globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
					WorldPosition origin3 = new WorldPosition(__instance.Scene, UIntPtr.Zero, globalFrame.origin, false);
					__result = new WorldFrame(globalFrame.rotation, origin3);
				}
				Vec2 v4 = Vec2.Invalid;
				ICollection<Vec2> value2 = __instance.Boundaries.First<KeyValuePair<string, ICollection<Vec2>>>().Value;
				if (value2.Count > 0)
				{
					v4 = value2.Aggregate((a, b) => a + b);
					v4 *= 1f / (float)value2.Count;
				}
				WorldPosition origin4 = new WorldPosition(__instance.Scene, UIntPtr.Zero, v4.ToVec3(0f), false);
				__result = new WorldFrame(Mat3.Identity, origin4);
				
			}
		}
	}



}
