using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeOfAbyss.UI;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeWorker_Mood : HopeWorker
    {
        public override bool HopeIsApplicableToCreature => pawn.needs.mood != null;

        public HopeWorker_Mood(Pawn pawn): base(pawn)
        {

        }

        public override void Tick150Interval()
        {
            float moodPercentage = pawn.needs.mood.CurInstantLevelPercentage;
            // Scale a variable belonging to [0, 1] to another variable belonging to [-1, 1]
            hopeLevel = ExpectedRange * (moodPercentage - 0.5f) * 2;
        }
    }
}
