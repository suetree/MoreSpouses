using SueMoreSpouses.data;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.sp
{
    class BattleHistorySPCharacterVM: ViewModel
    {
        private SpousesBattleRecordCharacter _battleRecordCharacter;

        private BattleHistorySPScoreVM _score;

        [DataSourceProperty]
        public BattleHistorySPScoreVM Score
        {
            get
            {
                return this._score;
            }
        }

        public BattleHistorySPCharacterVM(SpousesBattleRecordCharacter character)
        {
            this._battleRecordCharacter = character;
            this._score = new BattleHistorySPScoreVM();
            this.Score.UpdateScores(character.Character.Name.ToString(), character.Remain, character.KillCount, character.Wounded, character.RunAway, character.Killed, 0);

        }
    }
}
