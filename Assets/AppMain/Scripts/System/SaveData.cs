using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;

namespace Tarot
{
	public class SaveData : Singleton<SaveData>
	{
		[Serializable]
		public class CommonSaveData
		{
			public List<Data> DataList = new List<Data>();
			public string YourName = string.Empty;
			public string MyName = string.Empty;
			// GamerStoriesの名残
			public List<ChapterMaster.Data> ChapterList = new List<ChapterMaster.Data>();
			public List<RewardMaster.Data> RewardList = new List<RewardMaster.Data>();
		}
		CommonSaveData m_commonSaveData = new CommonSaveData();
		public CommonSaveData CommonData { get { return m_commonSaveData; } }

		[Serializable]
		public class Data
		{
			public int SaveSlotNo = 0;  // 0はオートセーブ
			public int Phase = 0;
			public int CharaId = 0;
			public int PlaceId = 0;
			public int LastDateCount = 0;
			public List<int> LastCharaIdList = new List<int>();
			public List<int> SelectCharaIdList = new List<int>();
			public List<CharacterData.Data> CharacteDataList = new List<CharacterData.Data>();
			public string DateTime = string.Empty;
			public string ScreenshotName = string.Empty;

			/// <summary>適切な形でDateTimeを取得する</summary>
			public DateTime GetDateTime()
			{
				if (!string.IsNullOrEmpty(DateTime))
				{
					return System.DateTime.Parse(DateTime, null, DateTimeStyles.RoundtripKind);
				}
				return new DateTime(); // デフォルトのDateTime
			}

			public async UniTask Initialize()
			{
				//for (int i = 0; i < await CharacterMaster.Instance.Count(); i++)
				//{
				//	var charaData = new CharacterData.Data();
				//	charaData.CharacterId = i + 1;
				//	CharacteDataList.Add(charaData);
				//	LastCharaIdList.Add(i + 1);
				//}
				//Phase = 10100;
			}
		}

		public Queue<UIGameViewLogWindow.Log> Logs = new Queue<UIGameViewLogWindow.Log>();

		private async void Awake()
		{
			m_commonSaveData = await LoadCommonData();
		}

		/// <summary>フォルダとファイルの存在チェック</summary>
		public async UniTask Check(string folderPath, string filePath)
		{
			if (Directory.Exists(folderPath))
			{

			}
			else
			{
				Directory.CreateDirectory(folderPath);
			}
			await CheckFile(filePath);
		}

