using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PieceController : MonoBehaviour{
    private List<RenderedPiece> candidates;
    private RenderedPuzzle board;
    private int selected = NONE;
    private const int NONE = -1;

    void Update() {

    }
    public void rotateSelected() {
        if (selected != NONE)
            candidates[selected].rotate();
    }
    private void selectOnPosition(Vector3 position) {
        for (int i = 0; i < candidates.Count; i++) {
            RenderedPiece candidate = candidates[i];
            if (positionIsInCandidate(position, candidate)) {
                selected = i;
                return;
            }
        }
        selected = NONE;
    }
    private bool positionIsInCandidate(Vector3 position, RenderedPiece candidate) {
        List<Vector3> blockPositions = candidate.getBlockPositions();
        float sideLength = candidate.blockSize();
        foreach (Vector3 blockPosition in blockPositions) {
            Vector3 distance = blockPosition - position;
            if ((Mathf.Abs(distance.x) < sideLength / 2)
                &&
                (Mathf.Abs(distance.y) < sideLength / 2)) {
                return true;
            }
        }
        return false;
    }
    private void moveSelectedFor(Vector3 distance) {
        if (selected != NONE)
            candidates[selected].moveFor(distance);
    }
    private void unSelect() {
        selected = NONE;
    }
    private void tryToInsertSelected() {
        if(selected != NONE) {
            RenderedPiece selectedPiece = candidates[selected];
            board.tryToInsert(selectedPiece.getBlockPositions());
        }
    }
    private void resetSelected() {
        if (selected != NONE)
            candidates[selected].reset();
    }
}

