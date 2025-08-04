using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace JourneysOfRealPeople
{
	public class LanguagePopup : CommonPopup
	{
		[SerializeField] LangButton[] m_langButtons = null;

		string m_nowLangCode = string.Empty;

		/// <summary>言語切り替え</summary>
		public async void ChangeLang(LangButton lang)
		{
			m_nowLangCode = lang.MyLang;
			LocalizationSettings.SelectedLocale = Locale.CreateLocale(m_nowLangCode);
			await LocalizationSettings.InitializationOperation.Task;
			UpdateButtons();
			PlayerPrefs.SetString("selected-locale", m_nowLangCode);
		}

		/// <summary>ボタンを更新</summary>
		void UpdateButtons()
		{
			foreach(var button in m_langButtons)
			{
				button.ChangeLang(m_nowLangCode);
			}
		}
	}
}

