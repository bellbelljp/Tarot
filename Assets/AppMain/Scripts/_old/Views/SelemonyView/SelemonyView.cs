using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class SelemonyView : ViewBase
	{
		[SerializeField] SelemonyViewItem m_itemObj = null;
		[SerializeField] Transform m_itemPos = null;
		[SerializeField] SelemonyViewDetail m_detailView = null;
		[SerializeField] ButtonEx m_decideButton = null;

		List<CharacterMaster.Data> m_data = new List<CharacterMaster.Data>();
		List<int> m_selectedMember = new List<int>();

		int m_canSelectNum = 0;

		SaveData.Data m_saveData = null;

		private async void OnEnable()
		{
			m_saveData = await SaveData.Instance.Load();
			m_selectedMember = new List<int>();
			m_detailView.gameObject.SetActive(false);
			m_decideButton.interactable = false;

			switch (m_saveData.Phase)
			{
				case 30100:
					m_canSelectNum = csdef.LastSelemony.FIRST_PHASE;
					break;
				case 30200:
					m_canSelectNum = csdef.LastSelemony.SECOND_PHASE;
					break;
				case 30300:
					break;
			}

			for (int i = 0; i < m_itemPos.childCount; i++)
			{
				var child = m_itemPos.GetChild(i);
				if(!child.gameObject.activeSelf)
					continue;

				Destroy(child.gameObject);
			}

			m_data = await CharacterMaster.Instance.GetAllExceptHide();

			for (int i = 0;i < m_data.Count; i++)
			{
				var obj = Instantiate(m_itemObj, m_itemPos);
				var item = obj.gameObject.GetComponent<SelemonyViewItem>();
				var isActive = m_saveData.LastCharaIdList.Contains(m_data[i].MasterId);
				item.SetParam(m_data[i], isActive);
				obj.gameObject.SetActive(true);
			}
		}

		/// <summary>キャラクター詳細表示</summary>
		public void ShowDetail(SelemonyViewItem item)
		{
			m_detailView.SetParam(item, m_saveData, flg => {
				if (flg && !m_selectedMember.Contains(item.CharaData.MasterId))
				{
					m_selectedMember.Add(item.CharaData.MasterId);
				}
				else if(!flg && m_selectedMember.Contains(item.CharaData.MasterId))
				{
					m_selectedMember.Remove(item.CharaData.MasterId);
				}

				var canNext = m_selectedMember.Count == m_canSelectNum;
				m_decideButton.interactable = canNext;
			});
			m_detailView.gameObject.SetActive(true);
		}

		public void ClickDecide()
		{
			Scene.CommonPopupManager.ShowSelectPopup("メンバーを決定してよろしいですか？", MoveToResult);
		}
		
		/// <summary>結果へ遷移</summary>
		async void MoveToResult()
		{
			// セーブデータ読み込み
			//m_saveData.Phase += 10000;
			//m_saveData.CharaId = m_selectedMember[0];
			//m_saveData.SelectCharaIdList = new List<int>(m_selectedMember);
			//// FIXME:ResultでRemoveとかしたい
			//m_saveData.LastCharaIdList = new List<int>(m_selectedMember);
			//SaveData.Instance.Save(m_saveData);
			//await Scene.ChangeView(ViewName.Story, csdef.Phase.SELEMONY);
		}
	}
}
