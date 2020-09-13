using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class Hope: IExposable
    {
        public HopeDef def;
        public HopeWorker worker;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");

            throw new NotImplementedException();
        }
    }
}
