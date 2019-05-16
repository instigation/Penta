using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Penta;
using System;

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
    public Color[] __pieceColors;
    public GameObject __boardParent, __pieceParent;
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
        spawnOriginPosition = UnityUtils.getLocalPositionOfUIElement(__candidateLeftTopBlockSpawnPoint);
        Color[] pieceColors = new Color[__pieceColors.Length];
        Array.Copy(__pieceColors, pieceColors, __pieceColors.Length);
        pieceColors.Shuffle();
        for (int i = 0; i < piecesInCoordinate.Count; i++) {
            List<Coordinate> pieceInCoordinate = piecesInCoordinate[i];
            if (__randomizeRotation)
                Utils.rotateRandomlySavingWidth(pieceInCoordinate);
            Utils.leftTopToOrigin(pieceInCoordinate);
            PieceRenderer renderer = new PieceRenderer(__pieceBlocks[i], __gapPerBlockSize, __pieceParent);
            RenderedPiece renderedPiece = renderer.render(pieceInCoordinate, spawnOriginPosition, pieceColors[i]);
            if (i == 2)
            {
                spawnOriginPosition.y -= 4 * UnityUtils.getWidthOfUIElement(__pieceBlocks[0]) + __gapBtwPieceBoxes;
                spawnOriginPosition.x = UnityUtils.getAnchoredPositionOfUIElement(__candidateLeftTopBlockSpawnPoint).x;
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
        spawnOriginPosition.x = renderedPiece.getRightMostXAnchoredPosition() + __gapBtwPieceBoxes + renderedPiece.blockSize();
    }
    private RenderedPuzzle renderPuzzle(Puzzle p) {
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        Utils.centerToOrigin(piecesInCoordinate);
        PuzzleRenderer renderer = new PuzzleRenderer(__boardBlock, __boardBlockBackground, __gapPerBlockSize, __boardParent);
        RenderedPuzzle ret = renderer.render(piecesInCoordinate, UnityUtils.getAnchoredPositionOfUIElement(__centerOfBoard));
        return ret;
    }
}

public class PieceRenderer{
    private GameObject parent;
    private GameObject pieceBlock;
    private float gapPerBlockSize;
    private Vector3 leftMostPosition;

    public PieceRenderer(GameObject pieceBlock, float gapPerBlockSize, GameObject parent) {
        this.pieceBlock = pieceBlock;
        this.gapPerBlockSize = gapPerBlockSize;
        this.parent = parent;
    }
    public RenderedPiece render(List<Coordinate> blocks, Vector3 originPosition, Color pieceColor) {
        return renderBlocksAtOrigin(blocks, originPosition, pieceColor);
    }
    private RenderedPiece renderBlocksAtOrigin(List<Coordinate> blocks, Vector3 originPosition, Color pieceColor) {
        ///<remarks>
        /// origin means (0,0) in Coordinates
        ///</remarks>
        List<Block> renderedBlocks = new List<Block>();
        float distance = blockSize() * (1 + gapPerBlockSize);
        Color originalColor = pieceBlock.GetComponent<Image>().color;
        pieceBlock.GetComponent<Image>().color = pieceColor;
        foreach (Coordinate block in blocks) {
            Vector3 position = originPosition + UnityUtils.toVector3(block) * distance;
            Block renderedBlock = renderOneBlockAt(position).GetComponent<Block>();
            renderedBlocks.Add(renderedBlock);
        }
        pieceBlock.GetComponent<Image>().color = originalColor;
        return new RenderedPiece(renderedBlocks, blocks);
    }
    private float blockSize() {
        return UnityUtils.getWidthOfUIElement(pieceBlock);
    }
    private GameObject renderOneBlockAt(Vector3 position)
    {
        return MonoBehaviourUtils.renderBlockWithPosition(pieceBlock, position, parent);
    }
}

public class PuzzleRenderer{
    private GameObject parent;
    private GameObject boardBlock;
    private GameObject boardBlockBackground;
    private float gapPerBlockSize;

    public PuzzleRenderer(GameObject boardBlock, GameObject boardBlockBackground, float gapPerBlockSize, GameObject parent) {
        this.boardBlock = boardBlock;
        this.boardBlockBackground = boardBlockBackground;
        this.gapPerBlockSize = gapPerBlockSize;
        this.parent = parent;
    }
    public RenderedPuzzle render(List<List<Coordinate>> pieces, Vector3 centerPosition) {
        float distance = blockSize() * (1 + gapPerBlockSize);
        List<List<GameObject>> background = renderBlocks(pieces, centerPosition, distance, boardBlockBackground);
        List<List<GameObject>> board = renderBlocks(pieces, centerPosition, distance, boardBlock);
        List<List<BackgroundBlock>> backgroundBlocks = new List<List<BackgroundBlock>>();
        foreach(List<GameObject> subList in background)
        {
            List<BackgroundBlock> temp = new List<BackgroundBlock>();
            foreach(GameObject element in subList)
            {
                temp.Add(element.GetComponent<BackgroundBlock>());
            }
            backgroundBlocks.Add(temp);
        }
        List<List<Block>> boardBlocks = new List<List<Block>>();
        foreach (List<GameObject> subList in board)
        {
            List<Block> temp = new List<Block>();
            foreach (GameObject element in subList)
            {
                temp.Add(element.GetComponent<Block>());
            }
            boardBlocks.Add(temp);
        }
        return new RenderedPuzzle(boardBlocks, backgroundBlocks);
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
        return MonoBehaviourUtils.renderBlockWithPosition(block, position, parent);
    }
    public void showAns() {
        // 정답을 rendering하게 되면 정답이 조각보다 위에 위치해서
        // 조각을 끌어다 놔도 정답 밑에 깔리게 되는 이슈를 유의하자.
    }
}
