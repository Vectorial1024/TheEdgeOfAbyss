using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeOfAbyss.UI
{
    public class MainTabWindow_HopeOverview : MainTabWindow_PawnTable
    {
        protected override PawnTableDef PawnTableDef => EdgeOfAbyss_PawnTableDefOf.HopeOverview;
    }
}
