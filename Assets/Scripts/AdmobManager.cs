// //using GoogleMobileAds.Api;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// using UnityEngine.Events;
// using GoogleMobileAds.Common;
// using UnityEngine.UI;
// using System;
// using GoogleMobileAds.Api;
//
// public class AdmobManager : MonoBehaviour
// {
//     public static AdmobManager instance;
//     [Header("COnfig")]
//     [Header("Interstitial ADS")]
//     private string InterstitialAd_Android_ID = "ca-app-pub-9179752697212712/4369945140";
//     private string InterstitialAd_IOS_ID = "ca-app-pub-9179752697212712/6636601519";
//     [Header("Reward ADS")]
//     private string RewardedAd_Android_ID = "ca-app-pub-9179752697212712/2219436281";
//     private string RewardedAd_IOS_ID = "ca-app-pub-9179752697212712/9070181442";
//     [Header("Native ADS")]
//     private string NativeAd_Android_ID = "ca-app-pub-9179752697212712/4654027937";
//     private string NativeAd_IOS_ID = "ca-app-pub-9179752697212712/9620984391";
//
//     [Header("Banner ADS")]
//     private string Banner_Android_ID = "ca-app-pub-9179752697212712/1763548784";
//     private string Banner_IOS_ID = "ca-app-pub-9179752697212712/1384274835";
//
//     [Header("Banner2 ADS")]
//     private string Banner2_Android_ID = "ca-app-pub-9179752697212712/2416294059";
//     private string Banner2_IOS_ID = "ca-app-pub-9179752697212712/1384274835";
//
//     private BannerView bannerView;
//     private BannerView banner2View;
//     private InterstitialAd interstitialAd;
//     private RewardedAd rewardedAd;
//     private bool isRewardAd_Loaded = false;
//
//
//
//     public enum ADS_CALLBACK_STATE
//     {
//         SUCCESS,
//         FAIL
//     }
//
//    
//
//     private void Awake()
//     {
//         instance = this;
//         DontDestroyOnLoad(this);
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         MobileAds.SetiOSAppPauseOnBackground(true);
//
//         List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
//
//         // Add some test device IDs (replace with your own device IDs).
// #if UNITY_IPHONE
//         deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
// #elif UNITY_ANDROID
//         deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
//         deviceIds.Add("864816020118760");
//         deviceIds.Add("864816020118778");
//         deviceIds.Add("495ECB59AE9A355C");
// #endif
//
//
//         // Configure TagForChildDirectedTreatment and test device IDs.
//         RequestConfiguration requestConfiguration =
//             new RequestConfiguration.Builder()
//             .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
//             .SetTestDeviceIds(deviceIds).build();
//
//         MobileAds.SetRequestConfiguration(requestConfiguration);
//
//         // Initialize the Google Mobile Ads SDK.
//         MobileAds.Initialize(HandleInitCompleteAction);
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
//
//
//     private void HandleInitCompleteAction(InitializationStatus initstatus)
//     {
//         // Callbacks from GoogleMobileAds are not guaranteed to be called on
//         // main thread.
//         // In this example we use MobileAdsEventExecutor to schedule these calls on
//         // the next Update() loop.
//         MobileAdsEventExecutor.ExecuteInUpdate(() =>
//         {
//             Init_InterstitialAd();
//             InitLoadRewardedAd();
//             //Init_Banner();
//             //InitLoadNativeAd();
//         });
//     }
//
//     #region HELPER METHODS
//
//     private AdRequest CreateAdRequest()
//     {
//         return new AdRequest.Builder()
//             .AddTestDevice(AdRequest.TestDeviceSimulator)
//             .TagForChildDirectedTreatment(false)
//             .AddExtra("color_bg", "9B30FF")
//             .Build();
//     }
//
//     #endregion
//
//
//     #region INTERSTITIAL ADS
//     public const int TIME_SHOWINTERTITIAL_NOT_SHOWINTERTITIAL = 45;
//     public long timeLastShowIntertitial= 0;
//     public void Init_InterstitialAd()
//     {
// #if UNITY_EDITOR
//         string adUnitId = "unused";
// #elif UNITY_ANDROID
//             string adUnitId = InterstitialAd_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = InterstitialAd_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // Clean up interstitial before using it
//         if (interstitialAd != null)
//         {
//             interstitialAd.Destroy();
//         }
//
//         interstitialAd = new InterstitialAd(adUnitId);
//
//         // Add Event Handlers
//         interstitialAd.OnAdLoaded += HandleOnAdLoaded_InterstitialAd;
//         interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad_InterstitialAd;
//         interstitialAd.OnAdOpening += HandleOnAdOpened_InterstitialAd;
//         interstitialAd.OnAdClosed += HandleOnAdClosed_InterstitialAd;
//         interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication_InterstitialAd;
//
//         // Load an interstitial ad
//         interstitialAd.LoadAd(CreateAdRequest());
//     }
//     public void RequestAndLoadInterstitialAd()
//     {
//         Init_InterstitialAd();
//     }
//     private void ShowInterstitialAd()
//     {
//         
//         if (interstitialAd.IsLoaded())
//         {
//             interstitialAd.Show();
//             FirebaseManager.Instance.LogShowInter(Config.currLevel);
//         }
//         else
//         {
//             RequestAndLoadInterstitialAd();
//         }
//     }
//
//     public void DestroyInterstitialAd()
//     {
//         if (interstitialAd != null)
//         {
//             interstitialAd.Destroy();
//         }
//     }
//
//
//     public void HandleOnAdLoaded_InterstitialAd(object sender, EventArgs args)
//     {
//         Debug.Log("HandleOnAdLoaded_InterstitialAd event received");
//
//     }
//
//     public void HandleOnAdFailedToLoad_InterstitialAd(object sender, AdFailedToLoadEventArgs args)
//     {
//         Debug.Log("HandleOnAdFailedToLoad_InterstitialAd event received with message: "
//                             + args.Message);
//     }
//
//     public void HandleOnAdOpened_InterstitialAd(object sender, EventArgs args)
//     {
//         Debug.Log("HandleOnAdOpened_InterstitialAd event received");
//         
//     }
//
//     public void HandleOnAdClosed_InterstitialAd(object sender, EventArgs args)
//     {
//         timeLastShowIntertitial = Config.GetTimeStamp();
//         Debug.Log("HandleOnAdClosed_InterstitialAd event received");
//         InterstitialAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
//         RequestAndLoadInterstitialAd();
//     }
//
//     public void HandleOnAdLeavingApplication_InterstitialAd(object sender, EventArgs args)
//     {
//         Debug.Log("HandleOnAdLeavingApplication_InterstitialAd event received");
//     }
//
//
//     public bool isInterstititalAds_Avaiable()
//     {
//         if (Config.GetRemoveAd()) return false;
//         if (Config.GetTimeStamp() - timeLastShowReward <= TIME_SHOWREWARD_NOT_SHOWINTERTITIAL)
//         {
//             Debug.Log("NOT Show    InterstitialAd");
//             return false;
//         }
//         Debug.Log("isInterstititalAds_AvaiableisInterstititalAds_AvaiableisInterstititalAds_Avaiable");
//         Debug.Log(Config.GetTimeStamp());
//         Debug.Log(timeLastShowIntertitial);
//         Debug.Log(Config.GetTimeStamp() - timeLastShowIntertitial);
//         if (Config.GetTimeStamp() - timeLastShowIntertitial <= TIME_SHOWINTERTITIAL_NOT_SHOWINTERTITIAL)
//         {
//             Debug.Log("NOT Show AAAAAAAAAAAAAAAAAA  InterstitialAd");
//             return false;
//         }
//         if (interstitialAd.IsLoaded())
//         {
//             return true;
//         }
//         else
//         {
//             RequestAndLoadInterstitialAd();
//         }
//         return false;
//     }
//
//     private Action<ADS_CALLBACK_STATE> InterstitialAd_CallBack = delegate (ADS_CALLBACK_STATE _state) { };
//     public void ShowInterstitialAd_CallBack(Action<ADS_CALLBACK_STATE> _interstitialAd_CallBack) {
//         InterstitialAd_CallBack = _interstitialAd_CallBack;
//         ShowInterstitialAd();
// // #if UNITY_EDITOR
// //         InterstitialAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
// // #endif
//     }
//     #endregion
//
//
//     #region REWARDED ADS
//     public const int TIME_SHOWREWARD_NOT_SHOWINTERTITIAL = 30;
//     public long timeLastShowReward = 0;
//
//     public void InitLoadRewardedAd()
//     {
//         isRewardAd_Loaded = false;
// #if UNITY_EDITOR
//         isRewardAd_Loaded = true;
//         string adUnitId = "unused";
// #elif UNITY_ANDROID
//             string adUnitId = RewardedAd_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = RewardedAd_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // create new rewarded ad instance
//         rewardedAd = new RewardedAd(adUnitId);
//
//         // Add Event Handlers
//         rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//         rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//         rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//         rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//         rewardedAd.OnAdClosed += HandleRewardedAdClosed;
//         rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//
//         // Create empty ad request
//         rewardedAd.LoadAd(CreateAdRequest());
//     }
//     public void RequestAndLoadRewardedAd()
//     {
//
//         Debug.Log("RequestAndLoadRewardedAdRequestAndLoadRewardedAd");
//         
//
//         isRewardAd_Loaded = false;
// #if UNITY_EDITOR
//         isRewardAd_Loaded = true;
//         string adUnitId = "unused";
// #elif UNITY_ANDROID
//             string adUnitId = RewardedAd_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = RewardedAd_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // create new rewarded ad instance
//         rewardedAd = new RewardedAd(adUnitId);
//
//         // Add Event Handlers
//         rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//         rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//         rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//         rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//         rewardedAd.OnAdClosed += HandleRewardedAdClosed;
//         rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//
//         // Create empty ad request
//         rewardedAd.LoadAd(CreateAdRequest());
//     }
//
//     private void ShowRewardedAd()
//     {
//         Debug.Log("ShowRewardedAdShowRewardedAdShowRewardedAd");
//         if (rewardedAd != null && isRewardAd_Loaded)
//         {
//             rewardedAd.Show();
//            
//         }
//         else
//         {
//             RequestAndLoadRewardedAd();
//         }
//     }
//
//
//     public void HandleRewardedAdLoaded(object sender, EventArgs args)
//     {
//         Debug.Log("HandleRewardedAdLoaded event received");
//         isRewardAd_Loaded = true;
//     }
//
//     public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
//     {
//         Debug.Log(
//             "HandleRewardedAdFailedToLoad event received with message: "
//                              + args.Message);
//        
//         RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.FAIL);
//     }
//
//     public void HandleRewardedAdOpening(object sender, EventArgs args)
//     {
//         Debug.Log("HandleRewardedAdOpening event received");
//     }
//
//     public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//     {
//         Debug.Log(
//             "HandleRewardedAdFailedToShow event received with message: "
//                              + args.Message);
//
//         RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.FAIL);
//     }
//
//     public void HandleRewardedAdClosed(object sender, EventArgs args)
//     {
//         Debug.Log("HandleRewardedAdClosed event received");
//
//         // RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.FAIL);
//         RequestAndLoadRewardedAd();
//     }
//
//     public void HandleUserEarnedReward(object sender, Reward args)
//     {
//         string type = args.Type;
//         double amount = args.Amount;
//         Debug.Log(
//             "HandleRewardedAdRewarded event received for "
//                         + amount.ToString() + " " + type);
//
//         timeLastShowReward = Config.GetTimeStamp();
//         RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
//         FirebaseManager.Instance.LogRewarded();
//         //RequestAndLoadRewardedAd();
//     }
//
//     private Action<ADS_CALLBACK_STATE> RewardAd_CallBack = delegate (ADS_CALLBACK_STATE _state) { };
//     public void ShowRewardAd_CallBack(Action<ADS_CALLBACK_STATE> _rewardAd_CallBack, string where = "", int level = 0)
//     {
//         RewardAd_CallBack = _rewardAd_CallBack;
//         ShowRewardedAd();
// // #if UNITY_EDITOR
// //         RewardAd_CallBack.Invoke(ADS_CALLBACK_STATE.SUCCESS);
// // #endif
//     }
//
//     public bool isRewardAds_Avaiable()
//     {
//         if (rewardedAd != null && isRewardAd_Loaded)
//         {
//             return true;
//         }
//         else
//         {
//             RequestAndLoadRewardedAd();
//         }
//         return false;
//     }
//
//     #endregion
//
//    
//     #region BANNER_ADS
//     public void Init_Banner()
//     {
//         if (Config.GetRemoveAd()) return;
// #if UNITY_EDITOR
//         string adUnitId = "unused";
// #elif UNITY_ANDROID
//             string adUnitId = Banner_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = Banner_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // Clean up interstitial before using it
//         if (bannerView != null)
//         {
//             bannerView.Destroy();
//         }
//
//         bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
//
//         // Add Event Handlers
//         bannerView.OnAdLoaded += HandleOnAdLoaded_BanenrAd;
//         bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad_BanenrAd;
//         bannerView.OnAdOpening += HandleOnAdOpened_BanenrAd;
//         bannerView.OnAdClosed += HandleOnAdClosed_BanenrAd;
//         bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication_BanenrAd;
//
//         // Load an interstitial ad
//         bannerView.LoadAd(CreateAdRequest());
//     }
//
//     public void Request_Banner()
//     {
//         if (Config.GetRemoveAd()) return;
// #if UNITY_EDITOR
//         string adUnitId = "unused";
//         return;
// #elif UNITY_ANDROID
//             string adUnitId = Banner_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = Banner_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // Clean up interstitial before using it
//         if (bannerView != null)
//         {
//             bannerView.Destroy();
//         }
//
//         AdSize adSize = new AdSize(320, 250);
//         bannerView = new BannerView(adUnitId, adSize, AdPosition.Top);
//
//         // Add Event Handlers
//         bannerView.OnAdLoaded += HandleOnAdLoaded_BanenrAd;
//         bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad_BanenrAd;
//         bannerView.OnAdOpening += HandleOnAdOpened_BanenrAd;
//         bannerView.OnAdClosed += HandleOnAdClosed_BanenrAd;
//         bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication_BanenrAd;
//
//         // Load an interstitial ad
//         bannerView.LoadAd(CreateAdRequest());
//     }
//
//
//     public void HandleOnAdLoaded_BanenrAd(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdLoaded event received");
//         
//     }
//
//     public void HandleOnAdFailedToLoad_BanenrAd(object sender, AdFailedToLoadEventArgs args)
//     {
//         MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
//                             + args.Message);
//     }
//
//     public void HandleOnAdOpened_BanenrAd(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdOpened event received");
//     }
//
//     public void HandleOnAdClosed_BanenrAd(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdClosed event received");
//     }
//
//     public void HandleOnAdLeavingApplication_BanenrAd(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdLeavingApplication event received");
//     }
//
//     public void ShowBannerAd() {
//         if (bannerView != null)
//         {
//             bannerView.Show();
//         }
//     }
//
//     public void HideBannerAd()
//     {
//         if (bannerView != null)
//         {
//             bannerView.Hide();
//         }
//     }
//     #endregion
//
//
//     #region BANNER_ADS
//     public void Init_Banner2()
//     {
//         if (Config.GetRemoveAd()) return;
// #if UNITY_EDITOR
//         string adUnitId = "unused";
// #elif UNITY_ANDROID
//             string adUnitId = Banner2_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = Banner2_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // Clean up interstitial before using it
//         if (banner2View != null)
//         {
//             banner2View.Destroy();
//         }
//
//         banner2View = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
//
//         // Add Event Handlers
//         banner2View.OnAdLoaded += HandleOnAdLoaded_Banenr2Ad;
//         banner2View.OnAdFailedToLoad += HandleOnAdFailedToLoad_Banenr2Ad;
//         banner2View.OnAdOpening += HandleOnAdOpened_Banenr2Ad;
//         banner2View.OnAdClosed += HandleOnAdClosed_Banenr2Ad;
//         banner2View.OnAdLeavingApplication += HandleOnAdLeavingApplication_Banenr2Ad;
//
//         // Load an interstitial ad
//         banner2View.LoadAd(CreateAdRequest());
//     }
//
//     public void Request_Banner2()
//     {
//         if (Config.GetRemoveAd()) return;
// #if UNITY_EDITOR
//         string adUnitId = "unused";
//         return;
// #elif UNITY_ANDROID
//             string adUnitId = Banner2_Android_ID;
// #elif UNITY_IPHONE
//             string adUnitId = Banner2_IOS_ID;
// #else
//             string adUnitId = "unexpected_platform";
// #endif
//
//         // Clean up interstitial before using it
//         if (banner2View != null)
//         {
//             banner2View.Destroy();
//         }
//
//         banner2View = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
//
//         // Add Event Handlers
//         banner2View.OnAdLoaded += HandleOnAdLoaded_Banenr2Ad;
//         banner2View.OnAdFailedToLoad += HandleOnAdFailedToLoad_Banenr2Ad;
//         banner2View.OnAdOpening += HandleOnAdOpened_Banenr2Ad;
//         banner2View.OnAdClosed += HandleOnAdClosed_Banenr2Ad;
//         banner2View.OnAdLeavingApplication += HandleOnAdLeavingApplication_Banenr2Ad;
//
//         // Load an interstitial ad
//         banner2View.LoadAd(CreateAdRequest());
//     }
//
//
//     public void HandleOnAdLoaded_Banenr2Ad(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdLoaded event received");
//
//     }
//
//     public void HandleOnAdFailedToLoad_Banenr2Ad(object sender, AdFailedToLoadEventArgs args)
//     {
//         MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
//                             + args.Message);
//     }
//
//     public void HandleOnAdOpened_Banenr2Ad(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdOpened event received");
//     }
//
//     public void HandleOnAdClosed_Banenr2Ad(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdClosed event received");
//     }
//
//     public void HandleOnAdLeavingApplication_Banenr2Ad(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleAdLeavingApplication event received");
//     }
//
//     public void ShowBanner2Ad()
//     {
//         if (banner2View != null)
//         {
//             banner2View.Show();
//         }
//     }
//
//     public void HideBanner2Ad()
//     {
//         if (banner2View != null)
//         {
//             banner2View.Hide();
//         }
//     }
//     #endregion
//
//    
//
//     private void OnDestroy()
//     {
//         if (bannerView != null) {
//             bannerView.Destroy();
//             banner2View.Destroy();
//         }
//     }
//
//     public void DestroyBanner() {
//         if (bannerView != null)
//         {
//             bannerView.Destroy();
//             banner2View.Destroy();
//         }
//     }
// }
