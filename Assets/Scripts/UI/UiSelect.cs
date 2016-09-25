using System.Diagnostics;
using Assets.Scripts.Utility;

namespace Assets.Scripts.UI
{
    public static class UiSelect {

        //Me class governs visual changes to the map, UI, and units as the player clicks and moves around.
        //It also mediates between the Client object, and the raw stats/resources a player has accumulated.
        public static int PlayerId;
        public static ISelectable Selected, Previous;
        public static SelectType CurrentType, LastType;

        public static void Select(this ISelectable target, SelectType type)
        {
            if (Selected != null) Selected.OnDeselect();
            LastType = CurrentType;
            CurrentType = type;
            Previous = Selected;
            Selected = target;
            target.OnSelect();
        }
    }

    public enum SelectType
    {
        Unit,
        Tile,
        Station
    }
}