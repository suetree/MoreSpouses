using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SueMoreSpouses.Utils
{
    class HeroFaceUtils
    {
		public static void UpdatePlayerCharacterBodyProperties(Hero hero, BodyProperties properties, bool isFemale)
		{
			ReflectUtils.ReflectPropertyAndSetValue("StaticBodyProperties", properties.StaticProperties, hero);
			hero.Weight = properties.Weight;
			hero.Build = properties.Build;
			hero.UpdatePlayerGender(isFemale);
		}
	}
}
