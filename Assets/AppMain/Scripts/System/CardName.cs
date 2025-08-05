using UnityEngine;

namespace Tarot
{
	public class CardName
	{
		public static string GetGenre(int index)
		{
			switch (index)
			{
				case 100: return "恋愛";
				case 200: return "仕事";
				case 300: return "家庭";
				case 400: return "お金";
				case 500: return "健康";
				case 600: return "勉強";
				default: return "エラー";
			}
		}
		/// <summary>カードの位置名を取得</summary>
		public static string GetDirection(bool isNormal)
		{
			return isNormal ? "正" : "逆";
		}

		/// <summary>タロット名を取得</summary>
		public static string GetTarotName(int cardNumber)
		{
			switch (cardNumber)
			{
				case 0: return "愚者";
				case 1: return "魔術師";
				case 2: return "女教皇";
				case 3: return "女帝";
				case 4: return "皇帝";
				case 5: return "法王";
				case 6: return "恋人";
				case 7: return "戦車";
				case 8: return "力";
				case 9: return "隠者";
				case 10: return "運命の輪";
				case 11: return "正義";
				case 12: return "吊るされた男";
				case 13: return "死神";
				case 14: return "節制";
				case 15: return "悪魔";
				case 16: return "塔";
				case 17: return "星";
				case 18: return "月";
				case 19: return "太陽";
				case 20: return "審判";
				case 21: return "世界";
				default: return "不明なカード";
			}
		}
	}
}

