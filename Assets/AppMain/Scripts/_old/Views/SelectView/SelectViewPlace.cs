using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JourneysOfRealPeople
{
	public class SelectViewPlace : MonoBehaviour
	{
		[SerializeField] Image m_placeImage = null;
		[SerializeField] TextMeshProUGUI m_placeName = null;
		[SerializeField] ButtonEx m_button = null;
		[SerializeField] GameObject m_selected = null;

		int m_placeId = 0;
		public int PlaceId { get { return m_placeId; } }

		public void SetParam(int placeId, bool isActive)
		{
			SetSelectedOff();
			m_placeId = placeId;
			SetSprite();
			m_placeName.text = "仮：" + placeId;
			m_button.interactable = isActive;
		}

		private void SetSprite()
		{
			m_placeImage.enabled = false;
			BackgroundMaster.Instance.SetSprite(m_placeId, sp =>
			{
				if (sp != null)
				{
					m_placeImage.sprite = sp;
					m_placeImage.enabled = true;
				}
			});
		}

		public void SetSelected()
		{
			var isSelected = !m_selected.activeSelf;
			m_selected.SetActive(isSelected);
		}

		public void SetSelectedOff()
		{
			Debug.Log("オフ：" + m_placeId);
			m_selected.SetActive(false);
		}
	}
}
