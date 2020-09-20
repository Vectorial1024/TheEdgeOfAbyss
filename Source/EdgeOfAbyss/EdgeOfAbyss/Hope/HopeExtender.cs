using EdgeOfAbyss.Hope;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public static class HopeExtender
    {
        public static Need_Hope GetNeedHope(this Pawn pawn)
        {
            return pawn?.needs.AllNeeds.Where((Need need) => need is Need_Hope).FirstOrDefault() as Need_Hope;
        }

        public static HopeWorker_Food GetFoodHope(this Need_Hope hope)
        {
            if (hope == null)
            {
                return null;
            }
            return hope.AllHopeWorkers.Where((HopeWorker worker) => worker is HopeWorker_Food).FirstOrDefault() as HopeWorker_Food;
        }

        public static HopeWorker_Rest GetRestHope(this Need_Hope hope)
        {
            if (hope == null)
            {
                return null;
            }
            return hope.AllHopeWorkers.Where((HopeWorker worker) => worker is HopeWorker_Rest).FirstOrDefault() as HopeWorker_Rest;
        }

        public static HopeWorker_TotalHope GetTotalHope(this Pawn pawn)
        {
            if (pawn == null)
            {
                return null;
            }
            return pawn.GetNeedHope().AllHopeWorkers.Where((HopeWorker worker) => worker is HopeWorker_TotalHope).First() as HopeWorker_TotalHope;
        }
    }
}
