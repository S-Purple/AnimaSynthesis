using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AnimaSynthesis
{
    public class Plant_AnimaTree : Plant
    {
        public static readonly List<ThingDef> AnimaTreeStagesInOrder = new List<ThingDef>
        {
            AS_DefOf.AS_Plant_GrowthTreeAnima,
            AS_DefOf.Plant_TreeAnima,
            AS_DefOf.AS_Plant_MatureTreeAnima,
            AS_DefOf.AS_Plant_ElderTreeAnima
        };
        public override float GrowthRate => 0f; // all growth logic is handled custom

        private int lastInteractedTick;

        public bool IsRecentlyInteracted => Find.TickManager.TicksGame - lastInteractedTick < GenDate.TicksPerDay * 14; // less 14 days is recent here
        public float ShrinkRatePerTick => 1f / (60000f * 5); // 5 days must pass until shrinked to 0 from full growth 

        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(2000))
            {
                TickLong();
            }
        }
        public override void TickLong()
        {
            base.TickLong();
            if (!IsRecentlyInteracted)
            {
                growthInt -= ShrinkRatePerTick * 2000f;
                if (growthInt < 0)
                {
                    growthInt = 0;
                }
            }

            if (this.growthInt == 0 && CanConvertToPreviousStage())
            {
                ConvertToPreviousStage();
            }
            else if (this.growthInt >= 1f && CanConvertToNextStage())
            {
                this.growthInt = 0;
                ConvertToNextStage();
            }
        }

        private bool CanConvertToPreviousStage()
        {
            return AnimaTreeStagesInOrder.First() != this.def;
        }

        private void ConvertToPreviousStage()
        {
            var curIndex = AnimaTreeStagesInOrder.IndexOf(this.def);
            var previousStage = AnimaTreeStagesInOrder[curIndex - 1];
            ConvertTo(previousStage);
        }
        private bool CanConvertToNextStage()
        {
            return AnimaTreeStagesInOrder.Last() != this.def;
        }

        private void ConvertToNextStage()
        {
            var curIndex = AnimaTreeStagesInOrder.IndexOf(this.def);
            var nextStage = AnimaTreeStagesInOrder[curIndex + 1];
            ConvertTo(nextStage);
        }
        private void ConvertTo(ThingDef thingDef)
        {
            var animaTree = ThingMaker.MakeThing(thingDef) as Plant_AnimaTree;
            animaTree.lastInteractedTick = this.lastInteractedTick;
            var position = this.Position;
            var map = this.Map;
            this.Destroy();
            GenSpawn.Spawn(animaTree, position, map);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastInteractedTick, "lastInteractedTick");
        }
    }
}
