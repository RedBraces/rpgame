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

        public Map(int height, int width, bool generateMap = true)
        {
            _map = new TileType[height, width];
            GenerateEmptyMap();

            RoomPercentage = 30;
            mapSpaceElements = new List<MapSpaceElement>();

            if(generateMap) PopulateMap();
        }

        public TileType GetMapTile(int height, int width)
        {
            return _map[height, width];
        }

        #region MapChecking
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

        internal int CountMapSpaceElement(MapSpaceElementType type)
        {
            int count = 0;

            foreach(MapSpaceElement m in this.mapSpaceElements)
            {
                if (m.ElementType == type) count++;
            }

            return count;
        }
        #endregion

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
                bool success = GenerateRoom();
                continue; 
                // Should rooms be linked together with a corridor?
                if(success & this.CountMapSpaceElement(MapSpaceElementType.Room) > 1)
                {
                    GenerateCorridorFromRoom(this.mapSpaceElements[this.mapSpaceElements.Count - 1]);
                    return;
                }
            }
        }

        /// <summary>
        /// Attempts to generate a room
        /// </summary>
        /// <returns>TRUE if room was succesfully created, FALSE otherwise</returns>
        internal bool GenerateRoom()
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

            if (!ValidateMapSpaceElement(r)) return false;

            // Add generated room to the list of elements:
            //mapSpaceElements.Add(r);
            PlaceMapElementOnMap(r);

            return true;
        }

        internal void GenerateCorridorFromRoom(MapSpaceElement room)
        {
            if (room.ElementType != MapSpaceElementType.Room) throw new Exception("Map space element is not a room!");

            // Randomize in what direction an existing map space element should be looked for first
            Random r = new Random();
            Direction d = (Direction)r.Next(0, 3);

            bool elementFound = false;
            while(!elementFound)
            {
                // No element found. Next direction!
                if ((int)d < 3) d++;
                else d = 0;
            }
        }

        internal MapSpaceElement FindMapSpaceElement(MapSpaceElement element, Direction dir)
        {
            // Find the correct place to start looking for based on the direction
            Coordinate start = element.FindEndPointBasedOnDirection(dir);

            MapSpaceElement foundElement = null;
            int x, y;

            bool endOfRowReached = false;
            bool entireMapIterated = false;

            // Set the significant co-ordinate depending on the direction...
            switch (dir)
            {
                case Direction.Down:
                case Direction.Up:
                    y = start.y;
                    x = 0;
                    break;
                case Direction.Left:
                case Direction.Right:
                    x = start.x;
                    y = 0;
                    break;
                default:
                    throw new Exception("Unknown direction " + dir.ToString());
            }

            while(!entireMapIterated)
            {
                // Go to next row, horizontal or vertical
                switch (dir)
                {
                    case Direction.Down:
                        y++;

                        if(y >= this.MapHeight)
                        {
                            entireMapIterated = true;
                            continue;
                        }
                        break;
                    case Direction.Up:
                        y--;

                        if(y <= 0)
                        {
                            entireMapIterated = true;
                            continue;
                        }
                        break;
                    case Direction.Right:
                        x++;

                        if(x >= this.MapWidth)
                        {
                            entireMapIterated = true;
                            continue;
                        }
                        break;
                    case Direction.Left:
                        x--;

                        if(x <= 0)
                        {
                            entireMapIterated = true;
                            continue;
                        }
                        break;
                }

                endOfRowReached = false;

                while (!endOfRowReached)
                {
                    foreach (MapSpaceElement e in this.mapSpaceElements)
                    {
                        if (e.Guid == element.Guid) continue; // This is the same element. This should never happen, but you never know... skip.

                        if (e.CoordinateIsInElement(x, y))
                        {
                            // Element found. But is it the closest element?
                            if (foundElement == null) foundElement = e; // It's the first element
                            else
                            {
                                if (MapSpaceElement.CountDistanceBetweenElements(element, e) < MapSpaceElement.CountDistanceBetweenElements(element, foundElement))
                                {
                                    foundElement = e;
                                }
                            }

                            return foundElement; // Return the found element
                        }
                    }

                    // This row checked. Go to the next one.
                    switch(dir)
                    {
                        case Direction.Down:
                        case Direction.Up:
                            x++;

                            if(x > this.MapWidth)
                            {
                                endOfRowReached = true;
                                x = 0;
                            }
                            break;
                        case Direction.Left:
                        case Direction.Right:
                            y++;

                            if(y > this.MapHeight)
                            {
                                endOfRowReached = true;
                                y = 0;
                            }
                            break;
                    }
                }
            }

            return null; // Nothing found!
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
            mapSpaceElements.Add(m);
            foreach (Coordinate c in m.Coordinates)
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

        internal enum Direction
        {
            Left = 0,
            Right = 1,
            Up = 2,
            Down = 3
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
            private Guid _guid;


            public List<Coordinate> Coordinates
            {
                get { return _coordinates; }
            }

            public Guid Guid
            {
                get { return _guid; }
            }

            public MapSpaceElement()
            {
                _coordinates = new List<Coordinate>();
                _guid = Guid.NewGuid();
            }

            public void AddCoordinate(Coordinate c)
            {
                Coordinates.Add(c);
            }

            /// <summary>
            /// Returns the endpoint co-ordinate of the map space element based on direction.
            /// For LEFT and UP: the top left
            /// For RIGHT: the top right
            /// For DOWN: the bottom left
            /// </summary>
            /// <param name="dir"></param>
            /// <returns></returns>
            public Coordinate FindEndPointBasedOnDirection(Direction dir)
            {
                Coordinate endpoint = null;
                
                foreach(Coordinate c in this._coordinates)
                {
                    if(endpoint == null)
                    {
                        endpoint = c;
                        continue;
                    }

                    switch (dir)
                    {
                        case Direction.Left:
                        case Direction.Up:
                            if (c.x <= endpoint.x && c.y <= endpoint.y) endpoint = c;
                            break;
                        case Direction.Right:
                            if (c.x >= endpoint.x && c.y <= endpoint.y) endpoint = c;
                            break;
                        case Direction.Down:
                            if (c.x <= endpoint.x && c.y >= endpoint.y) endpoint = c;
                            break;
                        default:
                            throw new Exception("Unknown direction " + dir.ToString());
                    }    
                }

                return endpoint;
            }

            /// <summary>
            /// Checks if the map space element is located
            /// </summary>
            /// <param name="x">The X co-ordinate</param>
            /// <param name="y">The Y co-ordinate</param>
            /// <returns>TRUE if the co-ordinate is in the element, otherwise FALSE</returns>
            public bool CoordinateIsInElement(int x, int y)
            {
                foreach(Coordinate c in this.Coordinates)
                {
                    if (c.x == x && c.y == y) return true;
                }
                return false;
            }

            #region StaticMethods
            public static int CountDistanceBetweenElements(MapSpaceElement e1, MapSpaceElement e2)
            {
                int distance = 0;
                bool distanceCalculated = false;

                foreach(Coordinate c1 in e1.Coordinates)
                {
                    foreach(Coordinate c2 in e2.Coordinates)
                    {
                        int d = Math.Abs(c1.x - c2.x) + Math.Abs(c1.y - c2.y);
                        if (!distanceCalculated)
                        {
                            distance = d;
                            distanceCalculated = true;
                        }
                        else if (d < distance) distance = d;
                    }
                }

                return distance;
            }
            #endregion
        }
        #endregion

    }
}
