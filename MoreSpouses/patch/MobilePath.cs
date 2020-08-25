using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SueMoreSpouses.patch
{
   // [HarmonyPatch(typeof(MobileParty), "FillPartyStacks")]
    class MobilePath
    {
        static bool Prefix(ref MobileParty __instance, PartyTemplateObject pt, MobileParty.PartyTypeEnum partyType, int troopNumberLimit = -1)
        {
			if (partyType == MobileParty.PartyTypeEnum.Bandit)
			{
				float gameProcess = MiscHelper.GetGameProcess();
				float num = 0.4f + 0.6f * gameProcess;
				int expr_21 = MBRandom.RandomInt(2);
				float num2 = (expr_21 == 0) ? MBRandom.RandomFloat : (MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * 4f);
				float num3 = (expr_21 == 0) ? (num2 * 0.8f + 0.2f) : (1f + num2);
				float randomFloat = MBRandom.RandomFloat;
				float randomFloat2 = MBRandom.RandomFloat;
				float randomFloat3 = MBRandom.RandomFloat;
				float f = (pt.Stacks.Count > 0) ? ((float)pt.Stacks[0].MinValue + num * num3 * randomFloat * (float)(pt.Stacks[0].MaxValue - pt.Stacks[0].MinValue)) : 0f;
				float f2 = (pt.Stacks.Count > 1) ? ((float)pt.Stacks[1].MinValue + num * num3 * randomFloat2 * (float)(pt.Stacks[1].MaxValue - pt.Stacks[1].MinValue)) : 0f;
				float f3 = (pt.Stacks.Count > 2) ? ((float)pt.Stacks[2].MinValue + num * num3 * randomFloat3 * (float)(pt.Stacks[2].MaxValue - pt.Stacks[2].MinValue)) : 0f;
				__instance.AddElementToMemberRoster(pt.Stacks[0].Character, MBRandom.RoundRandomized(f), false);
				if (pt.Stacks.Count > 1)
				{
					__instance.AddElementToMemberRoster(pt.Stacks[1].Character, MBRandom.RoundRandomized(f2), false);
				}
				if (pt.Stacks.Count > 2)
				{
					__instance.AddElementToMemberRoster(pt.Stacks[2].Character, MBRandom.RoundRandomized(f3), false);
					return false;
				}
			}
			else
			{
				if (partyType == MobileParty.PartyTypeEnum.Villager)
				{
					for (int i = 0; i < pt.Stacks.Count; i++)
					{
						CharacterObject character = pt.Stacks[0].Character;
						__instance.AddElementToMemberRoster(character, troopNumberLimit, false);
					}
					return false;
				}
				if (troopNumberLimit < 0)
				{
					float gameProcess2 = MiscHelper.GetGameProcess();
					for (int j = 0; j < pt.Stacks.Count; j++)
					{
						int numberToAdd = (int)(gameProcess2 * (float)(pt.Stacks[j].MaxValue - pt.Stacks[j].MinValue)) + pt.Stacks[j].MinValue;
						CharacterObject character2 = pt.Stacks[j].Character;
						__instance.AddElementToMemberRoster(character2, numberToAdd, false);
					}
					return false;
				}
				for (int k = 0; k < troopNumberLimit; k++)
				{
					int num4 = -1;
					float num5 = 0f;
					for (int l = 0; l < pt.Stacks.Count; l++)
					{
						num5 += ((partyType == MobileParty.PartyTypeEnum.GarrisonParty && pt.Stacks[l].Character.IsArcher) ? 6f : ((partyType == MobileParty.PartyTypeEnum.GarrisonParty && !pt.Stacks[l].Character.IsMounted) ? 2f : 1f)) * ((float)(pt.Stacks[l].MaxValue + pt.Stacks[l].MinValue) / 2f);
					}
					float num6 = MBRandom.RandomFloat * num5;
					for (int m = 0; m < pt.Stacks.Count; m++)
					{
						num6 -= ((partyType == MobileParty.PartyTypeEnum.GarrisonParty && pt.Stacks[m].Character.IsArcher) ? 6f : ((partyType == MobileParty.PartyTypeEnum.GarrisonParty && !pt.Stacks[m].Character.IsMounted) ? 2f : 1f)) * ((float)(pt.Stacks[m].MaxValue + pt.Stacks[m].MinValue) / 2f);
						if (num6 < 0f)
						{
							num4 = m;
							break;
						}
					}
					if (num4 < 0)
					{
						num4 = 0;
					}
					CharacterObject character3 = pt.Stacks[num4].Character;
					__instance.AddElementToMemberRoster(character3, 1, false);
				}
			}
			return false;
        }
    }
}
