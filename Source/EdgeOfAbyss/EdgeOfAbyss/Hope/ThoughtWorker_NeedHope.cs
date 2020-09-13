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
    public class ThoughtWorker_NeedHope: ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            //return ThoughtState.ActiveAtStage(1);
            // HopeState enum should directly correspond to the thought worker stages.
            HopeState state = GetHopeStateOf(p);
            return ThoughtState.ActiveAtStage((int)state);
            /*
            switch (state)
            {
                case HopeState.VERY_HOPELESS:
                    return ThoughtState.ActiveAtStage(0);
                case HopeState.HOPELESS:
                    return ThoughtState.ActiveAtStage(1);
                case HopeState.NEUTRAL:
                    return ThoughtState.ActiveAtStage(2);
                case HopeState.HOPEFUL:
                    return ThoughtState.ActiveAtStage(3);
                case HopeState.VERY_HOPEFUL:
                    return ThoughtState.ActiveAtStage(4);
                default:
                    throw new InvalidOperationException("Unrecognized hope state.");
            }
            */
        }

        public override float MoodMultiplier(Pawn p)
        {
            Need_Hope hope = p.GetNeedHope();
            HopeWorker_TotalHope totalHope = p.GetTotalHope();
            if (hope == null || totalHope == null)
            {
                return 0;
            }
            float ratio = totalHope.CurrentHopeLevel / hope.MaxHopeRange;
            /*
             * We have a piecewise function:
             * from 0 to 0.5: linear 0.5x
             * from 0.5 onwards: quadratic x^2, capped to 1 (or -1 if negative)
             */
            if (Mathf.Abs(ratio) < 0.5f)
            {
                return 0.5f * Mathf.Abs(ratio);
            }
            else
            {
                float outputRatio = Mathf.Pow(Mathf.Clamp(ratio, -1, 1), 2);
                return outputRatio;
            }
        }

        public static HopeState GetHopeStateOf(Pawn p)
        {
            Need_Hope hope = p.GetNeedHope();
            HopeWorker_TotalHope totalHope = p.GetTotalHope();
            if (hope == null || totalHope == null)
            {
                return HopeState.NEUTRAL;
            }
            float hopePercentage = totalHope.CurrentHopeLevel / hope.MaxHopeRange;
            if (hopePercentage >= 1)
            {
                return HopeState.VERY_HOPEFUL;
            }
            if (hopePercentage >= 0.5f)
            {
                return HopeState.HOPEFUL;
            }
            if (hopePercentage >= 0.1f)
            {
                return HopeState.SLIGHTLY_HOPEFUL;
            }
            if (hopePercentage <= -0.1f)
            {
                return HopeState.SLIGHTLY_HOPELESS;
            }
            if (hopePercentage <= -0.5f)
            {
                return HopeState.HOPELESS;
            }
            if (hopePercentage <= -1)
            {
                return HopeState.VERY_HOPELESS;
            }
            return HopeState.NEUTRAL;
        }
    }
}
