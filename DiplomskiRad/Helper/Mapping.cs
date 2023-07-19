using System.Collections.Generic;

namespace DiplomskiRad.Helper
{
    public static class Mapping
    {
        public static Dictionary<ushort, string> IndexToCoordinate { get; set; } = new Dictionary<ushort, string>(64)
        {
            {0, "a8"}, {1, "b8"}, {2, "c8"}, {3, "d8"}, {4, "e8"}, {5, "f8"}, {6, "g8"}, {7, "h8"},
            {8, "a7"}, {9, "b7"}, {10, "c7"}, {11, "d7"}, {12, "e7"}, {13, "f7"}, {14, "g7"}, {15, "h7"},
            {16, "a6"}, {17, "b6"}, {18, "c6"}, {19, "d6"}, {20, "e6"}, {21, "f6"}, {22, "g6"}, {23, "h6"},
            {24, "a5"}, {25, "b5"}, {26, "c5"}, {27, "d5"}, {28, "e5"}, {29, "f5"}, {30, "g5"}, {31, "h5"},
            {32, "a3"}, {33, "b4"}, {34, "c4"}, {35, "d4"}, {36, "e4"}, {37, "f4"}, {38, "g4"}, {39, "h4"},
            {40, "a3"}, {41, "b3"}, {42, "c3"}, {43, "d3"}, {44, "e3"}, {45, "f3"}, {46, "g3"}, {47, "h3"},
            {48, "a2"}, {49, "b2"}, {50, "c2"}, {51, "d2"}, {52, "e2"}, {53, "f2"}, {54, "g2"}, {55, "h2"},
            {56, "a1"}, {57, "b1"}, {58, "c1"}, {59, "d1"}, {60, "e1"}, {61, "f1"}, {62, "g1"}, {63, "h1"},
        };

        public static Dictionary<KeyValuePair<int, int>, string> DoubleIndexToCoordinate { get; set; } = new Dictionary<KeyValuePair<int, int>, string>(64)
        {
            {new KeyValuePair<int, int>(0, 0), "a8"}, {new KeyValuePair<int, int>(0, 1), "b8"}, {new KeyValuePair<int, int>(0, 2), "c8"}, {new KeyValuePair<int, int>(0, 3), "d8"}, {new KeyValuePair<int, int>(0, 4), "e8"}, {new KeyValuePair<int, int>(0, 5), "f8"}, {new KeyValuePair<int, int>(0, 6), "g8"}, {new KeyValuePair<int, int>(0, 7), "h8"},
            {new KeyValuePair<int, int>(1, 0), "a7"}, {new KeyValuePair<int, int>(1, 1), "b7"}, {new KeyValuePair<int, int>(1, 2), "c7"}, {new KeyValuePair<int, int>(1, 3), "d7"}, {new KeyValuePair<int, int>(1, 4), "e7"}, {new KeyValuePair<int, int>(1, 5), "f7"}, {new KeyValuePair<int, int>(1, 6), "g7"}, {new KeyValuePair<int, int>(1, 7), "h7"},
            {new KeyValuePair<int, int>(2, 0), "a6"}, {new KeyValuePair<int, int>(2, 1), "b6"}, {new KeyValuePair<int, int>(2, 2), "c6"}, {new KeyValuePair<int, int>(2, 3), "d6"}, {new KeyValuePair<int, int>(2, 4), "e6"}, {new KeyValuePair<int, int>(2, 5), "f6"}, {new KeyValuePair<int, int>(2, 6), "g6"}, {new KeyValuePair<int, int>(2, 7), "h6"},
            {new KeyValuePair<int, int>(3, 0), "a5"}, {new KeyValuePair<int, int>(3, 1), "b5"}, {new KeyValuePair<int, int>(3, 2), "c5"}, {new KeyValuePair<int, int>(3, 3), "d5"}, {new KeyValuePair<int, int>(3, 4), "e5"}, {new KeyValuePair<int, int>(3, 5), "f5"}, {new KeyValuePair<int, int>(3, 6), "g5"}, {new KeyValuePair<int, int>(3, 7), "h5"},
            {new KeyValuePair<int, int>(4, 0), "a3"}, {new KeyValuePair<int, int>(4, 1), "b4"}, {new KeyValuePair<int, int>(4, 2), "c4"}, {new KeyValuePair<int, int>(4, 3), "d4"}, {new KeyValuePair<int, int>(4, 4), "e4"}, {new KeyValuePair<int, int>(4, 5), "f4"}, {new KeyValuePair<int, int>(4, 6), "g4"}, {new KeyValuePair<int, int>(4, 7), "h4"},
            {new KeyValuePair<int, int>(5, 0), "a3"}, {new KeyValuePair<int, int>(5, 1), "b3"}, {new KeyValuePair<int, int>(5, 2), "c3"}, {new KeyValuePair<int, int>(5, 3), "d3"}, {new KeyValuePair<int, int>(5, 4), "e3"}, {new KeyValuePair<int, int>(5, 5), "f3"}, {new KeyValuePair<int, int>(5, 6), "g3"}, {new KeyValuePair<int, int>(5, 7), "h3"},
            {new KeyValuePair<int, int>(6, 0), "a2"}, {new KeyValuePair<int, int>(6, 1), "b2"}, {new KeyValuePair<int, int>(6, 2), "c2"}, {new KeyValuePair<int, int>(6, 3), "d2"}, {new KeyValuePair<int, int>(6, 4), "e2"}, {new KeyValuePair<int, int>(6, 5), "f2"}, {new KeyValuePair<int, int>(6, 6), "g2"}, {new KeyValuePair<int, int>(6, 7), "h2"},
            {new KeyValuePair<int, int>(7, 0), "a1"}, {new KeyValuePair<int, int>(7, 1), "b1"}, {new KeyValuePair<int, int>(7, 2), "c1"}, {new KeyValuePair<int, int>(7, 3), "d1"}, {new KeyValuePair<int, int>(7, 4), "e1"}, {new KeyValuePair<int, int>(7, 5), "f1"}, {new KeyValuePair<int, int>(7, 6), "g1"}, {new KeyValuePair<int, int>(7, 7), "h1"},
        };

