using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public StageChanger __stageChanger;
    public GameObject __gameOverPanel;
    private GeneralInput input;
    private RenderedPuzzleSet puzzleSet;
    private AndroidLogger logger;
    private BonusCalculator bonusCalculator;
    private float blockDestroyAnimationClipTimeInSecond;

    // Use this for initialization
    void Start () {
        setInput();
        setBonusCalculator();
    }
    private void setInput()
    {
        InputValidator workingspaceValidator = new EmptyValidator();
        input = new MouseInputWrapper(__canvas, workingspaceValidator);
    }
    private void setBonusCalculator()
    {
        bonusCalculator = new BonusCalculator(__progressBar, __bonusText, __canvas);
    }
    void OnEnable()
    {
        startStage();
    }
    private void OnDisable()
    {
        endStage();
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
        blockDestroyAnimationClipTimeInSecond = puzzleSet.getDestroyAnimationTimeInSecond();
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

    // Update is called once per frame
    void Update ()
    {
        if (isGameOvered())
        {
            pauseStage();
        }
        else
        {
            input.update();
            if (!input.noTouching())
            {
                //only process the first touch
                if (input.touchBegan())
                {
                    __controller.selectOnPosition(input.anchoredPosition(), __renderer.__gapBtwPieceBoxes / 2);
                    __controller.tryToExtractSelected();
                    __controller.moveSelectedToAnchoredPosition(input.anchoredPosition());
                }
                else if (input.touchMoved())
                {
                    __controller.moveSelectedForAnchoredVector(input.deltaAnchoredPosition());
                }
                else if (input.touchEnded())
                {
                    if (__controller.tryToInsertSelected())
                    {
                        float currentStageTime = __timer.getCurrentStageTime();
                        InsertionResult insertionResult = puzzleSet.board.lastInsertionResult();
                        bonusCalculator.calculateOnInsertion(insertionResult, currentStageTime);
                        addTime(bonusCalculator.getBonusTime());
                        addScore(bonusCalculator.getBonusScore());
                        bonusCalculator.playBonusText(puzzleSet.board.topOfLastInsertedAnchoredPosition());
                        if (puzzleSet.board.isSolved())
                            clearPuzzle();
                    }
                    __controller.unSelect();
                }
                __controller.highlightBoardBySelected();
            }
        }
    }

    private class BonusCalculator
    {
        private float lastCorrectInsertionTime;
        private bool isLastInsertionCorrect;
        private bool isStreakOccured;
        private bool isCorrectInsertionOccured;
        private int streak;
        //private int combo;
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
            //combo = 0;
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
                    /* if (currentStageTime - lastCorrectInsertionTime <= 2.0f)
                         combo++;
                     else
                         combo = 0; */
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
            return isCorrectInsertionOccured ? progressBar.getStage() * (streak + 1) : 0;//(combo + streak + 1) : 0;
        }
        public void playBonusText(Vector2 position)
        {
            if (isCorrectInsertionOccured)
            {
                GameObject ret = Instantiate(bonusText);
                ret.transform.SetParent(canvas.transform, false);
                UnityUtils.moveUIElementToPosition(ret, position);
                ret.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = getScoreText();
                ret.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = getComboText();
            }
        }
        private string getComboText()
        {
            string ret; //= "\n+" + getBonusScore().ToString();
            if (streak != 0)
                ret = streak.ToString() + " Combo!";// + ret;
            else ret = "";
            //if (combo != 0)
            //    ret = "Fast! x" + combo.ToString() + ret;
            return ret;
        }

        private string getScoreText()
        {
            string ret = "+" + getBonusScore().ToString();
            return ret;
        }
    }


    private void clearPuzzle() {
        puzzleSet.destroy();
        StartCoroutine(clearPuzzleAfterDestroy());
    }
    private IEnumerator clearPuzzleAfterDestroy()
    {
        yield return new WaitForSeconds(blockDestroyAnimationClipTimeInSecond);
        __progressBar.progressByOne();
        if (__progressBar.isEnded())
            __stageChanger.toStage(Stage.MENU);
        // TODO: render skippable ad, revive, ...
        else
            renderPuzzle();
    }
    private bool isGameOvered()
    {
        return __timer.getLeftoverTimeInSecond() < 0.0f;
    }
    private void addScore(int amount) {
        __scoreChanger.changeGradually(amount);
    }
    private void addTime(float amount)
    {
        __timer.manuallyChangeTime(amount);
    }
    public void resetStage()
    {
        endStage();
        __scoreChanger.reset();
        __progressBar.resetStage();
        __stageChanger.toStage(Stage.MENU);
    }
    public void endStage()
    {
        puzzleSet.destroy();
        __timer.pause();
        __gameOverPanel.SetActive(false);
    }
    public void startStage()
    {
        // progressByOne과 setBonusCalculator의 순서는 중요한데, bonus calculator가 stage number에 영향받기 때문
        __progressBar.progressByOne();
        setBonusCalculator();
        renderPuzzle();

        __gameOverPanel.SetActive(false);
        __timer.refillTime();
        __timer.run();
    }
    public void pauseStage()
    {
        __gameOverPanel.SetActive(true);
        __timer.pause();
    }
    public void reviveStage()
    {
        __gameOverPanel.SetActive(false);
        __timer.refillTime();
        __timer.run();
    }
}