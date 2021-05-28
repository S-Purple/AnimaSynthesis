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
    public class Hediff_HealingEffect : HediffWithComps
    {
        public Thing parent;

        private CompRegenerationField compRegenerationField;
        private CompRegenerationField CompRegenerationField
        {
            get
            {
                if (parent != null && compRegenerationField is null)
                {
                    compRegenerationField = parent.TryGetComp<CompRegenerationField>();
                }
                return compRegenerationField;
            }
        }
        public override void Tick()
        {
            base.Tick();
            if (parent != null && this.pawn.Position.DistanceTo(CompRegenerationField.parent.Position) > CompRegenerationField.Props.radius)
            {
                this.pawn.health.RemoveHediff(this);
            }
            else if (parent == null || parent.Destroyed || !parent.Spawned || (!CompRegenerationField?.Active ?? true))
            {
                this.pawn.health.RemoveHediff(this);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref parent, "source");
        }
    }
}
