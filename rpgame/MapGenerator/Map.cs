using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    /// <summary>
    /// Enumeration listing the different types of tiles on a map
    /// </summary>
    public enum TileType
    {
        Wall,
        Room,
        Corridor
    }

    public class Map
    {
        private TileType[,] _map;

        /// <summary>
        /// This is how much of the map should be covered in rooms when the map is complete
        /// </summary>
        int RoomPercentage { get; set; }

        private List<MapSpaceElement> mapSpaceElements;

        #region Properties
        public int MapWidth
        {
            get
            {
                return _map.GetLength(1);
            }
        }

        public int MapHeight
        {
            get
            {
                return _map.GetLength(0);
            }
        }
        #endregion

        public Map(int height, int width)
        {
            _map = new TileType[height, width];
            GenerateEmptyMap();

            RoomPercentage = 30;
            mapSpaceElements = new List<MapSpaceElement>();

            PopulateMap();
        }

        public TileType GetMapTile(int height, int width)
        {
            return _map[height, width];
        }

        public int GetMapFillRate()
        {
            int SquaresFilled = 0;
            int TotalSquares = 0;

            for(int h = 0; h < this.MapHeight; h++)
            {
                for(int w = 0; w < this.MapWidth; w++)
                {
                    TotalSquares++;

                    if (this.GetMapTile(h, w) != TileType.Wall) SquaresFilled++;
                }
            }

            double fillRate = ((double)SquaresFilled / (double)TotalSquares) * 100;

            return Convert.ToInt32(Math.Ceiling(fillRate));
        }

        #region MapGeneration
        public void GenerateEmptyMap()
        {
            for(int h = 0; h < this.MapHeight; h++)
            {
                for(int w = 0; w < this.MapWidth; w++)
                {
                    _map[h, w] = TileType.Wall;
                }
            }
        }

        /// <summary>
        /// This function populates the map
        /// </summary>
        public void PopulateMap()
        {
            while (GetMapFillRate() < RoomPercentage)
            {
                GenerateRoom();

                int Fillrate = GetMapFillRate();
            }
            //int fillRate = GetMapFillRate();
        }

        public void GenerateRoom()
        {
            Random randomNumber = new Random();

            int startHeight = randomNumber.Next(1, this.MapHeight - 2);
            int startWidth = randomNumber.Next(1, this.MapWidth - 2);

            int roomWidth = randomNumber.Next(3, 10);
            int roomHeight = randomNumber.Next(3, 10);

            MapSpaceElement r = new MapSpaceElement();
            r.ElementType = MapSpaceElementType.Room;

            // Populate the room:
            for(int h = 0; h < roomHeight; h++)
            {
                for(int w = 0; w < roomWidth; w++)
                {
                    r.AddCoordinate(new Coordinate(startWidth + w, startHeight + h, TileType.Room));
                }
            }

            if (!ValidateMapSpaceElement(r)) return;

            // Add generated room to the list of elements:
            mapSpaceElements.Add(r);
            PlaceMapElementOnMap(r);
        }

        /// <summary>
        /// Checks if a map space element is valid eg. can be placed on the map without conflicts
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        internal bool ValidateMapSpaceElement(MapSpaceElement m)
        {
            foreach(Coordinate c in m.Coordinates)
            {
                // Validation 1: element fits into map
                //if (c.x == 0 || c.y == 0) return false; // cannot be adjacent to a wall!
                if (c.x >= (_map.GetLength(1) - 1) || c.y >= (_map.GetLength(0)) - 1) return false; // element out of bounds

                // Validation 2: elements do not overlap:
                TileType curTile = this.GetMapTile(c.y, c.x);

                if (curTile != TileType.Wall) return false;

                // Validation 3: a room cannot be directly adjacent to another room
                // There must be a space of at least one square of another type
                if(m.ElementType == MapSpaceElementType.Room)
                {
                    if (this.GetMapTile(c.y - 1, c.x - 1) == TileType.Room) return false;
                    if (this.GetMapTile(c.y - 1, c.x) == TileType.Room) return false;
                    if (this.GetMapTile(c.y - 1, c.x + 1) == TileType.Room) return false;

                    if (this.GetMapTile(c.y, c.x - 1) == TileType.Room) return false;
                    if (this.GetMapTile(c.y, c.x + 1) == TileType.Room) return false;

                    if (this.GetMapTile(c.y + 1, c.x - 1) == TileType.Room) return false;
                    if (this.GetMapTile(c.y + 1, c.x) == TileType.Room) return false;
                    if (this.GetMapTile(c.y + 1, c.x + 1) == TileType.Room) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Draws the map element on the map
        /// </summary>
        /// <param name="m">The map element</param>
        internal void PlaceMapElementOnMap(MapSpaceElement m)
        {
            foreach(Coordinate c in m.Coordinates)
            {
                _map[c.y, c.x] = c.type;
            }
        }
        #endregion

        #region SupportClasses
        /// <summary>
        /// Defines the type of a map space element
        /// </summary>
        internal enum MapSpaceElementType
        {
            Room,
            Corridor
        }

        internal class Coordinate
        {
            public int x { get; set; }
            public int y { get; set; }
            public TileType type { get; set; }

            public Coordinate(int x, int y, TileType type)
            {
                this.x = x;
                this.y = y;
                this.type = type;
            }
        }

        /// <summary>
        /// Defines a map space element
        /// </summary>
        internal class MapSpaceElement
        {
            public MapSpaceElementType ElementType { get; set; }
            private List<Coordinate> _coordinates;

            public List<Coordinate> Coordinates
            {
                get { return _coordinates; }
            }

            public MapSpaceElement()
            {
                _coordinates = new List<Coordinate>();
            }

            public void AddCoordinate(Coordinate c)
            {
                Coordinates.Add(c);
            }
        }
        #endregion

    }
}
