using GoogleMobileAds.Api;
using UnityEngine;
using System.Collections;
using Penta;

public class AdGiver: IAdGiver
{
    private PuzzleStageController puzzleStageController;

    private InterstitialAd interstitial;
    private BannerView bannerView;
    private RewardBasedVideoAd rewardBasedVideoForRevive;
    private bool isRewarded;

    public AdGiver(PuzzleStageController puzzleStageController)
    {
        this.puzzleStageController = puzzleStageController;
        // TODO: get AppId

        requestInterstitial();

        // Get singleton reward based video ad reference.
        this.rewardBasedVideoForRevive = RewardBasedVideoAd.Instance;
        requestRewardBasedVideo();
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;


        // Called when an ad request has successfully loaded.
        rewardBasedVideoForRevive.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideoForRevive.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideoForRevive.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideoForRevive.OnAdClosed += HandleRewardBasedVideoClosed;

    }
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }
    public void HandleOnAdClosed(object sender, System.EventArgs args)
    {
        requestInterstitial();
    }

    public void requestBanner()
    {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
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

    public void hideBanner()
    {
            bannerView.Hide();
    }

    public void showBanner()
    {
            bannerView.Show();
    }

    private void requestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
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

    public void showIfLoaded()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }

    private void requestRewardBasedVideo()
    {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideoForRevive.LoadAd(request, adUnitId);
    }

    public void HandleRewardBasedVideoLoaded(object sender, System.EventArgs args)
    {
        isRewarded = false;
    }
    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Rewarded video ad failed to load: " + args.Message);
        // Handle the ad failed to load event.
    }
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        puzzleStageController.reviveStage();
        isRewarded = true;
    }
    public void HandleRewardBasedVideoClosed(object sender, System.EventArgs args)
    {
        requestRewardBasedVideo();
        if(!isRewarded)
            puzzleStageController.resetStage();
    }

    public void userOptToWatchReviveAd()
    {
        if (rewardBasedVideoForRevive.IsLoaded())
        {
            rewardBasedVideoForRevive.Show();
        }
    }
}
