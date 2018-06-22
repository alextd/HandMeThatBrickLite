using RimWorld;
using Verse;

namespace HMTBLite
{
	public class WorkGiver_DeliverResourcesToFrames : WorkGiver_ConstructDeliverResourcesToFrames
	{
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !Settings.HaulToConstruction || base.ShouldSkip(pawn, forced);
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (!(t is Frame frame))
			{
#if DEBUG
				Log.Error($"HMTB Lite :: Tried to pass frame of type {t.GetType()}");
#endif

				return false;
			}

			return !frame.MaterialsNeeded().NullOrEmpty() && base.HasJobOnThing(pawn, t, forced);
		}
	}
}
