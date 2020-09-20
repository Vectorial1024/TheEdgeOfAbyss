using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss
{
    public class EdgeOfAbyss_Constants
    {
        public static readonly int TicksPerHour = 2500;

        public static readonly int TicksPerDay = 60000;

        public static readonly float MaxHoursToFillOneRest = 10.5f;

        public static readonly float MaxTicksToFillOneRest = MaxHoursToFillOneRest * TicksPerHour;
    }
}
