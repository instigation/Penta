using System;
namespace Penta
{
    public interface IAdGiver
    {
        void hideBanner();
        void showBanner();
        void tryToShowInterstitial();
        void tryToShowRewardBasedVideo();
        // Gives 0 if Ad never occured after the start.
        double latestAdClosedTimeFromStartInSeconds();
    }
}
