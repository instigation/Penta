using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRenderer : MonoBehaviour {

    public RenderedPuzzleSet render(Puzzle p) {

    }
}

public class RenderedPuzzleSet {
    public List<RenderedPiece> candidates;
    public RenderedPuzzle board;
}

public class RenderedPiece {
    public void rotate();
    public void move(Vector3);
    public void resetPosition();
    public void reset();
}

public class RenderedPuzzle {
    public void tryToInsert(RenderedPiece);
    public void extract(RenderedPiece);
    public bool isSolved();
}