using HarmonyLib;
using SandBox.GauntletUI;
using SueMoreSpouses.view;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace SueMoreSpouses.Patch
{
    [HarmonyPatch(typeof(ScreenBase))]
    class ClanScreenLayerPatch
    {
		internal static GauntletLayer screenLayer;

		internal static SpouseClanVM spouseClanView;

		[HarmonyPatch("AddLayer")]
		public static void Postfix(ref ScreenBase __instance)
		{
			GauntletClanScreen gauntletClanScreen = __instance as GauntletClanScreen;
			bool flag = gauntletClanScreen != null && ClanScreenLayerPatch.screenLayer == null;
			if (flag)
			{
				ClanScreenLayerPatch.screenLayer = new GauntletLayer(100, "GauntletLayer");
				//Traverse traverse = Traverse.Create(gauntletClanScreen);
				//ClanVM value = traverse.Field<ClanVM>("_dataSource").Value;
				ClanScreenLayerPatch.spouseClanView = new SpouseClanVM(gauntletClanScreen);
				ClanScreenLayerPatch.screenLayer.LoadMovie("SpouseScreen", ClanScreenLayerPatch.spouseClanView);
				ClanScreenLayerPatch.screenLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
				gauntletClanScreen.AddLayer(ClanScreenLayerPatch.screenLayer);
			}
		}

		[HarmonyPatch("RemoveLayer")]
		public static void Prefix(ref ScreenBase __instance, ref ScreenLayer layer)
		{
			bool flag = __instance is GauntletClanScreen && ClanScreenLayerPatch.screenLayer != null && layer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			if (flag)
			{
				__instance.RemoveLayer(ClanScreenLayerPatch.screenLayer);
				ClanScreenLayerPatch.spouseClanView.OnFinalize();
				ClanScreenLayerPatch.spouseClanView = null;
				ClanScreenLayerPatch.screenLayer = null;
			}
		}
	}
}
