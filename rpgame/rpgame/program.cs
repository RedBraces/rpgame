using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpgame
{
    class program
    {
        static void Main(string[] args)
        {
            Map m = new Map(24, 75);

            DrawMap(m);

            //Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        public static void DrawMap(Map map)
        {
            //Console.WriteLine("In drawmap");

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

        public static string GetSymbolForMapTile(TileType TileType)
        {
            switch(TileType)
            {
                case TileType.Corridor:
                    return "_";
                case TileType.Room:
                    return ".";
                case TileType.Wall:
                    return "#";
                default:
                    throw new Exception("Unknown tile type!");
            }
        }

    }
}
