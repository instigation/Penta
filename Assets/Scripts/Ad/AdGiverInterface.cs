using System;
namespace Penta
{
    public interface IAdGiver
    {
        void hideBanner();
        void showBanner();
        void showInterstitialIfLoaded();
        void showRewardBasedVideoIfLoaded();
        // Gives 0 if Ad never occured after the start.
        double latestAdClosedTimeFromStartInSeconds();
    }
}
