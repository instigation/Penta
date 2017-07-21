using System.Collections.Generic;


public enum difficulty { EASY = 1, NORMAL = 2, HARD = 3 }


public class PuzzleGenerator {
    private int num_pieces;
    private difficulty puzzle_difficulty;
    private Puzzle puzzle;
    private bool fitting_suceed;

    public PuzzleGenerator(int num_pieces, difficulty puzzle_difficulty) {
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
            grouped_pieces[i] = Piece.getPiecesByDifficulty(puzzle_difficulty);
        }
        List<Piece> ret = new List<Piece>();
        foreach (List<Piece> pieces in grouped_pieces) {
            pieces.Shuffle();
            ret.AddRange(pieces);
        }
        return ret.ToArray();
    }
    private PlacedPiece fitPieceOverHurdle(Piece p, int hurdle) {
        List<PlacedPiece> pieces_placed_near = puzzle.getNearPlacedPieceOf(p);
        foreach(PlacedPiece placed_piece in pieces_placed_near) {
            if (puzzle.touchingSideWith(placed_piece) >= hurdle)
                return placed_piece;
        }
        return new PlacedPiece();
    }
}


public class Puzzle {

}

public class PlacedPiece {

}

public class Piece {

}
