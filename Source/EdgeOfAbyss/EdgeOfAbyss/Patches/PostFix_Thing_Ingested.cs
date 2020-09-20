using EdgeOfAbyss.Hope;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.Patches
{
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("Ingested", MethodType.Normal)]
    public class PostFix_Thing_Ingested
    {
        [HarmonyPostfix]
        public static void Ingested(Thing __instance, float __result, Pawn ingester)
        {
            // some pawns do not have Hope, e.g., animals.
            // sometimes not even the hope objects are initialized too
            ingester.GetNeedHope()?.GetFoodHope()?.Notify_FoodConsumed(__instance, __result);
        }
    }
}
