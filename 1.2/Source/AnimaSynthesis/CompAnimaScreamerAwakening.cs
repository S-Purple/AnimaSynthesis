using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AnimaSynthesis
{

    public class CompProperties_AnimaScreamerAwakening : CompProperties
	{
		public int cooldownTicks;
		public CompProperties_AnimaScreamerAwakening()
		{
			compClass = typeof(CompAnimaScreamerAwakening);
		}
	}

	public class CompAnimaScreamerAwakening : ThingComp
    {
        public CompProperties_AnimaScreamerAwakening Props => this.props as CompProperties_AnimaScreamerAwakening;

        private int activatedTick;
        public bool HasCooldown => activatedTick > 0 && Find.TickManager.TicksGame < activatedTick + Props.cooldownTicks;
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var g in base.CompGetGizmosExtra())
            {
                yield return g;
            }
            var command = new Command_Action
            {
                defaultLabel = "AS.AnimaScreamer".Translate(),
                defaultDesc = "AS.AnimaScreamerDesc".Translate(),
                action = delegate
                {
                    var map = this.parent.Map;
                    var pos = this.parent.Position;
                    var faction = this.parent.Faction;
                    this.parent.Destroy();
                    var monster = PawnGenerator.GeneratePawn(AS_DefOf.AS_AnimaScreamer, faction);
                    GenSpawn.Spawn(monster, pos, map);
                }
            };
            if (HasCooldown)
            {
                command.Disable("AS.AnimaScreamerCooldownActive".Translate());
            }
            yield return command;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref activatedTick, "activatedTick");
        }

    }
}
