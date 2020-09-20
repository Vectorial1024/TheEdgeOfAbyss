using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeOfAbyss.UI;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeWorker_TotalHope : HopeWorker
    {
        Need_Hope hope = null;

        public override bool HopeIsApplicableToCreature => pawn.IsCapableOfThought();

        public HopeWorker_TotalHope(Pawn pawn): base(pawn)
        {

        }

        public override void Tick150Interval()
        {
            // Recalculate hope level every once in a while
            hopeLevel = 0;
            if (hope == null)
            {
                hope = pawn.GetNeedHope();
            }
            if (hope == null)
            {
                return;
            }
            foreach (HopeWorker worker in hope.AllHopeWorkers)
            {
                if (worker == this)
                {
                    continue;
                }
                hopeLevel += worker.CurrentHopeLevel;
            }
        }
    }
}
