using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Penta;

public enum InsertionResult { CORRECT, WRONG, MISS }

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
    public float getDestroyAnimationTimeInSecond()
    {
        // Because the destroy time of puzzle and candidates are the same.
        return candidates[0].getDestroyAnimationTimeInSecond();
    }
}

public class StructuredPiece {
    protected List<Block> blocks;
    protected List<Coordinate> coors;

    public StructuredPiece(List<Block> blocks, List<Coordinate> coors) {
        this.blocks = blocks;
        this.coors = coors;
    }
    public Vector2 centerAnchoredPosition() {
        Utils.centerToOrigin(coors);
        return getOriginAnchoredPosition();
    }
    public Vector2 getBottomAnchoredPosition()
    {
        Coordinate centerBottomCoors = Utils.getCenterBottom(coors);
        return getAnchoredPositionOf(centerBottomCoors);
    }
    // origin means (0,0) in coordinate
    public Vector2 getOriginAnchoredPosition() {
        return getAnchoredPositionOf(new Coordinate(0, 0));
    }
    private Vector2 getAnchoredPositionOf(Coordinate coor)
    {
        Vector2 defalutPosition = blocks[0].getAnchoredPosition();
        float x = defalutPosition.x, y = defalutPosition.y;
        for (int i = 0; i < coors.Count; i++)
        {
            if (coors[i].x == coor.x)
                x = blocks[i].getAnchoredPosition().x;
            if (coors[i].y == coor.y)
                y = blocks[i].getAnchoredPosition().y;
        }
        return new Vector2(x, y);

    }
    public void rotateClockWiseAQuarterWithPivotAnchoredPosition(Vector2 pivotPosition) {
        Utils.rotateSetForTimes(coors, 1);
        rotateGameObjectsWithPivotAnchoredPosition(pivotPosition);
    }
    private void rotateGameObjectsWithPivotAnchoredPosition(Vector2 pivotPosition) {
        foreach (Block block in blocks) {
            Vector2 blockPosition = block.getAnchoredPosition();
            Vector2 distance = blockPosition - pivotPosition;
            Vector2 newPosition = pivotPosition + UnityUtils.rotateClockwiseAQuater(distance);
            block.moveToAnchoredPosition(newPosition);
        }
    }
    public List<Vector2> getBlockAnchoredPositions() {
        List<Vector2> ret = new List<Vector2>();
        foreach (Block block in blocks) {
            Vector3 newPosition = block.getAnchoredPosition();
            ret.Add(newPosition);
        }
        return ret;
    }
    public float getRightMostXAnchoredPosition() {
        float maxXPos = blocks[0].getAnchoredPosition().x; 
        foreach (Block block in blocks) {
            float x = block.getAnchoredPosition().x;
            if (maxXPos < x)
                maxXPos = x;
        }
        return maxXPos;
    }
    public float blockSize() {
        // TODO: WHAT block size?
        // assumes the block to be square
        return blocks[0].getWidth();
    }
    public void moveForAnchoredVector(Vector2 deltaPosition) {
        foreach (Block block in blocks)
            block.moveForAnchoredVector(deltaPosition);
    }
    public void scale(float factor)
    {
        Vector2 pivotPosition = getOriginAnchoredPosition();
        foreach (Block block in blocks)
        {
            Vector2 blockPosition = block.getAnchoredPosition();
            block.moveToAnchoredPosition(pivotPosition + factor * (blockPosition - pivotPosition));
            block.scale(factor);
        }
    }
    public float getDestroyAnimationTimeInSecond()
    {
        // TODO
        return 1.0f;
    }
}

public class RenderedPiece : StructuredPiece{
    private Vector3 centerOriginalAnchoredPosition;
    // The renderedPiece is either candidate size or boardFit size.
    private bool isCandidateSize = true;
    private readonly float candidateBoardBlockSizeRatio = 2.0f;

