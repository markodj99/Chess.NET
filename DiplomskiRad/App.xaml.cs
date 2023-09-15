using DiplomskiRad.Stores;
using DiplomskiRad.ViewModels;
using System.Windows;
using DiplomskiRad.Views;

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
            var engineStrengthEvaluatedStore = new EngineStrengthEvaluatedStore();
            var newGameStore = new NewGameStore();

            var colorSelectionViewModel = new ColorSelectionViewModel(colorSelectionStore);
            var engineStrengthViewModel = new EngineStrengthViewModel(engineStrengthStore);
            var chessBoardPuzzleViewModel = new ChessBoardPuzzleViewModel(engineStrengthEvaluatedStore);
            var chessBoardGameViewModel = new ChessBoardGameViewModel(newGameStore);

            var mainViewModel = 
                new MainWindowViewModel(colorSelectionViewModel, engineStrengthViewModel, chessBoardPuzzleViewModel, 
                    chessBoardGameViewModel, colorSelectionStore, engineStrengthStore, engineStrengthEvaluatedStore, newGameStore);

            MainWindow = new MainWindow()
            {
                DataContext = mainViewModel
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