		/// <summary>ファイルの存在をチェック</summary>
		async UniTask CheckFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				// 存在しているのでOK
				return;
			}
			else
			{
				await CreateNewData(filePath);
			}
		}

		/// <summary>新規データ作成</summary>
		/// <param name="filePath"></param>
		async UniTask CreateNewData(string filePath)
		{
			// 新規作成
			var savedata = new SaveData.Data();
			await savedata.Initialize();
			// Jsonに変換
			var json = JsonUtility.ToJson(savedata);

			// 書き込み
			// isAppend = true:追記, false:上書き
			bool isAppend = false;
			using (var fw = new StreamWriter(filePath, isAppend, Encoding.GetEncoding("UTF-8")))
			{
				fw.Write(json);
			}
		}

		/// <summary>スクショ取得</summary>
		public async UniTask<string> GetScreenShot(int saveSlotIndex = 0)
		{
			string folderPath = Application.persistentDataPath + "/savedata";
			var fileName = string.Format("save{0}.png", saveSlotIndex);
			var filePath = string.Format("{0}/{1}", folderPath, fileName);
			await TakeScreenShotAndSaveAsync(filePath);
			return fileName;
		}

		/// <summary>スクショ撮る</summary>
		async UniTask TakeScreenShotAndSaveAsync(string filePath)
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			ScreenCapture.CaptureScreenshot(filePath);
		}

		/// <summary>セーブ</summary>
		public void Save(Data data, int saveSlotIndex = 0)
		{
			data.DateTime = DateTime.Now.ToString("o");
			data.SaveSlotNo = saveSlotIndex;

			// スクショをスロット毎に保存しなおす
			if (saveSlotIndex != 0)
			{
				string folderPath = Application.persistentDataPath + "/savedata";
				var fileName = $"save{saveSlotIndex}.png";
				var newScreenshotFilePath = string.Format("{0}/{1}", folderPath, fileName);
				var oldScreenshotFilePath = string.Format("{0}/{1}", folderPath, data.ScreenshotName);
				if (File.Exists(oldScreenshotFilePath))
				{
					File.Copy(oldScreenshotFilePath, newScreenshotFilePath, overwrite: true);
					data.ScreenshotName = fileName;
				}
			}

			var saveSlot = m_commonSaveData.DataList.Find(it => it.SaveSlotNo == saveSlotIndex);
			if (saveSlot == null)
			{
				data.SaveSlotNo = saveSlotIndex;
				m_commonSaveData.DataList.Add(data);
			}
			else
			{
				var slotIndex = m_commonSaveData.DataList.FindIndex(it => it == saveSlot);
				m_commonSaveData.DataList[slotIndex] = data;
			}

			SaveCommonData();
		}

		public async void SaveCommonData()
		{
			var json = JsonUtility.ToJson(m_commonSaveData);

			string folderPath = Application.persistentDataPath + "/savedata";
			string filePath = folderPath + "/savedata.json";
			await Check(folderPath, filePath);

			bool isAppend = false;
			using (var fw = new StreamWriter(filePath, isAppend, Encoding.GetEncoding("UTF-8")))
			{
				fw.Write(json);
			}
		}

		/// <summary>ロード</summary>
		public async UniTask<Data> Load(int saveSlot = 0)
		{
			string folderPath = Application.persistentDataPath + "/savedata";
			string filePath = folderPath + "/savedata.json";
			await Check(folderPath, filePath);

			using (var sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
			{
				// データ読み込み
				string result = sr.ReadToEnd();
				if (!string.IsNullOrEmpty(result))
				{
					var dataList = JsonUtility.FromJson<CommonSaveData>(result);
					if (dataList != null)
					{
						var data = dataList.DataList.Find(it => it.SaveSlotNo == saveSlot);
						if (data != null)
							return data;
					}
				}
			}

			await CreateNewData(filePath);
			using (var sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
			{
				// データ読み込み
				string result = sr.ReadToEnd();
				return JsonUtility.FromJson<Data>(result);
			}
		}

		/// <summary>セーブデータをリストでロード</summary>
		public async UniTask<CommonSaveData> LoadCommonData()
		{
			string folderPath = Application.persistentDataPath + "/savedata";
			string filePath = folderPath + "/savedata.json";
			await Check(folderPath, filePath);

			using (var sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
			{
				// データ読み込み
				string result = sr.ReadToEnd();
				if (!string.IsNullOrEmpty(result))
					return JsonUtility.FromJson<CommonSaveData>(result);
			}

			await CreateNewData(filePath);
			using (var sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
			{
				// データ読み込み
				string result = sr.ReadToEnd();
				return JsonUtility.FromJson<CommonSaveData>(result);
			}
		}

		/// <summary>スクショロード</summary>
		public async UniTask<Sprite> LoadScreenshotAsync(string fileName)
		{
			string folderPath = Application.persistentDataPath + "/savedata";
			string filePath = string.Format("{0}/{1}", folderPath, fileName);
			if (File.Exists(filePath))
			{
				byte[] imageBytes = File.ReadAllBytes(filePath);
				Texture2D texture = new Texture2D(2, 2);
				texture.LoadImage(imageBytes);

				await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

				return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			}
			return null;
		}

		/// <summary>リセットデータ</summary>
		public void Reset()
		{
			var lang = PlayerPrefs.GetString("selected-locale", "ja");
			m_commonSaveData.DataList.Clear();
			m_commonSaveData.YourName = string.Empty;
			m_commonSaveData.MyName = string.Empty;
			m_commonSaveData.ChapterList.Clear();
			m_commonSaveData.RewardList.Clear();
			SaveCommonData();
			PlayerPrefs.DeleteAll();
			// Langだけは登録しなおす
			PlayerPrefs.SetString("selected-locale", lang);

			try
			{
				var path = Application.persistentDataPath;
				DeleteFilesInDirectory(path);

				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("削除中にエラー：" + ex.Message);
			}
		}

		void DeleteFilesInDirectory(string directoryPath)
		{
			if (Directory.Exists(directoryPath))
			{
				foreach (var path in Directory.GetFiles(directoryPath))
				{
					File.Delete(path);
				}

				foreach (var subDirectoryPath in Directory.GetDirectories(directoryPath))
				{
					DeleteFilesInDirectory(subDirectoryPath);
					Directory.Delete(subDirectoryPath, true);
				}
			}
		}

		///// <summary>リワードを持っているか</summary>
		//public bool HasReward(RewardData.Data rewardData)
		//{
		//	var reward = m_commonSaveData.RewardList.Find(it => it.MasterId == rewardData.MasterId);
		//	return reward != null;
		//}

		///// <summary>デバッグ用全リワード取得</summary>
		//public async UniTask GetAllReward()
		//{
		//	var rewardList = await RewardData.Instance.GetAll();
		//	m_commonSaveData.RewardList = new List<RewardData.Data>(rewardList);
		//}
	}
}
