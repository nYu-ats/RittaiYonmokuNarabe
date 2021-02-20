using GoogleMobileAds.Api;
using UnityEngine;
using System;

public class DisplayAdvertise: MonoBehaviour
{
    [SerializeField] int adDisplayFrequency = 3;
    private InterstitialAd interstitial;
    private const string adUnitId = "ca-app-pub-5395055065464098/4370175999";
    private bool adClose = false;
    public bool AdClose{get {return adClose;}}

    void Start(){
        MobileAds.Initialize(initStatus => {});
        RequestInterstitial();
    }
    
    public void ShowAdvertise(){
        if(HomeController.HomeReadCount % adDisplayFrequency == 0){
            if(this.interstitial.IsLoaded()){
                this.interstitial.Show();
            }
            else{
                adClose = true;
            }
        }
        else{
            adClose = true;
        }
    }
    private void RequestInterstitial()
    {
        //インタースティシャルオブジェクトの作成
        this.interstitial = new InterstitialAd(adUnitId);
        //広告の読み込み
        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        adClose = true;
        interstitial.Destroy();
        this.interstitial.OnAdClosed -= HandleOnAdClosed;
    }
}