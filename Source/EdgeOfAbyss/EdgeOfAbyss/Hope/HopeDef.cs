using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EdgeOfAbyss.Hope
{
    public class HopeDef : Def
    {
        public int displayOrder;

        public string hopeName;

        public string flavorText;

        public string hint;

        public Type hopeWorker;

        public int expectedRange;

        public int ExpectedRange => Mathf.Max(0, expectedRange);

        public static HopeWorker MakeNewWorkerFromDef(HopeDef def, Pawn pawn)
        {
            if (def != null)
            {
                Type testType = typeof(HopeWorker);
                HopeWorker calculator = (HopeWorker)Activator.CreateInstance(testType);
                calculator.pawn = pawn;
                return calculator;
            }
            return null;
        }
    }
}
