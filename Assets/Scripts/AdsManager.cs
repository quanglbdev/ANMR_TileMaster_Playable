using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : Singleton<AdsManager>
{
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private string bannerAdId;
    private string interstitialAdId;
    private string rewardAdId;

    public static int AD_COOLDOWN_AFTER_INTERSTITIAL = 25;
    public static int AD_COOLDOWN_AFTER_REWARD = 30;
    public static int MIN_LEVEL_SHOW_AD = 4;
    public static bool SHOW_BANNER = true;

    private DateTime lastInterstitialTime;
    private DateTime lastRewardTime;


    // Start is called before the first frame update
    void Start()
    {
        //Config.NoAdsPurchased += DestroyBannerAd;
#if UNITY_ANDROID
        bannerAdId = "ca-app-pub-3940256099942544/6300978111";
        interstitialAdId = "ca-app-pub-3940256099942544/1033173712";
        rewardAdId = "ca-app-pub-3940256099942544/5224354917";
#endif

        MobileAds.SetiOSAppPauseOnBackground(true);

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

        // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
        deviceIds.Add("32A53CF9C36FD07D");
        deviceIds.Add("38FC5CF3A5A0CA78");
        deviceIds.Add("3D551E4F4F30C4E1");
        deviceIds.Add("31F6664B69EB314B");
#endif

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(initStatus => { });
        MobileAds.Initialize(HandleInitCompleteAction);

        lastInterstitialTime = DateTime.Now;
        lastRewardTime = DateTime.Now;
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        Debug.Log("Initialization complete.");

        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // the main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            PrintStatus("Initialization complete.");
#if !FREE_ADS
            //RequestBanner();
#endif
            RequestReward();
            RequestInterstitial();
        });
    }

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .Build();
    }

    #endregion


    #region BANNER

    private void RequestBanner()
    {
        if (!SHOW_BANNER)
            return;
        if (!Config.GetRemoveAd())
        {
            PrintStatus("Requesting Banner ad.");

            // Clean up banner before reusing
            if (bannerView != null)
            {
                bannerView.Destroy();
            }
#if UNITY_EDITOR
            bannerView = new BannerView(bannerAdId, new AdSize(320, 100), AdPosition.Bottom);
#else
            bannerView = new BannerView(bannerAdId, AdSize.SmartBanner, AdPosition.Bottom);
#endif

            // Add Event Handlers
            bannerView.OnAdLoaded += HandleOnBannerAdLoaded;
            bannerView.OnAdFailedToLoad += HandleOnBannerAdFailedToLoad;
            bannerView.OnAdOpening += HandleOnBannerAdOpened;
            bannerView.OnAdClosed += HandleOnBannerAdClosed;
            bannerView.OnPaidEvent += HandleOnBannerAdPaid;

            // Load a banner ad
            bannerView.LoadAd(CreateAdRequest());
        }
    }

    private void HandleOnBannerAdClosed(object sender, EventArgs e)
    {
    }

    private void HandleOnBannerAdOpened(object sender, EventArgs e)
    {
    }

    private void HandleOnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        Invoke("RequestBanner", 5);
    }

    private void HandleOnBannerAdLoaded(object sender, EventArgs e)
    {
    }

    private void HandleOnBannerAdPaid(object sender, EventArgs e)
    {
    }

    [Button]
    public void HideBanner()
    {
        if (bannerView != null)
            bannerView.Hide();
    }

    [Button]
    public void UnhideBanner()
    {
        if (bannerView != null)
            bannerView.Show();
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    #endregion

    #region INTERSTITIAL

    private void RequestInterstitial()
    {
        PrintStatus("Requesting Interstitial ad.");
        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        // Initialize an InterstitialAd.
        interstitialAd = new InterstitialAd(interstitialAdId);

        // Add Event Handlers
        interstitialAd.OnAdLoaded += HandleOnInterstitialAdLoaded;
        interstitialAd.OnAdFailedToLoad += HandleOnInterstitialAdFailedToLoad;
        interstitialAd.OnAdOpening += HandleOnInterstitialAdOpened;
        interstitialAd.OnAdClosed += HandleOnInterstitialAdClosed;
        interstitialAd.OnAdDidRecordImpression += HandleOnInterstitialAdRecordImpression;
        interstitialAd.OnAdFailedToShow += HandleOnInterstitialAdFailedToShow;
        interstitialAd.OnPaidEvent += HandleOnInterstitialAdPaid;


        interstitialAd.LoadAd(CreateAdRequest());
    }

    public void ShowInterstitialAd()
    {
        if (Config.GetRemoveAd())
            return;

        if ((Config.GetDateTimeNow() - lastInterstitialTime).TotalSeconds < AD_COOLDOWN_AFTER_INTERSTITIAL)
            return;
        if ((Config.GetDateTimeNow() - lastRewardTime).TotalSeconds < AD_COOLDOWN_AFTER_REWARD)
            return;
        FirebaseManager.Instance.LogEligibleInter();
        if (interstitialAd != null && interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
        else
        {
            PrintStatus("Interstitial ad is not ready yet.");
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    private void HandleOnInterstitialAdClosed(object sender, EventArgs e)
    {
        RequestInterstitial();
        lastInterstitialTime = DateTime.Now;
    }

    private void HandleOnInterstitialAdOpened(object sender, EventArgs e)
    {
        FirebaseManager.Instance.LogShowInter();
    }

    private void HandleOnInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        FirebaseManager.Instance.LogShowInterFail();
        Invoke("RequestInterstitial", 5);
    }

    private void HandleOnInterstitialAdRecordImpression(object sender, EventArgs e)
    {        
    }

    private void HandleOnInterstitialAdFailedToShow(object sender, EventArgs e)
    {

    }

    private void HandleOnInterstitialAdLoaded(object sender, EventArgs e)
    {
        FirebaseManager.Instance.LogLoadInterSuccess();
    }

    private void HandleOnInterstitialAdPaid(object sender, EventArgs e)
    {
    }

    #endregion

    #region REWARD

    FirebaseManager.RewardFor rewardFor;

    private void RequestReward()
    {
        PrintStatus("Requesting Reward ad.");
        // Clean up interstitial before using it
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }

        // Initialize an RewardedAd.
        rewardedAd = new RewardedAd(rewardAdId);

        // Add Event Handlers
        rewardedAd.OnAdLoaded += HandleOnRewardAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleOnRewardAdFailedToLoad;
        rewardedAd.OnAdOpening += HandleOnRewardAdOpened;
        rewardedAd.OnAdClosed += HandleOnRewardAdClosed;
        rewardedAd.OnAdDidRecordImpression += HandleOnRewardAdRecordImpression;
        rewardedAd.OnAdFailedToShow += HandleOnRewardAdFailedToShow;
        rewardedAd.OnPaidEvent += HandleOnRewardAdPaid;
        rewardedAd.OnUserEarnedReward += HandleOnRewardAdEarnedReward;

        rewardedAd.LoadAd(CreateAdRequest());
    }

    public bool isRewardAds_Available => rewardedAd != null && rewardedAd.IsLoaded();

    private void ShowRewardedAd(FirebaseManager.RewardFor reward)
    {
        FirebaseManager.Instance.LogEligibleReward();
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            rewardFor = reward;
            rewardedAd.Show();
            
        }
        else
        {
            PrintStatus("Rewarded ad is not ready yet.");
            NotificationPopup.instance.AddNotification("Ad not ready");
            NotReadyAd_CallBack?.Invoke();
        }
    }

    private void HandleOnRewardAdClosed(object sender, EventArgs e)
    {
        if (ClosedAd_CallBack != null)
            ClosedAd_CallBack.Invoke();
        RequestReward();
        lastRewardTime = DateTime.Now;
    }

    private void HandleOnRewardAdOpened(object sender, EventArgs e)
    {
        FirebaseManager.Instance.LogShowRewardSuccess();
    }

    private void HandleOnRewardAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        Invoke("RequestReward", 5);
    }

    private void HandleOnRewardAdRecordImpression(object sender, EventArgs e)
    {
    }

    private void HandleOnRewardAdFailedToShow(object sender, EventArgs e)
    {
        FirebaseManager.Instance.LogShowRewardFail();
    }

    private void HandleOnRewardAdLoaded(object sender, EventArgs e)
    {
        FirebaseManager.Instance.LogLoadReward();
    }

    private void HandleOnRewardAdPaid(object sender, EventArgs e)
    {
    }

    private void HandleOnRewardAdEarnedReward(object sender, EventArgs e)
    {
        rewarded = true;
        FirebaseManager.Instance.LogRewarded();
        UnityMainThreadDispatcher.Instance().Enqueue(AdCloseCall());
    }

    private IEnumerator AdCloseCall()
    {
        yield return null;
        RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
        yield return null;
    }

    public enum ADS_CALLBACK_STATE
    {
        SUCCESS,
        FAIL
    }

    private Action ClosedAd_CallBack = delegate() { };
    private Action NotReadyAd_CallBack = delegate() { };
    private Action<ADS_CALLBACK_STATE> RewardAd_CallBack = delegate { };
    private bool rewarded;

    public void ShowRewardAd_CallBack(FirebaseManager.RewardFor rewardFor,
        Action<ADS_CALLBACK_STATE> _rewardAd_CallBack, Action _closedAd_CallBack = null)
    {
        rewarded = false;
        RewardAd_CallBack = _rewardAd_CallBack;
        ClosedAd_CallBack = _closedAd_CallBack;
#if FREE_ADS
        RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
        if (ClosedAd_CallBack != null) ClosedAd_CallBack.Invoke();
#else
        ShowRewardedAd(rewardFor);
#endif
    }
    
    public void ShowRewardAd_CallBack(FirebaseManager.RewardFor rewardFor,
        Action<ADS_CALLBACK_STATE> _rewardAd_CallBack, Action _closedAd_CallBack, Action _notReadyAd_CallBack = null)
    {
        rewarded = false;
        RewardAd_CallBack = _rewardAd_CallBack;
        ClosedAd_CallBack = _closedAd_CallBack;
        NotReadyAd_CallBack = _notReadyAd_CallBack;
#if FREE_ADS
        RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
        if (ClosedAd_CallBack != null) ClosedAd_CallBack.Invoke();
#else
        ShowRewardedAd(rewardFor);
#endif
    }

    #endregion

    #region Utility

    ///<summary>
    /// Log the message and update the status text on the main thread.
    ///<summary>
    private void PrintStatus(string message)
    {
        Debug.Log(message);
    }

    #endregion
}