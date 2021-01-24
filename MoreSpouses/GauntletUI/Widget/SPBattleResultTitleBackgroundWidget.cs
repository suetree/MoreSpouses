using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI;

namespace SueMoreSpouses.widget
{
    class SPBattleResultTitleBackgroundWidget : Widget
    {
		private int _battleResult;

		private Widget _victoryWidget;

		private Widget _defeatWidget;

		[Editor(false)]
		public int BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (this._battleResult != value)
				{
					this._battleResult = value;
					base.OnPropertyChanged(value, "BattleResult");
					this.BattleResultUpdated();
				}
			}
		}

		[Editor(false)]
		public Widget VictoryWidget
		{
			get
			{
				return this._victoryWidget;
			}
			set
			{
				if (this._victoryWidget != value)
				{
					this._victoryWidget = value;
					base.OnPropertyChanged(value, "VictoryWidget");
				}
			}
		}

		[Editor(false)]
		public Widget DefeatWidget
		{
			get
			{
				return this._defeatWidget;
			}
			set
			{
				if (this._defeatWidget != value)
				{
					this._defeatWidget = value;
					base.OnPropertyChanged(value, "DefeatWidget");
				}
			}
		}

		public SPBattleResultTitleBackgroundWidget(UIContext context) : base(context)
		{
		}

		private void BattleResultUpdated()
		{
			if (this.BattleResult == 1) 
			{
				//胜利的时候
				this.DefeatWidget.IsVisible = false;
				this.VictoryWidget.IsVisible = true;

			}
			else
			{
				this.DefeatWidget.IsVisible = true;
				this.VictoryWidget.IsVisible = false;
			}
			
		}
	}
}
