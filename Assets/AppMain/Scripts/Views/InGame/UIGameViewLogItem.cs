using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JourneysOfRealPeople
{
	public class UIGameViewLogItem : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI m_talkText = null;
		[SerializeField] TextMeshProUGUI m_nameText = null;

		public void SetParam(UIGameViewLogWindow.Log data)
		{
			var hasName = data.NameText != string.Empty && data.NameText != null;
			m_nameText.transform.parent.gameObject.SetActive(hasName);
			m_nameText.text = data.NameText;
			m_talkText.text = data.LogText;
		}
	}
}
