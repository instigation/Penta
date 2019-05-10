﻿using UnityEngine;
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
    public GameObject __clearTextInstantiationPosition;
    public Camera __camera;
    public float clearTextTimeInSecond;
    public Vector2 __offsetToMoveOnSelect;
    public AdManager __adManager;
    private AudioSource failSound;
    private GeneralInput input;
    private RenderedPuzzleSet puzzleSet;
    public BonusCalculator __bonusCalculator;
    private float blockDestroyAnimationClipTimeInSecond;
    private bool isRevived = false;

    // Use this for initialization
    void Start () {
        setInput();
        failSound = gameObject.GetComponents<AudioSource>()[0];
    }
    private void setInput()
    {
        InputValidator workingspaceValidator = new EmptyValidator();
        input = new MouseInputWrapper(__canvas, __camera, workingspaceValidator);
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
                        __bonusCalculator.calculateOnInsertion(insertionResult);
                        addTime(__bonusCalculator.getBonusTime());
                        addScore(__bonusCalculator.getBonusScore());
                        __bonusCalculator.playBonusText(puzzleSet.board.topOfLastInsertedAnchoredPosition());
                        __bonusCalculator.playBonusParticle(puzzleSet.board.topOfLastInsertedAnchoredPosition());
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

    private void clearPuzzle() {
        puzzleSet.destroy();
        __timer.pause();
        StartCoroutine(clearPuzzleAfterDestroy());
    }
    private IEnumerator clearPuzzleAfterDestroy()
    {
        yield return new WaitForSeconds(blockDestroyAnimationClipTimeInSecond);
        __progressBar.progressByOne();
        if (__progressBar.isEnded())
        {
            // saving score is important, so should be executed first.
            __scoreChanger.saveScore();
            playClearText();
            yield return new WaitForSeconds(clearTextTimeInSecond);
            __adManager.showInterstitialIfLoadedAndNotTooFrequent();
            __stageChanger.toStage(Stage.MENU);
        }
        else
        {
            renderPuzzle();
            yield return new WaitForSeconds(blockDestroyAnimationClipTimeInSecond); // Assume that (puzzle render time) equals (destory time).
            __timer.run();
        }
        yield return null;
    }
    private void playClearText()
    {
        GameObject ret = Instantiate(__clearText);
        ret.transform.SetParent(__canvas.transform, false);
        UnityUtils.moveUIElementToPosition(ret, UnityUtils.getPositionOfUIElement(__clearTextInstantiationPosition));
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
        __scoreChanger.resetAndSave();
        __progressBar.resetCurrentStage();
        __stageChanger.toStage(Stage.MENU);
        yield return null;
    }
    public void startStage()
    {
        ScoreChanger.setSavedScoreToZero(); // To make score zero when player terminates during the stage. The score is overwritten on puzzle clear.
        // progressByOne과 setBonusCalculator의 순서는 중요한데, bonus calculator가 stage number에 영향받기 때문
        __progressBar.progressByOne();
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