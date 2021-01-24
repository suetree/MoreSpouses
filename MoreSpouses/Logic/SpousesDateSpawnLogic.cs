using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SueMoreSpouses.logic
{
    class SpousesDateSpawnLogic : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
    {

        private List<Hero> _needSpawnHeros;
        private bool _isMissionInitialized;
        private bool _troopsInitialized;
        private BattleAgentLogic _battleAgentLogic;
        public bool IsSideDepleted(BattleSideEnum side)
        {
            return false;
        }

        public SpousesDateSpawnLogic(List<Hero> heros)
        {
            if(null != heros)
            {
                this._needSpawnHeros = heros;
            }
            else
            {
                this._needSpawnHeros = new List<Hero>();
            }
         
        }

        public void StopSpawner()
        {
            // throw new NotImplementedException();
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
                    this._battleAgentLogic.OnAgentBuild(current, Clan.PlayerClan.Banner);
                }
            }

            FlowMainPlayer();
        }

        private void FlowMainPlayer()
        {
            foreach (Agent agent in base.Mission.Agents)
            {
                if (agent != Agent.Main)
                {
                    if (agent.IsHuman)
                    {

                        CampaignAgentComponent campaignAgentComponent = agent.GetComponent<CampaignAgentComponent>();
                        if(null != campaignAgentComponent && null == campaignAgentComponent.AgentNavigator)
                        {
                            campaignAgentComponent.CreateAgentNavigator();
                        }
                      //  agent.SetLookAgent(Agent.Main);
                        agent.SetFollowedUnit(Agent.Main);
                    }
                  
                
                  
                    //agent.SetGuardedAgent(Agent.Main);
                }
            }
        }

        private void SpawnAgents()
        {
            GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("attacker_infantry");
            foreach (Hero hero in this._needSpawnHeros)
            {
                WorldFrame spawnPosition = WorldFrame.Invalid;
                spawnPosition = new WorldFrame(gameEntity.GetGlobalFrame().rotation, new WorldPosition(gameEntity.Scene, gameEntity.GetGlobalFrame().origin));
                SimpleAgentOrigin agentOrigin = new SimpleAgentOrigin(hero.CharacterObject, -1, null, default(UniqueTroopDescriptor));
                bool spawnWithHorse = true;
                Agent agent = Mission.Current.SpawnTroop(agentOrigin, true, false, spawnWithHorse, false, false, 0, 0, true, false, false, null, spawnPosition.ToGroundMatrixFrame());
                agent.UpdateSpawnEquipmentAndRefreshVisuals(hero.CivilianEquipment);
                if (!agent.IsMainAgent)
                {
                    SimulateAgent(agent);
                }
                
               // agent.SetGuardedAgent(Agent.Main);
                // Agent.Main

            }
           
        }

        public void SimulateAgent(Agent agent)
        {
            if (agent.IsHuman)
            {
                AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
                int num = MBRandom.RandomInt(35, 50);
                agent.PreloadForRendering();
                for (int i = 0; i < num; i++)
                {
                    if (agentNavigator != null)
                    {
                        agentNavigator.Tick(0.1f, true);
                    }
                    if (agent.IsUsingGameObject)
                    {
                        agent.CurrentlyUsedGameObject.SimulateTick(0.1f);
                    }
                }
            }
        }

    }
}
