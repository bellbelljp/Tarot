using UnityEngine;
//using GoogleMobileAds.Api;
using System;

namespace Tarot
{
	public class GoogleInterstitial : MonoBehaviour
	{
//#if UNITY_ANDROID
//		string m_adUnitId = "ca-app-pub-3940256099942544/1033173712";// テスト
//#elif UNITY_IPHONE
//    string m_adUnitId = "ca-app-pub-3940256099942544/4411468910";
//#else
//    string m_adUnitId = "unexpected_platform";
//#endif
//		private InterstitialAd m_interstitialAd;

//		Action m_getHandler = null;
//		Action m_errorHandler = null;

		//private void Start()
		//{
		//	LoadAd();
		//}

		//public void LoadAd()
		//{
		//	if (m_interstitialAd != null)
		//	{
		//		DestroyAd();
		//	}
		//	var adRequest = new AdRequest();

		//	InterstitialAd.Load(m_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
		//	{
		//		// エラー時
		//		if (error != null)
		//		{
		//			//TODO
		//			FailedReward();
		//			return;
		//		}
		//		// 不明な理由で操作失敗時。予期しないエラー。
		//		else if (ad == null)
		//		{
		//			//TODO
		//			FailedReward();
		//			return;
		//		}

		//		m_interstitialAd = ad;
		//		// イベント登録
		//		RegisterEventHandlers(ad);
		//	});
		//}

		///// <summary>広告表示</summary>
		//public void ShowAd(Action getHandler, Action errorHandler)
		//{
		//	if (m_interstitialAd != null && m_interstitialAd.CanShowAd())
		//	{
		//		m_interstitialAd.Show();

		//		m_getHandler = null;
		//		m_errorHandler = null;

		//		m_getHandler = getHandler;
		//		m_errorHandler = errorHandler;
		//	}
		//	else
		//	{
		//		m_errorHandler = null;
		//		m_errorHandler = errorHandler;
		//		errorHandler.Invoke();
		//		Debug.LogError("ShowAdエラー");
		//	}
		//}

		///// <summary>イベント登録</summary>
		//void RegisterEventHandlers(InterstitialAd ad)
		//{
		//	// 報酬獲得時
		//	ad.OnAdFullScreenContentClosed += () =>
		//	{
		//		EarnedReward();
		//	};

		//	// 失敗時
		//	ad.OnAdFullScreenContentFailed += (AdError error) =>
		//	{
		//		Debug.LogError("OnAdFullScreenContentFailed : " + error);
		//		FailedReward();
		//	};
		//}

		///// <summary>報酬獲得</summary>
		//void EarnedReward()
		//{
		//	DestroyAd();

		//	m_getHandler.Invoke();
		//	// 次の報酬をロードしておく
		//	LoadAd();
		//}

		///// <summary>報酬獲得失敗時</summary>
		//void FailedReward()
		//{
		//	DestroyAd();

		//	m_errorHandler.Invoke();
		//	// 次の報酬をロードしておく
		//	LoadAd();
		//}

		///// <summary>広告削除</summary>
		//void DestroyAd()
		//{
		//	if (m_interstitialAd != null)
		//	{
		//		m_interstitialAd.Destroy();
		//		m_interstitialAd = null;
		//	}
		//}
	}
}