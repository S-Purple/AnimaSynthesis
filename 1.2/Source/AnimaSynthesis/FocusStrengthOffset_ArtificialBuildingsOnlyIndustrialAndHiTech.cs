using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AnimaSynthesis
{
    public class FocusStrengthOffset_ArtificialBuildingsOnlyIndustrialAndHiTech : FocusStrengthOffset_ArtificialBuildings
    {
        protected override float SourceValue(Thing parent)
        {
			return parent.Spawned ? parent.Map.listerArtificialBuildingsForMeditation.GetForCell(parent.Position, radius).Where(x => x.def.techLevel >= TechLevel.Industrial).Count() : 0;
        }

		public override string InspectStringExtra(Thing parent, Pawn user = null)
		{
			if (parent.Spawned)
			{
				List<Thing> forCell = parent.Map.listerArtificialBuildingsForMeditation.GetForCell(parent.Position, radius).Where(x => x.def.techLevel >= TechLevel.Industrial).ToList();
				if (forCell.Count > 0)
				{
					TaggedString taggedString = "MeditationFocusImpacted".Translate() + ": " + (from c in forCell.Take(3)
																								select c.LabelShort).ToCommaList().CapitalizeFirst();
					if (forCell.Count > 3)
					{
						taggedString += " " + "Etc".Translate();
					}
					return taggedString;
				}
			}
			return "";
		}
	}
}
