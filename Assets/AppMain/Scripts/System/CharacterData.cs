using System.Collections.Generic;
using System.Globalization;
using System;
using UnityEngine;
using JetBrains.Annotations;

namespace Tarot
{
	public class CharacterData : Singleton<SaveData>
	{
		[Serializable]
		public class Data
		{
			public int CharacterId = 0;
			//public int LoveParam = 0;    // 愛情度
			public Dictionary<int, int> FriendParam = new Dictionary<int, int>();   // <FriendId, Param>他の子の友情度
			public List<int> OpenProfile = new List<int>();     // 解放プロフィール
			public List<int> OpenEvent = new List<int>();   // 見たイベントId
			public List<int> OpenStill = new List<int>();   // 解放済みスチル
			public List<int> GoodFriend = new List<int>();  // 仲良しなフレンド
			public List<int> BadFriend = new List<int>();   // 嫌いな人
			public Feeling MyFeeling = Feeling.Normal;   // 気分（嬉しい、普通、悲しい、怒り）

			public enum Feeling
			{
				Happy,
				Normal,
				Sad,
				Bad,
			}
		}
	}
}
