using System;
using System.Reflection;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace HMTBLite
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		private static readonly float toggleX = 5f;

		private static readonly float toggleY = 50f;

		static HarmonyPatches()
		{
			// Ensure maximum compatibility with mods by patching the current Work tab's class
			Type WorkTabWindowClass = DefDatabase<MainButtonDef>.GetNamed("Work").tabWindowClass;
			MethodInfo workTabWindowContents = AccessTools.Method(WorkTabWindowClass, "DoWindowContents");

			// Make the toggle more compatible with Fluffy's mod by moving it to the top of the WorkTab window
			if (LoadedModManager.RunningModsListForReading.Any(m => m.Name == "Work Tab"))
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

			harmony.Patch(workTabWindowContents, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Patch_MainTabWindow_Work)));

#if DEBUG
			Log.Message("HMTB Lite :: Injected Harmony patches");
#endif
		}

		//Add a Hand Me That Brick toggle to the currently used Work Tab
		public static void Patch_MainTabWindow_Work()
		{
			string text = "HMTB.ToggleHaulToConstruction".Translate();
			Vector2 textSize = Text.CalcSize(text);

			Rect hmtbToggleRect = new Rect(toggleX, toggleY, textSize.x + 10f + Widgets.CheckboxSize, textSize.y);

			Widgets.CheckboxLabeled(hmtbToggleRect, text, ref Settings.HaulToConstruction);
			Widgets.DrawHighlightIfMouseover(hmtbToggleRect);
			TooltipHandler.TipRegion(hmtbToggleRect, "HMTB.ToggleHaulToConstruction.Tooltip".Translate());
		}
	}
}
