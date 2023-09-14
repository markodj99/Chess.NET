using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Models.Game
{
    public class PuzzleManager : INotifyPropertyChanged
    {
        public List<ChessPuzzle> ChessPuzzles { get; set; }
        public int OrdinalNumber { get; set; }
        public int OrdinalMoveNumber { get; set; }
        private int _errorCount;
        public int ErrorCount
        {
            get => _errorCount;
            set
            {
                _errorCount = value;
                if (ErrorCount == 1) FirstSquare = "Red";
                if (ErrorCount == 2) SecondSquare = "Red";
                if (ErrorCount == 3) ThirdSquare = "Red";
            }
        }
        private int _rating;
        public int Rating
        {
            get => _rating;
            set
            {
                if (_rating == value) return;
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }
        private string _firstSquare;
        public string FirstSquare
        {
            get => _firstSquare;
            set
            {
                if (_firstSquare == value) return;
                _firstSquare = value;
                OnPropertyChanged(nameof(FirstSquare));
            }
        }
        private string _secondSquare;
        public string SecondSquare
        {
            get => _secondSquare;
            set
            {
                if (_secondSquare == value) return;
                _secondSquare = value;
                OnPropertyChanged(nameof(SecondSquare));
            }
        }
        private string _thirdSquare;
        public string ThirdSquare
        {
            get => _thirdSquare;
            set
            {
                if (_thirdSquare == value) return;
                _thirdSquare = value;
                OnPropertyChanged(nameof(ThirdSquare));
            }
        }

        public PuzzleManager() { }

        public void Initialize()
        {
            OrdinalNumber = 0;
            OrdinalMoveNumber = 0;
            ErrorCount = 0;
            Rating = 250;
            FirstSquare = "Green";
            SecondSquare = "Green";
            ThirdSquare = "Green";
            ChessPuzzles = ParseFile();
        }

        private readonly string[] _folders = { "Rating_250_950", "Rating_1000_2000", "Rating_2050_2800" };

        private List<ChessPuzzle> ParseFile()
        {
            var retVal = new List<ChessPuzzle>();

            string currentDirectory = Directory.GetCurrentDirectory();
            string targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Puzzles");

            foreach (var folder in _folders)
            {
                retVal.AddRange(GetPuzzleFromFolder(Path.Combine(targetFolder, folder)));
            }

            return retVal;
        }

        private List<ChessPuzzle> GetPuzzleFromFolder(string folderPath)
        {
            var random = new Random();
            var retVal = new List<ChessPuzzle>();
            string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

            foreach (var txtFile in txtFiles)
            {
                var potentialThree = new List<ChessPuzzle>(3);

                string filePath = Path.Combine(folderPath, txtFile);
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var parts = reader.ReadLine().Split("_");

                        var wpos = parts[0];
                        var bPos = parts[1];
                        var fm = parts[2];
                        var mo = parts[3];

                        wpos = Regex.Replace(wpos, @"[\[\]\s]", "");
                        bPos = Regex.Replace(bPos, @"[\[\]\s]", "");
                        var allWpos = new List<string>(wpos.Split(","));
                        var allBpos = new List<string>(bPos.Split(","));

                        var color = fm[1] == 'W' ? Color.White : Color.Black;

                        mo = Regex.Replace(mo, @"[\[\]\s]", "");
                        var moveOrder = new List<string>(mo.Split(","));

                        potentialThree.Add(new ChessPuzzle(allWpos, allBpos, color, moveOrder));
                    }
                }

                retVal.Add(potentialThree[random.Next(0, 3)]);
                potentialThree.Clear();
            }

            return retVal;
        }

        public ChessPuzzle GetCurrentPuzzle() => ChessPuzzles[OrdinalNumber];

        public void IncrementOrdinalNumber() => OrdinalNumber++;

        public bool Condition() => OrdinalNumber is 51 or > 51;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
