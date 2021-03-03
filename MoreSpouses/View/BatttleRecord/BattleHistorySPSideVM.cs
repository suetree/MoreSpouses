using SueMoreSpouses.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.sp
{
    class BattleHistorySPSideVM: ViewModel
    {
        private SpousesBattleRecordSide _side;

        private BattleHistorySPScoreVM _sideScore;
        private MBBindingList<BattleHistorySPPartyVM> _parties;
        private ImageIdentifierVM _bannerVisual;

        private ImageIdentifierVM _bannerVisualSmall;

        [DataSourceProperty]
        public BattleHistorySPScoreVM Score
        {
            get {
                return this._sideScore;
            }
        }


        [DataSourceProperty]
        public ImageIdentifierVM BannerVisual
        {
            get
            {
                return this._bannerVisual;
            }
            set
            {
                if (value != this._bannerVisual)
                {
                    this._bannerVisual = value;
                    base.OnPropertyChangedWithValue(value, "BannerVisual");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM BannerVisualSmall
        {
            get
            {
                return this._bannerVisualSmall;
            }
            set
            {
                if (value != this._bannerVisualSmall)
                {
                    this._bannerVisualSmall = value;
                    base.OnPropertyChangedWithValue(value, "BannerVisualSmall");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<BattleHistorySPPartyVM> Parties
        {
            get
            {
                return this._parties;
            }
            set
            {
                if (value != this._parties)
                {
                    this._parties = value;
                    base.OnPropertyChanged("Parties");
                }
            }
        }

        public  BattleHistorySPSideVM()
        {
            this.Parties = new MBBindingList<BattleHistorySPPartyVM>();
            this._sideScore = new BattleHistorySPScoreVM();
        }

        public void FillHistorySide(SpousesBattleRecordSide side)
        {
            this._side = side;
            this.Score.UpdateScores(side.Name, side.Remain, side.KillCount, side.Wounded, side.RunAway, side.Killed, 0);
            this.Parties.Clear();
            if (side.Parties.Count > 0)
            {
                side.Parties.ForEach(obj => {
                    if (null != obj)
                    {
                        Parties.Add(new BattleHistorySPPartyVM(obj));
                    }
                });
            }

            if (null != side.Banner)
            {
                BannerCode bannerCode = BannerCode.CreateFrom(side.Banner);
                this.BannerVisual = new ImageIdentifierVM(bannerCode, true);
                this.BannerVisualSmall = new ImageIdentifierVM(bannerCode, false);
            }
           

        }


	}
}
