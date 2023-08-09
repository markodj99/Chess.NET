using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DiplomskiRad.Helper
{
    public class Parser
    {
        public static List<ChessPuzzle> ParseFile()
        {
            var retVal = new List<ChessPuzzle>();

            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Puzzles");
            var filepath = Path.Combine(targetFolder, "Puzzles_1650.txt");

            using (var reader = new StreamReader(filepath))
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

                    retVal.Add(new ChessPuzzle(allWpos, allBpos, color, moveOrder));
                }
            }

            return retVal;
        }
    }
}
