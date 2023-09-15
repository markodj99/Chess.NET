using System;

namespace DiplomskiRad.Stores
{
    public class EngineStrengthEvaluatedStore
    {
        public event Action<int> RatingEvaluated;

        public void EvaulatedRating(int rating) => RatingEvaluated?.Invoke(rating);
    }
}
