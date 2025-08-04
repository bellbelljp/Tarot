using System;
using UnityEngine;
using TMPro;

namespace JourneysOfRealPeople
{
	public class SecretStoryPopup : CommonPopup
	{
		[SerializeField] SecretStorySelectButton m_obj = null;
		[SerializeField] Transform m_objPos = null;
		[SerializeField] TextMeshProUGUI m_subtitle = null;

		const int MAX_INDEX = 9;

		void Start()
		{
			m_subtitle.text = string.Empty;
			for (int i = 0; i < m_objPos.childCount; i++)
			{
				var child = m_objPos.GetChild(i);
				if (!child.gameObject.activeSelf)
					continue;
				Destroy(child.gameObject);
			}

			var lang = PlayerPrefs.GetString("selected-locale", "ja");

			for (int i = 0; i < MAX_INDEX; i++)
			{
				var obj = Instantiate(m_obj, m_objPos);
				obj.SetParam(i + 1, lang, SetSubtitle);
			}
		}

		/// <summary>字幕表示</summary>
		async void SetSubtitle(int index)
		{
			//var text = await string.Format(Language.SecretStory.Subtitle, index).Localize("SecretStory");
			//m_subtitle.text = text;
		}

		public override void Close()
		{
			SoundManager.Instance.StopVoice();
			base.Close();
		}
	}
}
