using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace SueMoreSpouses.patch
{
    [HarmonyPatch(typeof(HeroCreator), "DeliverOffSpring")]
    class HeroCreatorDeliverOffSpringPath
    {
        static void Postfix(ref Hero __result, Hero mother, Hero father, bool isOffspringFemale, int age = 1)
        {
            if (Hero.MainHero.Children.Contains(__result))
            {
                if (null != MoreSpouseSetting.Instance.SettingData.ChildrenNamePrefix && MoreSpouseSetting.Instance.SettingData.ChildrenNamePrefix.Length > 0)
                {
                    __result.Name = new TaleWorlds.Localization.TextObject(MoreSpouseSetting.Instance.SettingData.ChildrenNamePrefix + __result.Name.ToString());
                }

                if (null != MoreSpouseSetting.Instance.SettingData.ChildrenNameSuffix && MoreSpouseSetting.Instance.SettingData.ChildrenNameSuffix.Length > 0)
                {
                    __result.Name = new TaleWorlds.Localization.TextObject( __result.Name.ToString() + MoreSpouseSetting.Instance.SettingData.ChildrenNameSuffix);
                }
            }
        }
    }
}
