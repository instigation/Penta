using System.Collections.Generic;
using System; // for Abs

public enum Difficulty { EASY = 1, NORMAL = 2, HARD = 3 }

public class PuzzleGenerator {
    private int num_pieces;
    private Difficulty puzzle_difficulty;
    private Puzzle puzzle;
    private bool fitting_suceed;

    public PuzzleGenerator(int num_pieces, Difficulty puzzle_difficulty) {
        this.num_pieces = num_pieces;
        this.puzzle_difficulty = puzzle_difficulty;
    }
    public Puzzle generatePuzzle() {
        /// <param name="num_pieces">0에서 10 사이의 숫자를 넣을 것</param>
        puzzle = new Puzzle();
        while (puzzle.size() < num_pieces) {
            PlacedPiece fitted_piece = findTheHardestFit();
            puzzle.add(fitted_piece);
        }
        return puzzle;
    }

    private PlacedPiece findTheHardestFit() {
        PlacedPiece fitted_piece;
        int hurdle = determineHurdle(puzzle.size());
        while (true) {
            fitted_piece = findPieceThatSatisfiesHurdle(hurdle);
            if (fitted_piece.isEmpty())
                hurdle--;
            else
                break;
        }
        return fitted_piece;
    }
    private int determineHurdle(int piece_index) {
        int hurdle;
        if (piece_index <= 2)
            hurdle = (int)puzzle_difficulty + 1;
        else
            hurdle = (int)puzzle_difficulty;
        return hurdle;
    }
    private PlacedPiece findPieceThatSatisfiesHurdle(int hurdle) {
        Piece[] pieces = getEveryPiecesInDifficultyOrder();
        PlacedPiece fitted_piece;
        for (int index = 0; index < pieces.Length; index++) {
            fitted_piece = fitPieceOverHurdle(pieces[index], hurdle);
            if (!fitted_piece.isEmpty())
                return fitted_piece;
        }
        return new PlacedPiece();
    }
    private Piece[] getEveryPiecesInDifficultyOrder() {
        List<Piece>[] grouped_pieces = new List<Piece>[3];
        for (int i = 0; i < 3; i++) {
            // TODO: Refactor below line
            grouped_pieces[i] = Piece.getPiecesInDifficulty((Difficulty)(((int)puzzle_difficulty + i) % 3));
        }
        List<Piece> ret = new List<Piece>();
        foreach (List<Piece> pieces in grouped_pieces) {
            pieces.Shuffle();
            ret.AddRange(pieces);
        }
        return ret.ToArray();
    }
    private PlacedPiece fitPieceOverHurdle(Piece p, int hurdle) {
        List<PlacedPiece> pieces_placed_near = puzzle.getNearPlacedPiecesOf(p);
        foreach(PlacedPiece placed_piece in pieces_placed_near) {
            if (!puzzle.overlapWith(placed_piece) && 
                (puzzle.touchingSideWith(placed_piece) >= hurdle))
                return placed_piece;
        }
        return new PlacedPiece();
    }
}


public class Puzzle {
    private List<PlacedPiece> pieces;

    public int size() { return pieces.Count; }
    public void add(PlacedPiece pp) { pieces.Add(pp); }
    public List<PlacedPiece> getNearPlacedPiecesOf(Piece p) {
        List<PlacedPiece> ret = new List<PlacedPiece>();
        if (pieces.Count == 0) {
            // TODO: rotate initial piece randomly
            ret.Add(new PlacedPiece(p));
        }
        else {
            foreach(PlacedPiece piece in pieces) {
                ret.AddRange(piece.getNearPlacedPiecesOf(p));
            }
        }
        return ret;
    }
    public int touchingSideWith(PlacedPiece pp) {
        ///<param name="pp">
        ///pp는 this와 겹치지 않는다고 가정
        ///</param>
        int sum = 0;
        foreach (PlacedPiece piece in pieces) {
            sum += piece.touchingSideWith(pp);
        }
        return sum;
    }
    public bool overlapWith(PlacedPiece pp) {
        foreach (PlacedPiece piece in pieces) {
            if (piece.overlapWith(pp))
                return true;
        }
        return false;
    }
}

public enum Rotation { NORTH, EAST, SOUTH, WEST };
public static class Rotator {
    public static Rotation reverse(Rotation original) {
        return (Rotation)(((int)original + 2) % numRotation());
    }
    private static int numRotation() {
        return 4;
    }
    public static List<Rotation> getAll() {
        List<Rotation> ret = new List<Rotation>();
        foreach (Rotation rot in Enum.GetValues(typeof(Rotation)))
            ret.Add(rot);
        return ret;
    }
}

