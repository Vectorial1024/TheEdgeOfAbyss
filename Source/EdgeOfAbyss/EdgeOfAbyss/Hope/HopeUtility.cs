using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeUtility
    {
        public enum MoodTraitChangeDirection
        {
            UPWARD,
            DOWNWARD,
        }

        public static void TouchColonistPersonalityDueToHope(Pawn pawn, MoodTraitChangeDirection type)
        {
            // determine if is colonist (contains trait)
            TraitSet pawnTraits = pawn.story?.traits;
            if (pawnTraits == null)
            {
                return;
            }

            // is colonist (contains traits)
            /*
             * Rules:
             * If promote, change the degree by +1, else by -1
             * If change to 0 and will have too few traits (<= 1), then idk, it be the colonists' problem
             */

            int naturalMoodTraitDegree = pawnTraits.DegreeOfTrait(TraitDefOf.NaturalMood);
            if (naturalMoodTraitDegree == 2 && type == MoodTraitChangeDirection.UPWARD)
            {
                return;
            }
            if (naturalMoodTraitDegree == -2 && type == MoodTraitChangeDirection.DOWNWARD)
            {
                return;
            }

            int targetMoodTraitDegree;
            if (type == MoodTraitChangeDirection.UPWARD)
            {
                targetMoodTraitDegree = naturalMoodTraitDegree + 1;
            }
            else
            {
                targetMoodTraitDegree = naturalMoodTraitDegree - 1;
            }

            if (targetMoodTraitDegree == 0)
            {
                pawnTraits.allTraits.RemoveAll((trait) => !trait.ScenForced && trait.def == TraitDefOf.NaturalMood);
            }
            else
            {
                // degree changes
                if (naturalMoodTraitDegree != 0)
                {
                    // need to remove then add anyway
                    pawnTraits.allTraits.RemoveAll((trait) => !trait.ScenForced && trait.def == TraitDefOf.NaturalMood);
                }
                // adding new trait
                pawnTraits.GainTrait(new Trait(TraitDefOf.NaturalMood, targetMoodTraitDegree));
            }
        }

        public static void RemoveTrait(TraitSet traitSet, TraitDef targetTrait)
        {
            traitSet.allTraits.RemoveAll((trait) => trait.def == targetTrait);
        }
    }
}
