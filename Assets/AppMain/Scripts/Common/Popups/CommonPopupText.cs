using UnityEngine;
using TMPro;

namespace JourneysOfRealPeople
{
	public class CommonPopupText : CommonPopup
	{
		[SerializeField] TextMeshProUGUI m_text = null;

		public void SetParam(string text)
		{
			m_text.text = text;
		}
	}
}
