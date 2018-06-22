using Verse;

namespace HMTBLite
{
	public class Settings : ModSettings
	{
		public static bool HaulToConstruction = true;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref HaulToConstruction, "HMTB_HaulToConstruction", true);
		}
	}
}
