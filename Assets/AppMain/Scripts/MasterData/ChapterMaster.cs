using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class ChapterMaster : Singleton<ChapterMaster>
	{
		[Serializable]
		public class Data
		{
			public int ChapterId;
			public int CharaId;
			public string ChapterName;
			public string JpName;
		}

		const string SHEET_ID = "1QWVuhcTa6Om1vDPktLjobF3GFjimMS2Xhalaf1JhlbA";
		const string SHEET_NAME = "Chapter";
		const string FILE_NAME = "ChapterData.csv";
		const string FILE_PATH = "Assets/StreamingAssets/Chapter/";

		List<Data> m_dataList = new List<Data>();

		// Googleスプレッドシートからデータを取得するときに使用する
		/// <summary>チャプターデータ作成用</summary>
		//private async void Awake()
		//{
		//	await LoadData();
		//}

		public async UniTask<List<Data>> LoadDataByChara(int charaId)
		{
			try
			{
				if (m_dataList == null || m_dataList.Count == 0)
				{
					m_dataList = await LoadAllData();
				}
				return m_dataList.FindAll(it => it.CharaId == charaId);
			}
			catch (OperationCanceledException e)
			{
				return null;
			}
		}

		public async UniTask<List<Data>> LoadData()
		{
			try
			{
				if (m_dataList == null || m_dataList.Count == 0)
				{
					m_dataList = await LoadAllData();
				}
				return m_dataList;
			}
			catch (OperationCanceledException e)
			{
				return null;
			}
		}

		/// <summary>チャプターデータロード</summary>
		public async UniTask<List<Data>> LoadAllData()
		{
			try
			{
				// マスター変更したときはコメントアウトを外してMasterデータを格納する
				//{
				//	string data = await SpreadSheetReader.Instance.LoadSpreadSheet(SHEET_ID, SHEET_NAME);
				//	var filePath = FILE_PATH + FILE_NAME;
				//	SaveSpreadSheet(filePath, data);
				//	return null;
				//}

				var chapterData = await ReadCsvFileAsync(FILE_NAME);
				if (string.IsNullOrEmpty(chapterData))
				{
					return null;
				}
				else
				{
					m_dataList = await GetDataList(chapterData);
					return m_dataList;
				}
			}
			catch (OperationCanceledException e)
			{
				return null;
			}
		}

		/// <summary>スプレッドシートから取得したデータを保存する</summary>
		void SaveSpreadSheet(string filePath, string text)
		{
			File.WriteAllText(filePath, text);
			Debug.Log("CSV file saved to: " + filePath);
		}

		/// <summary>CSVファイルを非同期で読み込む</summary>
		async UniTask<string> ReadCsvFileAsync(string fileName)
		{
			var filePath = string.Format("{0}/Chapter/{1}", Application.streamingAssetsPath, fileName);
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

		/// <summary>チャプターデータ取得</summary>
		async UniTask<List<Data>> GetDataList(string sheetText)
		{
			try
			{
				List<string[]> cells = new List<string[]>(SpreadSheetReader.Instance.GetCellsData(sheetText));

				// データ格納
				List<Data> dataList = new List<Data>();
				foreach (var cell in cells)
				{
					Data data = new Data();
					data.ChapterId = cell[0].ReplaceCellToInt();
					data.CharaId = cell[1].ReplaceCellToInt();
					//data.ChapterName = await cell[2].ReplaceCell().Localize("Chapter");
					data.JpName = cell[3].ReplaceCell();
					dataList.Add(data);
				}
				return dataList;

			}
			catch (OperationCanceledException e)
			{
				return null;
			}
		}

		/// <summary>データ取得</summary>
		public async UniTask<Data> GetData(int charaId, int chapterId)
		{
			try
			{
				if (m_dataList == null || m_dataList.Count == 0)
				{
					m_dataList = await LoadAllData();
				}
				return m_dataList.Find(it => it.CharaId == charaId && it.ChapterId == chapterId);
			}
			catch (OperationCanceledException e)
			{
				return null;
			}
		}

		/// <summary>データ取得</summary>
		public async UniTask<Data> GetFirstChapter(int charaId)
		{
			try
			{
				if (m_dataList == null || m_dataList.Count == 0)
				{
					m_dataList = await LoadAllData();
				}
				var firstChapterId = m_dataList.FindAll(it => it.CharaId == charaId).Min(it => it.ChapterId);
				return m_dataList.Find(it => it.CharaId == firstChapterId);
			}
			catch (OperationCanceledException e)
			{
				return null;
			}
		}
	}
}
