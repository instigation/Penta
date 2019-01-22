using UnityEngine;
using UnityEngine.UI;

public class PuzzleStageController : MonoBehaviour {
    public PuzzleSetRenderer __renderer;
    public PieceController __controller;
    public GameObject __canvas;
    public Vector3 canvasPosition;
    public ScoreChanger __scoreChanger;
    public GameObject __workingArea;
    public Timer __timer;
    public ProgressBar __progressBar;
    public GameObject __bonusText;
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
        bonusCalculator = new BonusCalculator(__progressBar, __bonusText, __canvas);
    }

    // Update is called once per frame
    void Update () {
        input.update();
        if(!input.noTouching()) {
            //only process the first touch
            if (input.touchBegan()) {
                __controller.selectOnPosition(input.position(), __renderer.__gapBtwPieceBoxes/2);
                __controller.tryToExtractSelected();
                __controller.moveSelectedTo(input.position());
            }
            else if (input.touchMoved()) {
                __controller.moveSelectedFor(input.deltaPosition());
            }
            else if (input.touchEnded()) {
                if (__controller.tryToInsertSelected())
                {
                    float currentStageTime = __timer.getCurrentStageTime();
                    InsertionResult insertionResult = puzzleSet.board.lastInsertionResult();
                    bonusCalculator.calculateOnInsertion(insertionResult, currentStageTime);
                    addTime(bonusCalculator.getBonusTime());
                    addScore(bonusCalculator.getBonusScore());
                    bonusCalculator.playBonusText(puzzleSet.board.topOfLastInsertedPosition());
                    if (puzzleSet.board.isSolved())
                        clearStage();
                }
                __controller.unSelect();
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
        private GameObject bonusText;
        private GameObject canvas;

        public BonusCalculator(ProgressBar progressBar, GameObject bonusText, GameObject canvas)
        {
            isLastInsertionCorrect = false;
            lastCorrectInsertionTime = float.NegativeInfinity;
            isStreakOccured = false;
            isCorrectInsertionOccured = false;
            streak = 0;
            combo = 0;
            this.progressBar = progressBar;
            this.bonusText = bonusText;
            this.canvas = canvas;
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
        public void playBonusText(Vector2 position)
        {
            if (isCorrectInsertionOccured)
            {
                GameObject ret = Instantiate(bonusText);
                ret.transform.SetParent(canvas.transform, false);
                UnityUtils.moveUIElementToPosition(ret, position);
                ret.transform.GetChild(0).GetComponent<Text>().text = getBonusText();
            }
        }
        private string getBonusText()
        {
            string ret = "\n+" + getBonusScore().ToString();
            if (streak != 0)
                ret = "\nSmart! x" + streak.ToString() + ret;
            if (combo != 0)
                ret = "Fast! x" + combo.ToString() + ret;
            return ret;
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