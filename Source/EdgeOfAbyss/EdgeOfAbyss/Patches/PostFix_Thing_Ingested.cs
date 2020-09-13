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
            Need_Hope hope = ingester.GetNeedHope();
            //EdgeOfAbyssMain.LogError(ingester.ToString() + "(" + ingester.Name?.ToString() + "): ingested " + __instance.ToString());
            if (hope != null)
            {
                //EdgeOfAbyssMain.LogError(ingester.ToString() + "(" + ingester.Name?.ToString() + "): hope " + hope.ToString());
                HopeWorker_Food foodHope = hope.GetFoodHope();
                foodHope.Notify_FoodConsumed(__instance, __result);
            }
        }
    }
}
