using System.Collections.Generic;
using UnityEngine;

namespace Tarot
{
	public class TarotModel
	{
		int m_genre = -1;
		public int Genre { get { return m_genre; } set { m_genre = value; } }
		int m_leftCardIndex = -1;
		int m_centerCardIndex = -1;
		int m_rightCardIndex = -1;
		bool m_leftCardDirection = true;
		bool m_centerCardDirection = true;
		bool m_rightCardDirection = true;
		float m_halfWidth = 0;
		public float HalfHeight { get { return m_halfHeight; } set { m_halfHeight = value; } }
		public float HalfWidth { get { return m_halfWidth; } set { m_halfWidth = value; } }
		float m_halfHeight = 0;

		List<GameObject> m_cardList = new List<GameObject>();
		public List<GameObject> CardList { get { return m_cardList; } set { m_cardList = value; } }

		const string CARD_PATH = "Cards/{0:d2}";
		const int CARD_NUM = 22;


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
