public static class UiSelect {

	//This class governs visual changes to the map, UI, and units as the player clicks and moves around.
    //It also mediates between the Client object, and the raw stats/resources a player has accumulated.
    public static int PlayerId;
    public static ISelectable Selected;
    public static SelectType Type;

    public static void Select(ISelectable target)
    {
        if(Selected != null)Selected.OnDeselect();
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
