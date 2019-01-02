using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PuzzleStageController : MonoBehaviour {
    public PuzzleSetRenderer __renderer;
    public PieceController __controller;
    public GameObject __canvas;
    public ScoreChanger __scoreChanger;
    public GameObject __workingArea;
    private GeneralInput input;
    private RenderedPuzzleSet puzzleSet;
    private AndroidLogger logger;

    // Use this for initialization
    void Start () {
        renderPuzzle();
        renderAd();
        setInput();
    }
    private void setAndroidLogger() {
        logger = new AndroidLogger();
    }
    private void renderPuzzle() {
        int num = resolveNum();
        Difficulty difficulty = resolveDifficulty();
        PuzzleGenerator generator = new PuzzleGenerator(num, difficulty);
        Puzzle p = generator.generatePuzzle();
        puzzleSet = __renderer.render(p);
        __controller.setPuzzleSet(puzzleSet);
    }
    private int resolveNum() {
        return (int)resolve("num", 4);
    }
    private Difficulty resolveDifficulty() {
        return (Difficulty)resolve("difficulty", Difficulty.HARD);
    }
    private object resolve(object key, object defaultForNoKey) {
        if (GlobalInformation.contains(key))
            return GlobalInformation.getValue(key);
        else
            return defaultForNoKey;
    }
    private void renderAd() {
        BannerMaker.requestBanner();
    }
    private void setInput() {
        var rect = __canvas.GetComponent<RectTransform>().rect;
        InputValidator workingspaceValidator = new WorkingspaceValidator(__workingArea);
        input = new MouseInputWrapper(rect.width, rect.height, workingspaceValidator);
    }

    // Update is called once per frame
    void Update () {
        input.update();
        if(!input.noTouching()) {
            //only process the first touch
            if (input.touchBegan()) {
                Debug.Log("touch began!");
                __controller.selectOnPosition(input.position(), __renderer.__gapBtwBlocks);
                __controller.tryToExtractSelected();
            }
            else if (input.touchMoved()) {
                __controller.moveSelectedFor(input.deltaPosition());
            }
            else if (input.touchEnded()) {
                if (input.timeCollapsedAfterLastTouch() < 0.35f)
                    __controller.rotateSelected();
                Debug.Log("touch ended!");
                __controller.tryToInsertSelected();
                if (puzzleSet.board.isSolved())
                    clearStage();
            }
        }
    }
    private void clearStage() {
        clearPuzzle();
        addScore(50);
        renderPuzzle();
    }
    private void addScore(int amount) {
        StartCoroutine(__scoreChanger.changeGradually(amount));
    }
    private void clearPuzzle() {
        puzzleSet.destroy();
    }
}