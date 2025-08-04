//using UnityEngine;
//using UnityEngine.Advertisements;
//using System;

//namespace GamersStories
//{
//	public class UnityAds : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
//	{
//		string m_androidGameId = "5620448";
//		string m_androidRewardedId = "Rewarded_Android";

//		#pragma warning disable 0414
//		string m_iOSGameId = "5620449";
//		string m_iOSRewardedId = "Rewarded_iOS";

//		bool isRewardedLoaded = false;
//		[SerializeField] bool _testMode = true;

//		private string m_gameId;
//		private string m_rewardedId;

//		Action callback = null;

//		void Start()
//		{
//			InitAds();
//		}

//		void Update()
//		{
//			if (Advertisement.isInitialized == true && isRewardedLoaded == false)
//			{
//				Advertisement.Load(m_rewardedId, this);
//			}
//		}

//		/// <summary>広告を初期化</summary>
//		public void InitAds()
//		{
//			// プラットフォームを判定
//#if UNITY_IOS
//            m_gameId = m_iOSGameId;
//			m_rewardedId = m_iOSRewardedId;
//#elif UNITY_ANDROID
//			m_gameId = m_androidGameId;
//			m_rewardedId = m_androidRewardedId;
//#elif UNITY_EDITOR
//			m_gameId = m_androidGameId; //Only for testing the functionality in the Editor
//			m_rewardedId = m_androidRewardedId;
//#endif

//			// 広告の初期化
//			if (!Advertisement.isInitialized && Advertisement.isSupported)
//			{
//				Advertisement.Initialize(m_gameId, _testMode, this);
//			}
//		}

//		/// <summary>初期化成功</summary>
//		public void OnInitializationComplete()
//		{
//			Debug.Log("初期化が成功しました");
//		}

//		/// <summary>初期化失敗</summary>
//		public void OnInitializationFailed(UnityAdsInitializationError error, string message)
//		{
//			Debug.Log("初期化が失敗しました：" + message);

//			InitAds();
//		}

//		/// <summary>ロード成功</summary>
//		public void OnUnityAdsAdLoaded(string adUnitId)
//		{
//			isRewardedLoaded = true;
//			Debug.Log("読み込みが成功しました");
//		}

//		/// <summary>ロード失敗</summary>
//		public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
//		{
//			Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
//		}

//		/// <summary>広告を表示</summary>
//		public void ShowAds(Action action)
//		{
//			if (Advertisement.isInitialized == false)
//				return;
//			if (isRewardedLoaded == false)
//				return;

//			callback = null;
//			callback = action;
//			Advertisement.Show(m_rewardedId, this);
//		}

//		/// <summary>広告表示成功</summary>
//		public void OnUnityAdsShowStart(string _adUnitId)
//		{
//			Debug.Log("広告が表示されました");
//			callback.Invoke();
//		}

//		/// <summary>広告表示失敗</summary>
//		public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
//		{
//			Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
//			callback.Invoke();
//		}

//		public void OnUnityAdsShowClick(string placementId)
//		{
//			//throw new System.NotImplementedException();
//		}

//		public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
//		{
//			//throw new System.NotImplementedException();
//		}
//	}
//}
