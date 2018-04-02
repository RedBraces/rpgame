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

            bool isvalid = m.ValidateMapSpaceElement(ref m2);

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

            bool isvalid = m.ValidateMapSpaceElement(ref m2);

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

            bool isvalid = m.ValidateMapSpaceElement(ref m2);

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
        public void TestIfElementIsInMyLongitudeOrLatitude()
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
            m2.AddCoordinate(new Map.Coordinate(10, 10, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(10, 11, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(11, 10, TileType.Room));
            m2.AddCoordinate(new Map.Coordinate(11, 11, TileType.Room));

            m.PlaceMapElementOnMap(m2);

            Map.Coordinate c = new Map.Coordinate(50, 1, TileType.Wall);
            Map.Coordinate c2 = new Map.Coordinate(10, 50, TileType.Wall);

            var d1 = m.FindElementOnThisLongitudeOrLatitude(c, true);
            var d2 = m.FindElementOnThisLongitudeOrLatitude(c2, false);

            Assert.IsTrue(d1.Contains(Map.Direction.Left));
            Assert.IsTrue(d2.Contains(Map.Direction.Up));
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

        [Test]
        public void TestCorridorValidation()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();
            Map.MapSpaceElement m2 = new Map.MapSpaceElement();
            Map.MapSpaceElement m3 = new Map.MapSpaceElement();

            m1.ElementType = Map.MapSpaceElementType.Room;
            m2.ElementType = Map.MapSpaceElementType.Corridor;
            m3.ElementType = Map.MapSpaceElementType.Corridor;

            // First room:
            m1.AddCoordinate(new Map.Coordinate(1, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 1, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 2, TileType.Room));

            m.PlaceMapElementOnMap(m1);

            m2.AddCoordinate(new Map.Coordinate(2, 3, TileType.Corridor));
            m2.AddCoordinate(new Map.Coordinate(2, 4, TileType.Corridor));

            bool firstCorridorIsValid = m.ValidateMapSpaceElement(ref m2);

            m3.AddCoordinate(new Map.Coordinate(1, 3, TileType.Corridor));
            m3.AddCoordinate(new Map.Coordinate(2, 3, TileType.Corridor));
            m3.AddCoordinate(new Map.Coordinate(3, 3, TileType.Corridor));

            bool secondCorridotIsValid = m.ValidateMapSpaceElement(ref m3);

            Assert.IsTrue(firstCorridorIsValid);
            Assert.IsTrue(secondCorridotIsValid);
            Assert.AreEqual(2, m3.Coordinates.Count);
        }

        [Test]
        public void TestRoomValidationForCorridors()
        {
            Map.MapSpaceElement m1 = new Map.MapSpaceElement();
            Map.MapSpaceElement m2 = new Map.MapSpaceElement();

            m1.ElementType = Map.MapSpaceElementType.Room;
            m2.ElementType = Map.MapSpaceElementType.Corridor;

            m2.AddCoordinate(new Map.Coordinate(1, 1, TileType.Corridor));
            m2.AddCoordinate(new Map.Coordinate(2, 1, TileType.Corridor));
            m2.AddCoordinate(new Map.Coordinate(3, 1, TileType.Corridor));

            m.PlaceMapElementOnMap(m2);

            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(1, 2, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 3, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(2, 3, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 4, TileType.Room));
            m1.AddCoordinate(new Map.Coordinate(3, 4, TileType.Room));

            bool isRoomValid = m.ValidateMapSpaceElement(ref m1);

            Assert.IsFalse(isRoomValid);
        }
    }
}
