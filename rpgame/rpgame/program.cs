using MapGenerator;
using System;

namespace rpgame
{
    class program
    {
        static void Main(string[] args)
        {
            Map m = new Map(24, 75);

            DrawMap(m);
            Console.WriteLine(m.GetMapFillRate() + "% of map filled");

            Console.ReadKey();
        }

        /// <summary>
        /// This renders the map on screen
        /// </summary>
        /// <param name="map">The map to render</param>
        public static void DrawMap(Map map)
        {
            for (int h = 0; h < map.MapHeight; h++)
            {
                string mapRow = "";
                for (int w = 0; w < map.MapWidth; w++)
                {
                    mapRow += GetSymbolForMapTile(map.GetMapTile(h, w));
                }

                Console.WriteLine(mapRow);
            }
        }

        /// <summary>
        /// Returns the map icon for the given tile type
        /// </summary>
        /// <param name="TileType">The tile type to render</param>
        /// <returns></returns>
        public static string GetSymbolForMapTile(TileType TileType)
        {
            switch(TileType)
            {
                case TileType.Corridor:
                    return ".";
                case TileType.Room:
                    return ".";
                case TileType.Wall:
                    return "#";
                case TileType.StairsUp:
                    return "<";
                case TileType.StairsDown:
                    return ">";
                case TileType.DoorClosed:
                    return "+";
                case TileType.DoorOpen:
                    return "/";
                default:
                    throw new Exception("Unknown tile type!");
            }
        }

    }
}
