using GoogleMobileAds.Api;
using UnityEngine;

namespace GoogleAdmob
{
    public class DisplayAdvertise
    {
        private InterstitialAd interstitial;
        private const string adUnitId = "ca-app-pub-5395055065464098/4370175999";
        public void RequestInterstitial()
        {
            //インタースティシャルオブジェクトの作成
            this.interstitial = new InterstitialAd(adUnitId);
            //広告の読み込み
            AdRequest request = new AdRequest.Builder().Build();
            this.interstitial.LoadAd(request);
            //広告表示
            this.interstitial.Show();
        }
    }
}