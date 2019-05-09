using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuzzleStageController : MonoBehaviour {
    public GameObject __normalParticle;
    public GameObject __specialParticle;
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
    public GameObject __clearText;
    public GameObject __centerOfBoard;
    public Camera __camera;
    public float clearTextTimeInSecond;
    public Vector2 __offsetToMoveOnSelect;
    public AdManager __adManager;
    private AudioSource failSound;
    private GeneralInput input;
    private RenderedPuzzleSet puzzleSet;
    private BonusCalculator bonusCalculator;
    private float blockDestroyAnimationClipTimeInSecond;
    private bool isRevived = false;

    // Use this for initialization
    void Start () {
        setInput();
        setBonusCalculator();
        failSound = gameObject.GetComponent<AudioSource>();
    }
    private void setInput()
    {
        InputValidator workingspaceValidator = new EmptyValidator();
        input = new MouseInputWrapper(__canvas, __camera, workingspaceValidator);
    }
    private void setBonusCalculator()
    {
        bonusCalculator = new BonusCalculator(__progressBar, __bonusText, __normalParticle, __specialParticle, __canvas);
    }
    void OnEnable()
    {
        startStage();
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
        if (GlobalInformation.hasKey("num"))
            return GlobalInformation.getInt("num");
        else
            return 4;
    }
    private Difficulty resolveDifficulty()
    {
        if (GlobalInformation.hasKey("difficulty"))
            return (Difficulty)GlobalInformation.getInt("difficulty");
        else
            return Difficulty.HARD;
    }

    // Update is called once per frame
    void Update ()
    {
        if (isGameOvered())
        {
            if (isRevived)
                resetStage();
            else
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
                    __controller.moveSelectedToAnchoredPosition(input.anchoredPosition() + __offsetToMoveOnSelect);
                }
                else if (input.touchMoved())
                {
                    __controller.moveSelectedForAnchoredVector(input.deltaAnchoredPosition());
                }
                else if (input.touchEnded())
                {
                    if (__controller.tryToInsertSelected())
                    {
                        InsertionResult insertionResult = puzzleSet.board.lastInsertionResult();
                        bonusCalculator.calculateOnInsertion(insertionResult);
                        addTime(bonusCalculator.getBonusTime());
                        addScore(bonusCalculator.getBonusScore());
                        bonusCalculator.playBonusText(puzzleSet.board.topOfLastInsertedAnchoredPosition());
                        bonusCalculator.playBonusParticle(puzzleSet.board.topOfLastInsertedAnchoredPosition());
                        if (puzzleSet.board.isSolved())
                            clearPuzzle();
                        if (insertionResult == InsertionResult.WRONG)
                            failSound.Play();
                    }
                    __controller.unSelect();
                }
                __controller.highlightBoardBySelected();
            }
        }
    }

    private class BonusCalculator
    {
        private bool isLastInsertionCorrect;
        private bool isStreakOccured;
        private bool isCorrectInsertionOccured;
        private int streak;
        private ProgressBar progressBar;
        private GameObject bonusText;
        private GameObject normalParticle;
        private GameObject specialParticle;
        private GameObject canvas;

        public BonusCalculator(ProgressBar progressBar, GameObject bonusText, GameObject normalParticle, GameObject specialParticle, GameObject canvas)
        {
            isLastInsertionCorrect = false;
            isStreakOccured = false;
            isCorrectInsertionOccured = false;
            streak = 0;
            //combo = 0;
            this.progressBar = progressBar;
            this.bonusText = bonusText;
            this.normalParticle = normalParticle;
            this.specialParticle = specialParticle;
            this.canvas = canvas;
        }
        public void calculateOnInsertion(InsertionResult insertionResult)
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
                    isLastInsertionCorrect = true;
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
            return isCorrectInsertionOccured ? progressBar.getStage() * (streak + 1) : 0;
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
        public void playBonusParticle(Vector2 position)
        {
            if (isCorrectInsertionOccured)
            {
                if ((streak % 5 == 0) && (streak > 0))
                    playParticle(specialParticle, position);
                else
                    playParticle(normalParticle, position);
            }
        }
        private void playParticle(GameObject particle, Vector2 position)
        {
            UnityUtils.moveUIElementToPosition(particle, position);
            particle.GetComponent<ParticleSystem>().Play();
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
        {
            __timer.pause();
            playClearText();
            yield return new WaitForSeconds(clearTextTimeInSecond);
            __adManager.showInterstitialIfLoadedAndNotTooFrequent();
            __stageChanger.toStage(Stage.MENU);
            // TODO: render skippable ad, revive, ...
        }
        else
            renderPuzzle();
        yield return null;
    }
    private void playClearText()
    {
        GameObject ret = Instantiate(__clearText);
        ret.transform.SetParent(__canvas.transform, false);
        UnityUtils.moveUIElementToPosition(ret, UnityUtils.getPositionOfUIElement(__centerOfBoard));
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
        __gameOverPanel.SetActive(false);
        // Refill is necessary to avoid reactivation of the panel due to time being zero.
        __timer.pause();
        __timer.refillTime();
        puzzleSet.destroy();
        StartCoroutine(resetStageAfterDestroy());
    }
    private IEnumerator resetStageAfterDestroy()
    {
        yield return new WaitForSeconds(blockDestroyAnimationClipTimeInSecond);
        __scoreChanger.reset();
        __progressBar.resetStage();
        __stageChanger.toStage(Stage.MENU);
        yield return null;
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
        isRevived = false;
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
        isRevived = true;
    }
}