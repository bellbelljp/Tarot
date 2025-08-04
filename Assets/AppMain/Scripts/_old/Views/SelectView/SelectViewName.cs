using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace JourneysOfRealPeople
{
	public class SelectViewName : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI m_name = null;
		[SerializeField] Image m_charaImage = null;
		[SerializeField] ButtonEx m_button = null;

		int m_charaId;
		public int CharaId { get { return m_charaId; } }

		public void SetParam(CharacterMaster.Data data, bool isActive)
		{
			m_charaId = data.MasterId;
			m_name.text = data.Name;
			m_button.interactable = isActive;
			SetCharaSprite();
		}

		/// <summary>キャラクター画像セット</summary>
		void SetCharaSprite()
		{
			m_charaImage.enabled = false;
			CharacterMaster.Instance.SetSprite(m_charaId, sp =>
			{
				if (sp != null)
				{
					m_charaImage.sprite = sp;
					m_charaImage.enabled = true;
				}
			});
		}
	}
}
