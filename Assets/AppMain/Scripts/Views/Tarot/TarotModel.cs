using System.Collections.Generic;
using UnityEngine;

namespace Tarot
{
	public class TarotModel
	{
		int m_genre = -1;
		public int Genre { get { return m_genre; } set { m_genre = value; } }
		int m_leftCardIndex = -1;
		public int LeftCardIndex { get { return m_leftCardIndex; } set { m_leftCardIndex = value; } }
		int m_centerCardIndex = -1;
		public int CenterCardIndex { get { return m_centerCardIndex; } set { m_centerCardIndex = value; } }
		int m_rightCardIndex = -1;
		public int RightCardIndex { get { return m_rightCardIndex; } set { m_rightCardIndex = value; } }
		bool m_leftCardDirection = true;
		public bool LeftCardDirection { get { return m_leftCardDirection; } set { m_leftCardDirection = value; } }
		bool m_centerCardDirection = true;
		public bool CenterCardDirection { get { return m_centerCardDirection; } set { m_centerCardDirection = value; } }
		bool m_rightCardDirection = true;
		public bool RightCardDirection { get { return m_rightCardDirection; } set { m_rightCardDirection = value; } }
		float m_halfWidth = 0;
		public float HalfWidth { get { return m_halfWidth; } set { m_halfWidth = value; } }
		float m_halfHeight = 0;
		public float HalfHeight { get { return m_halfHeight; } set { m_halfHeight = value; } }

		List<GameObject> m_cardList = new List<GameObject>();
		public List<GameObject> CardList { get { return m_cardList; } set { m_cardList = value; } }

		const string CARD_PATH = "Cards/{0:d2}";
		const int CARD_NUM = 22;

		public void Init()
		{
			m_leftCardIndex =
			m_centerCardIndex =
			m_rightCardIndex = -1;
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
