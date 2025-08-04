using UnityEngine;
//using GoogleMobileAds.Api;

namespace JourneysOfRealPeople
{
	public class GoogleBanner : MonoBehaviour
	{
//#if UNITY_ANDROID
//		string m_adUnitId = "ca-app-pub-3940256099942544/6300978111"; // テスト用広告ユニットID
//#elif UNITY_IPHONE
//		string m_adUnitId = "ca-app-pub-3940256099942544/2934735716"; // テスト用広告ユニットID
//#else
//		string m_adUnitId = "unexpected_platform";
//#endif
//		private BannerView m_bannerView;
		//public void Start()
		//{
		//	//MobileAds.Initialize(initStatus => { });
		//	LoadAd();
		//}

		///// <summary>バナー広告作成</summary>
		//void CreateBannerView()
		//{
		//	//Debug.Log("バナー広告作成");
		//	if(m_bannerView != null)
		//	{
		//		DestroyAd();
		//	}

		//	// 320 * 50のバナーをボトムに作成
		//	m_bannerView = new BannerView(m_adUnitId, AdSize.Banner, AdPosition.Bottom);

		//	ListenToAdEvents();
		//}
		
		///// <summary>広告をロード</summary>
		//public void LoadAd()
		//{
		//	if(m_bannerView == null)
		//	{
		//		CreateBannerView();
		//	}

		//	var adRequest = new AdRequest();

		//	m_bannerView.LoadAd(adRequest);
		//}

		//void ListenToAdEvents()
		//{
		//	m_bannerView.OnBannerAdLoaded += () =>
		//	{
		//		//Debug.Log(m_bannerView.GetResponseInfo());
		//	};

		//	m_bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
		//	{
		//		Debug.LogError("エラー：" + error);
		//	};
		//}

		///// <summary>広告削除</summary>
		//void DestroyAd()
		//{
		//	if(m_bannerView != null)
		//	{
		//		m_bannerView.Destroy();
		//		m_bannerView = null;
		//	}
		//}
	}
}
