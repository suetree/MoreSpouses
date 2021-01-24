using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SueMoreSpouses.screen.state
{
    class FaceDetailsCreatorState : GameState
    {

		public Hero EditHero { set; get; }

		public FaceDetailsCreatorState()
		{
		}

		public FaceDetailsCreatorState(Hero hero)
		{
			this.EditHero = hero;
	
		}

		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

	}
}
