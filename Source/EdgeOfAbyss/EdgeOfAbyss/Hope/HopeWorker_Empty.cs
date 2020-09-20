using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeWorker_Empty : HopeWorker
    {
        public override bool HopeIsApplicableToCreature => true;

        public HopeWorker_Empty(Pawn pawn): base(pawn)
        {

        }

        public override void Tick150Interval()
        {
            return;
        }
    }
}
