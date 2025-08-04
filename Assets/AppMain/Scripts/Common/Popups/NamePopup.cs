using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Diagnostics;

namespace Tarot
{
	public class NamePopup : CommonPopup
	{
		[SerializeField] TMP_InputField m_nameInput = null;
		[SerializeField] TMP_InputField m_meInput = null;
		[SerializeField] GameObject m_meObj = null;
		[SerializeField] Button m_decideButton = null;

		string m_name = string.Empty;
		string m_me = string.Empty;

		private void OnEnable()
		{
			m_nameInput.text =
			m_name = SaveData.Instance.CommonData.YourName;
			m_meInput.text =
			m_me = SaveData.Instance.CommonData.MyName;
			ButtonCheck();

			// 現在の言語を取得
			Locale nowLang = LocalizationSettings.SelectedLocale;
			// 言語コードを取得
			string langCode = nowLang.Identifier.Code;
			var isJP = langCode == "ja";
			m_meObj.SetActive(isJP);
		}

		/// <summary>キーボード表示</summary>
		public void ShowKeybord()
		{
			Process.Start("osk.exe");
		}

		public void InputName(TMP_InputField input)
		{
			// FIXME:NGワード
			m_name = input.text;
			ButtonCheck();
		}

		public void InputMe(TMP_InputField input)
		{
			// FIXME:NGワード
			m_me = input.text;
			ButtonCheck();
		}

		public void ClickOK()
		{
			// 日本語だけだから、直接入れてしまう
			if (string.IsNullOrEmpty(m_me))
				m_me = "私";
			SaveData.Instance.CommonData.YourName = m_name;
			SaveData.Instance.CommonData.MyName = m_me;
			SaveData.Instance.SaveCommonData();
		}

		/// <summary>ボタンが押せるかチェック</summary>
		void ButtonCheck()
		{
			var buttonEnable = /*!string.IsNullOrEmpty(m_me) && */!string.IsNullOrEmpty(m_name);
			m_decideButton.enabled = buttonEnable;
		}
	}
}
