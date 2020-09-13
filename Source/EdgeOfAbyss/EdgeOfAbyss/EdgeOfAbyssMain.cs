using EdgeOfAbyss.UI;
using HugsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EdgeOfAbyss
{
    public class EdgeOfAbyssMain : ModBase
    {
        public static string MODID => "com.vectorial1024.rimworld.edgeofabyss";

        /// <summary>
        /// Already includes a space character.
        /// </summary>
        public static string MODPREFIX => "[V1024-ABYSS] ";

        public override string ModIdentifier => MODID;

        public override void DefsLoaded()
        {
            if (!base.ModIsActive)
            {
                return;
            }
            IEnumerable<ThingDef> allPawns = DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.thingClass == typeof(Pawn));
            foreach (ThingDef pawnThing in allPawns)
            {
                if (pawnThing.inspectorTabsResolved == null)
                {
                    pawnThing.inspectorTabsResolved = new List<InspectTabBase>(1);
                }
                pawnThing.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Hope)));
            }
        }

        public static void LogError(string message, bool ignoreLogLimit = false)
        {
            Log.Error(MODPREFIX + " " + message, ignoreLogLimit);
        }

        public static void LogWarning(string message, bool ignoreLogLimit = false)
        {
            Log.Warning(MODPREFIX + " " + message, ignoreLogLimit);
        }
    }
}
