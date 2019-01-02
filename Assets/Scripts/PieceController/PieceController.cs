using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PieceController : MonoBehaviour{
    // TODO : selected ?= NONE check를 없애자
    private List<RenderedPiece> candidates;
    private RenderedPuzzle board;
    private int selected = NONE;
    private const int NONE = -1;
    
    public void setPuzzleSet(RenderedPuzzleSet puzzleSet) {
        candidates = puzzleSet.candidates;
        board = puzzleSet.board;
    }
    public void rotateSelected() {
        if (selected != NONE)
            candidates[selected].rotate();
    }
    public void selectOnPosition(Vector3 position, float padding) {
        for (int i = 0; i < candidates.Count; i++) {
            RenderedPiece candidate = candidates[i];
            if (candidate.includesInNeighborhood(position, padding)) {
                selected = i;
                return;
            }
        }
        unSelect();
    }
    public void moveSelectedFor(Vector3 distance) {
        if (selected != NONE)
            candidates[selected].moveFor(distance);
    }
    private void unSelect() {
        selected = NONE;
    }
    public void tryToInsertSelected() {
        if(selected != NONE) {
            RenderedPiece selectedPiece = candidates[selected];
            Vector3 delta = board.tryToInsertAndReturnDelta(selectedPiece.getBlockPositions());
            if (!board.getRecentInsertionSuccess())
                resetSelected();
            moveSelectedFor(delta);
        }
    }
    private void resetSelected() {
        if (selected != NONE)
            candidates[selected].reset();
    }
    public void tryToExtractSelected() {
        if(selected != NONE) {
            RenderedPiece selectedPiece = candidates[selected];
            board.tryToExtract(selectedPiece.getBlockPositions());
        }
    }
}

