using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public abstract class HopeWorker : IExposable
    {
        public Pawn pawn;

        protected float hopeLevel;

        public HopeDef def;

        public readonly int HopeTickInterval = 150;

        public virtual float CurrentHopeLevel
        {
            get
            {
                return hopeLevel;
            }
        }

        public HopeWorker(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public void InitializeFromDef()
        {

        }

        public void UpdateHope()
        {
            Tick150Interval();
        }

        public abstract void Tick150Interval();

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref hopeLevel, "hopeLevel");
        }

        public int DisplayOrder => def.displayOrder;

        public string HopeName => def.hopeName;

        public string HopeDescription => def.description;

        public string HopeFlavorText => def.flavorText;

        public string Hint => def.hint;

        public int ExpectedRange => def.expectedRange;

        /// <summary>
        /// Indicate whether this Hope can be applied to this Pawn.
        /// <para/>
        /// This is used to detect pawns having impossible HopeWorkers attached to them because they
        /// e.g. simply do not have the relevant Need.
        /// <para/>
        /// Derived types should specify in what circumstances will the HopeWorker be valid/invalid.
        /// </summary>
        public virtual bool HopeIsApplicableToCreature => false;
    }
}
