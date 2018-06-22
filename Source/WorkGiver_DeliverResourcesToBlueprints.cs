using RimWorld;
using Verse;

namespace HMTBLite
{
	public class WorkGiver_DeliverResourcesToBlueprints : WorkGiver_ConstructDeliverResourcesToBlueprints
	{
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !Settings.HaulToConstruction || base.ShouldSkip(pawn, forced);
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (!(t is Blueprint blueprint))
			{
#if DEBUG
				Log.Error($"HMTB Lite :: Tried to pass blueprint of type {t.GetType()}");
#endif

				return false;
			}

			bool blueprintAllowed = blueprint is Blueprint_Install || !blueprint.MaterialsNeeded().NullOrEmpty();

			return blueprintAllowed && base.HasJobOnThing(pawn, t, forced);
		}
	}
}
