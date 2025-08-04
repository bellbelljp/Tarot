using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class CharacterMaster : Singleton<CharacterMaster>
	{
		/// <summary>キャラクターのパラメータ</summary>
		[Serializable]
		public class Data
		{
			public int MasterId = 0;
			public string Name = string.Empty;
			public string Intro = string.Empty;
			public bool Hide = false;
			public string NameWithCV = string.Empty;
		}

		const string CHARA_PATH = "Character/{0:d2}/{1}";
		//const string REWARD_PATH = "Picture/{0:D2}";

		List<Data> m_dataList = new List<Data>();
		const string SHEET_ID = "1QWVuhcTa6Om1vDPktLjobF3GFjimMS2Xhalaf1JhlbA";
		const string SHEET_NAME = "Character";
		const string DEFAULT_CHARA = "Normal";
		const string FILE_NAME = "CharacterData.csv";
		const string FILE_PATH = "Assets/StreamingAssets/Character/{0}";

		//// Googleスプレッドシートからデータを取得するときに使用する
		///// <summary>キャラクターデータ作成用</summary>
		//private async void Awake()
		//{
		//	LoadData().Forget();
		//}

		#region LoadData
		/// <summary>データロード</summary>
		public async UniTask<List<Data>> LoadData()
		{
			//// マスター変更したときはコメントアウトを外してMasterデータを格納する
			//{
			//	string data = await SpreadSheetReader.Instance.LoadSpreadSheet(SHEET_ID, SHEET_NAME);
			//	SaveSpreadSheet(FILE_NAME, data);
			//	return null;
			//}

			var	characterData = await ReadCsvFileAsync(FILE_NAME);
			if (string.IsNullOrEmpty(characterData))
			{
				return null;
			}
			else
			{
				m_dataList = await GetData(characterData);
				return m_dataList;
			}
		}

		/// <summary>スプレッドシートから取得したデータを保存する</summary>
		void SaveSpreadSheet(string fileName, string text)
		{
			var path = string.Format(FILE_PATH, fileName);
			File.WriteAllText(path, text);
			Debug.Log("CSV file saved to: " + fileName);
		}

		/// <summary>CSVファイルを非同期で読み込む</summary>
		async UniTask<string> ReadCsvFileAsync(string fileName)
		{
			// Assets/StreamingAssets/Character/FILE_NAME
			var filePath = string.Format("{0}/Character/{1}", Application.streamingAssetsPath, fileName);
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

		/// <summary>データ取得</summary>
		async UniTask<List<Data>> GetData(string sheetText)
		{
			List<string[]> cells = new List<string[]>(SpreadSheetReader.Instance.GetCellsData(sheetText));

			// データ格納
			List<Data> dataList = new List<Data>();
			foreach (var cell in cells)
			{
				Data data = new Data();
				//data.MasterId = cell[0].ReplaceCellToInt();
				//data.Name = await cell[1].ReplaceCell().Localize("Character");
				//data.Intro = await cell[2].ReplaceCell().Localize("Character");
				//data.Hide = cell[3].ReplaceCellToBool();
				//data.NameWithCV = await cell[4].ReplaceCell().Localize("Character");
				data.MasterId = cell[0].ReplaceCellToInt();
				data.Name = cell[1].ReplaceCell();
				data.Intro = cell[2].ReplaceCell();
				data.Hide = cell[3] == "true" ? true : false;
				data.NameWithCV = cell[4].ReplaceCell();
				dataList.Add(data);
			}
			return dataList;
		}
		#endregion

		public async UniTask<int> Count()
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList.Count;
		}

		/// <summary>全データ取得</summary>
		public async UniTask<List<Data>> GetAllExceptHide()
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			return m_dataList.FindAll(it => !it.Hide);
		}

		/// <summary>キャラ名取得</summary>
		public async UniTask<string> GetCharacterName(int characterNumber)
		{
			if (m_dataList == null || m_dataList.Count == 0)
			{
				m_dataList = await LoadData();
			}
			foreach (var param in m_dataList)
			{
				var typeInt = param.MasterId;
				if (typeInt == characterNumber)
					return param.Name;
			}
			return null;
		}

		/// <summary>画像セット</summary>
		public void SetSprite(/*string fileName,*/int charaId, Action<Sprite> callback)
		{
			var path = string.Format(CHARA_PATH, charaId);
			//var charaId = int.Parse(Regex.Replace(fileName, @"[^0-9]", ""));
			//var path = string.Format(CHARA_PATH, charaId, fileName);
			Sprite sp = Resources.Load<Sprite>(path);
			callback(sp);
		}

		/// <summary>画像セット</summary>
		public void SetSprite(string fileName, Action<Sprite> callback)
		{
			var charaId = int.Parse(Regex.Replace(fileName, @"[^0-9]", ""));
			var path = string.Format(CHARA_PATH, charaId, fileName);
			Sprite sp = Resources.Load<Sprite>(path);
			callback(sp);
		}

		/// <summary>画像セット</summary>
		public void SetDefaultSprite(int charaId, Action<Sprite> callback)
		{
			var path = string.Format(CHARA_PATH, charaId, charaId + DEFAULT_CHARA);
			Sprite sp = Resources.Load<Sprite>(path);
			callback(sp);
		}

		///// <summary>報酬写真セット</summary>
		//public void SetRewardPicture(int charaId, Action<Sprite> callback)
		//{
		//	var path = string.Format(REWARD_PATH, charaId);
		//	// べると菌のスチルの一か所のみ対応
		//	if (charaId == 0102)
		//		path = string.Format("Picture/{0:D4}", charaId);
		//	Sprite sp = Resources.Load<Sprite>(path);
		//	callback(sp);
		//}
	}
}
