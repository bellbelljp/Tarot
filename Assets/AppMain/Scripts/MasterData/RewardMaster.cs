using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class RewardMaster : Singleton<RewardMaster>
	{
		/// <summary>報酬のパラメータ</summary>
		[Serializable]
		public class Data
		{
			public int MasterId = 0;
			public int CharaId = 0;
			public int Para = 0;
			public string Name = string.Empty;
			public string Intro = string.Empty;
			public bool IsHide = true;
			public bool IsPicture = false;
		}

		//const string CHARA_PATH = "Chara/{0:D2}/{1}";

		List<Data> m_dataList = new List<Data>();
		const string SHEET_ID = "1US9nVHyOT97j81aw_FyabiIeonOUkEsdubnL33CkIow";
		const string SHEET_NAME = "Reward";
		const string FILE_NAME = "RewardData.csv";

		/// <summary>データロード</summary>
		public async UniTask<List<Data>> LoadData()
		{
			// マスター変更したときはコメントアウトを外してMasterデータを格納する
			{
				//string data = await SpreadSheetReader.Instance.LoadSpreadSheet(SHEET_ID, SHEET_NAME);
				//SaveSpreadSheet(FILE_PATH, data);
				//return null;
			}

			var rewardData = await ReadCsvFileAsync(FILE_NAME);
			if (string.IsNullOrEmpty(rewardData))
			{
				return null;
			}
			else
			{
				m_dataList = await SetData(rewardData);
				return m_dataList;
			}
		}

		/// <summary>スプレッドシートから取得したデータを保存する</summary>
		void SaveSpreadSheet(string filePath, string text)
		{
			File.WriteAllText(filePath, text);
		}

		/// <summary>CSVファイルを非同期で読み込む</summary>
		async UniTask<string> ReadCsvFileAsync(string fileName)
		{
			var filePath = string.Format("{0}/{1}", Application.streamingAssetsPath, fileName);
			if (File.Exists(filePath))
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					string line = await reader.ReadToEndAsync();
					return line;
				}
			}
			return null;
		}

		/// <summary>データ格納</summary>
		async UniTask<List<Data>> SetData(string sheetText)
		{
			List<string[]> cells = new List<string[]>(SpreadSheetReader.Instance.GetCellsData(sheetText));

			// データ格納
			List<Data> dataList = new List<Data>();
			foreach (var cell in cells)
			{
				Data data = new Data();
				data.MasterId = cell[0].ReplaceCellToInt();
				data.CharaId = cell[1].ReplaceCellToInt();
				data.Para = cell[2].ReplaceCellToInt();
				//data.Name = await cell[3].ReplaceCell().Localize("Reward");
				//data.Intro = await cell[4].ReplaceCell().Localize("Reward");
				data.IsHide = cell[5].ReplaceCellToBool();
				data.IsPicture = cell[6].ReplaceCellToBool();
				dataList.Add(data);
			}
			return dataList;
		}

		/// <summary>データ取得</summary>
		public async UniTask<Data> GetData(int masterId)
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList.Find(i => i.MasterId == masterId);
		}

		/// <summary>チャプターからデータ取得</summary>
		public async UniTask<Data> GetDataFromChapter(int charaId, int chapterId)
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			// 1桁目を照らし合わせる
			// chapterId = 2 なら rewardId = X02
			return m_dataList.Find(i => (i.MasterId % 100) == chapterId && i.CharaId == charaId);
		}

		/// <summary>データ取得</summary>
		public async UniTask<List<Data>> GetAll()
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList;
		}

		/// <summary>データ数</summary>
		public async UniTask<int> AllCount()
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList.Count;
		}

		/// <summary>キャラ毎のデータ取得</summary>
		public async UniTask<List<Data>> GetDataByChara(int charaId)
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList.FindAll(i => i.CharaId == charaId);
		}

		/// <summary>キャラ毎のデータ取得</summary>
		public async UniTask<int> CountByChara(int charaId)
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			var charaList = m_dataList.FindAll(i => i.CharaId == charaId);
			return charaList != null ? charaList.Count : 0 ;
		}

		/// <summary>写真報酬を取得できるか</summary>
		public async UniTask<bool> CanGetPictureReward(int charaId)
		{
			var rewardList = SaveData.Instance.CommonData.RewardList.FindAll(it => it.CharaId == charaId);
			var count = await CountByChara(charaId);
			return rewardList.Count == (count - 1) ? true : false;
		}

		/// <summary>キャラの獲得済み報酬数を返す</summary>
		public int CharaRewardCount(int charaId)
		{
			var rewardList = SaveData.Instance.CommonData.RewardList.FindAll(it => it.CharaId == charaId);
			return rewardList.Count;
		}

		/// <summary>写真報酬を取得</summary>
		public async UniTask<Data> GetPictureReward(int charaId)
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList.Find(it => it.CharaId == charaId && it.IsPicture);
		}

		/// <summary>写真報酬を持っているか</summary>
		public async UniTask<bool> HasBellKinPictureReward()
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			var bellId = 1;
			var kinId = 2;
			var hasRewardList = SaveData.Instance.CommonData.RewardList.FindAll(it => it.CharaId == bellId || it.CharaId == kinId);
			var baseRewardList = m_dataList.FindAll(it => it.CharaId == bellId || it.CharaId == kinId);
			return hasRewardList.Count == baseRewardList.Count;
		}
	}
}
