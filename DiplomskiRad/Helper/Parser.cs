using System;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DiplomskiRad.Helper
{
    public class Parser
    {
        private static string[] _folders = { "Rating_250_950", "Rating_1000_2000", "Rating_2050_2800" };

        public static List<ChessPuzzle> ParseFile()
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

        private static List<ChessPuzzle> GetPuzzleFromFolder(string folderPath)
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
            }

            return retVal;
        }
    }
}
