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

        public float CurrentHopeLevel
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

        public void ExposeData()
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
    }
}