    public RenderedPiece(List<Block> rBlocks, List<Coordinate> blocks) 
        :base(rBlocks, blocks) {
        centerOriginalAnchoredPosition = centerAnchoredPosition();
    }
    public void rotate() {
        rotateClockWiseAQuarterWithPivotAnchoredPosition(centerAnchoredPosition());
    }
    public void reset() {
        Vector3 currentOriginAnchoredPosition = centerAnchoredPosition();
        moveForAnchoredVector(centerOriginalAnchoredPosition - currentOriginAnchoredPosition);
    }
    public bool includes(Vector2 point) {
        List<Vector2> blockAnchoredPositions = getBlockAnchoredPositions();
        float sideLength = blockSize();
        foreach(Vector2 blockAnchoredPosition in blockAnchoredPositions) {
            var square = new UnityUtils.Square(blockAnchoredPosition, sideLength);
            if (square.includes(point))
                return true;
        }
        return false;
    }
    public bool includesInNeighborhood(Vector2 point, float padding) {
        // If padding=0, the neighborhood means the envelope covering the piece.
        // {neighborhood width} = {envelope width} + 2*padding
        // (similar for height)
        List<Vector2> blockAnchoredPositions = getBlockAnchoredPositions();
        float sideLength = blockSize();
        float left=100000, right=-100000, top=-100000, bottom=100000;
        foreach (Vector2 blockAnchoredPosition in blockAnchoredPositions) {
            float x = blockAnchoredPosition.x;
            if (x < left)
                left = x;
            if (x > right)
                right = x;
            float y = blockAnchoredPosition.y;
            if (y > top)
                top = y;
            if (y < bottom)
                bottom = y;
        }
        var rect = new UnityUtils.Rectangle(new Vector2((left+right)/2, (top+bottom)/2), right-left+sideLength+2*padding, top-bottom+sideLength+2*padding);
        return rect.includes(point);

    }
    public void destroy() {
        foreach(Block block in blocks) {
            block.destroy();
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
    private List<List<Block>> board;
    private List<List<BackgroundBlock>> background;
    private List<List<bool>> isOccupied;
    private List<List<bool>> isHighlighted;
    private const float rangePerHalfBlockSize = 0.9999f;
    private bool recentInsertionSuccess;
    private InsertionResult recentInsertionResult;
    private BoardComparer lastInsertionComparer;

    // precondition: board is aligned as List of pieces. That is, List of List of GameObject = List of pieces.
    public RenderedPuzzle(List<List<Block>> board, List<List<BackgroundBlock>> background) {
        this.board = board;
        this.background = background;
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

    private class BoardComparer {
        // TODO: readonly로 바꾸기
        private List<List<Block>> board;
        private List<List<bool>> isInvalid;
        private List<Vector2> target;
        private bool isTargetFits;
        // Tuple이 없어서 대신 Coordinate를 사용함
        private List<Coordinate> fittedIndexes;
        private Vector2 deltaAnchoredPosition;
        
        public BoardComparer(List<List<Block>> board, List<List<bool>> isInvalid, List<Vector2> target) {
            this.board = board;
            this.isInvalid = isInvalid;
            this.target = target;
            fittedIndexes = new List<Coordinate>();
            compare();
        }
        private void compare() {
            isTargetFits = true;
            foreach(Vector3 blockPosition in target) {
                compareOne(blockPosition);
            }
        }
        private void compareOne(Vector2 blockAnchoredPosition) {
            // Precondition: Anchor positions should be the same.
            for (int i = 0; i < board.Count; i++) {
                for (int j = 0; j < board[i].Count; j++) {
                    Vector2 centerAnchoredPosition = board[i][j].getAnchoredPosition();
                    float blockSize = board[i][j].getWidth();
                    float range = (blockSize / 2) * rangePerHalfBlockSize;
                    var rangeSquare = new UnityUtils.Square(centerAnchoredPosition, range * 2);
                    if (rangeSquare.includes(blockAnchoredPosition)) {
                        if (isInvalid[i][j]) {
                            isTargetFits = false;
                            return;
                        }
                        else {
                            deltaAnchoredPosition = centerAnchoredPosition - blockAnchoredPosition;
                            fittedIndexes.Add(new Coordinate(i, j));
                            return;
                        }
                    }
                }
            }
            isTargetFits = false;
        }
        public bool fits() { return isTargetFits; }
        public Vector2 getDelta() { return isTargetFits? deltaAnchoredPosition : new Vector2(0, 0); }
        public List<Coordinate> getPartiallyFittedIndexes() { return fittedIndexes; }
    }

    public void highlightClosestBlocks(List<Vector2> blockAnchoredPositions)
    {
        BoardComparer comp = new BoardComparer(board, isOccupied, blockAnchoredPositions);
        resetBlockColors();
        highlightBlocksAt(comp.getPartiallyFittedIndexes());
    }
    private void highlightBlocksAt(List<Coordinate> indexes)
    {
        foreach (Coordinate index in indexes)
        {
            board[index.x][index.y].setColor(Color.green);
        }
    }
    public void resetBlockColors()
    {
        foreach(List<Block> blockList in board)
        {
            foreach(Block block in blockList)
            {
                block.resetColor();
            }
        }
    }

    public Vector3 tryToInsertAndReturnDelta(List<Vector2> blockAnchoredPositions) {
        lastInsertionComparer = new BoardComparer(board, isOccupied, blockAnchoredPositions);
        if (lastInsertionComparer.fits()) {
            List<Coordinate> partiallyFittedIndexes = lastInsertionComparer.getPartiallyFittedIndexes();
            occupyBlocksAt(partiallyFittedIndexes);
            recentInsertionSuccess = true;
            if (Utils.isXConsistent(partiallyFittedIndexes))
            {
                recentInsertionResult = InsertionResult.CORRECT;
                playBlinking(partiallyFittedIndexes);
            }
            else
            {
                recentInsertionResult = InsertionResult.WRONG;
            }
        }
        else
        {
            recentInsertionResult = InsertionResult.MISS;
            recentInsertionSuccess = false;
        }
        return lastInsertionComparer.getDelta();
    }
    private void playBlinking(List<Coordinate> blinkTargetCoordinates)
    {
        foreach(Coordinate coor in blinkTargetCoordinates)
        {
            background[coor.x][coor.y].blink();
        }
    }
    private void occupyBlocksAt(List<Coordinate> indexes) {
        foreach (Coordinate index in indexes)
            isOccupied[index.x][index.y] = true;
    }
    public bool tryToExtract(List<Vector2> blockAnchoredPositions) {
        // postcondition: extract fail iff the piece is in the correct position
        List<List<bool>> isNotOccupied = new List<List<bool>>();
        foreach(List<bool> subList in isOccupied)
        {
            List<bool> newList = new List<bool>();
            foreach (bool isBlockOccupied in subList)
                newList.Add(!isBlockOccupied);
            isNotOccupied.Add(newList);
        }
        BoardComparer comp = new BoardComparer(board, isNotOccupied, blockAnchoredPositions);
        if (comp.fits())
        {
            if (Utils.isXConsistent(comp.getPartiallyFittedIndexes()))
            {
                return false;
            }
            else
            {
                releaseBlocksAt(comp.getPartiallyFittedIndexes());
                return true;
            }
        }
        else
            return true;
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
        foreach (List<BackgroundBlock> subList in background)
            foreach (BackgroundBlock block in subList)
                block.destroy();
    }
    private void destroyBoard()
    {
        foreach (List<Block> subList in board)
            foreach (Block block in subList)
                block.destroy();
    }
    public bool getRecentInsertionSuccess() {
        return recentInsertionSuccess;
    }
    public InsertionResult lastInsertionResult()
    {
        return recentInsertionResult;
    }
    public Vector2 topOfLastInsertedAnchoredPosition()
    {
        List<Coordinate> fittedIndexes = lastInsertionComparer.getPartiallyFittedIndexes();
        float max_x = float.NegativeInfinity, min_x = float.PositiveInfinity, max_y = float.NegativeInfinity;
        foreach(Coordinate index in fittedIndexes)
        {
            Vector2 pos = board[index.x][index.y].getAnchoredPosition();
            if (max_x < pos.x)
                max_x = pos.x;
            if (min_x > pos.x)
                min_x = pos.x;
            if (max_y < pos.y)
                max_y = pos.y;
        }
        return new Vector2((min_x + max_x) / 2, max_y);
    }
}