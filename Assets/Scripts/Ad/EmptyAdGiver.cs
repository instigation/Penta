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
    public void requestBanner() { }
    public void hideBanner() { }
    public void showBanner() { }
    public void showIfLoaded() { }
    public void userOptToWatchReviveAd() {
        puzzleStageController.reviveStage();
    }
}
