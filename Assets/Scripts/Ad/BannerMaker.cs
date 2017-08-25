using GoogleMobileAds.Api;
using UnityEngine;
using System.Collections;

public static class BannerMaker {
    public static void requestBanner() {
        #if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the bottom of the screen.
        BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
        .Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
}
