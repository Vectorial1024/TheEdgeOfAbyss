using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeOfAbyss.Hope;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EdgeOfAbyss.Patches
{
    [HarmonyPatch(typeof(Need_Rest))]
    [HarmonyPatch("TickResting", MethodType.Normal)]
    public class PostFix_PawnNeed_Resting
    {
        [HarmonyPostfix]
        public static void TickResting(Pawn ___pawn, float restEffectiveness)
        {
            // some pawns do not have Hope, e.g., animals.
            // sometimes not even the hope objects are initialized too
            ___pawn.GetNeedHope()?.GetRestHope()?.Notify_TickResting(restEffectiveness);
        }
    }
}
