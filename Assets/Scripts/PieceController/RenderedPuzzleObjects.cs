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
    protected List<GameObject> blocks;
    protected List<Coordinate> coors;

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
    public float blockSize() {
        return UnityUtils.getWidthOfUIElement(blocks[0]);
    }
    public void moveFor(Vector3 distance) {
        foreach (GameObject block in blocks)
            UnityUtils.moveUIElementForDistance(block, distance);
    }
}

public class RenderedPiece : StructuredPiece{
    Vector3 origin;

    public RenderedPiece(List<GameObject> rBlocks, List<Coordinate> blocks) 
        :base(rBlocks, blocks) { }
    public void rotate() {
        origin = centerPosition();
        rotateClockWiseAQuarterWithOriginPosition(origin);
    }
    public void reset() {
        // TODO
    }
    public bool includes(Vector3 point) {
        List<Vector3> blocks = getBlockPositions();
        float sideLength = blockSize();
        foreach(Vector3 block in blocks) {
            var square = new UnityUtils.Square(block, sideLength);
            if (square.includes(point))
                return true;
        }
        return false;
    }
}

public class RenderedPuzzle {
    private List<List<GameObject>> board;
    private List<List<bool>> isOccupied;
    private const float rangePerHalfBlockSize = 0.9f;

    public RenderedPuzzle(List<List<GameObject>> renderedBlocks) {
        board = renderedBlocks;
        initMaskAsLengthOfBlocks(ref isOccupied);
    }
    private void initMaskAsLengthOfBlocks(ref List<List<bool>> mask) {
        mask = new List<List<bool>>();
        for (int i = 0; i < board.Count; i++) {
            List<bool> subList = new List<bool>();
            for (int j = 0; j < board[i].Count; j++) {
                subList.Add(false);
            }
            mask.Add(subList);
        }
    }

    private class Comparer {
        // TODO: readonly로 바꾸기
        private List<List<GameObject>> board;
        private List<List<bool>> covered;
        private List<Vector3> target;
        private bool isTargetFits;
        private bool isTargetCovered;
        // Tuple이 없어서 대신 Coordinate를 사용함
        private List<Coordinate> fittedIndexes;
        private List<Coordinate> coveredIndexes;
        private Vector3 delta;
        
        public Comparer(List<List<GameObject>> board, List<List<bool>> covered, List<Vector3> target) {
            this.board = board;
            this.covered = covered;
            this.target = target;
            fittedIndexes = new List<Coordinate>();
            coveredIndexes = new List<Coordinate>();
            compare();
        }
        private void compare() {
            isTargetFits = true;
            isTargetCovered = true;
            foreach(Vector3 block in target) {
                compareOne(block);
            }
        }
        private void compareOne(Vector3 blockPosition) {
            for (int i = 0; i < board.Count; i++) {
                for (int j = 0; j < board[i].Count; j++) {
                    Vector3 center = UnityUtils.getPositionOfUIElement(board[i][j]);
                    float blockSize = UnityUtils.getWidthOfUIElement(board[i][j]);
                    float range = (blockSize / 2) * rangePerHalfBlockSize;
                    var rangeSquare = new UnityUtils.Square(center, range * 2);
                    if (rangeSquare.includes(blockPosition)) {
                        if (covered[i][j]) {
                            coveredIndexes.Add(new Coordinate(i, j));
                            isTargetFits = false;
                            return;
                        }
                        else {
                            delta = center - blockPosition;
                            fittedIndexes.Add(new Coordinate(i, j));
                            isTargetCovered = false;
                            return;
                        }
                    }
                }
            }
            isTargetFits = false;
            isTargetCovered = false;
        }
        public bool fits() { return isTargetFits; }
        public bool isCovered() { return isTargetCovered; }
        public Vector3 getDelta() { return isTargetFits? delta : new Vector3(0, 0); }
        public List<Coordinate> getFittedIndexes() {
            if(fits())
                return fittedIndexes;
            else
                return new List<Coordinate>();
        }
        public List<Coordinate> getCoveredIndexes() {
            if (isCovered())
                return coveredIndexes;
            else
                return new List<Coordinate>();
        }
    }

    public Vector3 tryToInsertAndReturnDelta(List<Vector3> blockPositions) {
        Comparer comp = new Comparer(board, isOccupied, blockPositions);
        if (comp.fits()) {
            occupyBlocksAt(comp.getFittedIndexes());
        }
        return comp.getDelta();
    }
    private void occupyBlocksAt(List<Coordinate> indexes) {
        foreach (Coordinate index in indexes)
            isOccupied[index.x][index.y] = true;
    }
    public void tryToExtract(List<Vector3> blockPositions) {
        Comparer comp = new Comparer(board, isOccupied, blockPositions);
        if(comp.isCovered())
            releaseBlocksAt(comp.getCoveredIndexes());
    }
    private void releaseBlocksAt(List<Coordinate> indexes) {
        foreach (Coordinate index in indexes)
            isOccupied[index.x][index.y] = false;
    }
    public bool isSolved() {
        foreach (List<bool> subList in isOccupied)
            foreach (bool isBlockOccupied in subList)
                if (!isBlockOccupied)
                    return false;
        return true;
    }
    private void debugOccupied() {
        Debug.Log("start====================");
        foreach (List<bool> a in isOccupied)
            foreach (bool b in a)
                Debug.Log(b);
        Debug.Log("end====================");
    }
}