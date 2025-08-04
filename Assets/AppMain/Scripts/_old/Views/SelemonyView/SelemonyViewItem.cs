using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JourneysOfRealPeople
{
	public class SelemonyViewItem : MonoBehaviour
	{
		[SerializeField] Image m_charaImg = null;
		[SerializeField] TextMeshProUGUI m_charaName = null;
		[SerializeField] GameObject m_selectedObj = null;
		[SerializeField] ButtonEx m_button = null;

		CharacterMaster.Data m_charaData;
		public CharacterMaster.Data CharaData { get { return m_charaData; } }

		bool m_isSelected = false;
		public bool isSelected { get { return m_isSelected; } }

		public void SetParam(CharacterMaster.Data data, bool isActive)
		{
			SetSelected(false);
			m_charaData = data;
			SetCharacterSprite();
			m_charaName.text = data.Name;
			m_button.interactable = isActive;
		}

		void SetCharacterSprite()
		{
			m_charaImg.enabled = false;
			CharacterMaster.Instance.SetSprite(m_charaData.MasterId, sp =>
			{
				if (sp != null)
				{
					m_charaImg.sprite = sp;
					m_charaImg.enabled = true;
				}
			});
		}

		/// <summary>キャラクターを選択するか否か</summary>
		public void SetSelected(bool flg)
		{
			m_isSelected = flg;
			m_selectedObj.SetActive(flg);
		}
	}
}
