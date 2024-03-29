﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PieceController : MonoBehaviour{
    // TODO : selected ?= NONE check를 없애자
    private List<RenderedPiece> candidates;
    private List<bool> isInserted;
    private RenderedPuzzle board;
    private int selected = NONE;
    private const int NONE = -1;


    public void setPuzzleSet(RenderedPuzzleSet puzzleSet) {
        candidates = puzzleSet.candidates;
        board = puzzleSet.board;

        int n = candidates.Count;
        isInserted = new List<bool>();
        for (int i = 0; i < n; i++)
            isInserted.Add(false);
        selected = NONE;
    }
    public void rotateSelected() {
        if (selected != NONE)
            candidates[selected].rotate();
    }
    public void selectOnPosition(Vector3 position, float padding) {
        for (int i = 0; i < candidates.Count; i++) {
            RenderedPiece candidate = candidates[i];
            if (isInserted[i])
            {
                if (candidate.includes(position))
                {
                    select(i);
                    return;
                }
            }
            else
            {
                if (candidate.includesInNeighborhood(position, padding))
                {
                    select(i);
                    return;
                }
            }
        }
    }
    public void moveSelectedForAnchoredVector(Vector2 vector) {
        if (selected != NONE)
            candidates[selected].moveForAnchoredVector(vector);
    }
    public void moveSelectedToAnchoredPosition(Vector2 position)
    {
        if (selected != NONE)
            candidates[selected].moveForAnchoredVector(position - candidates[selected].getBottomAnchoredPosition());
    }
    private void select(int index) {
        selected = index;
        candidates[selected].setToBoardFitSize();
        candidates[selected].setSiblingIndexFrom(100); // Assumes (# of blocks) < 100. That is, make them to be the last sibling.
    }
    public void unSelect()
    {
        if(selected != NONE)
        {
            candidates[selected].resetSiblingIndex();
            selected = NONE;
        }
    }
    public bool tryToInsertSelected() {
        // postcondition: return false iff it wasn't insertion attempt. That is, iff there is no selected.
        if (selected != NONE)
        {
            RenderedPiece selectedPiece = candidates[selected];
            Vector3 delta = board.tryToInsertAndReturnDelta(selectedPiece.getBlockAnchoredPositions());
            if (!board.getRecentInsertionSuccess())
                resetSelected();
            else
                isInserted[selected] = true;
            moveSelectedForAnchoredVector(delta);
            return true;
        }
        else return false;
    }
    public void highlightBoardBySelected()
    {
        if (selected != NONE)
        {
            RenderedPiece selectedPiece = candidates[selected];
            board.highlightClosestBlocks(selectedPiece.getBlockAnchoredPositions());
        }
        else
            board.resetBlockColors();

    }
    private void resetSelected() {
        if (selected != NONE)
        {
            candidates[selected].reset();
            candidates[selected].setToCandidateSize();
            candidates[selected].resetSiblingIndex();
        }
    }
    public void tryToExtractSelected() {
        if(selected != NONE) {
            RenderedPiece selectedPiece = candidates[selected];
            bool extractionSuccess = board.tryToExtract(selectedPiece.getBlockAnchoredPositions());
            if (extractionSuccess)
                isInserted[selected] = false;
            else
            {
                unSelect();
            }
        }
    }
}

