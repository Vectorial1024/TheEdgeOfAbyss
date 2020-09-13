using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EdgeOfAbyss.UI
{
    public class ITab_Pawn_Hope : ITab
    {
        private Vector2 hopeScrollPosition;

        public ITab_Pawn_Hope()
        {
            size = HopeCardUtility.FullSize;
            labelKey = "V1024_TabHope";
            tutorTag = "V1024_TutoTag_Needs";
        }

        public override bool IsVisible
        {
            get
            {
                // Colonists, androids, etc can have hope, while simple-minded creatures such as cows cannot.
                return SelPawn?.IsCapableOfThought() ?? false;
            }
        }

        public override void OnOpen()
        {
            hopeScrollPosition = default;
        }

        protected override void UpdateSize()
        {
            size = HopeCardUtility.GetSize(base.SelPawn);
        }

        protected override void FillTab()
        {
            HopeCardUtility.DrawHopeCard(new Rect(0f, 0f, size.x, size.y), base.SelPawn, ref hopeScrollPosition);
            //throw new NotImplementedException();
        }
    }
}
