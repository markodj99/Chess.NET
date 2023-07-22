using System;
using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Stores
{
    public class ColorSelectionStore
    {
        public event Action<Color> ColorSelected;

        public void SelectColor(Color color) => ColorSelected?.Invoke(color);
    }
}
