using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Tarot
{
	public class SecretStorySelectButton : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI m_text = null;

		UnityEvent<int> m_callback = new UnityEvent<int>();

		int m_index = 0;
		string m_lang = "ja";

		public async  void SetParam(int index, string lang, UnityAction<int> action)
		{
			m_index = index;
			//var text = await string.Format(Language.SecretStory.Secret, index).Localize("SecretStory");
			//m_text.text = text;
			m_lang = lang;
			m_callback.AddListener(action);
		}

		public void OnClick()
		{
			m_callback.Invoke(m_index);
			SoundManager.Instance.PlaySecretStory(m_index, m_lang);
		}
	}
}
