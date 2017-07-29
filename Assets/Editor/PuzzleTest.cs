using NUnit.Framework;
using System.Collections.Generic;

public class RotatorTest {
    [Test]
    public void reverseTest() {
        Assert.That(Rotator.reverse(Rotation.WEST), Is.EqualTo(Rotation.EAST));
        Assert.That(Rotator.reverse(Rotation.EAST), Is.EqualTo(Rotation.WEST));
        Assert.That(Rotator.reverse(Rotation.SOUTH), Is.EqualTo(Rotation.NORTH));
        Assert.That(Rotator.reverse(Rotation.NORTH), Is.EqualTo(Rotation.SOUTH));
    }
}

public class CoordinateTest {
    [Test]
    public void compareTest() {
        Assert.That(new Coordinate(1, 2), Is.EqualTo(new Coordinate(1, 2)));
        Assert.IsTrue((new Coordinate(1, 2)) == (new Coordinate(1, 2)));
        Assert.IsTrue((new Coordinate(1, 1)) != (new Coordinate(1, 2)));
    }

    [Test]
    public void addTest() {
        var c1 = new Coordinate(1, 2);
        var c2 = new Coordinate(3, 5);
        var correctSum = new Coordinate(4, 7);
        Assert.That(c1 + c2, Is.EqualTo(correctSum));
    }

    [Test]
    public void adjacencyTest() {
        var point = new Coordinate(1, 1);
        var adjacentPoint1 = new Coordinate(1, 2);
        var adjacentPoint2 = new Coordinate(0, 1);
        var farPoint = new Coordinate(2, 2);
        Assert.IsTrue(point.adjacentTo(adjacentPoint1));
        Assert.IsTrue(point.adjacentTo(adjacentPoint2));
        Assert.IsFalse(point.adjacentTo(farPoint));
    }
}

public class PieceTest { 
    public class getBlocksTest {

        [Test]
        public void northTest() {
            var pieceWithDefaultRotation = new Piece(0, Rotation.NORTH);
            var coordinates = pieceWithDefaultRotation.getBlockCoordinatesRelativeToCenter();
            var center = new Coordinate(0, 0);
            Assert.That(coordinates.Count, Is.EqualTo(5));
            Assert.That(coordinates, Has.Member(center));
            Assert.That(coordinates, Has.Member(new Coordinate(-1, 0)));
            Assert.That(coordinates, Has.Member(new Coordinate(1, 1)));
        }

        [Test]
        public void eastTest() {
            var pieceEast = new Piece(0, Rotation.EAST);
            var coordinates = pieceEast.getBlockCoordinatesRelativeToCenter();
            var center = new Coordinate(0, 0);
            Assert.That(coordinates.Count, Is.EqualTo(5));
            Assert.That(coordinates, Has.Member(center));
            Assert.That(coordinates, Has.Member(new Coordinate(1, -1)));
            Assert.That(coordinates, Has.Member(new Coordinate(1, 0)));
        }

        [Test]
        public void southTest() {
            var pieceEast = new Piece(2, Rotation.SOUTH);
            var coordinates = pieceEast.getBlockCoordinatesRelativeToCenter();
            var center = new Coordinate(0, 0);
            Assert.That(coordinates.Count, Is.EqualTo(5));
            Assert.That(coordinates, Has.Member(center));
            Assert.That(coordinates, Has.Member(new Coordinate(1, 1)));
            Assert.That(coordinates, Has.Member(new Coordinate(0, -2)));
        }

        [Test]
        public void westTest() {
            var pieceEast = new Piece(2, Rotation.WEST);
            var coordinates = pieceEast.getBlockCoordinatesRelativeToCenter();
            var center = new Coordinate(0, 0);
            Assert.That(coordinates.Count, Is.EqualTo(5));
            Assert.That(coordinates, Has.Member(center));
            Assert.That(coordinates, Has.Member(new Coordinate(1, -1)));
            Assert.That(coordinates, Has.Member(new Coordinate(-2, 0)));
        }

    }
    public class getPiecesInDifficultyTest {
        [Test]
        public void lengthTest() {
            int total = 0;
            foreach (Difficulty diff in DifficultyIterator.getAll())
                total += Piece.getPiecesInDifficulty(diff).Count;
            Assert.AreEqual(total, 17 * Rotator.numRotation());
        }
    }
}

