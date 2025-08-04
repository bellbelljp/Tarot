using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace JourneysOfRealPeople
{
	/// <summary>Googleスプレッドシートから読み込み</summary>
	public class SpreadSheetReader : Singleton<SpreadSheetReader>
	{
		/// <summary>スプレッドシート読み込み</summary>
		public async UniTask<string> LoadSpreadSheet(string id, string name)
		{
			UnityWebRequest request = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/" + id + "/gviz/tq?tqx=out:csv&sheet=" + name);
			await request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log(request.error);
				return null;
			}
			else
			{
				return request.downloadHandler.text;
			}
		}

		/// <summary>セルデータ取得</summary>
		public List<string[]> GetCellsData(string sheetTexts)
		{
			List<string[]> cells = new List<string[]>();
			// StringReader:文字列を読み込むための型
			StringReader reader = new StringReader(sheetTexts);
			// 1行目を取り出す
			reader.ReadLine();
			// Peek():StringReaderに文字がない場合に-1を返す
			while (reader.Peek() != -1)
			{
				// 1行ずつ読み込み
				string line = reader.ReadLine();
				// 行のセルは,で区切られる。セルごとに分けて配列化
				string[] elements = line.Split(",");
				cells.Add(elements);
			}
			return cells;
		}
	}
}
