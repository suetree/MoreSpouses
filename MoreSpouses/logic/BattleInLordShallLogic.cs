using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SueMoreSpouses.logic
{
    class BattleInLordShallLogic : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
    {
		private BattleAgentLogic _battleAgentLogic;

		private bool _isMissionInitialized;

        private bool _troopsInitialized;

        private int _numberOfMaxTroopForPlayer;

        private int _numberOfMaxTroopForEnemy;

        private int _playerSideSpawnedTroopCount;

        private int _otherSideSpawnedTroopCount;

        private readonly IMissionTroopSupplier[] _troopSuppliers;


        public BattleInLordShallLogic(IMissionTroopSupplier[] suppliers, int numberOfMaxTroopForPlayer, int numberOfMaxTroopForEnemy)
        {
            this._troopSuppliers = suppliers;
            this._numberOfMaxTroopForPlayer = numberOfMaxTroopForPlayer;
            this._numberOfMaxTroopForEnemy = numberOfMaxTroopForEnemy;
        }

		public override void OnBehaviourInitialize()
		{
			base.OnBehaviourInitialize();
			this._battleAgentLogic = Mission.Current.GetMissionBehaviour<BattleAgentLogic>();
		}

		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this.SpawnAgents();
				this._isMissionInitialized = true;
				return;
			}
			if (!this._troopsInitialized)
			{
				this._troopsInitialized = true;
				foreach (Agent current in base.Mission.Agents)
				{
					this._battleAgentLogic.OnAgentBuild(current, null);
				}
			}
		}

		private void SpawnAgents()
		{
			
			IMissionTroopSupplier[] troopSuppliers = this._troopSuppliers;
			for (int i = 0; i < troopSuppliers.Length; i++)
			{
				foreach (IAgentOriginBase current in troopSuppliers[i].SupplyTroops(this._numberOfMaxTroopForPlayer + this._numberOfMaxTroopForEnemy).ToList<IAgentOriginBase>())
				{
					bool flag = current.IsUnderPlayersCommand || current.Troop.IsPlayerCharacter;
					if ((!flag || this._numberOfMaxTroopForPlayer >= this._playerSideSpawnedTroopCount) && (flag || this._numberOfMaxTroopForEnemy >= this._otherSideSpawnedTroopCount))
					{
						
						Mission.Current.SpawnTroop(current, flag, false, false, false, false, 0, 0, true, false, false, null, null);
						if (flag)
						{
							this._playerSideSpawnedTroopCount++;
						}
						else
						{
							this._otherSideSpawnedTroopCount++;
						}
					}
				}
			}
		}

		public void StopSpawner()
		{
		}

		public bool IsSideDepleted(BattleSideEnum side)
		{
			if (side == base.Mission.PlayerTeam.Side)
			{
				return this._troopSuppliers[(int)side].NumRemovedTroops == this._playerSideSpawnedTroopCount;
			}
			return this._troopSuppliers[(int)side].NumRemovedTroops == this._otherSideSpawnedTroopCount;
		}

    }
}