public class PlacedPieceTest {
    [Test]
    public void overlapTest() {
        var me = new PlacedPiece(new Piece(0, Rotation.NORTH), new Coordinate(0, 0));
        var other1 = new PlacedPiece(new Piece(2, Rotation.EAST), new Coordinate(2, 1));
        var other2 = new PlacedPiece(new Piece(6, Rotation.SOUTH), new Coordinate(1, 3));
        var farOne = new PlacedPiece(new Piece(5, Rotation.SOUTH), new Coordinate(5, 5));
        Assert.IsTrue(me.overlapWith(other1));
        Assert.IsTrue(me.overlapWith(other2));
        Assert.IsFalse(me.overlapWith(farOne));
    }

    [Test]
    public void touchingSideTest() {
        var oneSide1 = new PlacedPiece(new Piece(0, Rotation.NORTH), new Coordinate(0, 0));
        var oneSide2 = new PlacedPiece(new Piece(1, Rotation.NORTH), new Coordinate(2, -2));
        Assert.That(oneSide1.touchingSideWith(oneSide2), Is.EqualTo(1));

        var twoSide1 = oneSide1;
        var twoSide2 = new PlacedPiece(new Piece(1, Rotation.NORTH), new Coordinate(2, -1));
        Assert.That(twoSide1.touchingSideWith(twoSide2), Is.EqualTo(2));

        var threeSide1 = oneSide1;
        var threeSide2 = new PlacedPiece(new Piece(2, Rotation.NORTH), new Coordinate(1, -2));
        Assert.That(threeSide1.touchingSideWith(threeSide2), Is.EqualTo(3));

        var fourSide1 = oneSide1;
        var fourSide2 = new PlacedPiece(new Piece(5, Rotation.SOUTH), new Coordinate(1, 0));
        Assert.That(fourSide1.touchingSideWith(fourSide2), Is.EqualTo(4));

        var fiveSide1 = oneSide1;
        var fiveSide2 = new PlacedPiece(new Piece(9, Rotation.EAST), new Coordinate(-2, 0));
        Assert.That(fiveSide1.touchingSideWith(fiveSide2), Is.EqualTo(5));
    }

    [Test]
    public void getNearPlacedPiecesOfTest() {
        // Arrange
        PlacedPiece me = new PlacedPiece(new Piece(0, Rotation.NORTH), new Coordinate(0, 0));
        List<Piece> somePieces = new List<Piece>();
        Rotation rot = Rotation.EAST;
        for (int i = 0; i < 17; i++)
            somePieces.Add(new Piece(i, rot));

        // Assert
        foreach(Piece p in somePieces) {
            List<PlacedPiece> nears = me.getNearPlacedPiecesOf(p);
            Assert.That(nears.Count, Is.AtLeast(1));
            foreach(PlacedPiece candidate in nears) {
                Assert.IsFalse(me.overlapWith(candidate));
                Assert.That(me.touchingSideWith(candidate), Is.AtLeast(1));
            }
        }
    }
}

public class PuzzleTest {
    private static readonly PlacedPiece pp1 = new PlacedPiece(new Piece(0, Rotation.NORTH), new Coordinate(0, 0));
    private static readonly PlacedPiece pp2 = new PlacedPiece(new Piece(2, Rotation.EAST), new Coordinate(2, -1));
    private static readonly Puzzle puzzle1 = new Puzzle(new List<PlacedPiece> { pp1, pp2 });
    private static readonly PlacedPiece overlapsWithPuzzle1 = new PlacedPiece(new Piece(4, Rotation.NORTH), new Coordinate(2, 0));
    private static readonly PlacedPiece pp3 = new PlacedPiece(new Piece(6, Rotation.SOUTH), new Coordinate(3, 1));
    private static readonly Puzzle puzzle2 = new Puzzle(new List<PlacedPiece> { pp1, pp2, pp3 });
    private static readonly PlacedPiece disjointWithPuzzle2 = new PlacedPiece(new Piece(8, Rotation.WEST), new Coordinate(0, 3));

    [Test]
    public void overlapTest() {
        Assert.IsTrue(puzzle1.overlapWith(overlapsWithPuzzle1));
        Assert.IsFalse(puzzle2.overlapWith(disjointWithPuzzle2));
    }

    [Test]
    public void touchingSideTest() {
        Assert.That(puzzle1.touchingSideWith(pp3), Is.EqualTo(3));
    }
}

public class PuzzleGeneratorTest {
    [Test]
    public void generatePuzzleTest() {
        PuzzleGenerator pgen = new PuzzleGenerator(4, Difficulty.EASY);
        Puzzle puzzle = pgen.generatePuzzle();

        Assert.That(puzzle.size(), Is.EqualTo(4));
    }
}