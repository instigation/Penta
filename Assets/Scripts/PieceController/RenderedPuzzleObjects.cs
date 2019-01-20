using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderedPuzzleSet {
    public List<RenderedPiece> candidates;
    public RenderedPuzzle board;

    public RenderedPuzzleSet(List<RenderedPiece> candidates, RenderedPuzzle board) {
        this.candidates = candidates;
        this.board = board;
    }
    public void destroy() {
        foreach (RenderedPiece candidate in candidates)
            candidate.destroy();
        board.destroy();
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
    // origin means (0,0) in coordinate
    public Vector3 getOriginPosition() {
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
    public void rotateClockWiseAQuarterWithPivotPosition(Vector3 pivotPosition) {
        Utils.rotateSetForTimes(coors, 1);
        rotateGameObjectsWithPivotPosition(pivotPosition);
    }
    private void rotateGameObjectsWithPivotPosition(Vector3 pivotPosition) {
        foreach (GameObject block in blocks) {
            Vector3 blockPosition = UnityUtils.getPositionOfUIElement(block);
            Vector3 distance = blockPosition - pivotPosition;
            Vector3 newPosition = pivotPosition + UnityUtils.rotateClockwiseAQuater(distance);
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
        // assumes the block to be square
        return UnityUtils.getWidthOfUIElement(blocks[0]);
    }
    public void moveFor(Vector3 distance) {
        foreach (GameObject block in blocks)
            UnityUtils.moveUIElementForDistance(block, distance);
    }
    public void scale(float factor)
    {
        Vector3 pivotPosition = getOriginPosition();
        foreach (GameObject block in blocks)
        {
            Vector3 blockPosition = UnityUtils.getPositionOfUIElement(block);
            UnityUtils.moveUIElementToPosition(block, pivotPosition + factor*(blockPosition - pivotPosition));
            UnityUtils.scaleSizeOfUIElement(block, factor);
        }
    }
}

public class RenderedPiece : StructuredPiece{
    private Vector3 centerOriginalPosition;
    // The renderedPiece is either candidate size or boardFit size.
    private bool isCandidateSize = true;
    private readonly float candidateBoardBlockSizeRatio = 2.0f;

    public RenderedPiece(List<GameObject> rBlocks, List<Coordinate> blocks) 
        :base(rBlocks, blocks) {
        centerOriginalPosition = centerPosition();
    }
    public void rotate() {
        rotateClockWiseAQuarterWithPivotPosition(centerPosition());
    }
    public void reset() {
        Vector3 currentOriginPosition = centerPosition();
        moveFor(centerOriginalPosition - currentOriginPosition);
    }
    public bool includes(Vector3 point) {
        List<Vector3> blocks = getBlockPositions();
        Debug.Log("includes");
        Debug.Log(blocks[0]);
        float sideLength = blockSize();
        foreach(Vector3 block in blocks) {
            var square = new UnityUtils.Square(block, sideLength);
            if (square.includes(point))
                return true;
        }
        return false;
    }
    public bool includesInNeighborhood(Vector3 point, float padding) {
        // If padding=0, the neighborhood means the envelope covering the piece.
        // {neighborhood width} = {envelope width} + 2*padding
        // (similar for height)
        List<Vector3> blocks = getBlockPositions();
        Debug.Log("includesInNeighborhood");
        Debug.Log(blocks[0]);
        float sideLength = blockSize();
        Debug.Log(sideLength);
        float left=100000, right=-100000, top=-100000, bottom=100000;
        foreach (Vector3 block in blocks) {
            float x = block.x;
            if (x < left)
                left = x;
            if (x > right)
                right = x;
            float y = block.y;
            if (y > top)
                top = y;
            if (y < bottom)
                bottom = y;
        }
        var rect = new UnityUtils.Rectangle(new Vector3((left+right)/2, (top+bottom)/2, 0), right-left+sideLength+2*padding, top-bottom+sideLength+2*padding);
        return rect.includes(point);

    }
    public void destroy() {
        foreach(GameObject block in blocks) {
            Object.Destroy(block);
        }
    }
    public void setToCandidateSize()
    {
        if (!isCandidateSize)
        {
            scale(1 / candidateBoardBlockSizeRatio);
            isCandidateSize = true;
        }
    }
    public void setToBoardFitSize()
    {
        if (isCandidateSize)
        {
            scale(candidateBoardBlockSizeRatio);
            isCandidateSize = false;
        }
    }
}

public class RenderedPuzzle {
    private List<List<GameObject>> board;
    private List<List<GameObject>> background;
    private List<List<bool>> isOccupied;
    private List<List<bool>> isHighlighted;
    private Color originalBoardColor;
    private const float rangePerHalfBlockSize = 0.9999f;
    private bool recentInsertionSuccess;

    public RenderedPuzzle(List<List<GameObject>> board, List<List<GameObject>> background) {
        this.board = board;
        this.background = background;
        initMaskAsLengthOfBlocks(ref isOccupied);
        foreach (List<GameObject> blockList in board)
            foreach (GameObject block in blockList)
                originalBoardColor = block.GetComponent<Image>().color;
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
        public List<Coordinate> getPartiallyFittedIndexes() {
            return fittedIndexes;
        }
        public List<Coordinate> getCoveredIndexes() {
            if (isCovered())
                return coveredIndexes;
            else
                return new List<Coordinate>();
        }
    }

    public void highlightClosestBlocks(List<Vector3> blockPositions)
    {
        Comparer comp = new Comparer(board, isOccupied, blockPositions);
        resetBlockColors();
        highlightBlocksAt(comp.getPartiallyFittedIndexes());
    }
    private void highlightBlocksAt(List<Coordinate> indexes)
    {
        foreach (Coordinate index in indexes)
        {
            board[index.x][index.y].GetComponent<Image>().color = Color.green;
        }
    }
    public void resetBlockColors()
    {
        foreach(List<GameObject> blockList in board)
        {
            foreach(GameObject block in blockList)
            {
                block.GetComponent<Image>().color = originalBoardColor;
            }
        }
    }

    public Vector3 tryToInsertAndReturnDelta(List<Vector3> blockPositions) {
        Comparer comp = new Comparer(board, isOccupied, blockPositions);
        if (comp.fits()) {
            occupyBlocksAt(comp.getPartiallyFittedIndexes());
            recentInsertionSuccess = true;
        }
        else
            recentInsertionSuccess = false;
        return comp.getDelta();
    }
    private void occupyBlocksAt(List<Coordinate> indexes) {
        foreach (Coordinate index in indexes)
            isOccupied[index.x][index.y] = true;
    }
    public bool tryToExtract(List<Vector3> blockPositions) {
        Comparer comp = new Comparer(board, isOccupied, blockPositions);
        if (comp.isCovered())
        {
            releaseBlocksAt(comp.getCoveredIndexes());
            return true;
        }
        else
            return false;
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
    public void destroy() {
        destroyBackground();
        destroyBoard();
    }
    private void destroyBackground() {
        destroyDoubleListedGameObject(background);
    }
    private void destroyBoard() {
        destroyDoubleListedGameObject(board);
    }
    private void destroyDoubleListedGameObject(List<List<GameObject>> target) {
        foreach (List<GameObject> subList in target)
            foreach (GameObject element in subList)
                Object.Destroy(element);
    }
    public bool getRecentInsertionSuccess() {
        return recentInsertionSuccess;
    }
}