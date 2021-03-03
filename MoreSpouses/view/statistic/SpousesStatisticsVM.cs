using SueMoreSpouses.Behavior;
using SueMoreSpouses.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view
{
    class SpousesStatisticsVM : ViewModel
    {
        MBBindingList<SpouseStatisticsItemVM> _statisticsViews;
        SelectorVM<SelectorItemVM> _roleTypeVM;
        int currentRoleType = 0;
        SelectorVM<SelectorItemVM> _sortTypeVM;
        int currentSortType = 0;
        SelectorVM<SelectorItemVM> _orderTypeVM;
        int currentOrderType = 1;


        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> DropdownRoleType
        {
            get
            {
                return this._roleTypeVM;
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> DropdownSortType
        {
            get
            {
                return this._sortTypeVM;
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> DropdownOrderType
        {
            get
            {
                return this._orderTypeVM;
            }
        }

        [DataSourceProperty]
        public MBBindingList<SpouseStatisticsItemVM> SpouseStatistics
        {
            get
            {
                return this._statisticsViews;
            }
        }

        public SpousesStatisticsVM()
        {
            this._statisticsViews = new MBBindingList<SpouseStatisticsItemVM>();
            GenerateStatsData();
            InitRoleTypeData();
            InitSortTypeData();
            InitOrderTypeData();

        }

        private void GenerateStatsData()
        {
            if (this._statisticsViews.Count > 0)
            {
                this._statisticsViews.Clear();
            }
            List<SpousesHeroStatistic> spousesStatsDatas = Campaign.Current.GetCampaignBehavior<SpousesStatsBehavior>().SpousesStats();
            spousesStatsDatas.Sort((x, y) => {

                int result = 0;
                switch (currentSortType)
                {
                    default:
                        result = x.TotalKillCount.CompareTo(y.TotalKillCount);
                        break;
                    case 1:
                        result = x.MVPCount.CompareTo(y.MVPCount);
                        break;
                    case 2:
                        result = x.ZeroCount.CompareTo(y.ZeroCount);
                        break;
                    case 3:
                        result = x.FightCount.CompareTo(y.FightCount);
                        break;
                }

                if (currentOrderType == 1)
                {
                    result = -1 * result;
                }

                return result;
            }
                );

            spousesStatsDatas.ForEach((obj) => {
                if (null != obj.StatsHero)
                {
                    if (CanAddStatisticsItems(obj))
                    {
                        this._statisticsViews.Add(new SpouseStatisticsItemVM(obj));
                    }
                }
            });
            base.OnPropertyChanged("SpouseStatistics");
        }

        private bool IsSpouse(Hero hero)
        {
            return Hero.MainHero.ExSpouses.Contains(hero) || (null != Hero.MainHero.Spouse && hero == Hero.MainHero.Spouse);
        }

        private bool CanAddStatisticsItems(SpousesHeroStatistic spousesStats)
        {
            bool result = false;
            if (currentRoleType != 0)
            {
                if (currentRoleType == 1)
                {
                    if (IsSpouse(spousesStats.StatsHero))
                    {
                        result = true;
                    }
                }
                else if (currentRoleType == 2)
                {
                    if (spousesStats.StatsHero.IsPlayerCompanion)
                    {
                        result = true;
                    }
                }
                else if (currentRoleType == 3)
                {
                    if (Hero.MainHero.Children.Contains(spousesStats.StatsHero))
                    {
                        result = true;
                    }
                }
                else
                {
                    if (!Hero.MainHero.Children.Contains(spousesStats.StatsHero)
                        && !IsSpouse(spousesStats.StatsHero)
                        && !spousesStats.StatsHero.IsPlayerCompanion)
                    {
                        result = true;
                    }
                }
            }
            else
            {
                result = true;
            }
            return result;
        }

        private void InitRoleTypeData()
        {
            List<TextObject> texts = new List<TextObject>();
            texts.Add(new TextObject("{=sms_battle_record_stats_role_all}All"));
            texts.Add(new TextObject("{=sms_battle_record_stats_role_spouse}Spouse"));
            texts.Add(new TextObject("{=sms_battle_record_stats_role_companion}Companion"));
            texts.Add(new TextObject("{=sms_battle_record_stats_role_children}Children"));
            texts.Add(new TextObject("{=sms_battle_record_stats_role_other}Other"));
            this._roleTypeVM = new SelectorVM<SelectorItemVM>(texts, currentRoleType, (item) =>
            {
                currentRoleType = item.SelectedIndex;
                GenerateStatsData();
            });
        }

        private void InitSortTypeData()
        {
            List<TextObject> texts = new List<TextObject>();
            texts.Add(new TextObject(TotalKillCountText));
            texts.Add(new TextObject(MVPCountText));
            texts.Add(new TextObject(ZeroCountText));
            texts.Add(new TextObject(FightCountText));
            this._sortTypeVM = new SelectorVM<SelectorItemVM>(texts, currentSortType, (item) =>
            {
                currentSortType = item.SelectedIndex;
                GenerateStatsData();
            });
        }

        private void InitOrderTypeData()
        {
            List<TextObject> texts = new List<TextObject>();
            texts.Add(new TextObject("{=sms_battle_record_stats_order_asc}ASC"));
            texts.Add(new TextObject("{=sms_battle_record_stats_order_desc}DESC"));
            this._orderTypeVM = new SelectorVM<SelectorItemVM>(texts, currentOrderType, (item) =>
            {
                currentOrderType = item.SelectedIndex;
                GenerateStatsData();
            });
        }

        [DataSourceProperty]
        public string NameText
        {
            get
            {
                return new TextObject("{=sms_battle_record_stats_label_name}Name", null).ToString();
            }

        }

        [DataSourceProperty]
        public string TotalKillCountText
        {
            get
            {
                return new TextObject("{=sms_battle_record_stats_label_kill}Kill Count", null).ToString();
            }

        }

        [DataSourceProperty]
        public string MVPCountText
        {
            get
            {
                return new TextObject("{=sms_battle_record_stats_label_mvp}MVP Count", null).ToString();
            }

        }

        [DataSourceProperty]
        public string ZeroCountText
        {
            get
            {
                return new TextObject("{=sms_battle_record_stats_label_zero}Zero Count", null).ToString();
            }

        }

        [DataSourceProperty]
        public string FightCountText
        {
            get
            {
                return new TextObject("{=sms_battle_record_stats_label_fight}Fight Count", null).ToString();
            }

        }
    }
}
