﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AnimaSynthesis
{
    public class MapPawns
    {
        public MapPawns(List<Pawn> pawns)
        {
            this.pawns = pawns;
            this.lastTickCheck = Find.TickManager.TicksAbs;
        }
        public List<Pawn> pawns;
        public int lastTickCheck;
    }

    public class CompProperties_RegenerationField : CompProperties
	{
		public float radius;
		public CompProperties_RegenerationField()
		{
			compClass = typeof(CompRegenerationField);
		}
	}

	public class CompRegenerationField : ThingComp
    {
        public CompProperties_RegenerationField Props => this.props as CompProperties_RegenerationField;
        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.Spawned)
            {
                foreach (var pawn in GetAllPawns(this.parent.Map))
                {
                    if (!pawn.Dead && pawn.Spawned && pawn.Position.DistanceTo(this.parent.Position) <= Props.radius)
                    {
                        if (pawn.health.hediffSet.GetFirstHediffOfDef(AS_DefOf.AS_HealingEffect) is null)
                        {
                            var hediff = HediffMaker.MakeHediff(AS_DefOf.AS_HealingEffect, pawn) as Hediff_HealingEffect;
                            hediff.parent = this.parent;
                            pawn.health.AddHediff(hediff);
                        }
                    }
                }
            }
        }

        public static Dictionary<Map, MapPawns> mapPawns = new Dictionary<Map, MapPawns>();
        public static List<Pawn> GetAllPawns(Map map)
        {
            if (mapPawns.TryGetValue(map, out MapPawns mapPawns2))
            {
                if (Find.TickManager.TicksAbs > mapPawns2.lastTickCheck + 60)
                {
                    mapPawns2.pawns = map.mapPawns.AllPawns.ToList();
                    mapPawns2.lastTickCheck = Find.TickManager.TicksAbs;
                }
                return mapPawns2.pawns;
            }
            else
            {
                var pawns = map.mapPawns.AllPawns.ToList();
                mapPawns[map] = new MapPawns(pawns);
                return pawns;
            }
        }
    }
}
