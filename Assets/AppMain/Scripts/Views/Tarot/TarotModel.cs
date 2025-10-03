using UnityEngine;

namespace Tarot
{
	public class TarotModel
	{
		const string CARD_PATH = "Cards/{0:d2}";

		public Sprite LoadCardImage(int value)
		{
			var path = string.Format(CARD_PATH, value);
			return Resources.Load<Sprite>(path);
		}
	}
}
