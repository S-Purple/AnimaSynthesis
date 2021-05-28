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
	public class CompProperties_RainCallOnFire : CompProperties
	{
		public CompProperties_RainCallOnFire()
		{
			compClass = typeof(CompRainCallOnFire);
		}
	}

	public class CompRainCallOnFire : ThingComp
    {
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
			TryCallRainIfFireFound();
        }

        public override void CompTick()
        {
            base.CompTick();
			TryCallRainIfFireFound();
		}

        public override void CompTickLong()
        {
            base.CompTickLong();
			TryCallRainIfFireFound();
		}
        public void TryCallRainIfFireFound()
        {
			if (this.parent.Map != null && this.parent.Map.weatherManager.RainRate < 0.1f)
            {
				var fire = this.parent.Position.GetFirstThing<Fire>(this.parent.Map);
				if (fire != null)
                {
					if (DefDatabase<WeatherDef>.AllDefs.Where(x => x.rainRate >= 0.1f && !x.isBad)
						.TryRandomElementByWeight(x => CurrentWeatherCommonality(this.parent.Map, x), out WeatherDef result))
                    {
						this.parent.Map.weatherManager.TransitionTo(result);
						Traverse.Create(this.parent.Map.weatherDecider).Field("curWeatherDuration").SetValue(result.durationRange.RandomInRange);
                    }
                }
            }
		}

		private float CurrentWeatherCommonality(Map map, WeatherDef weather)
		{
			if (map.weatherManager.curWeather != null && !map.weatherManager.curWeather.repeatable && weather == map.weatherManager.curWeather)
			{
				return 0f;
			}
			if (!weather.temperatureRange.Includes(map.mapTemperature.OutdoorTemp))
			{
				return 0f;
			}

			BiomeDef biome = map.Biome;
			for (int i = 0; i < biome.baseWeatherCommonalities.Count; i++)
			{
				WeatherCommonalityRecord weatherCommonalityRecord = biome.baseWeatherCommonalities[i];
				if (weatherCommonalityRecord.weather == weather)
				{
					float num = weatherCommonalityRecord.commonality;
					if (map.fireWatcher.LargeFireDangerPresent && weather.rainRate > 0.1f)
					{
						num *= 15f;
					}
					if (weatherCommonalityRecord.weather.commonalityRainfallFactor != null)
					{
						num *= weatherCommonalityRecord.weather.commonalityRainfallFactor.Evaluate(map.TileInfo.rainfall);
					}
					return num;
				}
			}
			return 0f;
		}
	}
}
