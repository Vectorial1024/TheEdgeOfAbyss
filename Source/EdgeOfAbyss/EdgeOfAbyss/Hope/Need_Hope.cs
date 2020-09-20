using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class Need_Hope : Need
    {
        List<HopeWorker> allHopeWorkers = new List<HopeWorker>();
        /// <summary>
        /// Do NOT use this as the list of all HopeWorkers of this pawn.
        /// This is placed as a property because it is needed to preserve data across different Scribe-Load states.
        /// </summary>
        List<HopeWorker> allWorkersFromFile = new List<HopeWorker>();
        int expectedMaxHopeRange;
        private readonly int TicksPerDay = 60000;
        private readonly int TicksPerUpdateInterval = 150;

        public List<HopeWorker> AllHopeWorkers => allHopeWorkers;

        public int MaxHopeRange => expectedMaxHopeRange;

        public bool HopeIsLow => CurLevelPercentage <= 0.25f;

        public bool HopeIsHigh => CurLevelPercentage >= 0.75f;

        private static int Comparer_HopeWorker_ByDisplayOrder(HopeWorker a, HopeWorker b)
        {
            return a.DisplayOrder - b.DisplayOrder;
        }

        public Need_Hope(Pawn pawn): base(pawn)
        {
            // Initialize HopeWorkers from the defs list
            InitializeHopeWorkersFromDefs();
        }

        private void InitializeHopeWorkersFromDefs()
        {
            expectedMaxHopeRange = 0;
            allHopeWorkers = new List<HopeWorker>();
            foreach (HopeDef checkingHopeDef in DefDatabase<HopeDef>.AllDefsListForReading)
            {
                //EdgeOfAbyssMain.LogError("Looping through HopeDef " + checkingHopeDef.defName);
                //EdgeOfAbyssMain.LogError("It has target type = " + checkingHopeDef.hopeWorker);

                expectedMaxHopeRange += checkingHopeDef.ExpectedRange;
                Type hopeWorkerType = checkingHopeDef.hopeWorker;
                if (hopeWorkerType != null)
                {
                    HopeWorker newWorker = (HopeWorker)Activator.CreateInstance(checkingHopeDef.hopeWorker, pawn);
                    newWorker.def = checkingHopeDef;
                    newWorker.InitializeFromDef();
                    allHopeWorkers.Add(newWorker);
                }
            }
            allHopeWorkers.Sort(Comparer_HopeWorker_ByDisplayOrder);
        }

        private void CompareAndApplyHopeWorkers(List<HopeWorker> listFromDefs, List<HopeWorker> listFromFile)
        {
            // 1. Make new list
            List<HopeWorker> validWorkers = new List<HopeWorker>();

            // 2. Add all valid workers from file
            foreach (HopeWorker worker in listFromFile)
            {
                if (worker.def == null)
                {
                    continue;
                }
                if (!worker.HopeIsApplicableToCreature)
                {
                    continue;
                }
                validWorkers.Add(worker);
            }

            // 3. Add all workers from constructor (must not exist in the list)
            foreach (HopeWorker workerFromDef in listFromDefs)
            {
                // Find the 1st one, if exists
                HopeWorker existingWorker = listFromFile.Where((HopeWorker workerFromFile) => workerFromFile.GetType() == workerFromDef.GetType()).FirstOrDefault();
                if (existingWorker == null)
                {
                    // Does not already exist; this must be a new worker.
                    if (workerFromDef.HopeIsApplicableToCreature)
                    {
                        validWorkers.Add(workerFromDef);
                    }
                }
            }

            // 4. Apply changes
            allHopeWorkers = validWorkers;

            // 5. Sort the list, so we can display them as we wish.
            allHopeWorkers.Sort(Comparer_HopeWorker_ByDisplayOrder);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Scribe_Collections.Look(ref allHopeWorkers, "hopeWorkers", LookMode.Deep, pawn);
            }
            else
            {
                Scribe_Collections.Look(ref allWorkersFromFile, "hopeWorkers", LookMode.Deep, pawn);
                if (Scribe.mode == LoadSaveMode.PostLoadInit)
                {
                    CompareAndApplyHopeWorkers(allHopeWorkers, allWorkersFromFile);
                }
            }
        }

        public override void NeedInterval()
        {
            float maxAllowedHopeRange = 0;
            foreach (HopeWorker worker in allHopeWorkers)
            {
                if (!IsFrozen)
                {
                    worker.UpdateHope();
                }
                maxAllowedHopeRange += worker.def.ExpectedRange;
            }
            float currentTotalHope = pawn.GetTotalHope().CurrentHopeLevel;

            curLevelInt = (currentTotalHope + maxAllowedHopeRange) / (maxAllowedHopeRange * 2);

            // determine hopelessness/hopefulness events
            /*
            if (HopeIsLow && DetermineMtbSuccessFromHope_Demotion())
            {
                EdgeOfAbyssMain.LogError(pawn.Name.ToString() + ": mood demotion");
            }
            else if (HopeIsHigh && DetermineMtbSuccessFromHope_Promotion())
            {
                EdgeOfAbyssMain.LogError(pawn.Name.ToString() + ": mood promotion");
            }
            */
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = int.MaxValue, float customMargin = -1, bool drawArrows = true, bool doTooltip = true)
        {
            if (threshPercents == null)
            {
                threshPercents = new List<float>();
            }
            threshPercents.Clear();
            // threshold: low mood
            threshPercents.Add(0.25f);
            // threshold: 0-position
            threshPercents.Add(0.5f);
            // threshold: high mood
            threshPercents.Add(0.75f);
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }

        public float TotalHope
        {
            get
            {
                float total = 0;
                foreach (HopeWorker worker in allHopeWorkers)
                {
                    total += worker.CurrentHopeLevel;
                }
                return total;
            }
        }

        private bool DetermineMtbSuccessFromHope_Demotion()
        {
            // Demotion along the mood bonus spectrum (i.e. towards Depressive) 
            // will always take MTB = 3 days because the human soul is weak and fragile.
            int moodSpectrumLevel = pawn.story.traits.GetTrait(TraitDefOf.NaturalMood)?.Degree ?? 0;
            if (moodSpectrumLevel == -2)
            {
                // cannot demote beyond Depressive
                return false;
            }
            return Rand.MTBEventOccurs(2, TicksPerDay, TicksPerUpdateInterval);
        }

        private bool DetermineMtbSuccessFromHope_Promotion()
        {
            // Promotion along the mood bonus spectrum (i.e. towards Sanguine)
            // will take MTB = 6/12/18/24 days because mental preparedness is hard to come by.
            int moodSpectrumLevel = pawn.story.traits.GetTrait(TraitDefOf.NaturalMood)?.Degree ?? 0;
            switch (moodSpectrumLevel)
            {
                case -2:
                    return Rand.MTBEventOccurs(4, TicksPerDay, TicksPerUpdateInterval);
                case -1:
                    return Rand.MTBEventOccurs(6, TicksPerDay, TicksPerUpdateInterval);
                case 0:
                    return Rand.MTBEventOccurs(8, TicksPerDay, TicksPerUpdateInterval);
                case 1:
                    return Rand.MTBEventOccurs(10, TicksPerDay, TicksPerUpdateInterval);
                case 2:
                    // cannot promote beyond Sanguine
                    return false;
                default:
                    throw new InvalidOperationException("Invalid NaturalMood trait degree");
            }
        }
    }
}
