using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.CampaignSystem.Hero;

namespace SueMoreSpouses.Behavior
{
    class SpouseClanLeaderFixBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickFixClanLeader));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }

        public void DailyTickFixClanLeader()
        {

            //玩家家族领导人修正， 玩家家族领导人不是玩家时，将会修改成玩家
            if (Clan.PlayerClan.Leader != Hero.MainHero)
            {
                Clan.PlayerClan.SetLeader(Hero.MainHero);
                InformationManager.DisplayMessage(new InformationMessage("MoreSpouses: player clan leader fix"));
            }

            //家族领导人的家族不是该家族时，将会把领导人修改成该家族的人员
            IEnumerable<Clan> clans =  Clan.All.ToList().Where(obj => obj.IsEliminated && (null != obj.Leader && obj.Leader.Clan != obj ));
            if (clans.Count() > 0)
            {
                for (int i = 0; i < clans.Count(); i++)
                {
                    Clan current = clans.ElementAt(i);

                    if (null != current.Leader && null != current.Leader.Clan && current.Leader.Clan != current )
                    {
                        Hero sourc = current.Leader;
                        List<Hero> otherHeros = current.Heroes.Where((obj) => (obj != sourc )).ToList();
                        Hero target = null;
                        if (otherHeros.Count > 0)
                        {
                            target = otherHeros.GetRandomElement();
                            current.SetLeader(target);
                        }
                        else
                        {
                            CharacterObject character = CharacterObject.FindFirst(obj => obj.Culture == current.Culture && obj.Occupation == Occupation.Lord);
                            target = HeroCreator.CreateSpecialHero(character, sourc.HomeSettlement, null, null, -1);
                            target.ChangeState(CharacterStates.Dead);
                            target.Clan = current;
                            CampaignEventDispatcher.Instance.OnHeroCreated(target, false);
                            current.SetLeader(target);
                        }

                        if (null != GameComponent.CampaignEventDispatcher())
                        {
                            GameComponent.CampaignEventDispatcher().OnClanLeaderChanged(sourc, target);
                        }

                        InformationManager.DisplayMessage(new InformationMessage("MoreSpouses: " + current.Name + " clan leader fix"));

                    }
                }
            }

            //修正国王领导
            IEnumerable<Kingdom> kingdoms = Kingdom.All.ToList().Where(obj => obj.RulingClan == Clan.PlayerClan  && (!obj.Clans.Contains(Clan.PlayerClan)) );
            if (kingdoms.Count() > 0)
            {
                for (int i = 0; i < kingdoms.Count(); i++)
                {
                    Kingdom current = kingdoms.ElementAt(i);
                    IEnumerable<Clan> aliveClans = current.Clans.Where(obj => !obj.IsEliminated);
                    if (aliveClans.Count() > 0)
                    {
                        current.RulingClan = aliveClans.GetRandomElementInefficiently();
                    }
                    else
                    {
                        current.RulingClan = current.Clans.GetRandomElement();
                    }

                    InformationManager.DisplayMessage(new InformationMessage("MoreSpouses: " + current.Name + " kingdom leader fix"));

                }
            }

            //修正玩家部队拥有者不是玩家
            if (null != MobileParty.MainParty && null != MobileParty.MainParty.Party && null != MobileParty.MainParty.Party.Owner
                && MobileParty.MainParty.Party.Owner != Hero.MainHero)
            {
                MobileParty.MainParty.Party.Owner = Hero.MainHero;
            }



        }
    }
}
