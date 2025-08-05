using UnityEngine;
using System;
using System.Threading;

namespace Tarot
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
	}
}
