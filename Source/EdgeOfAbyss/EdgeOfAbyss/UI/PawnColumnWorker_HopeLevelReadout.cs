using EdgeOfAbyss.Hope;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.UI
{
    public class PawnColumnWorker_HopeLevelReadout : PawnColumnWorker_Text
    {
        // Placeholder
        private float GetHopeFor(Pawn pawn)
        {
            HopeWorker_TotalHope worker = pawn.GetTotalHope();
            if (worker == null)
            {
                return 0;
            }
            return worker.CurrentHopeLevel;
        }

        protected override string GetTextFor(Pawn pawn)
        {
            //return "testing string";
            //return (EdgeOfAbyss_PawnTableDefOf.TestHopeDef != null).ToString();
            return GetHopeFor(pawn).ToStringDecimalIfSmall();
        }

        protected override string GetTip(Pawn pawn)
        {
            return EdgeOfAbyss_NeedDefOf.Hope.description;
        }
    }
}
