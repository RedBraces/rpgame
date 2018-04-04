﻿using MapGenerator;
using System;

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
