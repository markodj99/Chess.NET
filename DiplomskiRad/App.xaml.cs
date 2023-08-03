using DiplomskiRad.Stores;
using DiplomskiRad.ViewModels;
using System.Windows;

namespace DiplomskiRad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var colorSelectionStore = new ColorSelectionStore();
            var engineStrengthStore = new EngineStrengthStore();

            var colorSelectionViewModel = new ColorSelectionViewModel(colorSelectionStore);
            var engineStrengthViewModel = new EngineStrengthViewModel(engineStrengthStore);
            var chessBoardPuzzleViewModel = new ChessBoardPuzzleViewModel();
            var chessBoardGameViewModel = new ChessBoardGameViewModel();

            var mainViewModel = 
                new MainWindowViewModel(colorSelectionViewModel, engineStrengthViewModel, chessBoardPuzzleViewModel, chessBoardGameViewModel,
                    colorSelectionStore, engineStrengthStore);

            MainWindow = new MainWindow()
            {
                DataContext = mainViewModel
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
