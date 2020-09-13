using EdgeOfAbyss.Hope;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace EdgeOfAbyss.UI
{
    public class HopeCardUtility
    {
		public static readonly Vector2 FullSize = new Vector2(400, 300);

		public static readonly int HopeBarHeight = 70;

		public static readonly int EntryHeight = 24;

		public static readonly int ScrollerMargin = 16;

		public static readonly int RowContentWidth = 380;

		public static readonly int HopeNameWidth = 340;

		public static readonly int HopeValueWidth = 40;

		public static readonly Color NeutralColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

		public static readonly Color PositiveColor = new Color(0.1f, 1f, 0.1f);

		public static readonly Color NegativeColor = new Color(0.8f, 0.4f, 0.4f);

		private static StringBuilder sharedStringBuilder = new StringBuilder();

		public static Vector2 GetSize(Pawn pawn)
		{
			// UpdateDisplayNeeds(pawn);
			if (pawn.needs.mood != null)
			{
				return FullSize;
			}
			int count = 7;
			return new Vector2(225f, count * Mathf.Min(70f, FullSize.y / count));
		}

		[Obsolete]
		private static void DrawRow(Rect givenArea, int index, string text)
		{
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.BeginGroup(givenArea);

			//
			Rect boundingRect = new Rect(0, 0, givenArea.width, EntryHeight);
			Widgets.DrawHighlightIfMouseover(boundingRect);
			Rect textRect = boundingRect;
			textRect.xMin += 10;
			textRect.xMax -= 10;
			Widgets.Label(textRect, text);
			TooltipHandler.TipRegion(boundingRect, "Tooltip");
			GUI.EndGroup();
		}

		private static void DrawRow(Rect givenArea, HopeWorker worker)
		{
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.BeginGroup(givenArea);

			// Main stuff
			Rect boundingRect = new Rect(0, 0, givenArea.width, EntryHeight);
			Widgets.DrawHighlightIfMouseover(boundingRect);

			// Tooltip
			sharedStringBuilder.Clear();
			sharedStringBuilder.AppendLine(worker.HopeDescription);
			sharedStringBuilder.AppendLine("\n");
			sharedStringBuilder.AppendLine(worker.HopeFlavorText);
			sharedStringBuilder.AppendLine("\n");
			sharedStringBuilder.AppendLine(worker.Hint);
			string descriptionString = worker.HopeDescription;
			TaggedString flavorString = ColoredText.Colorize(worker.HopeFlavorText, GenColor.FromHex("a0a0a0"));
			TaggedString hintString = ColoredText.Colorize("Hint: ", ColoredText.CurrencyColor) + worker.Hint;
			TaggedString overallString = descriptionString + "\n\n" + flavorString + "\n\n" + hintString;
			TooltipHandler.TipRegion(boundingRect, overallString);

			int valueWidth = 40;

			// Label stuff
			Rect textRect = new Rect(0, 0, boundingRect.width - valueWidth, boundingRect.height);
			textRect.xMin += 10;
			textRect.xMax -= 10;
			Widgets.Label(textRect, worker.HopeName.CapitalizeFirst());

			// Value stuff
			Text.WordWrap = false;
			Rect valueRect = new Rect(boundingRect.width - valueWidth, 0, valueWidth, boundingRect.height);
			Text.Anchor = TextAnchor.MiddleRight;
			float value = worker.CurrentHopeLevel;
			if (value >= 0)
			{
				GUI.color = PositiveColor;
			}
			else
			{
				GUI.color = NegativeColor;
			}
			Widgets.Label(valueRect, value.ToString("##0.0"));

			// Reset stuff
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			Text.WordWrap = true;

			// End group
			GUI.EndGroup();
		}

		private static void DrawRow_Summary(Rect givenArea, Pawn pawn)
		{
			HopeDef totalHopeDef = EdgeOfAbyss_HopeDefOf.TotalHope;

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.BeginGroup(givenArea);

			// Main stuff
			Rect boundingRect = new Rect(0, 0, givenArea.width, EntryHeight);
			Widgets.DrawHighlightIfMouseover(boundingRect);
			TooltipHandler.TipRegion(boundingRect, "This is the sum of all the hope levels of this person.\n\nHaving low, negative total hope may damage this person's personality.");

			int valueWidth = 40;

			// Label stuff
			Rect textRect = new Rect(0, 0, boundingRect.width - valueWidth, boundingRect.height);
			textRect.xMin += 10;
			textRect.xMax -= 10;
			Widgets.Label(textRect, "Total hope");

			// Value stuff
			Text.WordWrap = false;
			Rect valueRect = new Rect(boundingRect.width - valueWidth, 0, valueWidth, boundingRect.height);
			Text.Anchor = TextAnchor.MiddleRight;
			float value = pawn.GetNeedHope().TotalHope;
			if (value >= 5)
			{
				GUI.color = PositiveColor;
			}
			else if (value <= -5)
			{
				GUI.color = NegativeColor;
			}
			else
			{
				GUI.color = NeutralColor;
			}
			Widgets.Label(valueRect, value.ToString("##0.0"));

			// Reset stuff
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			Text.WordWrap = true;

			// End group
			GUI.EndGroup();
		}

		private static void DrawHopeListing(Rect listingRect, Pawn pawn, ref Vector2 thoughtScrollPosition)
		{
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			Text.Font = GameFont.Small;
			//PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(pawn.needs.mood, thoughtGroupsPresent);
			int hopeLineCount = 23;
			float height = hopeLineCount * EntryHeight;
			Rect viewingWindow = new Rect(0f, 0f, listingRect.width - ScrollerMargin, height);
			Widgets.BeginScrollView(listingRect, ref thoughtScrollPosition, viewingWindow);
			Text.Anchor = TextAnchor.MiddleLeft;
			int currentHeight = 0;
			float upperPosition = thoughtScrollPosition.y - EntryHeight;
			float lowerPosition = thoughtScrollPosition.y + listingRect.height;
			// Print summation row
			// Note: "total hope" is now a special kind of hope that is always displayed at the top and sums hope of other hopes
			/*
			Rect summaryRowRect = new Rect(0, currentHeight, viewingWindow.width, EntryHeight);
			DrawRow_Summary(summaryRowRect, pawn);
			currentHeight += EntryHeight;
			*/
			Need_Hope hope = pawn.GetNeedHope();
			foreach (HopeWorker worker in hope.AllHopeWorkers)
			{
				if (currentHeight > upperPosition && currentHeight < lowerPosition)
				{
					Rect rowRect = new Rect(0, currentHeight, viewingWindow.width, EntryHeight);
					DrawRow(rowRect, worker);
				}
				currentHeight += EntryHeight;
			}
			Widgets.EndScrollView();
			Text.Anchor = TextAnchor.UpperLeft;
		}


		public static void DrawHopeCard(Rect givenArea, Pawn target, ref Vector2 thoughtScrollPosition)
		{
			if (target.needs.mood == null)
			{
				// We could do something about this later
				return;
			}
			GUI.BeginGroup(givenArea);
			Rect rect2 = new Rect(0f, 0f, givenArea.width * 0.8f, HopeBarHeight);
			// find the hope component
			foreach (Need_Hope hope in target.needs.AllNeeds.Where((Need need) => need is Need_Hope))
			{
				hope.DrawOnGUI(rect2);
				break;
			}
			//target.needs.mood.DrawOnGUI(rect2);
			DrawHopeListing(new Rect(0f, 80f, givenArea.width, givenArea.height - HopeBarHeight - 10f).ContractedBy(10f), target, ref thoughtScrollPosition);
			GUI.EndGroup();
		}
	}
}
