using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace JourneysOfRealPeople
{
	public class LangButton : MonoBehaviour
	{
		[SerializeField] GameObject m_on = null;
		[SerializeField] GameObject m_off = null;

		public string MyLang = null;

		void Start()
		{
			// 現在の言語を取得
			Locale nowLang = LocalizationSettings.SelectedLocale;
			// 言語コードを取得
			string langCode = nowLang.Identifier.Code;
			var flg = langCode == MyLang;
			m_on.SetActive(flg);
			m_off.SetActive(!flg);
		}

		public void ChangeLang(string code)
		{
			var flg = code == MyLang;
			m_on.SetActive(flg);
			m_off.SetActive(!flg);
		}
	}
}
