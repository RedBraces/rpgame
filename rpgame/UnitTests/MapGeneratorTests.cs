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
            m = new Map(100, 100, false);
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

        [Test]
        public void TestFindingRoomEndpoints()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();
            m1.ElementType = Map.MapSpaceElementType.Room;

            m1.AddCoordinate(new Map.Coordinate(1, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 3, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 3, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 3, TileType.Room));

            Map.Coordinate left = m1.FindEndPointBasedOnDirection(Map.Direction.Left);
            Map.Coordinate right = m1.FindEndPointBasedOnDirection(Map.Direction.Right);
            Map.Coordinate up = m1.FindEndPointBasedOnDirection(Map.Direction.Up);
            Map.Coordinate down = m1.FindEndPointBasedOnDirection(Map.Direction.Down);

            Assert.AreEqual(1, left.x);
            Assert.AreEqual(1, left.y);
            Assert.AreEqual(1, up.x);
            Assert.AreEqual(1, up.y);

            Assert.AreEqual(3, right.x);
            Assert.AreEqual(1, right.y);

            Assert.AreEqual(1, down.x);
            Assert.AreEqual(3, down.y);
        }

        [Test]
        public void TestCalculateDistanceBetweenElements()
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

            // Second room:
            m2.AddCoordinate(new Map.Coordinate(4, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(4, 2, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(5, 1, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(5, 2, TileType.Room));

            int d1 = Map.MapSpaceElement.CountDistanceBetweenElements(m1, m2);
            int d2 = Map.MapSpaceElement.CountDistanceBetweenElements(m2, m1);

            Assert.AreEqual(d1, d2);
            Assert.AreEqual(2, d1);
        }

        [Test]
        public void TestFindingAdjacentElement()
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

            m.PlaceMapElementOnMap(m2);

            var fe1 = m.FindMapSpaceElement(m2, Map.Direction.Left);
            var fe2 = m.FindMapSpaceElement(m1, Map.Direction.Right);

            Assert.AreEqual(m1.Guid, fe1.Guid);
            Assert.AreEqual(m2.Guid, fe2.Guid);
        }

        [Test]
        public void TestFindingDirectionFromCoordinate()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();

            m1.ElementType = Map.MapSpaceElementType.Room;

            // First room:
            m1.AddCoordinate(new Map.Coordinate(1, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));

            var dirs = m1.WhereAmIComparedToCoordinate(3, 3);

            Assert.IsTrue(dirs.Contains(Map.Direction.Up));
            Assert.IsTrue(dirs.Contains(Map.Direction.Left));
        }
    }
}
