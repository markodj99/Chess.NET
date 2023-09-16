using DiplomskiRad.Models.Enums;
using System;

namespace DiplomskiRad.Stores
{
    public class NewGameStore
    {
        public event Action NewGameClicked;

        public void NewGameClick() => NewGameClicked?.Invoke();
    }
}
