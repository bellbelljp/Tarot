using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

namespace Tarot
{
	public class StoryMaster : Singleton<StoryMaster>
	{
		[Serializable]
		public class Data
		{
			[Tooltip("ID")] public int Id = 0;
			[Tooltip("キャラ")] public int Chara = 0;
			[Tooltip("台詞"), Multiline(3)] public string Talk = string.Empty;
			[Tooltip("場所")] public int Place = 0;
			[Tooltip("チャプター")] public int Chapter = 0;
			[Tooltip("パラム")] public int Param = 0;
			[Tooltip("エフェクト")] public string Effect = string.Empty;
			[Tooltip("左")] public string Left = string.Empty;
			[Tooltip("中央")] public string Center = string.Empty;
			[Tooltip("右")] public string Right = string.Empty;
			[Tooltip("BGM")] public string BGM = string.Empty;
			[Tooltip("SE")] public string SE = string.Empty;
			[Tooltip("解放")] public int Release = 0;
		}

		const string JAPANESE = "1yjILaCIhLhfWF9OQZyOyEHqJHnsbMqNFh4gp3NBfs5s";
		const string ENGLISH = "";
		const string TW = "";
		const string CN = "";

		const string COMMON_FILE_NAME = "CommonData{0:D3}_{1}.csv";
		const string COMMON_SHEET_NAME = "CommonData{0:D3}";

		const string CHARA_FILE_NAME = "TalkData{0:D3}_{1}.csv";
		const string CHARA_SHEET_NAME = "TalkData{0:D3}";

		const string VOICE_PATH5 = "Voice/{0:D2}/{0:D2}{1:D5}";

		const string FILE_PATH = "Assets/StreamingAssets/StoryData/";

		List<Data> m_dataList = new List<Data>();

		const int COMMON_STORY_ID = 99; // 共通ストーリーID

		// Googleスプレッドシートからデータを取得するときに使用する
		/// <summary>ストーリーデータ作成用</summary>
		//private async void Awake()
		//{
		//	await LoadData(99, 0);

		//	for (int i = 0; i < 3; i++)
		//	{
		//		await LoadData(i + 1, 0);
		//	}
		//}

