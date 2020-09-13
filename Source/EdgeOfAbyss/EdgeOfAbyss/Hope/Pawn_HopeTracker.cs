using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.Hope
{
    [Obsolete("We have changed to use Need_Hope instead.", true)]
    public class Pawn_HopeTracker : IExposable
    {
        /*
         * Also patch to Pawn.Tick(), call a self-made function to tick the pawn_hopetracker of the pawn,
         * therefore ticking all hope calculators below us.
         * Refer to Pawn_NeedsTracker.NeedsTrackerTick() for "tick once every x ticks" feature.
         * Currently set to tick every 150 in-game ticks.
         */

        public Dictionary<string, float> allHopeLevels;
        public List<HopeWorker> hopeLevelCalculators;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref allHopeLevels, "allHopeLevels");
            Scribe_Collections.Look(ref hopeLevelCalculators, "hlcList", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                EnsureAllHopeTrackersAreHere();
            }
        }

        private void EnsureAllHopeTrackersAreHere()
        {
            if (hopeLevelCalculators == null)
            {
                hopeLevelCalculators = new List<HopeWorker>();
            }
            if (allHopeLevels == null)
            {
                allHopeLevels = new Dictionary<string, float>();
            }
        }

        private void EnsureHopeDefIsAvailable<T>() where T : HopeDef
        {
            /*
            if (GetHopeCalculatorOfDef<T>() == null)
            {
                HopeDef def = null;
                Pawn pawn = null;
                HopeWorker calculator = HopeDef.MakeNewCalculatorFromDef(def, pawn);
                if (calculator != null)
                {
                    hopeLevelCalculators.Add(calculator);
                }
            }
            */
        }

        public HopeWorker GetHopeLevelCalculator<T>() where T : HopeWorker
        {
            foreach (HopeWorker calculator in hopeLevelCalculators)
            {
                if (calculator is T)
                {
                    return calculator;
                }
            }

            return null;
        }

        public HopeWorker GetHopeCalculatorOfDef<T>() where T : HopeDef
        {
            foreach (HopeWorker calculator in hopeLevelCalculators)
            {
                if (calculator.def is T)
                {
                    return calculator;
                }
            }

            return null;
        }
    }
}
