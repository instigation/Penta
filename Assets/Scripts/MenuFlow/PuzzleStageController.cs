using UnityEngine;

public class PuzzleStageController : MonoBehaviour {
    public PuzzleSetRenderer __renderer;
    public PieceController __controller;
    public GameObject __canvas;
    public Vector3 canvasPosition;
    public ScoreChanger __scoreChanger;
    public GameObject __workingArea;
    public Timer __timer;
    public ProgressBar __progressBar;
    private GeneralInput input;
    private RenderedPuzzleSet puzzleSet;
    private AndroidLogger logger;
    private BonusCalculator bonusCalculator;

    // Use this for initialization
    void Start () {
        renderPuzzle();
        renderAd();
        setInput();
        setBonusCalculator();
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
        InputValidator workingspaceValidator = new WorkingspaceValidator(__workingArea);
        input = new MouseInputWrapper(__canvas, workingspaceValidator);
    }
    private void setBonusCalculator()
    {
        bonusCalculator = new BonusCalculator(__progressBar);
    }

    // Update is called once per frame
    void Update () {
        input.update();
        if(!input.noTouching()) {
            //only process the first touch
            if (input.touchBegan()) {
                Debug.Log("touch began!");
                __controller.selectOnPosition(input.position(), __renderer.__gapBtwPieceBoxes/2);
                __controller.tryToExtractSelected();
                __controller.moveSelectedTo(input.position());
            }
            else if (input.touchMoved()) {
                __controller.moveSelectedFor(input.deltaPosition());
            }
            else if (input.touchEnded()) {
                Debug.Log("touch ended!");
                __controller.tryToInsertSelected();
                __controller.unSelect();
                float currentStageTime = __timer.getCurrentStageTime();
                InsertionResult insertionResult = puzzleSet.board.lastInsertionResult();
                bonusCalculator.calculateOnInsertion(insertionResult, currentStageTime);
                addTime(bonusCalculator.getBonusTime());
                addScore(bonusCalculator.getBonusScore());
                if (puzzleSet.board.isSolved())
                    clearStage();
            }
            __controller.highlightBoardBySelected();
        }
    }

    private class BonusCalculator
    {
        private float lastCorrectInsertionTime;
        private bool isLastInsertionCorrect;
        private bool isStreakOccured;
        private bool isCorrectInsertionOccured;
        private int streak;
        private int combo;
        private ProgressBar progressBar;

        public BonusCalculator(ProgressBar progressBar)
        {
            isLastInsertionCorrect = false;
            lastCorrectInsertionTime = float.NegativeInfinity;
            isStreakOccured = false;
            isCorrectInsertionOccured = false;
            streak = 0;
            combo = 0;
            this.progressBar = progressBar;
        }
        public void calculateOnInsertion(InsertionResult insertionResult, float currentStageTime)
        {
            switch (insertionResult)
                {
                case InsertionResult.CORRECT:
                    isCorrectInsertionOccured = true;
                    if (isLastInsertionCorrect)
                    {
                        streak++;
                        isStreakOccured = true;
                    }
                    else
                        isStreakOccured = false;
                    if (currentStageTime - lastCorrectInsertionTime <= 2.0f)
                        combo++;
                    else
                        combo = 0;
                    isLastInsertionCorrect = true;
                    lastCorrectInsertionTime = currentStageTime;
                    break;
                case InsertionResult.WRONG:
                    isCorrectInsertionOccured = false;
                    streak = 0;
                    isStreakOccured = false;
                    isLastInsertionCorrect = false;
                    break;
                case InsertionResult.MISS:
                    isCorrectInsertionOccured = false;
                    isStreakOccured = false;
                    break;
            }
        }
        public float getBonusTime()
        {
            return isStreakOccured ? 1 : 0;
        }
        public int getBonusScore()
        {
            return isCorrectInsertionOccured? progressBar.getStage()*(combo + streak + 1) : 0;
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
    private void addTime(float amount)
    {
        StartCoroutine(__timer.manuallyChangeTime(amount));
    }
    private void clearPuzzle() {
        puzzleSet.destroy();
    }
}