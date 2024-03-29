﻿using System;
using UnityEngine;
using Penta;

public class AdManager: MonoBehaviour {
    public PuzzleStageController __puzzleStageController;
    private IAdGiver adGiver;
    private void Start()
    {
        if (GlobalInformation.getOrInitBool("isAdRemoved", false))
        {
            adGiver = new EmptyAdGiver(__puzzleStageController);
        }
        else
            adGiver = new AdGiver(__puzzleStageController);
    }
    public void removeAd() {
        adGiver.hideBanner();
        adGiver = new EmptyAdGiver(__puzzleStageController);
        GlobalInformation.setBool("isAdRemoved", true);
    }
    public bool isAdRemoved()
    {
        return GlobalInformation.getOrInitBool("isAdRemoved", false);
    }
    public void hideBanner() { adGiver.hideBanner(); }
    public void showBanner() { adGiver.showBanner(); }
    public void showInterstitialIfLoadedAndNotTooFrequent()
    {
        double currentTime = Time.fixedTime;
        if(currentTime - adGiver.latestAdClosedTimeFromStartInSeconds() > 60)
        {
            adGiver.tryToShowInterstitial();
        }
    }

    public void showRewardBasedVideoIfLoaded()
    {
        adGiver.tryToShowRewardBasedVideo();
    }
}