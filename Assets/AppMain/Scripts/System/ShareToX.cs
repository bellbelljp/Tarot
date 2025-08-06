using System.Runtime.InteropServices;
using System.Web; // .NET Framework 用のエンコード（必要に応じて）
using UnityEngine;

namespace Tarot
{
	public class ShareToX : MonoBehaviour
	{
		// JavaScript関数のインポート（WebGLでのみ有効）
		[DllImport("__Internal")]
		private static extern void OpenTwitterWindow(string url);

		// シェアしたいテキストとURL（改行などもOK）
		string tweetText = "タロットをプレイしたよ！#unity1week";
		string tweetURL = "https://unityroom.com/games/tarot";
		string[] tweetTags = { "unity1room" };
		string gameId = "tarot";

		public void Share()
		{
			// URLエンコード（Unity標準ではWWW.EscapeURLを使う）
			string text = WWW.EscapeURL(tweetText);
			string url = WWW.EscapeURL(tweetURL);

			string twitterURL = $"https://twitter.com/intent/tweet?text={text}&url={url}";

#if UNITY_WEBGL && !UNITY_EDITOR
			        OpenTwitterWindow(twitterURL);
#else
			Application.OpenURL(twitterURL);
#endif
		}
	}
}

