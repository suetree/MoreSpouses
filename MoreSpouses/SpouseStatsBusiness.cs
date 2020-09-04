using HarmonyLib;
using SueMoreSpouses.data;
using SueMoreSpouses.data.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SueMoreSpouses
{
    class SpouseStatsBusiness
    {
        
        List<SpousesHeroStatistic> _spousesStats;
        List<SpousesBattleRecord> _spousesBattleRecordDatas;

       Dictionary<CharacterObject, int> TempHeroStatisticRecordDic = new Dictionary<CharacterObject, int>();

        Hero lastHero = null;
        int lastKillCount = 0;
        SpousesBattleRecordSide attackerSide;
        SpousesBattleRecordSide defenderSide;
        SpousesBattleRecord spousesBattleRecord;

        Banner attackerBanner;
        Banner defenderBanner;

        BattleSideEnum PlayerSide;

        public SpouseStatsBusiness(List<SpousesHeroStatistic> spousesStats, List<SpousesBattleRecord> spousesBattleRecordDatas) 
        {
            this._spousesStats = spousesStats;
            this._spousesBattleRecordDatas = spousesBattleRecordDatas;
        }

        public void EndCountHeroBattleData(int battleResultIndex, float renownChange, float influenceChange, float moraleChange, float goldChange, float playerEarnedLootPercentage)
        {


            if (null != defenderSide && null != attackerSide)
            {
                if (null == spousesBattleRecord)
                {
                    spousesBattleRecord = new SpousesBattleRecord();
                    spousesBattleRecord.BattleResultIndex = battleResultIndex;
                    spousesBattleRecord.RecordReward = new SpousesBattleRecordReward(renownChange, influenceChange, moraleChange, goldChange, playerEarnedLootPercentage);
                }
                spousesBattleRecord.AttackerSide = attackerSide;
                spousesBattleRecord.DefenderSide = defenderSide;

                if (_spousesBattleRecordDatas.Count >= 20)
                {
                    _spousesBattleRecordDatas.RemoveAt(_spousesBattleRecordDatas.Count - 1);
                }
                if (null != spousesBattleRecord)
                {
                    _spousesBattleRecordDatas.Insert(0, spousesBattleRecord);
                }
            }


            if (TempHeroStatisticRecordDic.Count > 0)
            {
                foreach (var item in TempHeroStatisticRecordDic)
                {
                    CountHeroBattleDataForStatistic(item.Key, item.Value);
                }
            }


            if (null != lastHero)
            {
                this._spousesStats.ForEach((obj) => {
                    if (obj.StatsHero == lastHero)
                    {
                        obj.MVPCount += 1;
                    }
                });
            }
            ResetData();
        }

        public void Initialize()
        {
            ResetData();
            this.PlayerSide = (PlayerEncounter.PlayerIsAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
            if (null != MobileParty.MainParty.MapEvent)
            {
                attackerBanner = MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.Banner;
                defenderBanner = MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.Banner;
            }
        }

        public void ResetData()
        {
            lastHero = null;
            lastKillCount = 0;
            attackerSide = null;
            defenderSide = null;
            attackerBanner = null;
            defenderBanner = null;
            spousesBattleRecord = null;
            TempHeroStatisticRecordDic.Clear();
        }

        private void RecordHeroBattleStatistic(CharacterObject character, int killCount)
        {
            if (null != character && null != MobileParty.MainParty)
            {
                CharacterObject heroCharacter = character  as CharacterObject;
                if (heroCharacter.IsHero  && heroCharacter.HeroObject.Clan == Clan.PlayerClan)
                {
                    if (TempHeroStatisticRecordDic.ContainsKey(heroCharacter))
                    {
                        int count = TempHeroStatisticRecordDic.GetValueSafe(heroCharacter);
                        count += killCount;
                        TempHeroStatisticRecordDic[heroCharacter] = count;
                    }
                    else
                    {
                        TempHeroStatisticRecordDic.Add(heroCharacter, killCount);
                    }
                }
            }
        }

        public void RecordBattleData(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int remain, int killed, int killCount, int wounder, int wounded )
        {

            if (null != (character as CharacterObject))
            {
                RecordHeroBattleStatistic((CharacterObject)character, killCount);
            }
  
            if (side == BattleSideEnum.Attacker)
            {
                if (null == attackerSide)
                {
                    Banner banner = null ;
                    if (null != attackerBanner)
                    {
                        banner = attackerBanner;
                    }else if (null != MobileParty.MainParty.MapEvent)
                    {
                        banner = MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.Banner;
                    }
                    attackerSide = new SpousesBattleRecordSide(GameTexts.FindText("str_battle_result_army", "attacker").ToString(), banner);
                }
                SetSideData(attackerSide, battleCombatant,character, remain, killCount, killed, wounder, wounded);
            }
            else
            {
                if (null == defenderSide)
                {
                    Banner banner = null; ;
                    if (null != defenderBanner)
                    {
                        banner = defenderBanner;
                    }
                    else if (null != MobileParty.MainParty.MapEvent)
                    {
                        banner = MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.Banner;
                    }
                    defenderSide = new SpousesBattleRecordSide(GameTexts.FindText("str_battle_result_army", "defender").ToString(), banner);
                }
                SetSideData(defenderSide, battleCombatant, character, remain, killCount, killed, wounder, wounded);
            }

        }

        private void SetSideData(SpousesBattleRecordSide side, IBattleCombatant battleCombatant, BasicCharacterObject character, int remain, int killCount, int killed, int wounder, int wounded)
        {

            if (null != side)
            {
                side.Remain += remain;
                side.KillCount += killCount;
                side.Killed += killed;
                side.Wounded += wounded;
                side.RunAway += wounder;
                SpousesBattleRecordParty party = side.GetPartyByUniqueId(battleCombatant.GetHashCode() + "");
                if (null == party)
                {
                    party = new SpousesBattleRecordParty(battleCombatant.GetHashCode() + "");
                    party.Name = battleCombatant.Name.ToString();
                    side.Parties.Add(party);
                }
                    party.Remain += remain;
                    party.KillCount += killCount;
                    party.Killed += killed;
                    party.Wounded += wounded;
                    party.RunAway += wounder;
                    SpousesBattleRecordCharacter battleRecordCharacter = party.GetBattleRecordCharacter(character);
                    if (null == battleRecordCharacter)
                    {
                        battleRecordCharacter = new SpousesBattleRecordCharacter(character);
                        party.Characters.Add(battleRecordCharacter);
                    }
                    battleRecordCharacter.Remain += remain;
                    battleRecordCharacter.KillCount += killCount;
                    battleRecordCharacter.Killed += killed;
                    battleRecordCharacter.Wounded += wounded;
                    battleRecordCharacter.RunAway += wounder;
            }
        }


         public void CountHeroBattleDataForStatistic(CharacterObject character, int killCount)
        {
            if (null != character && character.IsHero && null != _spousesStats)
            {

                if (null != character  && null != character.HeroObject.Clan && character.HeroObject.Clan == Clan.PlayerClan)
                {
                    bool containHero = false;
                    _spousesStats.ForEach((obj) => {
                        if (obj.StatsHero == character.HeroObject)
                        {
                            obj.TotalKillCount += killCount;
                            obj.FightCount += 1;
                            if (killCount == 0)
                            {
                                obj.ZeroCount += 1;
                            }
                            containHero = true;
                        }
                    });
                    if (!containHero)
                    {
                        SpousesHeroStatistic stats = new SpousesHeroStatistic(character.HeroObject);
                        stats.TotalKillCount = killCount;
                        stats.FightCount += 1;
                        if (killCount == 0)
                        {
                            stats.ZeroCount += 1;
                        }
                        _spousesStats.Add(stats);
                    }

                    if (null == lastHero)
                    {
                        lastHero = character.HeroObject;
                        lastKillCount  = killCount;
                    }
                    else
                    {
                        if (killCount > lastKillCount)
                        {
                            lastHero = character.HeroObject;
                            lastKillCount = killCount;
                        }
                    }
                }
            }
        }
    }
}
