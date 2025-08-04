using System.Text.RegularExpressions;
using UnityEngine;

namespace Tarot
{
	public class GameViewCharacter : MonoBehaviour
	{
		[SerializeField] GameObject m_kenty = null;
		[SerializeField] LipSync m_kentyLip = null;
		[SerializeField] GameObject m_rieko = null;
		[SerializeField] LipSync m_riekoLip = null;
		[SerializeField] GameObject m_bell = null;
		[SerializeField] LipSync m_bellLip = null;

		string m_expression = string.Empty;

		public void SetCharacter(string fileName, int talkChara)
		{
			var showCharaId = int.Parse(Regex.Replace(fileName, @"[^0-9]", ""));
			// 口パク
			//var expression = Regex.Replace(fileName, showCharaId.ToString(), "");
			//SetExpression(showCharaId, talkChara, expression);

			m_kenty.SetActive(false);
			m_rieko.SetActive(false);
			m_bell.SetActive(false);
			switch (showCharaId)
			{
				case 1:
					m_kenty.SetActive(true);
					break;
				case 2:
					m_rieko.SetActive(true);
					break;
				case 3:
					m_bell.SetActive(true);
					break;
			}
		}

		public void SetExpression(int showChara, int talkChara, string expression = "")
		{
			if (expression != "")
				m_expression = expression;
			switch (talkChara)
			{
				case 1:
					m_kentyLip.SetExpression(m_expression);
					break;
				case 2:
					m_riekoLip.SetExpression(m_expression);
					break;
				case 3:
					m_bellLip.SetExpression(m_expression);
					break;
			}

			switch (showChara)
			{
				case 1:
					m_kentyLip.SetExpression(m_expression);
					break;
				case 2:
					m_riekoLip.SetExpression(m_expression);
					break;
				case 3:
					m_bellLip.SetExpression(m_expression);
					break;
			}
		}

		// 菌ちゃんと警察の会話の名残
		/// <summary>口パクオフを解除</summary>
		public void StopVoice()
		{
			//m_riekoLip.IsQuietly = false;
			//m_policeLip.IsQuietly = false;
			//SoundManager.Instance.StopVoice();
		}
	}
}
