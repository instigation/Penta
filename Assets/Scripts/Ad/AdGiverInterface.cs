using System;
namespace Penta
{
    public interface IAdGiver
    {
        void requestBanner();
        void hideBanner();
        void showBanner();
        void showIfLoaded();
        void userOptToWatchReviveAd();
    }
}
