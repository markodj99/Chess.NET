using System;
using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Stores
{
    public class EngineStrengthStore
    {
        public event Action<int, RatingEvaluation> StrengthSelected;

        public void SelectedStrength(int strength, RatingEvaluation evaluation) => StrengthSelected?.Invoke(strength, evaluation);
    }
}
