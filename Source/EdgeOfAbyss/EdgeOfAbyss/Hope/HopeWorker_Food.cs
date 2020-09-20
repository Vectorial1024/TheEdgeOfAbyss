using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeWorker_Food : HopeWorker
    {
        public override bool HopeIsApplicableToCreature => pawn.needs.food != null;

        public HopeWorker_Food(Pawn pawn): base(pawn)
        {
            // no assumption on food level to be made
        }

        public static List<ThoughtDef> BadThoughtsFromBadFood = new List<ThoughtDef>()
        {
            // general bad food
            ThoughtDefOf.AteRawFood,
            ThoughtDefOf.AteRottenFood,
            // insect meat & incompatible food
            ThoughtDefOf.AteInsectMeatDirect,
            ThoughtDefOf.AteInsectMeatAsIngredient,
            EdgeOfAbyss_ThoughtDefOf.AteKibble,
            // corpses and cannibalism
            ThoughtDefOf.AteCorpse,
            ThoughtDefOf.AteHumanlikeMeatDirect,
            ThoughtDefOf.AteHumanlikeMeatAsIngredient,
        };

        private float HungerRate => pawn.ageTracker.CurLifeStage.hungerRateFactor * pawn.RaceProps.baseHungerRate * pawn.health.hediffSet.HungerRateFactor * pawn.GetStatValue(StatDefOf.HungerRateMultiplier);

        /// <summary>
        /// Calculates the instantaneous food hope fall rate per tick.
        /// <para/>
        /// The fall rate penalty from malnutrition is already included in this.
        /// </summary>
        public float CurrentFoodHopeFallPerTick
        {
            get
            {
                // calculate fall rate from natural hunger
                Need_Food foodNeed = pawn.needs.food;
                float baseFoodHopeFallPerTick = foodNeed.FoodFallPerTickAssumingCategory(HungerCategory.Fed);
                float adjustedFoodHopeFallPerTick;
                switch (foodNeed.CurCategory)
                {
                    case HungerCategory.Fed:
                        adjustedFoodHopeFallPerTick = baseFoodHopeFallPerTick * 1;
                        break;
                    case HungerCategory.Hungry:
                        adjustedFoodHopeFallPerTick = baseFoodHopeFallPerTick * 2;
                        break;
                    case HungerCategory.UrgentlyHungry:
                        adjustedFoodHopeFallPerTick = baseFoodHopeFallPerTick * 3;
                        break;
                    case HungerCategory.Starving:
                        adjustedFoodHopeFallPerTick = baseFoodHopeFallPerTick * 4;
                        break;
                    default:
                        throw new InvalidOperationException("Incorrect HungerCategory for HopeWorker_Food");
                }
                // calculate fall rate penalty from malnutrition
                float malnutritionSeverity = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition)?.Severity ?? 0;
                if (malnutritionSeverity > 0)
                {
                    if (malnutritionSeverity >= 0.75f)
                    {
                        adjustedFoodHopeFallPerTick *= 4;
                    }
                    else if (malnutritionSeverity >= 0.5f)
                    {
                        adjustedFoodHopeFallPerTick *= 3;
                    }
                    else
                    {
                        // malnutritionSeverity > 0
                        adjustedFoodHopeFallPerTick *= 2;
                    }
                }
                return adjustedFoodHopeFallPerTick;
            }
        }

        public override void Tick150Interval()
        {
            // hope level affected by 2 things:
            // 1. Standard decay from natural hunger
            // 2. Extra decay from malnutrition

            hopeLevel -= CurrentFoodHopeFallPerTick * HopeTickInterval;
            if (ExpectedRange > 0 && hopeLevel < -ExpectedRange)
            {
                hopeLevel = -ExpectedRange;
            }
            //hopeLevel = Mathf.Clamp(hopeLevel, -ExpectedRange, ExpectedRange);
            return;
        }

        public void Notify_FoodConsumed(Thing food, float consumedNutrition)
        {
            // EdgeOfAbyssMain.LogError("Eating " + food.ToString() + ", " + consumedNutrition);
            // determine actual amounts of nutrition consumed
            float actualNutritionRestored = Mathf.Min(consumedNutrition, pawn.needs.food.NutritionWanted);
            List<ThoughtDef> foodThoughts = FoodUtility.ThoughtsFromIngesting(pawn, food, food.def);
            float foodHopeRestoration;
            if (CalculateFoodHopeRestoration_VarietyMatters(food, consumedNutrition, actualNutritionRestored, out foodHopeRestoration))
            {
                // successfully calculated food hope through the Variety Matters submod
            }
            else
            {
                // this always returns something
                foodHopeRestoration = CalculateFoodHopeRestoration_Vanilla(food, consumedNutrition, actualNutritionRestored);
            }
            
            // Royalty DLC
            if (foodThoughts.Contains(ThoughtDefOf.AteFoodInappropriateForTitle))
            {
                // this is just a blind guess right now
                // not saying the food is trash, but the food is not exactly something I want
                foodHopeRestoration *= 0.75f;
            }

            AdjustHopeLevel(foodHopeRestoration);
        }

        protected void AdjustHopeLevel(float amount)
        {
            hopeLevel += amount;
            if (hopeLevel > ExpectedRange)
            {
                hopeLevel = ExpectedRange;
            }
        }

        // let the potential VarietyMatters sub-mod override this.
        public bool CalculateFoodHopeRestoration_VarietyMatters(Thing food, float consumedNutrition, float effectiveConsumedNutrition, out float hopeRestoration)
        {
            hopeRestoration = 0;
            return false;
        }

        // must always succeed here.
        public float CalculateFoodHopeRestoration_Vanilla(Thing food, float consumedNutrition, float effectiveConsumedNutrition)
        {
            float hopeAdjustment = effectiveConsumedNutrition;
            List<ThoughtDef> foodThoughts = FoodUtility.ThoughtsFromIngesting(pawn, food, food.def);
            // Mainly to check if the food is freaking raw
            if (foodThoughts.Contains(ThoughtDefOf.AteAwfulMeal))
            {
                // nutrient paste dispenser
                hopeAdjustment *= 0.75f;
            }
            else if (foodThoughts.Contains(ThoughtDefOf.AteFineMeal))
            {
                hopeAdjustment *= 1.25f;
            }
            else if (foodThoughts.Contains(ThoughtDefOf.AteLavishMeal))
            {
                hopeAdjustment *= 1.5f;
            }
            else if (foodThoughts.Any(thought => BadThoughtsFromBadFood.Contains(thought)))
            {
                // basically the rest of the possible bad thoughts, because we cannot easily detect
                // eating e.g. Simple Meal, Berries, Pemmican etc. otherwise.
                hopeAdjustment = 0;
            }

            return hopeAdjustment;
        }
    }
}
