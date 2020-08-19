using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace SueMoreSpouses.patch
{
    // [HarmonyPatch(typeof(MarriageAction), "ApplyInternal")]
    class MarryActionPath
    {

        static bool Prefix(Hero firstHero, Hero secondHero, bool showNotification)
        {

			firstHero.Spouse = secondHero;
			secondHero.Spouse = firstHero;
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(firstHero, secondHero, Campaign.Current.Models.MarriageModel.GetEffectiveRelationIncrease(firstHero, secondHero), false);
			Clan clanAfterMarriage = Campaign.Current.Models.MarriageModel.GetClanAfterMarriage(firstHero, secondHero);
			if (firstHero.Clan != clanAfterMarriage)
			{
				Clan clan = firstHero.Clan;
				if (firstHero.GovernorOf != null)
				{
					ChangeGovernorAction.ApplyByGiveUpCurrent(firstHero);
				}
				if (firstHero.PartyBelongedTo != null)
				{
					if (clan.Kingdom != clanAfterMarriage.Kingdom)
					{
						if (firstHero.PartyBelongedTo.Army != null)
						{
							if (firstHero.PartyBelongedTo.Army.LeaderParty == firstHero.PartyBelongedTo)
							{
								firstHero.PartyBelongedTo.Army.DisperseArmy(Army.ArmyDispersionReason.Unknown);
							}
							else
							{
								firstHero.PartyBelongedTo.Army = null;
							}
						}
						IFaction kingdom = clanAfterMarriage.Kingdom;
						FactionHelper.FinishAllRelatedHostileActionsOfNobleToFaction(firstHero, kingdom ?? clanAfterMarriage);
					}
					DisbandPartyAction.ApplyDisband(firstHero.PartyBelongedTo);
					if (firstHero.PartyBelongedTo != null)
					{
						firstHero.PartyBelongedTo.Party.Owner = null;
					}
					firstHero.ChangeState(Hero.CharacterStates.Fugitive);
					MobileParty expr_105 = firstHero.PartyBelongedTo;
					if (expr_105 != null)
					{
						expr_105.MemberRoster.RemoveTroop(firstHero.CharacterObject, 1);
					}
				}
				firstHero.Clan = clanAfterMarriage;
				using (IEnumerator<Hero> enumerator = clan.Heroes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.UpdateHomeSettlement();
					}
				}
				using (IEnumerator<Hero> enumerator = clanAfterMarriage.Heroes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.UpdateHomeSettlement();
					}
					
				}
			}
			Clan clan2 = secondHero.Clan;
			if (secondHero.GovernorOf != null)
			{
				ChangeGovernorAction.ApplyByGiveUpCurrent(secondHero);
			}
			if (secondHero.PartyBelongedTo != null)
			{
				MobileParty partyBelongedTo = secondHero.PartyBelongedTo;
				if (clan2.Kingdom != clanAfterMarriage.Kingdom)
				{
					if (partyBelongedTo.Army != null)
					{
						if (partyBelongedTo.Army.LeaderParty == partyBelongedTo)
						{
							partyBelongedTo.Army.DisperseArmy(Army.ArmyDispersionReason.Unknown);
						}
						else
						{
							partyBelongedTo.Army = null;
						}
					}
					FactionHelper.FinishAllRelatedHostileActionsOfNobleToFaction(secondHero, clanAfterMarriage.Kingdom);
				}
				DisbandPartyAction.ApplyDisband(partyBelongedTo);
				partyBelongedTo.Party.Owner = null;
				secondHero.ChangeState(Hero.CharacterStates.Fugitive);
				if (partyBelongedTo.MemberRoster.Contains(secondHero.CharacterObject))
				{
					partyBelongedTo.MemberRoster.RemoveTroop(secondHero.CharacterObject, 1);
				}
			}
			secondHero.Clan = clanAfterMarriage;
			using (IEnumerator<Hero> enumerator = clan2.Heroes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.UpdateHomeSettlement();
				}
			}
			using (IEnumerator<Hero> enumerator = clanAfterMarriage.Heroes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.UpdateHomeSettlement();
				}
			}
			return false;
        }
    }
}
