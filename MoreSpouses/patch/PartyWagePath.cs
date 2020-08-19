using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.patch
{
    //[HarmonyPatch(typeof(DefaultPartyWageModel), "GetTotalWage")]
    class PartyWagePath
    {
        static bool Prefix(ref int __result, MobileParty mobileParty, StatExplainer explanation = null)
        {
            if (mobileParty == MobileParty.MainParty)
            {
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (int i = 0; i < mobileParty.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = mobileParty.MemberRoster.GetElementCopyAtIndex(i);
					CharacterObject character = elementCopyAtIndex.Character;
					if (character.IsHero)
					{
						if (elementCopyAtIndex.Character != mobileParty.Party.Owner.CharacterObject)
						{
							num3 += elementCopyAtIndex.Character.TroopWage;
						}
					}
					else
					{
						if (character.Tier < 4)
						{
							num += elementCopyAtIndex.Character.TroopWage * elementCopyAtIndex.Number;
						}
						else if (character.Tier == 4)
						{
							num2 += elementCopyAtIndex.Character.TroopWage * elementCopyAtIndex.Number;
						}
						else if (character.Tier > 4)
						{
							num3 += elementCopyAtIndex.Character.TroopWage * elementCopyAtIndex.Number;
						}
						if (character.IsInfantry)
						{
							num4 += elementCopyAtIndex.Number;
						}
						if (character.IsMounted)
						{
							num5 += elementCopyAtIndex.Number;
						}
					}
				}
				if (mobileParty.HasPerk(DefaultPerks.Leadership.LevySergeant, false))
				{
					ExplainedNumber explainedNumber = new ExplainedNumber(1f, null);
					if (explanation != null)
					{
						explanation.AddLine(DefaultPerks.Leadership.LevySergeant.Name.ToString(), (float)(num + num2) - (float)(num + num2) * explainedNumber.ResultNumber, StatExplainer.OperationType.Add);
					}
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.LevySergeant, mobileParty, ref explainedNumber);
					num = MathF.Round(explainedNumber.ResultNumber * (float)num);
					num2 = MathF.Round(explainedNumber.ResultNumber * (float)num2);
				}
				if (mobileParty.HasPerk(DefaultPerks.Leadership.VeteransRespect, false))
				{
					ExplainedNumber explainedNumber2 = new ExplainedNumber(1f, null);
					if (explanation != null)
					{
						explanation.AddLine(DefaultPerks.Leadership.VeteransRespect.Name.ToString(), (float)(num2 + num3) - (float)(num2 + num3) * explainedNumber2.ResultNumber, StatExplainer.OperationType.Add);
					}
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Leadership.VeteransRespect, mobileParty, ref explainedNumber2);
					num2 = MathF.Round(explainedNumber2.ResultNumber * (float)num2);
					num3 = MathF.Round((float)num3 * explainedNumber2.ResultNumber);
				}
				ExplainedNumber explainedNumber3 = new ExplainedNumber(1f, null);
				if (mobileParty.IsGarrison && mobileParty.CurrentSettlement != null && mobileParty.CurrentSettlement.IsTown)
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.MilitaryTradition, mobileParty.CurrentSettlement.Town, ref explainedNumber3);
					PerkHelper.AddPerkBonusForTown(DefaultPerks.TwoHanded.Berserker, mobileParty.CurrentSettlement.Town, ref explainedNumber3);
					float num6 = (float)num4 / (float)mobileParty.MemberRoster.TotalRegulars;
					if (num6 > 0f && mobileParty.CurrentSettlement.Town.Governor != null)
					{
						Hero governor = mobileParty.CurrentSettlement.Town.Governor;
						if (governor != null && governor.GetPerkValue(DefaultPerks.Polearm.StandardBearer) && governor.CurrentSettlement != null && governor.CurrentSettlement == mobileParty.CurrentSettlement)
						{
							explainedNumber3.AddFactor(DefaultPerks.Polearm.StandardBearer.SecondaryBonus * num6 * 0.01f, DefaultPerks.Polearm.StandardBearer.Name);
						}
					}
					float num7 = (float)num5 / (float)mobileParty.MemberRoster.TotalRegulars;
					if (num7 > 0f && mobileParty.CurrentSettlement.Town.Governor != null)
					{
						Hero governor2 = mobileParty.CurrentSettlement.Town.Governor;
						if (governor2 != null && governor2.GetPerkValue(DefaultPerks.Riding.CavalryTactics) && governor2.CurrentSettlement != null && governor2.CurrentSettlement == mobileParty.CurrentSettlement)
						{
							explainedNumber3.AddFactor(DefaultPerks.Riding.CavalryTactics.SecondaryBonus * num7 * 0.01f, DefaultPerks.Riding.CavalryTactics.Name);
						}
					}
				}
				int num8 = num + num2 + num3;
				ExplainedNumber explainedNumber4 = new ExplainedNumber((float)num8, explanation, null);
				float value = (mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan.Kingdom != null && !mobileParty.LeaderHero.Clan.IsUnderMercenaryService && mobileParty.LeaderHero.Clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.MilitaryCoronae)) ? 0.1f : 0f;
				explainedNumber4.AddFactor(value, DefaultPolicies.MilitaryCoronae.Name);
				explainedNumber4.AddFactor(explainedNumber3.ResultNumber - 1f, new TextObject("{=7BiaPpo2}Perk Effects", null));
				__result = (int)explainedNumber4.ResultNumber;
				return false;
			}

            return true;
        }
    }
}
