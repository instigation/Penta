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

public class StructuredPiece {
    private List<GameObject> blocks;
    private List<Coordinate> coors;

    public StructuredPiece(List<GameObject> blocks, List<Coordinate> coors) {
        this.blocks = blocks;
        this.coors = coors;
    }
    public Vector3 centerPosition() {
        Utils.centerToOrigin(coors);
        return getOriginPosition();
    }
    public Vector3 leftMostPosition() {
        Utils.leftMostToOrigin(coors);
        return getOriginPosition();
    }
    private Vector3 getOriginPosition() {
        List<Vector3> blockPositions = UnityUtils.getPositionsOfUIElements(blocks);
        Vector3 defalutPosition = UnityUtils.getPositionOfUIElement(blocks[0]);
        float x = defalutPosition.x, y = defalutPosition.y;
        for (int i = 0; i < coors.Count; i++) {
            if (coors[i].x == 0)
                x = UnityUtils.getPositionOfUIElement(blocks[i]).x;
            if (coors[i].y == 0)
                y = UnityUtils.getPositionOfUIElement(blocks[i]).y;
        }
        return new Vector3(x, y);
    }
    public void rotateClockWiseAQuarterWithOriginPosition(Vector3 originPosition) {
        Utils.rotateSetForTimes(coors, 1);
        rotateGameObjectsWithOriginPosition(originPosition);
    }
    private void rotateGameObjectsWithOriginPosition(Vector3 originPosition) {
        foreach (GameObject block in blocks) {
            Vector3 blockPosition = UnityUtils.getPositionOfUIElement(block);
            Vector3 distance = blockPosition - originPosition;
            Vector3 newPosition = originPosition + UnityUtils.rotateClockwiseAQuater(distance);
            UnityUtils.moveUIElementToPosition(block, newPosition);
        }
    }
    public List<Vector3> getBlockPositions() {
        List<Vector3> ret = new List<Vector3>();
        foreach (GameObject block in blocks) {
            Vector3 newPosition = UnityUtils.getPositionOfUIElement(block);
            ret.Add(newPosition);
        }
        return ret;
    }
    public float getRightMostXPosition() {
        float maxXPos = UnityUtils.getPositionOfUIElement(blocks[0]).x;
        foreach (GameObject block in blocks) {
            float x = UnityUtils.getPositionOfUIElement(block).x;
            if (maxXPos < x)
                maxXPos = x;
        }
        return maxXPos;
    }
    public float getBlockSize() {
        return UnityUtils.getWidthOfUIElement(blocks[0]);
    }
    public void moveFor(Vector3 distance) {
        foreach (GameObject block in blocks)
            UnityUtils.moveUIElementForDistance(block, distance);
    }
}

public abstract class OrientedPiece {
    protected Vector3 origin;
    protected StructuredPiece piece;
    public OrientedPiece(StructuredPiece piece) {
        this.piece = piece;
    }
    public void rotateClockWiseAQuarter() {
        piece.rotateClockWiseAQuarterWithOriginPosition(origin);
    }
}
public class CenterOrientedPiece : OrientedPiece {
    public CenterOrientedPiece(StructuredPiece piece) : base(piece) {
        origin = piece.centerPosition();
    }
}
public class LeftMostOrientedPiece : OrientedPiece {
    public LeftMostOrientedPiece(StructuredPiece piece) : base(piece) {
        origin = piece.leftMostPosition();
    }
}

public class RenderedPiece {
    private StructuredPiece structuredBlock;

    public RenderedPiece(List<GameObject> rBlocks, List<Coordinate> blocks) {
        structuredBlock = new StructuredPiece(rBlocks, blocks);
    }
    public List<Vector3> getBlockPositions() {
        return structuredBlock.getBlockPositions();
    }
    public float getRightMostXPosition() {
        return structuredBlock.getRightMostXPosition();
    }
    public void rotate() {
        CenterOrientedPiece coPiece = new CenterOrientedPiece(structuredBlock);
        coPiece.rotateClockWiseAQuarter();
    }
    public void moveFor(Vector3 distance) {
        structuredBlock.moveFor(distance);
    }
    public void reset() {
        // TODO
    }
    public float blockSize() {
        return structuredBlock.getBlockSize();
    }
}

public class RenderedPuzzle {
    private List<List<GameObject>> renderedBlocks;
    private List<List<bool>> isOccupied;
    private const float rangePerHalfBlockSize = 0.9f;

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