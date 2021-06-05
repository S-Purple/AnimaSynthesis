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
    public class CompProperties_DefensiveShield : CompProperties_ProjectileInterceptor
    {
        public int activeTicks;
		public CompProperties_DefensiveShield()
		{
			compClass = typeof(CompDefensiveShield);
		}
	}

	public class CompDefensiveShield : CompProjectileInterceptor
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Traverse.Create(this).Field("shutDown").SetValue(true);
            }
        }
        public new CompProperties_DefensiveShield Props => this.props as CompProperties_DefensiveShield;

        private int activatedTick;
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var g in base.CompGetGizmosExtra())
            {
                yield return g;
            }
            var command = new Command_Action
            {
                defaultLabel = "AS.DefensiveShield".Translate(),
                defaultDesc = "AS.DefensiveShieldDesc".Translate(),
                action = delegate
                {
                    activatedTick = Find.TickManager.TicksGame;
                    Traverse.Create(this).Field("shutDown").SetValue(false);
                }
            };
            if (Active)
            {
                command.Disable("AS.DefensiveShieldActive".Translate());
            }
            yield return command;
        }
        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.Spawned && Active && Find.TickManager.TicksGame >= activatedTick + Props.activeTicks)
            {
                Traverse.Create(this).Field("shutDown").SetValue(true);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref activatedTick, "activatedTick");
        }
    }
}
