using System.Collections.Generic;
using UnityEngine;

public class PuzzleRenderer : MonoBehaviour {
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
    private RenderedPuzzleSet holdings = null;

    public void testing() {
        ///<summary>
        /// Only for testing purpose.
        /// 나중에는 Renderer는 render(Puzzle) 함수만 제공하고
        /// stage controller가 render 함수를 invoke하게 할 예정.
        ///</summary>
        if (holdings != null)
            holdings.destroy();
        PuzzleGenerator pg = new PuzzleGenerator(3, Difficulty.HARD);
        Puzzle p = pg.generatePuzzle();
        List<List<Coordinate>> ret = p.getBlocks();
        holdings = render(p);
    }

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
        for(int i = 0; i < piecesInCoordinate.Count; i++) {
            List<Coordinate> pieceInCoordinate = piecesInCoordinate[i];
            Utils.leftMostToOrigin(pieceInCoordinate);
            if (__randomizeRotation)
                Utils.rotateRandomly(pieceInCoordinate);
            // 실제 rendering은 RenderedPiece의 생성자에서.
            RenderedPiece renderedPiece = new RenderedPiece(pieceInCoordinate, __pieceBlocks[i], __gapPerBlockSize, __candidateLeftMostBlockSpawnPoint);
            moveSpawnPointToRightEndOf(renderedPiece);
            ret.Add(renderedPiece);
        }
        return ret;
    }
    private void moveSpawnPointToRightEndOf(RenderedPiece renderedPiece) {
        Vector3 rightMostPosition = UnityUtils.getPositionOfUIElement(__candidateLeftMostBlockSpawnPoint);
        rightMostPosition.x = renderedPiece.getRightMostXPosition() + __gapBtwBlocks;
        UnityUtils.moveGameObjectToPosition(__candidateLeftMostBlockSpawnPoint, rightMostPosition);
    }
    private RenderedPuzzle renderPuzzle(Puzzle p) {
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        Utils.centerToOrigin(piecesInCoordinate);
        RenderedPuzzle ret = new RenderedPuzzle(piecesInCoordinate, __boardBlock, __boardBlockBackground, __pieceBlocks, __gapPerBlockSize, __centerOfBoard);
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
    public void destroy() {
        foreach (RenderedPiece candidate in candidates)
            candidate.destroy();
        board.destroy();
    }
}

public class RenderedPiece {
    List<Coordinate> blocks;
    GameObject pieceBlock;
    float gapPerBlockSize;
    GameObject leftMost;
    List<GameObject> renderedBlocks;

    public RenderedPiece(List<Coordinate> pieceInCoordinate, GameObject pieceBlock, float gapPerBlockSize, GameObject leftMost) {
        blocks = pieceInCoordinate;
        this.pieceBlock = pieceBlock;
        this.gapPerBlockSize = gapPerBlockSize;
        this.leftMost = leftMost;
        renderedBlocks = new List<GameObject>();

        renderAtAbsolutePosition();
    }
    public float getRightMostXPosition() {
        float maxXPos = UnityUtils.getPositionOfUIElement(renderedBlocks[0]).x;
        foreach(GameObject block in renderedBlocks) {
            float x = UnityUtils.getPositionOfUIElement(block).x;
            if (maxXPos < x)
                maxXPos = x;
        }
        return maxXPos;
    }
    public float width() {
        float widthNum = (float)Utils.getWidth(blocks);
        return (widthNum + (widthNum - 1) * gapPerBlockSize) * blockSize();
    }
    private void renderAtAbsolutePosition() {
        float distance = blockSize() * (1 + gapPerBlockSize);
        Vector3 leftMostPosition = leftMost.transform.position;
        foreach(Coordinate block in blocks) {
            Vector3 position = leftMostPosition + UnityUtils.toVector3(block) * distance;
            GameObject renderedBlock = renderOneBlockAt(position);
            renderedBlocks.Add(renderedBlock);
        }
    }
    private float blockSize() {
        return UnityUtils.getWidthOfUIElement(pieceBlock);
    }
    private GameObject renderOneBlockAt(Vector3 position) {
        return MonoBehaviourUtils.renderBlockWithPosition(pieceBlock, position);
    }
    public void destroy() {
        foreach (GameObject renderedBlock in renderedBlocks)
            Object.Destroy(renderedBlock);
    }
    /*
    public void rotate();
    public void move(Vector3);
    public void resetPosition();
    public void reset();
    */
}

public class RenderedPuzzle {
    private List<List<Coordinate>> pieces;
    private GameObject boardBlock;
    private GameObject boardBlockBackground;
    private GameObject[] pieceBlocks;
    private float gapPerBlockSize;
    private GameObject centerOfBoard;
    private List<List<GameObject>> renderedBlocks;

    public RenderedPuzzle(List<List<Coordinate>> piecesInCoordinate, GameObject boardBlock, GameObject boardBlockBackground, 
        GameObject[] pieceBlocks, float gapPerBlockSize, GameObject centerOfBoard) {
        pieces = piecesInCoordinate;
        this.boardBlock = boardBlock;
        this.boardBlockBackground = boardBlockBackground;
        this.pieceBlocks = pieceBlocks;
        this.gapPerBlockSize = gapPerBlockSize;
        this.centerOfBoard = centerOfBoard;
        renderedBlocks = new List<List<GameObject>>();

        render();
    }
    private void render() {
        float distance = blockSize() * (1 + gapPerBlockSize);
        Vector3 centerPosition = centerOfBoard.transform.position;
        foreach(List<Coordinate> piece in pieces) {
            foreach(Coordinate block in piece) {
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
    public void destroy() {
        foreach(List<GameObject> subList in renderedBlocks)
            foreach(GameObject renderedBlock in subList)
                Object.Destroy(renderedBlock);
    }
    /*
    public void tryToInsert(RenderedPiece);
    public void extract(RenderedPiece);
    public bool isSolved();
    public void showAns();
    */
}