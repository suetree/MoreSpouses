using SueMoreSpouses.data.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.data
{
    class SpousesSaveDefiner : SaveableTypeDefiner
    {
        //https://www.fileformat.info/tool/hash.htm PUT YOUR MOD'S NAME INTO THIS TOOL AND THEN SCROLL DOWN TO GET THE FIRST 6 NUMBERS OF YOUR MOD'S CRC32 HASH
        //AIValuesLife = c5e07920 -> just use first 6 digits e07920
        public SpousesSaveDefiner() : base((0x382a23 << 8) | 123)//this will ensure you use a base id unique to your mod... people need to do this or else you will have problems saving with many mods installed
        {
        }

        
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(SpousesHeroStatistic), 2009021);//DEFINING YOUR CLASS TO THE GAME, USE UNIQUE ID PER SAVEABLE CLASS
            AddClassDefinition(typeof(SpousesBattleRecord), 2009022); //YYMMDD + NO
            AddClassDefinition(typeof(SpousesBattleRecordSide), 2009023);
            AddClassDefinition(typeof(SpousesBattleRecordParty), 2009024);
            AddClassDefinition(typeof(SpousesBattleRecordCharacter), 2009025);
            AddClassDefinition(typeof(SpousesBattleRecordReward), 2009026);

        }
        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<SpousesHeroStatistic>));//DEFINING ANY CONTAINERS YOU PLAN TO USE
            ConstructContainerDefinition(typeof(List<SpousesBattleRecord>));
            ConstructContainerDefinition(typeof(List<SpousesBattleRecordSide>));
            ConstructContainerDefinition(typeof(List<SpousesBattleRecordParty>));
            ConstructContainerDefinition(typeof(List<SpousesBattleRecordCharacter>));
            ConstructContainerDefinition(typeof(List<SpousesBattleRecordReward>));
        }
    }
}
