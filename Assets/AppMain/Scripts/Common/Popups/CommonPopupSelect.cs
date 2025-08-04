using System;
using TMPro;
using UnityEngine;

namespace Tarot
{
	public class CommonPopupSelect : CommonPopup
	{
		[SerializeField] TextMeshProUGUI m_text = null;

		public void SetParamYesNoButton(string text, Action yes, Action no)
		{
			m_text.text = text;
			SetYesNoButton(yes, no);
		}

		public void SetParaOKButton(string text, Action yes)
		{
			m_text.text = text;
			SetOKButton(yes);
		}
	}
}
