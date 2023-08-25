using DiplomskiRad.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomskiRad.Stores
{
    public class EngineStrengthEvaluated
    {
        public event Action<int> RatingEvaluated;

        public void EvaulatedRating(int rating) => RatingEvaluated?.Invoke(rating);
    }
}
