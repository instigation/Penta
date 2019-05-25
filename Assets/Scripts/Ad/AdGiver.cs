using GoogleMobileAds.Api;
using UnityEngine;
using System.Collections;
using Penta;

public class AdGiver : IAdGiver
{
    private PuzzleStageController puzzleStageController;

    private InterstitialAd interstitial;
    private BannerView bannerView;
    private RewardBasedVideoAd rewardBasedVideo;
    private bool isRewarded;
    private double latestAdClosedTime = 0;

    public AdGiver(PuzzleStageController puzzleStageController)
    {
        this.puzzleStageController = puzzleStageController;
        MobileAds.SetiOSAppPauseOnBackground(true);
        // TODO: get AppId

        requestBanner();

        requestInterstitial();
        assignInterstitialHandlers();

        // Get singleton reward based video ad reference.
        rewardBasedVideo = RewardBasedVideoAd.Instance;
        requestRewardBasedVideo();
        assignRewardBasedVideoHandlers();
    }
    private void requestBanner()
    {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-9427427719096864/2663810991";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif
        // Create a 320x50 banner at the bottom of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    private void requestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-9427427719096864/9071762086";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }
    private void assignInterstitialHandlers()
    {
        interstitial.OnAdFailedToLoad += HandleOnInterstitialFailedToLoad;
        interstitial.OnAdClosed += HandleOnInterstitialClosed;
    }
    public void HandleOnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }
    public void HandleOnInterstitialClosed(object sender, System.EventArgs args)
    {
        latestAdClosedTime = Time.fixedTime;
        requestInterstitial();
    }
    private void requestRewardBasedVideo()
    {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-9427427719096864/7724565988";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }
    private void assignRewardBasedVideoHandlers()
    {
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed (either refused or finished watching).
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
    }
    public void HandleRewardBasedVideoLoaded(object sender, System.EventArgs args)
    {
        isRewarded = false;
    }
    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Rewarded video ad failed to load: " + args.Message);
        // TODO: Handle the ad failed to load event.
    }
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        puzzleStageController.reviveStage();
        isRewarded = true;
    }
    public void HandleRewardBasedVideoClosed(object sender, System.EventArgs args)
    {
        latestAdClosedTime = Time.fixedTime;
        requestRewardBasedVideo();
        if (!isRewarded)
            puzzleStageController.resetStage();
    }


    public void hideBanner()
    {
        bannerView.Hide();
    }

    public void showBanner()
    {
        bannerView.Show();
    }

    public void showInterstitialIfLoaded()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }

    public void showRewardBasedVideoIfLoaded()
    {
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
    }

    public double latestAdClosedTimeFromStartInSeconds()
    {
        return latestAdClosedTime;
    }
}
