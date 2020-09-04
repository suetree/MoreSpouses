using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI.Data;

namespace SueMoreSpouses.patch
{
   // [HarmonyPatch(typeof(GauntletView), "AddItemToList")]
    class TestPath
    {

        public static void Prefix(int index)
        {

        }
    }
}
