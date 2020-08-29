
using Newtonsoft.Json;
using SueMoreSpouses.data;
using SueMoreSpouses.view.setting;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.Engine;
namespace SueMoreSpouses
{
    class MoreSpouseSetting
    {
        private static MoreSpouseSetting _instance;

        public static MoreSpouseSetting Instance
        {
            get {
                if (null == _instance)
                {
                    _instance = new MoreSpouseSetting();
                }

                return _instance;
            }
          
        }

        public SettingData SettingData { set; get; }

        private MoreSpouseSetting()
        {
            this.LoadSettingData();
            if(null == this.SettingData)
            {
                this.SettingData = new SettingData();
            }
           
        }


        public void LoadSettingData()
        {
            string path = System.IO.Path.Combine(Utilities.GetConfigsPath(), "SueMoreSpouses", "SettingData.json");
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                try
                {
                    using (StreamReader streamReader = fileInfo.OpenText())
                    {
                        string json = streamReader.ReadToEnd();
                        if (json != null) { 
                            this.SettingData =(SettingData) JsonConvert.DeserializeObject(json, typeof(SettingData));
                            this.SettingData.InitData();
                        }
                       
                    }
                } catch (JsonException e)
                {
                    InformationManager.DisplayMessage(new InformationMessage("SueMoreSpouses load setting failed" + e.Message));
                }
            }
        }

        public void SaveSettingData()
        {
           try
           {
                string dic = System.IO.Path.Combine(Utilities.GetConfigsPath(), "SueMoreSpouses");
                string path = System.IO.Path.Combine(dic, "SettingData.json");

                System.IO.Directory.CreateDirectory(dic);

                string json = JsonConvert.SerializeObject(this.SettingData);
                StreamWriter streamWriter = new StreamWriter(path, false);
                streamWriter.Write(json);
                streamWriter.Flush();// 清空缓存
                streamWriter.Close();
            }
            catch (JsonException e)
            {
                InformationManager.DisplayMessage(new InformationMessage("SueMoreSpouses load setting failed" + e.Message));
            }
        }

       

        public List<SpouseSettingGroup> GenerateSettingsProperties()
        {

            SpouseSettingBuilder settingBuilder = new SpouseSettingBuilder();

            settingBuilder.BuildGroup("ExspouseGetPregnancy")
                .AddSettingsProperty(new SpouseSettingsProperty("ExspouseGetPregnancyEnable", SpouseSettingsType.BoolProperty, "{=suems_setting_exspouse_pregnancy_enable}Enable ex-spouse get pregnancy")
                .DefaultValue(SettingData.ExspouseGetPregnancyEnable))
                .AddSettingsProperty(new SpouseSettingsProperty("ExspouseGetPregnancyDailyChance", SpouseSettingsType.FloatProperty, "{=suems_setting_exspouse_pregnancy_daily_chance}Daily chance of MainPlayer's ex-spouse or spouse get pregnancy", 0f, 1)
                .DefaultValue(SettingData.ExspouseGetPregnancyDailyChance))
                .AddSettingsProperty(new SpouseSettingsProperty("ExspouseGetPregnancyDurationInDays", SpouseSettingsType.IntegerProperty, "{=suems_setting_exspouse_pregnancy_duration_in_days}Ex-spouse or spouse pregnancy duration in days", 2, 72)
                .DefaultValue(SettingData.ExspouseGetPregnancyDurationInDays));

            settingBuilder.BuildGroup("ChildrenFastGrowth")
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenFastGrowthEnable", SpouseSettingsType.BoolProperty, "{=suems_setting_children_fast_growth_enable}Enable  Children fast growth")
                   .DefaultValue(SettingData.ChildrenFastGrowthEnable))
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenFastGrowthCycleInDays", SpouseSettingsType.IntegerProperty, "{=suems_setting_children_fast_growth_cycle}Children fast growth cycle in days", 2, 72)
                   .DefaultValue(SettingData.ChildrenFastGrowthCycleInDays))
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenFastGrowtStopGrowUpAge", SpouseSettingsType.IntegerProperty, "{=suems_setting_children_fast_growth_stop_age}Children fast growth stop in age", 6, 36)
                   .DefaultValue(SettingData.ChildrenFastGrowtStopGrowUpAge))
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenFastGrowUpScope", SpouseSettingsType.SelectProperty, "{=suems_setting_children_fast_growth_scope}Children fast growth scope")
                   .SetSelectItems(SettingData.HeroSelectScope)
                   .DefaultValue(SettingData.ChildrenFastGrowUpScope)) ;


            settingBuilder.BuildGroup("ChildrenCustomization")
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenSkillFixEnable", SpouseSettingsType.BoolProperty, "{=suems_setting_children_skill_fix}Enable Children's skill fix")
                    .DefaultValue(SettingData.ChildrenSkillFixEnable))
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenNamePrefix", SpouseSettingsType.InputTextProperty, "{=suems_setting_children_name_prefix}Children name prefix")
                    .DefaultValue(SettingData.ChildrenNamePrefix))
                .AddSettingsProperty(new SpouseSettingsProperty("ChildrenNameSuffix", SpouseSettingsType.InputTextProperty, "{=suems_setting_children_name_suffix}Children name suffix")
                    .DefaultValue(SettingData.ChildrenNameSuffix));

            settingBuilder.BuildGroup("NPCCharaObject")
             .AddSettingsProperty(new SpouseSettingsProperty("NPCCharaObjectSkillAuto", SpouseSettingsType.BoolProperty, "{=sms_npc_join_auto_skill}NPC Automatic assignment skills")
             .DefaultValue(SettingData.NPCCharaObjectSkillAuto))
             .AddSettingsProperty(new SpouseSettingsProperty("NPCCharaObjectFromTier", SpouseSettingsType.IntegerProperty, "{=sms_npc_from_tier}NPC tier", 0, 6)
             .DefaultValue(SettingData.NPCCharaObjectFromTier));

            return settingBuilder.Group ;
        }


    }
}