		// masterId
		// COMMON_STORY_ID(99):共通
		// 01-98:キャラクター
		// チャプター指定の場合は、serifIdは0を指定すること
		/// <summary>ストーリーデータを台詞IDからロード</summary>
		public async UniTask<List<Data>> LoadData(int masterId, int serifId, int chapterId = 0)
		{
			var lang = PlayerPrefs.GetString("selected-locale", "ja");

			var fileName = string.Empty;
			var sheetName = string.Empty;
			switch (masterId)
			{
				case COMMON_STORY_ID:
					fileName = string.Format(COMMON_FILE_NAME, masterId, lang);
					sheetName = string.Format(COMMON_SHEET_NAME, masterId);
					break;
				default:
					fileName = string.Format(CHARA_FILE_NAME, masterId, lang);
					sheetName = string.Format(CHARA_SHEET_NAME, masterId);
					break;
			}
			string storyData = await ReadCsvFileAsync(fileName);

			#region CreateStoryDataFromGoogleSpreadSheet
			//// MEMO:スプレッドシートから取得したデータを保存する
			////{
			//var sheetId = string.Empty;
			//switch (lang)
			//{
			//	case "ja":
			//		sheetId = JAPANESE;
			//		break;
			//	case "en":
			//		sheetId = ENGLISH;
			//		break;
			//	case "zh-TW":
			//		sheetId = TW;
			//		break;
			//	case "zh-CN":
			//		sheetId = CN;
			//		break;
			//}

			//// 下のStoryDataとdataを入れ替える
			//string data = await SpreadSheetReader.Instance.LoadSpreadSheet(sheetId, sheetName);
			//var filePath = FILE_PATH + string.Format(fileName, masterId, lang);
			//SaveSpreadSheet(filePath, data);
			//return null;
			////}
			#endregion

			if (string.IsNullOrEmpty(storyData))
			{
				return null;
			}
			else
			{
				m_dataList = GetStoryData(storyData, serifId, chapterId);
				return m_dataList;
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
			var filePath = string.Format("{0}/StoryData/{1}", Application.streamingAssetsPath, fileName);
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

		/// <summary>ストーリーデータを読み込む</summary>
		List<Data> GetStoryData(string text, int serifId, int chapterId = 0)
		{
			List<string[]> cells = new List<string[]>();
			// StringReader:文字列を読み込むための型
			StringReader reader = new StringReader(text);
			// 1行目を取り出す
			reader.ReadLine();
			// Peek():StringReaderに文字がない場合に-1を返す
			while (reader.Peek() != -1)
			{
				// 1行ずつ読み込み
				string line = reader.ReadLine();
				string result = string.Empty;
				while (true)
				{
					int dashCount = 0;
					// ,を<c>に変換
					(result, dashCount) = CommaToC(result, line);

					// "(dashCount)が0もしくは奇数ならば、セルが終わっていないので改行
					// 例えば…
					// 
					// "ああああ
					// いいいい
					// うううう"
					// の時
					if (dashCount == 0 || dashCount % 2 == 1)
					{
						line += "<br>";
						line += reader.ReadLine();

						result = string.Empty;
					}
					else
					{
						break;
					}
				}

				// 行のセルは,で区切られる。セルごとに分けて配列化
				string[] elements = result.Split(",");
				cells.Add(elements);
			}

			var storydata = SetStoryData(cells, serifId, chapterId);
			return storydata;
		}

		/// <summary>ストーリーデータにデータ格納</summary>
		List<Data> SetStoryData(List<string[]> cells, int serifId, int chapter = 0)
		{
			string log = "";
			List<Data> stories = new List<Data>();
			foreach (var line in cells)
			{
				var data = new StoryMaster.Data();

				// 最初最後の囲みを消して、カンマ、改行などを元に戻してからデータ格納
				data.Id = line[0].ReplaceCellToInt();
				data.Chara = line[1].ReplaceCellToInt();
				data.Talk = line[2].ReplaceCell();
				data.Place = line[3].ReplaceCellToInt();
				data.Chapter = line[4].ReplaceCellToInt();
				data.Param = line[5].ReplaceCellToInt();
				data.Effect = line[6].ReplaceCell();
				data.Left = line[7].ReplaceCell();
				data.Center = line[8].ReplaceCell();
				data.Right = line[9].ReplaceCell();
				data.BGM = line[10].ReplaceCell();
				data.SE = line[11].ReplaceCell();
				data.Release = line[12].ReplaceCellToInt();

				// 指定されたTalkId以上のみを格納
				if (serifId != 0 && data.Id < serifId)
				{
					continue;
				}

				// 指定されたチャプター以上のみを格納
				if (chapter != 0 && data.Chapter < chapter)
					continue;

				//// 愛情度パラメーター分岐
				//if(loveParam != -1)
				//{
				//	if (loveParam < data.Conditoin)
				//		continue;
				//}

				stories.Add(data);
			}

			return stories;
		}

		/// <summary>カンマを<c> に変換する</summary>
		(string, int) CommaToC(string result, string line)
		{
			int dashCount = 0;
			bool isCell = false;
			foreach (var c in line)
			{
				// セル内で,が使用されていたら、<c>に置き換える
				if (c.ToString() == "," && isCell)
					result += "<c>";
				else
					result += c.ToString();

				// 奇数はセル開始
				// 偶数はセル終了
				if (c.ToString() == "\"")
				{
					dashCount++;
					isCell = !isCell;
				}
			}
			return (result, dashCount);
		}

		/// <summary>ボイス取得</summary>
		public AudioClip GetVoice(int charaId, int serifId)
		{
			string path = string.Empty;
			path = string.Format(VOICE_PATH5, charaId, serifId);
			return Resources.Load<AudioClip>(path);
		}
	}
}
