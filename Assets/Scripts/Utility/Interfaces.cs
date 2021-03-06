﻿namespace Assets.Scripts.Utility
{
    public interface ITurnable
    {
        void OnTurn();
    }

    public interface ISelectable
    {
        void OnSelect();
        void OnDeselect();
    }
}