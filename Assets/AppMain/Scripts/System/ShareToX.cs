using UnityEngine;
using System.Web; // .NET Framework 用のエンコード（必要に応じて）

namespace Tarot
{
	public class ShareToX : MonoBehaviour
	{
		// シェアしたいテキストとURL（改行などもOK）
		string tweetText = "タロットをプレイしたよ！#Unity1Week";
		string tweetURL = "https://unityroom.com/games/bellbelljp"; // 任意（ストアページなど）

		public void Share()
		{
			// URLエンコード（Unity標準ではWWW.EscapeURLを使う）
			string text = WWW.EscapeURL(tweetText);
			string url = WWW.EscapeURL(tweetURL);

			string twitterURL = $"https://twitter.com/intent/tweet?text={text}&url={url}";

			Application.OpenURL(twitterURL);
		}
	}
}

