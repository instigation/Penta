using UnityEngine;
using System.Collections;
using Penta;

public class EmptyAdGiver : IAdGiver
{
    private PuzzleStageController puzzleStageController;
    public EmptyAdGiver(PuzzleStageController puzzleStageController)
    {
        this.puzzleStageController = puzzleStageController;
    }
    public void hideBanner() { }
    public void showBanner() { }
    public void showInterstitialIfLoaded() { }
    public void showRewardBasedVideoIfLoaded()
    {
        puzzleStageController.reviveStage();
    }
    public double latestAdClosedTimeFromStartInSeconds()
    {
        return 0;
    }
}
