using System;
using UnityEngine;
using Penta;

public class AdManager: MonoBehaviour, IAdGiver {
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
        adGiver.requestBanner();
    }
    public void removeAd() {
        adGiver.hideBanner();
        adGiver = new EmptyAdGiver(__puzzleStageController);
        GlobalInformation.storeKeyValue("isAdRemoved", true);
    }
    public void requestBanner() { adGiver.requestBanner(); } 
    public void hideBanner() { adGiver.hideBanner(); }
    public void showBanner() { adGiver.showBanner(); }
    public void showIfLoaded() { adGiver.showIfLoaded(); }
    public void userOptToWatchReviveAd() { adGiver.userOptToWatchReviveAd(); }
}