public class Coordinate {
    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
    }
    public int x, y;
    public bool adjacentTo(Coordinate other) {
        return (Math.Abs(x - other.x) == 1) || (Math.Abs(y - other.y) == 1);
    }

    // From MSDN: https://msdn.microsoft.com/ko-kr/library/ms173147(v=vs.90).aspx
    public override bool Equals(System.Object obj) {
        // If parameter is null return false.
        if (obj == null) {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Coordinate p = obj as Coordinate;
        if ((System.Object)p == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }
    public bool Equals(Coordinate p) {
        // If parameter is null return false:
        if ((object)p == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }
    public override int GetHashCode() {
        return x ^ y;
    }

    public static Coordinate operator +(Coordinate c1, Coordinate c2) {
        return new Coordinate(c1.x + c2.x, c1.y + c2.y);
    }

    public override string ToString() {
        return String.Format("({0}, {1})", x, y);
    }
}

public class PlacedPiece {
    private Piece piece;
    private Coordinate center_coordinate;

    public PlacedPiece() { piece = null; }
    public PlacedPiece(Piece p) {
        piece = p;
        center_coordinate = new Coordinate(0, 0);
    }
    public PlacedPiece(Piece p, Coordinate cor) {
        piece = p;
        center_coordinate = cor;
    }
    public bool isEmpty() { return piece == null; }
    public List<PlacedPiece> getNearPlacedPiecesOf(Piece p) {
        List<PlacedPiece> ret = new List<PlacedPiece>();
        for (int x = -touchingRange(p, Rotation.WEST); x <= touchingRange(p, Rotation.EAST); x++) {
            for (int y = -touchingRange(p, Rotation.SOUTH); y <= touchingRange(p, Rotation.NORTH); y++) {
                Coordinate candidateCoordinate = center_coordinate + new Coordinate(x, y);
                PlacedPiece candidate = new PlacedPiece(p, candidateCoordinate);
                if (!overlapWith(candidate) && (touchingSideWith(candidate) > 0))
                    ret.Add(candidate);
            }
        }
        return ret;
    }
    private int touchingRange(Piece p, Rotation rot) {
        return 1 + piece.distanceFromCenterToEnd(rot) + p.distanceFromCenterToEnd(Rotator.reverse(rot));
    }
    public int touchingSideWith(PlacedPiece other) {
        ///<param name="other">
        ///other은 this와 겹치지 않는다고 가정
        ///</param>
        int num_touching_sides = 0;
        List<Coordinate> other_blocks = other.getBlocks();
        List<Coordinate> this_blocks = getBlocks();
        foreach (Coordinate other_block in other_blocks)
            foreach (Coordinate this_block in this_blocks)
                if (other_block.adjacentTo(this_block))
                    num_touching_sides++;
        return num_touching_sides;
    }
    public bool overlapWith(PlacedPiece other) {
        List<Coordinate> other_blocks = other.getBlocks();
        List<Coordinate> this_blocks = getBlocks();
        foreach (Coordinate other_block in other_blocks)
            foreach (Coordinate this_block in this_blocks)
                if (other_block == this_block)
                    return true;
        return false;
    }
    public List<Coordinate> getBlocks() {
        List<Coordinate> blocksAtOrigin = piece.getBlockCoordinatesRelativeToCenter();
        List<Coordinate> blocks = new List<Coordinate>();
        foreach (Coordinate block in blocksAtOrigin)
            blocks.Add(block + center_coordinate);
        return blocks;
    }
}

public class Piece {
    private static List<List<Coordinate>> blockCoordinates;
    private static int[][] pieceIndexesInDifficulty;
    private static List<int> centerIndex;
    private int blockIndex;
    private Rotation rot;

    public Piece(int blockIndex, Rotation rot) {
        this.blockIndex = blockIndex;
        this.rot = rot;
    }
    public static List<Piece> getPiecesInDifficulty(Difficulty difficulty) {
        // TODO : rearrange in some order by difficulty (Policy)
        List<Piece> ret = new List<Piece>();
        foreach(int index in getPieceIndexesInDifficulty(difficulty)) {
            List<Rotation> fourDirections = Rotator.getAll();
            foreach (Rotation rot in fourDirections)
                ret.Add(new Piece(index, rot));
        }
        return ret;
    }
    private static int[] getPieceIndexesInDifficulty(Difficulty difficulty) {
        return pieceIndexesInDifficulty[(int)difficulty];
    }
    public int distanceFromCenterToEnd(Rotation direction) {
        // TODO : dynamically store the corresponding value
        return 2;
    }
    public List<Coordinate> getBlockCoordinatesRelativeToCenter() {
        return blockCoordinates[blockIndex];
    }
}