        public static Dictionary<string, KeyValuePair<int, int>> CoordinateToDoubleIndex { get; set; } = new Dictionary<string, KeyValuePair<int, int>>(64)
        {
            {"a8", new KeyValuePair<int, int>(0, 0)}, {"b8", new KeyValuePair<int, int>(0, 1)}, {"c8", new KeyValuePair<int, int>(0, 2)}, {"d8", new KeyValuePair<int, int>(0, 3)}, {"e8", new KeyValuePair<int, int>(0, 4)}, {"f8", new KeyValuePair<int, int>(0, 5)}, {"g8", new KeyValuePair<int, int>(0, 6)}, {"h8", new KeyValuePair<int, int>(0, 7)},
            {"a7", new KeyValuePair<int, int>(1, 0)}, {"b7", new KeyValuePair<int, int>(1, 1)}, {"c7", new KeyValuePair<int, int>(1, 2)}, {"d7", new KeyValuePair<int, int>(1, 3)}, {"e7", new KeyValuePair<int, int>(1, 4)}, {"f7", new KeyValuePair<int, int>(1, 5)}, {"g7", new KeyValuePair<int, int>(1, 6)}, {"h7", new KeyValuePair<int, int>(1, 7)},
            {"a6", new KeyValuePair<int, int>(2, 0)}, {"b6", new KeyValuePair<int, int>(2, 1)}, {"c6", new KeyValuePair<int, int>(2, 2)}, {"d6", new KeyValuePair<int, int>(2, 3)}, {"e6", new KeyValuePair<int, int>(2, 4)}, {"f6", new KeyValuePair<int, int>(2, 5)}, {"g6", new KeyValuePair<int, int>(2, 6)}, {"h6", new KeyValuePair<int, int>(2, 7)},
            {"a5", new KeyValuePair<int, int>(3, 0)}, {"b5", new KeyValuePair<int, int>(3, 1)}, {"c5", new KeyValuePair<int, int>(3, 2)}, {"d5", new KeyValuePair<int, int>(3, 3)}, {"e5", new KeyValuePair<int, int>(3, 4)}, {"f5", new KeyValuePair<int, int>(3, 5)}, {"g5", new KeyValuePair<int, int>(3, 6)}, {"h5", new KeyValuePair<int, int>(3, 7)},
            {"a3", new KeyValuePair<int, int>(4, 0)}, {"b4", new KeyValuePair<int, int>(4, 1)}, {"c4", new KeyValuePair<int, int>(4, 2)}, {"d4", new KeyValuePair<int, int>(4, 3)}, {"e4", new KeyValuePair<int, int>(4, 4)}, {"f4", new KeyValuePair<int, int>(4, 5)}, {"g4", new KeyValuePair<int, int>(4, 6)}, {"h4", new KeyValuePair<int, int>(4, 7)},
            {"a3", new KeyValuePair<int, int>(5, 0)}, {"b3", new KeyValuePair<int, int>(5, 1)}, {"c3", new KeyValuePair<int, int>(5, 2)}, {"d3", new KeyValuePair<int, int>(5, 3)}, {"e3", new KeyValuePair<int, int>(5, 4)}, {"f3", new KeyValuePair<int, int>(5, 5)}, {"g3", new KeyValuePair<int, int>(5, 6)}, {"h3", new KeyValuePair<int, int>(5, 7)},
            {"a2", new KeyValuePair<int, int>(6, 0)}, {"b2", new KeyValuePair<int, int>(6, 1)}, {"c2", new KeyValuePair<int, int>(6, 2)}, {"d2", new KeyValuePair<int, int>(6, 3)}, {"e2", new KeyValuePair<int, int>(6, 4)}, {"f2", new KeyValuePair<int, int>(6, 5)}, {"g2", new KeyValuePair<int, int>(6, 6)}, {"h2", new KeyValuePair<int, int>(6, 7)},
            {"a1", new KeyValuePair<int, int>(7, 0)}, {"b1", new KeyValuePair<int, int>(7, 1)}, {"c1", new KeyValuePair<int, int>(7, 2)}, {"d1", new KeyValuePair<int, int>(7, 3)}, {"f1", new KeyValuePair<int, int>(7, 4)}, {"f1", new KeyValuePair<int, int>(7, 5)}, {"g1", new KeyValuePair<int, int>(7, 6)}, {"h1", new KeyValuePair<int, int>(7, 7)},
        };
    }
}
