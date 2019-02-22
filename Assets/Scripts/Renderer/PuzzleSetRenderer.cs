using System.Collections.Generic;
using UnityEngine;
using Penta;

public class PuzzleSetRenderer : MonoBehaviour {
    /// <summary>
    /// blocks나 boundary는 Unity editor에서 편하게 할당하기 위해 public으로 했지만
    /// 코드에서는 쓰지 않기 위해 python처럼 이름 앞에 __를 붙이고 쓰지 않기로 약속함.
    /// </summary>
    public GameObject __canvas;
    public GameObject __boardBlock;
    public GameObject __boardBlockBackground;
    public GameObject[] __pieceBlocks;
    public float __gapPerBlockSize;
    public GameObject __centerOfBoard;
    public GameObject __candidateLeftTopBlockSpawnPoint;
    // piece box means The 4x4 box covering the piece. 
    // It is always 4x4 because the envelope of the piece could be 4x2,3x3,2x4.
    public float __gapBtwPieceBoxes;
    private readonly bool __randomizeRotation = false;
    private Vector3 spawnOriginPosition;

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
        spawnOriginPosition = UnityUtils.getPositionOfUIElement(__candidateLeftTopBlockSpawnPoint);
        for (int i = 0; i < piecesInCoordinate.Count; i++) {
            List<Coordinate> pieceInCoordinate = piecesInCoordinate[i];
            if (__randomizeRotation)
                Utils.rotateRandomlySavingWidth(pieceInCoordinate);
            Utils.leftTopToOrigin(pieceInCoordinate);
            PieceRenderer renderer = new PieceRenderer(__pieceBlocks[i], __candidateLeftTopBlockSpawnPoint, __gapPerBlockSize);
            RenderedPiece renderedPiece = renderer.render(pieceInCoordinate, spawnOriginPosition);
            if (i == 2)
            {
                spawnOriginPosition.y -= 4 * UnityUtils.getWidthOfUIElement(__pieceBlocks[0]) + __gapBtwPieceBoxes;
                spawnOriginPosition.x = UnityUtils.getPositionOfUIElement(__candidateLeftTopBlockSpawnPoint).x;
            }
            else
            {
                spawnOriginPosition.x += 4 * UnityUtils.getWidthOfUIElement(__pieceBlocks[0]) + __gapBtwPieceBoxes;
            }
            ret.Add(renderedPiece);
        }
        return ret;
    }
    private void setSpawnPositionBasedOn(RenderedPiece renderedPiece) {
        spawnOriginPosition.x = renderedPiece.getRightMostXPosition() + __gapBtwPieceBoxes + renderedPiece.blockSize();
    }
    private RenderedPuzzle renderPuzzle(Puzzle p) {
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        Utils.centerToOrigin(piecesInCoordinate);
        PuzzleRenderer renderer = new PuzzleRenderer(__boardBlock, __boardBlockBackground, __pieceBlocks, __centerOfBoard, __gapPerBlockSize);
        RenderedPuzzle ret = renderer.render(piecesInCoordinate, UnityUtils.getPositionOfUIElement(__centerOfBoard));
        return ret;
    }
}

public class PieceRenderer{
    private GameObject pieceBlock;
    private float gapPerBlockSize;
    private Vector3 leftMostPosition;
    private GameObject spawnPoint;

    public PieceRenderer(GameObject pieceBlock, GameObject spawnPoint, float gapPerBlockSize) {
        this.pieceBlock = pieceBlock;
        this.spawnPoint = spawnPoint;
        this.gapPerBlockSize = gapPerBlockSize;
    }
    public RenderedPiece render(List<Coordinate> blocks, Vector3 originPosition) {
        return renderBlocksAtOrigin(blocks, originPosition);
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
    private GameObject renderOneBlockAt(Vector3 position)
    {
        return MonoBehaviourUtils.renderBlockWithPosition(pieceBlock, position);
    }
}

public class PuzzleRenderer{
    private GameObject boardBlock;
    private GameObject boardBlockBackground;
    private GameObject[] pieceBlocks;
    private float gapPerBlockSize;
    private GameObject spawnPoint;

    public PuzzleRenderer(GameObject boardBlock, GameObject boardBlockBackground,
        GameObject[] pieceBlocks, GameObject spawnPoint, float gapPerBlockSize) {
        this.boardBlock = boardBlock;
        this.boardBlockBackground = boardBlockBackground;
        this.pieceBlocks = pieceBlocks;
        this.spawnPoint = spawnPoint;
        this.gapPerBlockSize = gapPerBlockSize;
    }
    public RenderedPuzzle render(List<List<Coordinate>> pieces, Vector3 centerPosition) {
        float distance = blockSize() * (1 + gapPerBlockSize);
        List<List<GameObject>> background = renderBlocks(pieces, centerPosition, distance, boardBlockBackground);
        List<List<GameObject>> board = renderBlocks(pieces, centerPosition, distance, boardBlock);
        return new RenderedPuzzle(board, background);
    }
    private List<List<GameObject>> renderBlocks(List<List<Coordinate>> pieces, Vector3 centerPosition, float distance, GameObject blockObject) {
        List<List<GameObject>> renderedBlocks = new List<List<GameObject>>();
        foreach (List<Coordinate> piece in pieces) {
            List<GameObject> newPiece = new List<GameObject>();
            foreach (Coordinate block in piece) {
                Vector3 position = centerPosition + distance * UnityUtils.toVector3(block);
                GameObject renderedBlock = renderBlockAt(position, blockObject);
                newPiece.Add(renderedBlock);
            }
            renderedBlocks.Add(newPiece);
        }
        return renderedBlocks;
    }
    private float blockSize() {
        return UnityUtils.getWidthOfUIElement(boardBlock);
    }
    private GameObject renderBlockAt(Vector3 position, GameObject block) {
        return MonoBehaviourUtils.renderBlockWithPosition(block, position);
    }
    public void showAns() {
        // 정답을 rendering하게 되면 정답이 조각보다 위에 위치해서
        // 조각을 끌어다 놔도 정답 밑에 깔리게 되는 이슈를 유의하자.
    }
}