


using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace SueMoreSpouses
{
    class MoreSpouseSetting : AttributeGlobalSettings<MoreSpouseSetting>
    {

        public override string DisplayName => "SueMoreSpouses";
        public override string Id => "MoreSpouseSetting";

        public MoreSpouseSetting():base()
        {
            this.ChildrenStopGrowUpAge = 18;
            this.ChildrenGrowthCycleInDays = 18;
            this.ExspouseGetPregnancyDailyChance = 0.5f;
            this.PregnancyDurationInDays = 30;

        }

        [SettingPropertyBool("{=sue_more_spouses_setting_enable_exspouse_pregnancy}", Order = 20, RequireRestart = false, HintText = "{=sue_more_spouses_setting_enable_exspouse_pregnancy_hint}")]
        [SettingPropertyGroup("{=sue_more_spouses_setting_enable_exspouse_pregnancy}")]
        public bool ExspouseGetPregnancyEnable
        {
            get;
            set;
        }

        [SettingPropertyFloatingInteger("{=sue_more_spouses_setting_exspouse_pregnancy_daily_chance}",  0.00f, 1.00f, "0.00", Order = 21, RequireRestart = false, HintText = "{=sue_more_spouses_setting_exspouse_pregnancy_daily_chance_hint}")]
        [SettingPropertyGroup("{=sue_more_spouses_setting_enable_exspouse_pregnancy}")]
        public float ExspouseGetPregnancyDailyChance
        {
            get;
            set;
        }

        [SettingPropertyInteger("{=sue_more_spouses_setting_exspouse_pregnancy_duration_in_days}", 2, 72, "0", Order = 22, RequireRestart = false, HintText= "{=sue_more_spouses_setting_exspouse_pregnancy_duration_in_days_hint}")]
        [SettingPropertyGroup("{=sue_more_spouses_setting_enable_exspouse_pregnancy}")]
        public float PregnancyDurationInDays
        {
            get;
            set;
        }


        [SettingPropertyBool("{=sue_more_spouses_setting_enable_children_fast_growth}", Order = 10, RequireRestart = false, HintText = "{=sue_more_spouses_setting_enable_children_grow_up_hint}")]
        [SettingPropertyGroup("{=sue_more_spouses_setting_enable_children_fast_growth}")]
        public bool ChildrenFastGrowUpEnable
        {
            get;
            set;
        }

        [SettingPropertyInteger("{=sue_more_spouses_setting_children_fast_grow_up_cycle}", 2, 72, "0", Order = 11, RequireRestart = false, HintText = "{=sue_more_spouses_setting_children_fast_grow_up_cycle_hint}")]
        [SettingPropertyGroup("{=sue_more_spouses_setting_enable_children_fast_growth}")]
        public int ChildrenGrowthCycleInDays
        {
            get;
            set;
        }


        [SettingPropertyInteger("{=sue_more_spouses_setting_children_fast_grow_up_age}", 1, 22, "0", Order = 12, RequireRestart = false, HintText = "{=sue_more_spouses_setting_children_fast_grow_up_age_hint}")]
        [SettingPropertyGroup("{=sue_more_spouses_setting_enable_children_fast_growth}")]
        public int ChildrenStopGrowUpAge
        {
            get;
            set;
        }

      
    }
}
