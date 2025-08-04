using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.Overlays;

namespace JourneysOfRealPeople
{
	public class SelemonyViewDetail : MonoBehaviour
	{
		[SerializeField] Image m_charaImg = null;
		[SerializeField] TextMeshProUGUI m_charaName = null;
		[SerializeField] TextMeshProUGUI[] m_charaDetail = new TextMeshProUGUI[5];

		[Header("ボタン")]
		[SerializeField] GameObject m_unselectedButton = null;
		[SerializeField] GameObject m_selectedButton = null;

		SelemonyViewItem m_charaItem = null;
		Action<bool> m_selectAction = null;

		public void SetParam(SelemonyViewItem item, SaveData.Data saveData, Action<bool> selectAction)
		{
			m_charaItem = item;
			m_charaName.text = item.CharaData.Name;
			SetCharaSprite(item.CharaData.MasterId);
			for(int i = 0; i < m_charaDetail.Length; i++)
			{
				var detail = m_charaDetail[i];
				//detail.text = m_charaItem.CharaData.Details[i].Text;
			}
			SetCharacterDetails(saveData);
			SetSelectedFlg(item.isSelected);
			m_selectAction = selectAction;
		}

		void SetCharaSprite(int charaId)
		{
			m_charaImg.enabled = false;
			CharacterMaster.Instance.SetSprite(charaId, sp =>
			{
				if (sp != null)
				{
					m_charaImg.sprite = sp;
					m_charaImg.enabled = true;
				}
			});
		}

		void SetCharacterDetails(SaveData.Data saveData)
		{
			// プロフィールテキスト
			var charaData = saveData.CharacteDataList[m_charaItem.CharaData.MasterId - 1];
			for (int i = 0; i < m_charaDetail.Length; i++)
			{
				var profileText = string.Empty;
				//if (charaData.OpenProfile.Contains(i))
					//profileText = m_charaItem.CharaData.Details[i].Text;
				//else
				//	profileText = "???";
				m_charaDetail[i].text = profileText;
			}
		}

		void SetSelectedFlg(bool flg)
		{
			m_charaItem.SetSelected(flg);
			m_unselectedButton.SetActive(!flg);
			m_selectedButton.SetActive(flg);
		}

		/// <summary>キャラクター選択</summary>
		public void Select(bool flg)
		{
			SetSelectedFlg(flg);
			m_selectAction?.Invoke(flg);
		}

		/// <summary>戻る</summary>
		public void Back()
		{
			gameObject.SetActive(false);
		}
	}
}
