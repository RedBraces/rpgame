using System;
using MapGenerator;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class MapGeneratorTests
    {
        Map m;

        [SetUp]
        public void Initialize()
        {
            m = new Map(100, 100);
        }

        [Test]
        public void TestRoomsCannotOverlap()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();
            Map.MapSpaceElement m2 = new Map.MapSpaceElement();

            m1.ElementType = Map.MapSpaceElementType.Room;
            m2.ElementType = Map.MapSpaceElementType.Room;

            // First room:
            m1.AddCoordinate(new Map.Coordinate(1, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));

            m.PlaceMapElementOnMap(m1);

            // Second room:
            m2.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(3, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(3, 2, TileType.Room));

            bool isvalid = m.ValidateMapSpaceElement(m2);

            Assert.IsFalse(isvalid);
        }

        [Test]
        public void TestRoomsCannotBeAdjacentToEachOther()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();
            Map.MapSpaceElement m2 = new Map.MapSpaceElement();

            m1.ElementType = Map.MapSpaceElementType.Room;
            m2.ElementType = Map.MapSpaceElementType.Room;

            // First room:
            m1.AddCoordinate(new Map.Coordinate(1, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));

            m.PlaceMapElementOnMap(m1);

            // Second room:
            m2.AddCoordinate(new Map.Coordinate(3, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(3, 2, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(4, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(4, 2, TileType.Room));

            bool isvalid = m.ValidateMapSpaceElement(m2);

            Assert.IsFalse(isvalid);
        }

        [Test]
        public void RoomsCanBeNextToEachOtherWithOneEmptyRow()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();
            Map.MapSpaceElement m2 = new Map.MapSpaceElement();

            m1.ElementType = Map.MapSpaceElementType.Room;
            m2.ElementType = Map.MapSpaceElementType.Room;

            // First room:
            m1.AddCoordinate(new Map.Coordinate(1, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));

            m.PlaceMapElementOnMap(m1);

            // Second room:
            m2.AddCoordinate(new Map.Coordinate(4, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(4, 2, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(5, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(5, 2, TileType.Room));

            bool isvalid = m.ValidateMapSpaceElement(m2);

            Assert.IsTrue(isvalid);
        }
    }
}
