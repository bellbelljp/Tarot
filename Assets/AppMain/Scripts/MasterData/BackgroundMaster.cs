using System;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class BackgroundMaster : Singleton<BackgroundMaster>
	{
		const string BG_PATH = "BG/BG{0}";

		// FIXME:gb名も取得しなきゃだから、Master作るよ
		/// <summary>画像セット</summary>
		public void SetSprite(int bgId, Action<Sprite> callback)
		{
			var path = string.Format(BG_PATH, bgId);
			Sprite sp = Resources.Load<Sprite>(path);
			callback(sp);
		}
	}
}
