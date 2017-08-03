using System.Collections.Generic;
using UnityEngine;


public class RenderedPuzzleSet {
    public List<RenderedPiece> candidates;
    public RenderedPuzzle board;

    public RenderedPuzzleSet(List<RenderedPiece> candidates, RenderedPuzzle board) {
        this.candidates = candidates;
        this.board = board;
    }
}

public class RenderedPiece {
    private List<GameObject> renderedBlocks;
    private List<Coordinate> blocks;

    public RenderedPiece(List<GameObject> rBlocks, List<Coordinate> blocks) {
        renderedBlocks = rBlocks;
        this.blocks = blocks;
    }
    public List<Vector3> getBlockPositions() {
        List<Vector3> ret = new List<Vector3>();
        foreach (GameObject renderedBlock in renderedBlocks) {
            Vector3 newPosition = UnityUtils.getPositionOfUIElement(renderedBlock);
            ret.Add(newPosition);
        }
        return ret;
    }
    public float getRightMostXPosition() {
        float maxXPos = UnityUtils.getPositionOfUIElement(renderedBlocks[0]).x;
        foreach (GameObject block in renderedBlocks) {
            float x = UnityUtils.getPositionOfUIElement(block).x;
            if (maxXPos < x)
                maxXPos = x;
        }
        return maxXPos;
    }
    public void rotate() {
        Vector3 center = centerOfRenderedBlocks();
        Utils.rotateSetForTimes(blocks, 1);
        Utils.centerToOrigin(blocks);
        rotateWithOrigin(center);
    }
    private void rotateWithOrigin(Vector3 center) {
        foreach (GameObject block in renderedBlocks) {
            Vector3 blockPosition = UnityUtils.getPositionOfUIElement(block);
            Vector3 distance = blockPosition - center;
            Vector3 newPosition = center + UnityUtils.rotateClockwiseAQuater(distance);
            UnityUtils.moveUIElementToPosition(block, newPosition);
        }
    }
    private Vector3 centerOfRenderedBlocks() {
        ///<summary>
        /// Coordinate와 실제 rendering된 position이 '동기화' 되어 있다고 가정한다.
        /// '동기화'는 실제 position을 보고 만든 Coordinate가 가지고 있는 Coordinate
        /// 와 같은 것을 뜻한다.
        ///</summary>
        Utils.centerToOrigin(blocks);
        Vector3 defalutPosition = UnityUtils.getPositionOfUIElement(renderedBlocks[0]);
        float x = defalutPosition.x, y = defalutPosition.y;
        for (int i = 0; i < blocks.Count; i++) {
            if (blocks[i].x == 0)
                x = UnityUtils.getPositionOfUIElement(renderedBlocks[i]).x;
            if (blocks[i].y == 0)
                y = UnityUtils.getPositionOfUIElement(renderedBlocks[i]).y;
        }
        return new Vector3(x, y);
    }
    public void moveFor(Vector3 distance) {
        for (int i = 0; i < renderedBlocks.Count; i++)
            UnityUtils.moveUIElementForDistance(renderedBlocks[i], distance);
    }
    public void reset() {
        // TODO
    }
    public float blockSize() {
        return UnityUtils.getWidthOfUIElement(renderedBlocks[0]);
    }
}

public class RenderedPuzzle {
    private List<List<GameObject>> renderedBlocks;
    private List<List<bool>> isOccupied;
    private float rangePerHalfBlockSize = 0.9f;

    public RenderedPuzzle(List<List<GameObject>> renderedBlocks) {
        this.renderedBlocks = renderedBlocks;
        initIsOccupiedAsLengthOfBlocks();
    }
    private void initIsOccupiedAsLengthOfBlocks() {
        isOccupied = new List<List<bool>>();
        for (int i = 0; i < renderedBlocks.Count; i++) {
            List<bool> subList = new List<bool>();
            for (int j = 0; j < renderedBlocks[i].Count; j++) {
                subList.Add(false);
            }
            isOccupied.Add(subList);
        }
    }
    // TODO: 끔찍함. 분리해야함
    private Vector3 delta;
    public Vector3 tryToInsertAndReturnDelta(List<Vector3> blockPositions) {
        if (fits(blockPositions)) {
            occupyBlocksAt(blockPositions);
            return delta;
        }
        return new Vector3(0, 0);
    }
    private bool fits(List<Vector3> blocks) {
        foreach (Vector3 block in blocks) {
            if ((System.Object)boardBlockIndexNearEnoughToFit(block, true) == null)
                return false;
        }
        return true;
    }
    private Coordinate boardBlockIndexNearEnoughToFit(Vector3 blockPosition, bool checkOccupied = false) {
        ///<summary>
        /// returns null if there's no such block
        ///</summary>
        for (int i = 0; i < renderedBlocks.Count; i++) {
            for (int j = 0; j < renderedBlocks[i].Count; j++) {
                if (checkOccupied && isOccupied[i][j])
                    continue;
                Vector3 position = UnityUtils.getPositionOfUIElement(renderedBlocks[i][j]);
                float range = (blockSize() / 2) * rangePerHalfBlockSize;
                if (Vector3.Distance(blockPosition, position) < range) {
                    delta = position - blockPosition;
                    return new Coordinate(i, j);
                }
            }
        }
        return null;
    }
    private float blockSize() {
        return UnityUtils.getWidthOfUIElement(renderedBlocks[0][0]);
    }
    private void occupyBlocksAt(List<Vector3> blocks) {
        // Tuple이 없어서 대신 Coordinate를 사용함
        List<Coordinate> indexes = fittedIndexes(blocks);
        foreach (Coordinate index in indexes)
            isOccupied[index.x][index.y] = true;
    }
    public void extract(List<Vector3> blocks) {
        List<Coordinate> indexes = fittedIndexes(blocks);
        foreach (Coordinate index in indexes)
            isOccupied[index.x][index.y] = false;
    }
    private List<Coordinate> fittedIndexes(List<Vector3> blocks) {
        List<Coordinate> ret = new List<Coordinate>();
        foreach (Vector3 block in blocks) {
            Coordinate index = boardBlockIndexNearEnoughToFit(block);
            if ((System.Object)index != null)
                ret.Add(index);
        }
        return ret;
    }
    public bool isSolved() {
        foreach (List<bool> subList in isOccupied)
            foreach (bool isBlockOccupied in subList)
                if (!isBlockOccupied)
                    return false;
        return true;
    }
}