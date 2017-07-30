using System.Collections.Generic;
using UnityEngine;

public class PuzzleRenderer : MonoBehaviour {
    /// <summary>
    /// blocks나 boundary는 Unity editor에서 편하게 할당하기 위해 public으로 했지만
    /// 코드에서는 쓰지 않기 위해 python처럼 이름 앞에 __를 붙이고 쓰지 않기로 약속함.
    /// </summary>
    public GameObject __boardBlock;
    public GameObject __boardBoundary;
    public GameObject[] __pieceBlocks;

    public RenderedPuzzle render(Puzzle p) {
        List<RenderedPiece> candidates = renderCandidates(p);
        RenderedPuzzle board = renderPuzzle(p);
        return new RenderedPuzzle(candidates, board);
    }
    private List<RenderedPiece> renderCandidates(Puzzle p) {
        /// 친구들과만 놀고 싶지만 안될듯
        /// 친구들과 놀려면 예를 들어 puzzle한테 Coordinate 리스트를 받으면 되지만
        /// 그러면 rotate 구현도 추가작업 필요하고 DRY하지도 않음
        /// 생각해 보면 rotate/ reset만 문제임.
        /// 알고보니 DRY하게 가능함! 그냥 block의 set과 center만 알면 rotate하는건 그게
        /// piece인지 아닌지랑은 노상관임.
        
        // 여기서 deep copy된 걸로 받아야 함.
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        List<RenderedPiece> ret = new List<RenderedPiece>();
        foreach(List<Coordinate> pieceInCoordinate in piecesInCoordinate) {
            Utils.normalize(pieceInCoordinate);
            // 실제 rendering은 RenderedPiece의 생성자에서.
            RenderedPiece renderedPiece = new RenderedPiece(pieceInCoordinate, __pieceBlocks);
            ret.Add(renderedPiece);
        }
        return ret;
    }
    private RenderedPuzzle renderPuzzle(Puzzle p) {
        List<List<Coordinate>> piecesInCoordinate = p.getBlocks();
        RenderedPuzzle ret = new RenderedPuzzle(piecesInCoordinate, __boardBlock, __boardBoundary, __pieceBlocks);
        return ret;
    }
}

public class RenderedPuzzle {
    public List<RenderedPiece> candidates;
    public RenderedPuzzle board;

    public RenderedPuzzle(List<RenderedPiece> candidates, RenderedPuzzle board) {
        this.candidates = candidates;
        this.board = board;
    }
}

public class RenderedPiece {
    public void rotate();
    public void move(Vector3);
    public void resetPosition();
    public void reset();
}

public class RenderedPuzzle {
    public void tryToInsert(RenderedPiece);
    public void extract(RenderedPiece);
    public bool isSolved();
    public void showAns();
}