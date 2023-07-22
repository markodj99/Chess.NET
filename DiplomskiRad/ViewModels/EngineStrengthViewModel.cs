using System.ComponentModel;
using DiplomskiRad.Models;

namespace DiplomskiRad.ViewModels
{
    public class EngineStrengthViewModel : ViewModelBase
    {
        public SelectedStrength SelectedStrength;

        public EngineStrengthViewModel()
        {
            SelectedStrength = new SelectedStrength();
        }
    }
}
