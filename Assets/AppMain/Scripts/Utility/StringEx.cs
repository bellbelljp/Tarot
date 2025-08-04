using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace JourneysOfRealPeople
{
	public static class StringEx
	{
		public static int Count(this string str, char word)
		{
			var count = 0;
			foreach(var c in str)
			{
				if (c == word)
					count++;
			}
			return count;
		}

		/// <summary>ストーリーテキストを、自分の名前、一人称入りで変換する</summary>
		public static string ReplaceName(this string str)
		{
			//str = str.Replace(csdef.MY_NAME, SaveData.Instance.CommonData.MyName);
			//str = str.Replace(csdef.YOUR_NAME, SaveData.Instance.CommonData.YourName);
			return str;
		}

		/// <summary>セルを読み込める形に変換</summary>
		public static string ReplaceCell(this string str)
		{
			// 最初と最後の1つだけダブルクォーテーションを削除
			var trim0 = str.Trim();
			trim0 = trim0.Substring(1, trim0.Length - 2);
			// 連続するダブルクォーテーションを1つに置換
			var trim1 = trim0.Replace("\"\"", "\"");

			var replace0 = trim1.Replace("<br>", "\n");

			return replace0.Replace("<c>", ",");
		}

		/// <summary>セルをIntに変換</summary>
		public static int ReplaceCellToInt(this string str)
		{
			str = ReplaceCell(str);
			// @が含まれていたら、それより前の数字を返す
			if (str.Contains("@"))
				return int.Parse(str.Substring(0, str.IndexOf("@")));
			else if (string.IsNullOrEmpty(str))
				return 0;
			else
			{
				var index = 0;
				int.TryParse(str, out index);
				return index;
			}
		}

		/// <summary>セルを読み込める形に変換</summary>
		public static bool ReplaceCellToBool(this string str)
		{
			var trim0 = str.TrimStart('"').TrimEnd('"');
			return trim0 == "TRUE" ? true : false;
		}

		/// <summary>ポップアップ文言をローカライズする</summary>
		public async static UniTask<string> PopupLocalize(this string entryKey)
		{
			//return await Localize(entryKey, "Popup");
			return null;
		}

		/// <summary>文言をローカライズする</summary>
		public async static UniTask<string> Localize(this string entryKey, string tableName)
		{
			//var localizedString = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, entryKey);
			//return localizedString.Result;
			return await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, entryKey);
		}
	}
}
