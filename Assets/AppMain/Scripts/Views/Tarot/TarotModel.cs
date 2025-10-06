using System.Collections.Generic;
using UnityEngine;

namespace Tarot
{
	public class TarotModel
	{
		public int Genre { get; set; } = -1;
		public int LeftCardIndex { get; set; } = -1;
		public int CenterCardIndex { get; set; } = -1;
		public int RightCardIndex { get; set; } = -1;
		public bool LeftCardDirection { get; set; } = true;
		public bool CenterCardDirection { get; set; } = true;
		public bool RightCardDirection { get; set; } = true;
		public float HalfWidth { get; set; } = 0;
		public float HalfHeight { get; set; } = 0;
		public List<GameObject> CardList { get; set; } = new List<GameObject>();

		const string CARD_PATH = "Cards/{0:d2}";
		const int CARD_NUM = 22;

		public void Init()
		{
			LeftCardIndex = -1;
			CenterCardIndex = -1;
			RightCardIndex = -1;
			LeftCardDirection = true;
			CenterCardDirection = true;
			RightCardDirection = true;
		}

		public Sprite LoadCardImage(int value)
		{
			var path = string.Format(CARD_PATH, value);
			return Resources.Load<Sprite>(path);
		}

		public int GetCardNum()
		{
			return CARD_NUM;
		}
	}
}
