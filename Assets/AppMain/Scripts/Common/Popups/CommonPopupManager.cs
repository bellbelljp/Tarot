using UnityEngine;
using System;
using System.Threading;

namespace JourneysOfRealPeople
{
	public class CommonPopupManager : MonoBehaviour
	{
		[SerializeField] GameObject m_commonPopupTextObj = null;
		[SerializeField] GameObject m_commonPopupSelectObj = null;
		//[SerializeField] GameObject m_rewardPopup = null;
		//[SerializeField] GameObject m_charaPopup = null;
		[SerializeField] GameObject m_chapterPopup = null;
		//[SerializeField] GameObject m_soundPopup = null;
		//[SerializeField] GameObject m_namePopup = null;
		//[SerializeField] GameObject m_langPopup = null;
		//[SerializeField] GameObject m_secretPopup = null;

		/// <summary>テキストポップアップ表示</summary>
		public void ShowTextPopup(string text)
		{
			var popup = Instantiate(m_commonPopupTextObj, this.transform);
			var textPopup = popup.GetComponent<CommonPopupText>();
			textPopup.SetParam(text);
			textPopup.Open();
		}

		/// <summary>選択ポップアップ表示</summary>
		/// <param name="yes">指定しない場合はクリックで閉じる</param>
		/// <param name="no">指定しない場合はクリックで閉じる</param>
		public void ShowSelectPopup(string text, Action yes = null, Action no = null, bool changePos = false)
		{
			var popup = Instantiate(m_commonPopupSelectObj, this.transform);
			var selectPopup = popup.GetComponent<CommonPopupSelect>();
			selectPopup.SetParamYesNoButton(text, yes, no);
			selectPopup.Open();
			if (changePos)
				selectPopup.ChangeButtonPos();
		}

		/// <summary>OKポップアップ表示</summary>
		/// <param name="yes">指定しない場合はクリックで閉じる</param>
		public void ShowOKPopup(string text, Action yes = null)
		{
			var popup = Instantiate(m_commonPopupSelectObj, this.transform);
			var selectPopup = popup.GetComponent<CommonPopupSelect>();
			selectPopup.SetParaOKButton(text, yes);
			selectPopup.Open();
		}

		/// <summary>チャプターポップアップ表示</summary>
		public void ShowChapterPopup(int chapterId, Action<int> callback, CancellationToken token)
		{
			var popup = Instantiate(m_chapterPopup, this.transform);
			var chapterPopup = popup.GetComponent<ChapterPopup>();
			chapterPopup.SetParam(chapterId, parameter =>
			{
				callback.Invoke(parameter);
			}, token);
			chapterPopup.Open();
		}

		///// <summary>報酬ポップアップ表示</summary>
		//public void ShowRewardPopup(int charaId)
		//{
		//	var popup = Instantiate(m_rewardPopup, this.transform);
		//	var rewardPopup = popup.GetComponent<RewardPopup>();
		//	rewardPopup.SetParam(charaId);
		//	rewardPopup.Open();
		//}

		///// <summary>キャラクター選択ポップアップ表示</summary>
		//public void ShowCharaPopup()
		//{
		//	var popup = Instantiate(m_charaPopup, this.transform);
		//	var charaPopup = popup.GetComponent<CharaSelectPopup>();
		//	charaPopup.SetParam(charaId => ShowRewardPopup(charaId));
		//	charaPopup.Open();
		//}

		///// <summary>サウンドポップアップ表示</summary>
		//public void ShowSoundPopup()
		//{
		//	var popup = Instantiate(m_soundPopup, this.transform);
		//	var charaPopup = popup.GetComponent<SoundPopup>();
		//	charaPopup.Open();
		//}

		///// <summary>名前入力ポップアップ表示</summary>
		//public void ShowNamePopup(Action callback)
		//{
		//	var popup = Instantiate(m_namePopup, this.transform);
		//	var namePopup = popup.GetComponent<NamePopup>();
		//	namePopup.Open();
		//	namePopup.SetYesNoButton(callback, null);
		//}

		///// <summary>言語ポップアップ表示</summary>
		//public void ShowLangPopup(/*Action callback*/)
		//{
		//	var popup = Instantiate(m_langPopup, this.transform);
		//	var langPopup = popup.GetComponent<LanguagePopup>();
		//	langPopup.Open();
		//}

		///// <summary>開発秘話ポップアップ表示</summary>
		//public void ShowSecretPopup()
		//{			
		//	var popup = Instantiate(m_secretPopup, this.transform);
		//	var secretPopup = popup.GetComponent<SecretStoryPopup>();
		//	secretPopup.Open();
		//}
	}
}
