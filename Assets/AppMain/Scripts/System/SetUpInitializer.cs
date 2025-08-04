using UnityEngine;
using Cysharp.Threading.Tasks;
//using GoogleMobileAds.Api;
//using Steamworks;

namespace JourneysOfRealPeople
{
	public class SetUpInitializer : MonoBehaviour
	{
		public static bool IsInit { get; private set; } = false;

		const string ENDING_REWARD = "Ending{0}";
		const string BELL_ENDING = "BellEnding";
		const string KIN_ENDING = "KinEnding";
		const string CASEY_ENDING = "CaseyEnding";
		const string ALL_ENDING = "AllEnding";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		static async UniTask Init()
		{
			//await StoryData.Instance.LoadData();
			//await ChapterData.Instance.LoadAllData();
			//await EndingData.Instance.LoadData();
			//await CharacterMaster.Instance.LoadData();
			////SaveData.Instance.Reset();
			//await RewardData.Instance.LoadData();
			// バナー広告初期化
			//MobileAds.Initialize(initStatus => { });
			//CheckSteam();
		}

		private void Awake()
		{
			Destroy(gameObject);
			IsInit = true;
		}

		///// <summary>実績取得確認</summary>
		//static async void CheckSteam()
		//{
		//	try
		//	{
		//		if (SteamManager.Initialized)
		//		{
		//			// ユーザーの現在のデータと実績を非同期に要求後（必須）
		//			if (SteamUserStats.RequestCurrentStats())
		//			{
		//				// エンディングデータ
		//				var rewardList = SaveData.Instance.CommonData.RewardList;

		//				foreach (var reward in rewardList)
		//				{
		//					var endingData = await EndingData.Instance.GetDataFromReward(reward.MasterId);
		//					if (endingData == null)
		//						continue;
		//					// statsを更新
		//					// エンディング実績を更新
		//					string stat = string.Format(ENDING_REWARD, endingData.EndingId);
		//					SteamUserStats.SetStat(stat, 1);
		//				}

		//				// キャラ総合実績を更新
		//				var bellRewardCount = RewardData.Instance.CharaRewardCount(1);
		//				SteamUserStats.SetStat(BELL_ENDING, bellRewardCount);
		//				var kinRewardCount = RewardData.Instance.CharaRewardCount(2);
		//				SteamUserStats.SetStat(KIN_ENDING, kinRewardCount);
		//				var caseyRewardCount = RewardData.Instance.CharaRewardCount(3);
		//				SteamUserStats.SetStat(CASEY_ENDING, caseyRewardCount);

		//				// 総合実績を更新
		//				var allRewardNum = rewardList.Count;
		//				SteamUserStats.SetStat(ALL_ENDING, allRewardNum);

		//				// 更新を反映
		//				bool bSuccess = SteamUserStats.StoreStats();
		//				Debug.Log(bSuccess);

		//				//int ending = 0;
		//				//int bell = 0;
		//				//int kin = 0;
		//				//int casey = 0;
		//				//int all = 0;
		//				////データの取得(apiがAPI名でstring、valueが取得するデータでintかfloat)
		//				//SteamUserStats.GetStat(BELL_ENDING, out bell);
		//				//Debug.Log(string.Format("{0}:{1}", BELL_ENDING, bell));
		//				//SteamUserStats.GetStat(KIN_ENDING, out kin);
		//				//Debug.Log(string.Format("{0}:{1}", KIN_ENDING, kin));
		//				//SteamUserStats.GetStat(CASEY_ENDING, out casey);
		//				//Debug.Log(string.Format("{0}:{1}", CASEY_ENDING, casey));
		//				//SteamUserStats.GetStat(ALL_ENDING, out all);
		//				//Debug.Log(string.Format("{0}:{1}", ALL_ENDING, all));
		//				return;
		//			}
		//		}
		//	}
		//	catch
		//	{

		//	}
		//}
	}
}
