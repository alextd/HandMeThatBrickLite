using System;
using System.Reflection;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HMTBLite
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		private static readonly float toggleX = 5f;

		private static readonly float toggleY = 50f;

		static HarmonyPatches()
		{
			Type workTabWindowClass = DefDatabase<MainButtonDef>.GetNamed("Work").tabWindowClass;
			MethodInfo workTabDoWindowContents = AccessTools.Method(workTabWindowClass, "DoWindowContents");
			MethodInfo deliverToFramesShouldSkip = AccessTools.Method(typeof(WorkGiver_ConstructDeliverResourcesToFrames), nameof(WorkGiver_ConstructDeliverResourcesToFrames.ShouldSkip));
			MethodInfo deliverToBlueprintsShouldSkip = AccessTools.Method(typeof(WorkGiver_ConstructDeliverResourcesToBlueprints), nameof(WorkGiver_ConstructDeliverResourcesToBlueprints.ShouldSkip));

			// Compatibility with Fluffy's WorkTab mod
			if (workTabWindowClass.FullName.ToString() == "WorkTab.MainTabWindow_WorkTab")
			{
#if DEBUG
				Log.Message("HMTB Lite :: Modifying toggle Rect for Fluffy's WorkTab");
#endif

				toggleY = 5f;
			}

			// Patch with Harmony
#if DEBUG
			HarmonyInstance.DEBUG = true;
#endif

			HarmonyInstance harmony = HarmonyInstance.Create("dingo.hmtblite");

			harmony.Patch(workTabDoWindowContents, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Patch_Postfix_MainTabWindow_Work)), null);
			harmony.Patch(deliverToFramesShouldSkip, new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Patch_Prefix_WorkGivers_ShouldSkip)), null, null);
			harmony.Patch(deliverToBlueprintsShouldSkip, new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Patch_Prefix_WorkGivers_ShouldSkip)), null, null);

#if DEBUG
			Log.Message("HMTB Lite :: Injected Harmony patches");
#endif
		}

		// Add a Hand Me That Brick toggle to the currently used Work Tab
		public static void Patch_Postfix_MainTabWindow_Work()
		{
			string toggleLabel = "HMTB.ToggleHaulToConstruction".Translate();
			Vector2 toggleLabelSize = Text.CalcSize(toggleLabel);

			Rect hmtbToggleRect = new Rect(toggleX, toggleY, toggleLabelSize.x + 10f + Widgets.CheckboxSize, toggleLabelSize.y);

			Widgets.CheckboxLabeled(hmtbToggleRect, toggleLabel, ref Settings.HaulToConstruction);
			Widgets.DrawHighlightIfMouseover(hmtbToggleRect);
			TooltipHandler.TipRegion(hmtbToggleRect, "HMTB.ToggleHaulToConstruction.Tooltip".Translate());
		}

		// Add a toggle check to the WorkGivers
		public static bool Patch_Prefix_WorkGivers_ShouldSkip(WorkGiver_ConstructDeliverResources __instance, ref bool __result)
		{
			if (__instance.def.workType == WorkTypeDefOf.Hauling && !Settings.HaulToConstruction)
			{
				__result = true;

				return false;
			}

			return true;
		}
	}
}
