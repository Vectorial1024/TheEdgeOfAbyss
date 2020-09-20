using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeWorker_Rest : HopeWorker
    {
        float accumulatedRest;

        static readonly float cachedRestEffectivenessConversionValue = 1 / EdgeOfAbyss_Constants.MaxTicksToFillOneRest;

        public override bool HopeIsApplicableToCreature => pawn.needs.rest != null;

        public HopeWorker_Rest(Pawn pawn): base(pawn)
        {
            accumulatedRest = 0;
        }

        /// <summary>
        /// The multiplier for this hope after balancing the numbers
        /// </summary>
        public readonly float RestHopeGainMultiplier = 1.15f;
        
        public bool IsCurrentlyResting => pawn.needs.rest.GUIChangeArrow == 1;

        private float RestFallFactor => pawn.health.hediffSet.RestFallFactor;

        public float RestFallPerTick => RestFallFactor / EdgeOfAbyss_Constants.TicksPerDay;

        public float CurrentRestHopeFallPerTick
        {
            get
            {
                // calculate fall rate from natural exhausion; may need to listen to rest ticker to cancel the changes
                float baseFallRate = RestFallPerTick;
                switch (pawn.needs.rest.CurCategory)
                {
                    case RestCategory.Rested:
                        return baseFallRate;
                    case RestCategory.Tired:
                        return baseFallRate * 1.25f;
                    case RestCategory.VeryTired:
                        return baseFallRate * 1.5f;
                    case RestCategory.Exhausted:
                        return baseFallRate * 2;
                    default:
                        throw new InvalidOperationException("Incorrect RestCategory for HopeWorker_Rest");
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref accumulatedRest, "accumulatedRest");
        }

        public void Notify_TickResting(float restEffectiveness)
        {
            accumulatedRest += restEffectiveness;
        }

        public override void Tick150Interval()
        {
            if (!HopeIsApplicableToCreature)
            {
                return;
            }
            if (accumulatedRest > 0)
            {
                // should be (average accumulated rest) * 150 / (10.5 hour * 2500 tick per hour)
                // we know (average accumulated rest) approx= (accumulated rest) / 150
                // after simplify it becomes the following expression:
                // accumulated rest / (10.5 * 2500)
                // further optimize that and we have this real statement below
                float approxRestGained = accumulatedRest * cachedRestEffectivenessConversionValue;
                hopeLevel += approxRestGained * pawn.GetStatValue(StatDefOf.RestRateMultiplier) * RestHopeGainMultiplier;
                accumulatedRest = 0;
            }
            else
            {
                hopeLevel -= CurrentRestHopeFallPerTick * 150;
            }
        }
    }
}
