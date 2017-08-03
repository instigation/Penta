using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSetRenderer : MonoBehaviour {
    /// <summary>
    /// blocks나 boundary는 Unity editor에서 편하게 할당하기 위해 public으로 했지만
    /// 코드에서는 쓰지 않기 위해 python처럼 이름 앞에 __를 붙이고 쓰지 않기로 약속함.
    /// </summary>
    public GameObject __boardBlock;
    public GameObject __boardBlockBackground;
    public GameObject[] __pieceBlocks;
    public float __gapPerBlockSize;
    public GameObject __centerOfBoard;
    public GameObject __candidateLeftMostBlockSpawnPoint;
    public float __gapBtwBlocks;
    public bool __randomizeRotation = false;

    public RenderedPuzzleSet render(Puzzle p) {
        ///<summary>
        /// 순서가 이걸 해결하는지는 아직 확실하지 않지만
        /// board가 candidate보다 밑에 있어야 함.
        /// i.e. candidate가 board를 덮어야 해서
        /// board를 먼저 render함.
        ///</summary>
        RenderedPuzzle board = renderPuzzle(p);
        List<RenderedPiece> candidates = renderCandidates(p);
        return new RenderedPuzzleSet(candidates, board);
    }
    private List<RenderedPiece> renderCandidates(Puzzle p) {
        // 여기서 deep copy된 걸로 받아야 함.
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        List<RenderedPiece> ret = new List<RenderedPiece>();
        for (int i = 0; i < piecesInCoordinate.Count; i++) {
            List<Coordinate> pieceInCoordinate = piecesInCoordinate[i];
            if (__randomizeRotation)
                Utils.rotateRandomly(pieceInCoordinate);
            Utils.leftMostToOrigin(pieceInCoordinate);
            PieceRenderer renderer = new PieceRenderer(__pieceBlocks[i], __gapPerBlockSize);
            RenderedPiece renderedPiece = renderer.render(pieceInCoordinate, __candidateLeftMostBlockSpawnPoint);
            moveSpawnPointToRightEndOf(renderedPiece);
            ret.Add(renderedPiece);
        }
        return ret;
    }
    private void moveSpawnPointToRightEndOf(RenderedPiece renderedPiece) {
        Vector3 rightMostPosition = UnityUtils.getPositionOfUIElement(__candidateLeftMostBlockSpawnPoint);
        rightMostPosition.x = renderedPiece.getRightMostXPosition() + __gapBtwBlocks;
        UnityUtils.moveUIElementToPosition(__candidateLeftMostBlockSpawnPoint, rightMostPosition);
    }
    private RenderedPuzzle renderPuzzle(Puzzle p) {
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        Utils.centerToOrigin(piecesInCoordinate);
        PuzzleRenderer renderer = new PuzzleRenderer(__boardBlock, __boardBlockBackground, __pieceBlocks, __gapPerBlockSize);
        RenderedPuzzle ret = renderer.render(piecesInCoordinate, __centerOfBoard);
        return ret;
    }
}

public class RenderedPuzzleSet {
    public List<RenderedPiece> candidates;
    public RenderedPuzzle board;

    public RenderedPuzzleSet(List<RenderedPiece> candidates, RenderedPuzzle board) {
        this.candidates = candidates;
        this.board = board;
    }
}

public class PieceRenderer {
    private GameObject pieceBlock;
    private float gapPerBlockSize;
    private Vector3 leftMostPosition;

    public PieceRenderer(GameObject pieceBlock, float gapPerBlockSize) {
        this.pieceBlock = pieceBlock;
        this.gapPerBlockSize = gapPerBlockSize;
    }
    public RenderedPiece render(List<Coordinate> blocks, GameObject leftMost) {
        leftMostPosition = UnityUtils.getPositionOfUIElement(leftMost);
        return renderBlocksAtOrigin(blocks, leftMostPosition);
    }
    private RenderedPiece renderBlocksAtOrigin(List<Coordinate> blocks, Vector3 originPosition) {
        ///<remarks>
        /// origin means (0,0) in Coordinates
        ///</remarks>
        List<GameObject> renderedBlocks = new List<GameObject>();
        float distance = blockSize() * (1 + gapPerBlockSize);
        foreach (Coordinate block in blocks) {
            Vector3 position = originPosition + UnityUtils.toVector3(block) * distance;
            GameObject renderedBlock = renderOneBlockAt(position);
            renderedBlocks.Add(renderedBlock);
        }
        return new RenderedPiece(renderedBlocks, blocks);
    }
    private float blockSize() {
        return UnityUtils.getWidthOfUIElement(pieceBlock);
    }
    private GameObject renderOneBlockAt(Vector3 position) {
        return MonoBehaviourUtils.renderBlockWithPosition(pieceBlock, position);
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
public class PuzzleRenderer {
    private GameObject boardBlock;
    private GameObject boardBlockBackground;
    private GameObject[] pieceBlocks;
    private float gapPerBlockSize;

    public PuzzleRenderer(GameObject boardBlock, GameObject boardBlockBackground,
        GameObject[] pieceBlocks, float gapPerBlockSize) {
        this.boardBlock = boardBlock;
        this.boardBlockBackground = boardBlockBackground;
        this.pieceBlocks = pieceBlocks;
        this.gapPerBlockSize = gapPerBlockSize;
    }
    public RenderedPuzzle render(List<List<Coordinate>> pieces, GameObject centerOfBoard) {
        List<List<GameObject>> renderedBlocks = new List<List<GameObject>>();
        float distance = blockSize() * (1 + gapPerBlockSize);
        Vector3 centerPosition = UnityUtils.getPositionOfUIElement(centerOfBoard);
        foreach (List<Coordinate> piece in pieces) {
            foreach (Coordinate block in piece) {
                Vector3 position = centerPosition + distance * UnityUtils.toVector3(block);
                renderBackgroundBlockAt(position);
            }
        }
        foreach (List<Coordinate> piece in pieces) {
            List<GameObject> newPiece = new List<GameObject>();
            foreach (Coordinate block in piece) {
                Vector3 position = centerPosition + distance * UnityUtils.toVector3(block);
                GameObject renderedBlock = renderBlockAt(position);
                newPiece.Add(renderedBlock);
            }
            renderedBlocks.Add(newPiece);
        }
        return new RenderedPuzzle(renderedBlocks);
    }
    private float blockSize() {
        return UnityUtils.getWidthOfUIElement(boardBlock);
    }
    private void renderBackgroundBlockAt(Vector3 position) {
        MonoBehaviourUtils.renderBlockWithPosition(boardBlockBackground, position);
    }
    private GameObject renderBlockAt(Vector3 position) {
        return MonoBehaviourUtils.renderBlockWithPosition(boardBlock, position);
    }
    public void showAns() {
        // 정답을 rendering하게 되면 정답이 조각보다 위에 위치해서
        // 조각을 끌어다 놔도 정답 밑에 깔리게 되는 이슈를 유의하자.
    }
}