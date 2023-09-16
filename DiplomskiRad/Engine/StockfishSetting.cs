using System.Collections.Generic;

namespace DiplomskiRad.Engine
{
    public static class StockfishSetting
    {
        public static Dictionary<string, string> Engine { get; set; } = new Dictionary<string, string>(2)
        {
            { "Weaker", "fairy_stockfish.exe" }, { "Stronger", "stockfish.exe" }
        };
        public static Dictionary<int, List<string>> Setting { get; set; } = new Dictionary<int, List<string>>(52)
        {
            {250, new List<string>(3)
            {
                "setoption name Skill Level value -20", "depth 2", "movetime 5"
            }},
            {300, new List<string>(3)
            {
                "setoption name Skill Level value -19", "depth 2", "movetime 5"
            }},
            {350, new List<string>(3)
            {
                "setoption name Skill Level value -18", "depth 3", "movetime 10"
            }},
            {400, new List<string>(3)
            {
                "setoption name Skill Level value -17", "depth 3", "movetime 10"
            }},
            {450, new List<string>(3)
            {
                "setoption name Skill Level value -16", "depth 4", "movetime 15"
            }},
            {500, new List<string>(3)
            {
                "setoption name Skill Level value -15", "depth 4", "movetime 15"
            }},
            {550, new List<string>(3)
            {
                "setoption name Skill Level value -14", "depth 6", "movetime 20"
            }},
            {600, new List<string>(3)
            {
                "setoption name Skill Level value -13", "depth 6", "movetime 20"
            }},
            {650, new List<string>(3)
            {
                "setoption name Skill Level value -12", "depth 7", "movetime 50"
            }},
            {700, new List<string>(3)
            {
                "setoption name Skill Level value -11", "depth 7", "movetime 50"
            }},
            {750, new List<string>(3)
            {
                "setoption name Skill Level value -10", "depth 8", "movetime 50"
            }},
            {800, new List<string>(3)
            {
                "setoption name Skill Level value -9", "depth 8", "movetime 50"
            }},
            {850, new List<string>(3)
            {
                "setoption name Skill Level value -8", "depth 10", "movetime 100"
            }},
            {900, new List<string>(3)
            {
                "setoption name Skill Level value -7", "depth 10", "movetime 100"
            }},
            {950, new List<string>(3)
            {
                "setoption name Skill Level value -6", "depth 12", "movetime 100"
            }},
            {1000, new List<string>(3)
            {
                "setoption name Skill Level value -5", "depth 14", "movetime 150"
            }},
            {1050, new List<string>(3)
            {
                "setoption name Skill Level value -4", "depth 14", "movetime 150"
            }},
            {1100, new List<string>(3)
            {
                "setoption name Skill Level value -3", "depth 15", "movetime 150"
            }},
            {1150, new List<string>(3)
            {
                "setoption name Skill Level value -2", "depth 15", "movetime 150"
            }},
            {1200, new List<string>(3)
            {
                "setoption name Skill Level value -1", "depth 16", "movetime 200"
            }},
            {1250, new List<string>(3)
            {
                "setoption name Skill Level value 0", "depth 16", "movetime 200"
            }},
            {1300, new List<string>(3)
            {
                "setoption name Skill Level value -9", "depth 1", "movetime 50"
            }},
            {1350, new List<string>(3)
            {
                "setoption name Skill Level value 1", "depth 10", "movetime 100"
            }},
            {1400, new List<string>(3)
            {
                "setoption name Skill Level value 2", "depth 10", "movetime 100"
            }},
            {1450, new List<string>(3)
            {
                "setoption name Skill Level value 3", "depth 10", "movetime 100"
            }},
            {1500, new List<string>(3)
            {
                "setoption name Skill Level value 4", "depth 10", "movetime 100"
            }},
            {1550, new List<string>(3)
            {
                "setoption name Skill Level value 5", "depth 15", "movetime 200"
            }},
            {1600, new List<string>(3)
            {
                "setoption name Skill Level value 6", "depth 15", "movetime 200"
            }},
            {1650, new List<string>(3)
            {
                "setoption name Skill Level value 7", "depth 15", "movetime 200"
            }},
            {1700, new List<string>(3)
            {
                "setoption name Skill Level value 8", "depth 15", "movetime 200"
            }},
            {1750, new List<string>(3)
            {
                "setoption name Skill Level value 9", "depth 15", "movetime 200"
            }},
            {1800, new List<string>(3)
            {
                "setoption name Skill Level value 10", "depth 15", "movetime 200"
            }},
            {1850, new List<string>(3)
            {
                "setoption name Skill Level value 11", "depth 20", "movetime 500"
            }},
            {1900, new List<string>(3)
            {
                "setoption name Skill Level value 12", "depth 20", "movetime 500"
            }},
            {1950, new List<string>(3)
            {
                "setoption name Skill Level value 13", "depth 20", "movetime 500"
            }},
            {2000, new List<string>(3)
            {
                "setoption name Skill Level value 14", "depth 20", "movetime 500"
            }},
            {2050, new List<string>(3)
            {
                "setoption name Skill Level value 15", "depth 20", "movetime 500"
            }},
            {2100, new List<string>(3)
            {
                "setoption name Skill Level value 16", "depth 25", "movetime 2500"
            }},
            {2150, new List<string>(3)
            {
                "setoption name Skill Level value 17", "depth 25", "movetime 2500"
            }},
            {2200, new List<string>(3)
            {
                "setoption name Skill Level value 18", "depth 25", "movetime 2500"
            }},
            {2250, new List<string>(3)
            {
                "setoption name Skill Level value 19", "depth 25", "movetime 2500"
            }},
            {2300, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 25", "movetime 2500"
            }},
            {2350, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 30", "movetime 3500"
            }},
            {2400, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 30", "movetime 4500"
            }},
            {2450, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 30", "movetime 5500"
            }},
            {2500, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 32", "movetime 7000"
            }},
            {2550, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 32", "movetime 8000"
            }},
            {2600, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 34", "movetime 9500"
            }},
            {2650, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 34", "movetime 11000"
            }},
            {2700, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 35", "movetime 15000"
            }},
            {2750, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 35", "movetime 18000"
            }},
            {2800, new List<string>(3)
            {
                "setoption name Skill Level value 20", "depth 40", "movetime 30000"
            }}
        };
    }
}
