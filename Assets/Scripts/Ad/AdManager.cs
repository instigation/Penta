using System;
using UnityEngine;
using Penta;

public class AdManager: MonoBehaviour {
    public PuzzleStageController __puzzleStageController;
    private IAdGiver adGiver;
    private void Start()
    {
        if ((bool)GlobalInformation.getOrInitValue("isAdRemoved", false))
        {
            adGiver = new EmptyAdGiver(__puzzleStageController);
        }
        else
            adGiver = new AdGiver(__puzzleStageController);
    }
    public void removeAd() {
        adGiver.hideBanner();
        adGiver = new EmptyAdGiver(__puzzleStageController);
        GlobalInformation.storeKeyValue("isAdRemoved", true);
    }
    public void hideBanner() { adGiver.hideBanner(); }
    public void showBanner() { adGiver.showBanner(); }
    public void showInterstitialIfLoadedAndNotTooFrequent()
    {
        double currentTime = Time.fixedTime;
        if(currentTime - adGiver.latestAdClosedTimeFromStartInSeconds() > 60)
        {
            adGiver.showInterstitialIfLoaded();
        }
    }

    public void showRewardBasedVideoIfLoaded()
    {
        adGiver.showRewardBasedVideoIfLoaded();
    }
}