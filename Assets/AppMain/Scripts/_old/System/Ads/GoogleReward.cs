using UnityEngine;
//using GoogleMobileAds.Api;
using System;

namespace JourneysOfRealPeople
{
	public class GoogleReward : MonoBehaviour
	{
//#if UNITY_ANDROID
//		string m_adUnitId = "ca-app-pub-3940256099942544/5224354917";   // テスト
//#elif UNITY_IPHONE
//   string m_adUnitId = "ca-app-pub-3940256099942544/1712485313";
//#else
//   string m_adUnitId = "unexpected_platform";
//#endif

//		Action m_getHandler = null;
//		Action m_errorHandler = null;

//		private RewardedAd m_rewardedAd = null;
//		bool m_isEarnReward = false;

//		private void Start()
//		{
//			LoadAd();
//		}

//		void LoadAd()
//		{
//			if(m_rewardedAd != null)
//			{
//				DestroyAd();
//			}

//			var adRequest = new AdRequest();

//			RewardedAd.Load(m_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
//			{
//				// エラー時
//				if (error != null)
//				{
//					//TODO
//					Debug.LogError("LoadAdError: " + error);
//					FailedReward();
//					return;
//				}
//				// 不明な理由で操作失敗時。予期しないエラー。
//				if (ad == null)
//				{
//					// TODO
//					Debug.LogError("RewardedAd is null");
//					FailedReward();
//					return;
//				}

//				m_rewardedAd = ad;
//				RegisterEventHandlers(ad);
//				Debug.Log("Rewarded Ad loaded successfully");
//			});
//		}

//		/// <summary>広告表示</summary>
//		public void ShowAd(Action getHandler, Action errorHandler)
//		{
//			if(m_rewardedAd != null && m_rewardedAd.CanShowAd())
//			{
//				m_rewardedAd.Show((Reward reward) =>
//				{
//					Debug.Log(string.Format("Reward ad granted a reward:{0}{1}",
//						reward.Amount, reward.Type));
//					m_isEarnReward = true;

//#if UNITY_EDITOR
//					getHandler.Invoke();
//#else
//					m_getHandler = null;
//					m_errorHandler = null;
					
//					m_getHandler = getHandler;
//					m_errorHandler = errorHandler;
//#endif
//				});
//			}
//			else
//			{
//				m_errorHandler = null;
//				m_errorHandler = errorHandler;
//				errorHandler.Invoke();
//				Debug.LogError("ShowAdエラー" + m_rewardedAd);
//			}
//		}

//		/// <summary>イベント登録</summary>
//		void RegisterEventHandlers(RewardedAd ad)
//		{
//			// 報酬獲得時
//			ad.OnAdFullScreenContentClosed += () =>
//			{
//				EarnedReward();
//			};

//			// 失敗時
//			ad.OnAdFullScreenContentFailed += (AdError error) =>
//			{
//				Debug.LogError("OnAdFullScreenContentFailed : " + error);
//				FailedReward();
//			};
//		}

//		/// <summary>報酬獲得</summary>
//		void EarnedReward()
//		{
//			DestroyAd();
//#if UNITY_EDITOR

//#else
//			if (m_isEarnReward)
//			{
//				m_getHandler.Invoke();
//			}
//			else
//			{
//				m_errorHandler.Invoke();
//			}
//#endif
//			// 次の報酬をロードしておく
//			LoadAd();
//		}

//		/// <summary>報酬獲得失敗時</summary>
//		void FailedReward()
//		{
//			DestroyAd();
//			m_errorHandler.Invoke();
//			// 次の報酬をロードしておく
//			LoadAd();
//		}

//		/// <summary>広告削除</summary>
//		void DestroyAd()
//		{
//			if(m_rewardedAd != null)
//			{
//				m_rewardedAd.Destroy();
//				m_rewardedAd = null;
//			}
//		}
	}
}
