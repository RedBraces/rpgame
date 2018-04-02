using System;
using System.Collections.Generic;

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

            RoomPercentage = 35;
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
                // continue; 
                // Should rooms be linked together with a corridor?
                if(success & this.CountMapSpaceElement(MapSpaceElementType.Room) > 1)
                {
                    GenerateCorridorFromRoom(this.mapSpaceElements[this.mapSpaceElements.Count - 1]);
                    //return;
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

        internal bool GenerateCorridorFromRoom(MapSpaceElement room)
        {
            
            bool success = false;
            int counter = 0;
            Random r = new Random();
            Direction d = (Direction)r.Next(0, 3); // randomize starting direction

            while (!success && counter <= 3)
            {
                success = GenerateCorridorFromRoom(room, d);

                // Next direction
                if ((int)d == 3) d = 0;
                else d++;

                counter++;
            }

            return success;
        }

        internal bool GenerateCorridorFromRoom(MapSpaceElement room, Direction direction)
        {
            if (room.ElementType != MapSpaceElementType.Room) throw new Exception("Map space element is not a room!");

            // Randomize in what direction an existing map space element should be looked for first
            Random r = new Random();
            //Direction direction = (Direction)r.Next(0, 3);

            Coordinate currentPosition = null;
            int offset = 0;

            // Determine the starting cell
            switch(direction)
            {
                case Direction.Down:
                    offset = r.Next(0, room.Width - 1);
                    currentPosition = room.FindEndPointBasedOnDirection(Direction.Down);
                    currentPosition = new Coordinate(currentPosition.x + offset, currentPosition.y + 1, TileType.Corridor);
                    break;
                case Direction.Up:
                    offset = r.Next(0, room.Width - 1);
                    currentPosition = room.FindEndPointBasedOnDirection(Direction.Up);
                    currentPosition = new Coordinate(currentPosition.x + offset, currentPosition.y - 1, TileType.Corridor);
                    break;
                case Direction.Left:
                    offset = r.Next(0, room.Height - 1);
                    currentPosition = room.FindEndPointBasedOnDirection(Direction.Left);
                    currentPosition = new Coordinate(currentPosition.x - 1, currentPosition.y + offset, TileType.Corridor);
                    break;
                case Direction.Right:
                    offset = r.Next(0, room.Height - 1);
                    currentPosition = room.FindEndPointBasedOnDirection(Direction.Right);
                    currentPosition = new Coordinate(currentPosition.x + 1, currentPosition.y + offset, TileType.Corridor);
                    break;
                default:
                    throw new Exception("Unknown direction " + direction.ToString());
            }
            // With the starting position known, start building the corridor:
            bool corridorFinished = false;
            MapSpaceElement corridor = new MapSpaceElement();
            corridor.ElementType = MapSpaceElementType.Corridor;

            while(!corridorFinished)
            {
                corridor.AddCoordinate(new Coordinate(currentPosition.x, currentPosition.y, TileType.Corridor));

                // Move one step further
                switch(direction)
                {
                    case Direction.Down:
                        currentPosition.y++;
                        break;
                    case Direction.Up:
                        currentPosition.y--;
                        break;
                    case Direction.Left:
                        currentPosition.x--;
                        break;
                    case Direction.Right:
                        currentPosition.x++;
                        break;
                }

                if (currentPosition.x <= 0 || currentPosition.y <= 0) break;
                if (currentPosition.x >= this.MapWidth || currentPosition.y >= this.MapHeight) break;

                // Is the corridor already finished?
                if(this.IsThereAMapSpaceElementHere(currentPosition))
                {
                    corridorFinished = true;

                    if (!this.ValidateMapSpaceElement(corridor)) return false;
                    //bool s = this.ValidateMapSpaceElement(corridor);
                    this.PlaceMapElementOnMap(corridor);
                    return true;
                }

                // Determine in what direction to move after the next cell, eg. if there is a need for a turn
                List<Direction> longitude = this.FindElementOnThisLongitude(currentPosition);
                List<Direction> latitude = this.FindElementOnThisLatitude(currentPosition);

                // Again, based on direction, determine what to do next:
                switch(direction)
                {
                    case Direction.Down:
                    case Direction.Up:
                        if (latitude.Contains(direction)) continue; // No need to turn, there is an element in the current direction
                        if (longitude.Count <= 0) continue; // Nothing here, continue...
                        // Otherwise, randomize where to go:
                        direction = longitude[r.Next(0, longitude.Count - 1)];
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        if (longitude.Contains(direction)) continue; // Again, no need to turn
                        if (latitude.Count <= 0) continue; // Nothing here. Contunue...

                        // Where to turn?
                        direction = latitude[r.Next(0, latitude.Count - 1)];
                        break;
                }
            }
            
            // This should never be reached...
            return false;
        }

        #region FindingElements
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

        internal List<Direction> FindElementOnThisLongitude(Coordinate coordinate)
        {
            return FindElementOnThisLongitudeOrLatitude(coordinate, true);
        }

        internal List<Direction> FindElementOnThisLatitude(Coordinate coordinate)
        {
            return FindElementOnThisLongitudeOrLatitude(coordinate, false);
        }

        /// <summary>
        /// Finds elements on the map that are on the same longitude or latitude as the provided coordinate
        /// </summary>
        /// <param name="coordinate">The coordinate to search based on</param>
        /// <param name="isLongitude">Whether to look on the longitude or the latitude</param>
        /// <returns>A list of directions in which there are elements</returns>
        internal List<Direction> FindElementOnThisLongitudeOrLatitude(Coordinate coordinate, bool isLongitude)
        {
            List<Direction> d = new List<Direction>();

            foreach(MapSpaceElement m in this.mapSpaceElements)
            {
                foreach(Coordinate c in m.Coordinates)
                {
                    bool isMatch = false;

                    if (isLongitude && c.y == coordinate.y) isMatch = true;
                    else if (!isLongitude && c.x == coordinate.x) isMatch = true;

                    if(isMatch)
                    {
                        List<Direction> elementDirs = m.WhereAmIComparedToCoordinate(coordinate.x, coordinate.y);

                        // Depending on whether we're looking for longitude or latitude, we only want to know if the
                        // elements is up/down or left/right of the current position
                        foreach(Direction dir in elementDirs)
                        {
                            if (isLongitude && (dir == Direction.Up || dir == Direction.Down)) continue;
                            else if (!isLongitude && (dir == Direction.Left || dir == Direction.Right)) continue;

                            if (!d.Contains(dir)) d.Add(dir);
                        }

                        break;
                    }
                }
            }

            return d;
        }

        internal bool IsThereAMapSpaceElementHere(Coordinate coordinate)
        {
            foreach(MapSpaceElement m in this.mapSpaceElements)
            {
                if (m.CoordinateIsInElement(coordinate.x, coordinate.y)) return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Checks if a map space element is valid eg. can be placed on the map without conflicts, and in some cases
        /// edits the (potentially) invalid element
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        internal bool ValidateMapSpaceElement(MapSpaceElement m)
        {
            bool previousCellWasAdjacent = false;
            int removeFromIndex = 0;
            int counter = 0;

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
                    if (this.GetMapTile(c.y - 1, c.x - 1) != TileType.Wall) return false;
                    if (this.GetMapTile(c.y - 1, c.x) != TileType.Wall) return false;
                    if (this.GetMapTile(c.y - 1, c.x + 1) != TileType.Wall) return false;

                    if (this.GetMapTile(c.y, c.x - 1) != TileType.Wall) return false;
                    if (this.GetMapTile(c.y, c.x + 1) != TileType.Wall) return false;

                    if (this.GetMapTile(c.y + 1, c.x - 1) != TileType.Wall) return false;
                    if (this.GetMapTile(c.y + 1, c.x) != TileType.Wall) return false;
                    if (this.GetMapTile(c.y + 1, c.x + 1) != TileType.Wall) return false;
                }

                // Validation 4: a corridor cannot run adjacent to another corridor or room for more than one square
                // If that happens, remove extra squares that run adjacent.
                if(m.ElementType == MapSpaceElementType.Corridor && counter > 0)
                {
                    if (this.GetMapTile(c.y - 1, c.x) != TileType.Wall || this.GetMapTile(c.y - 1, c.x) != TileType.Wall
                        || this.GetMapTile(c.y, c.x + 1) != TileType.Wall || this.GetMapTile(c.y, c.x - 1) != TileType.Wall)
                    {
                        if (counter >= removeFromIndex && previousCellWasAdjacent) removeFromIndex = counter;
                        previousCellWasAdjacent = true;
                    }
                    else previousCellWasAdjacent = false;
                }

                counter++;
            }

            // Modification: if the space element type is a corridor and there are adjacent squares, remove extra squares
            if(m.ElementType == MapSpaceElementType.Corridor && removeFromIndex > 0)
            {
                m.RemoveCoordinatesStartingAt(removeFromIndex);
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

            public int Width
            {
                get
                {
                    int smallest = 0;
                    int largest = 0;
                    foreach (Coordinate c in this.Coordinates)
                    {
                        if (smallest == 0 || c.x < smallest) smallest = c.x;
                        if (largest == 0 || c.x > largest) largest = c.x;
                    }

                    return largest - smallest;
                }
            }

            public int Height
            {
                get
                {
                    int smallest = 0;
                    int largest = 0;

                    foreach (Coordinate c in this.Coordinates)
                    {
                        if (smallest == 0 || c.y < smallest) smallest = c.y;
                        if (largest == 0 || c.y > largest) largest = c.y;
                    }

                    return largest - smallest;
                }
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

            public void RemoveCoordinatesStartingAt(int position)
            {
                int remove = _coordinates.Count - position;
                this._coordinates.RemoveRange(position, _coordinates.Count - position);
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

            public List<Direction> WhereAmIComparedToCoordinate(int x, int y)
            {
                List<Direction> directions = new List<Direction>();

                bool isAbove = false;
                bool isBelow = false;
                bool isLeft = false;
                bool isRight = false;

                foreach(Coordinate c in this.Coordinates)
                {
                    if (c.x < x) isLeft = true;
                    if (c.x > x) isRight = true;
                    if (c.y < y) isAbove = true;
                    if (c.y > y) isBelow = true;
                }

                if (isLeft && !isRight) directions.Add(Direction.Left);
                if (isRight && !isLeft) directions.Add(Direction.Right);
                if (isAbove && !isBelow) directions.Add(Direction.Up);
                if (isBelow && !isAbove) directions.Add(Direction.Down);

                return directions;
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
