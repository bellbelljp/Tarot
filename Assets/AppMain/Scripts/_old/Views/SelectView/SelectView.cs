using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace JourneysOfRealPeople
{
	public class SelectView : ViewBase
	{
		[SerializeField] Image m_characterImage = null;
		[SerializeField] List<TextMeshProUGUI> m_characterDetailText = new List<TextMeshProUGUI>();
		[SerializeField] TextMeshProUGUI m_characterText = null;
		[SerializeField] SelectViewName m_charaItemPrefab = null;
		[SerializeField] Transform m_charaItemPos = null;
		[SerializeField] TextMeshProUGUI m_lastDateCountText = null;
		//[SerializeField] SelemonyViewDetail m_detailView = null;
		[SerializeField] SelectViewPlace m_placeObj = null;
		[SerializeField] Transform m_placePos = null;

		SelectViewName m_currentChara = null;
		List<CharacterMaster.Data> m_masterList = new List<CharacterMaster.Data>();

		// 残りデート回数
		int m_lastDateCount = 0;
		List<int> m_selectedChara = new List<int>();

		SaveData.Data m_saveData = new SaveData.Data();
		SelectViewPlace m_selectedPlace = null;

		public override async void SetParam(int param = 0)
		{
			m_saveData = await SaveData.Instance.Load();
			switch (m_saveData.Phase)
			{
				case csdef.Phase.SELECT + csdef.Round.FIRST:	// ラウンド１
					m_lastDateCount = csdef.LastDate.FIRST_PHASE;
					break;
				case csdef.Phase.SELECT + csdef.Round.SECOND:	// ラウンド２
					m_lastDateCount = csdef.LastDate.SECOND_PHASE;
					break;
				case csdef.Phase.SELECT + csdef.Round.THIRD:	// ラウンド３
					m_lastDateCount = csdef.LastDate.THIRD_PHASE;
					break;
				case csdef.Phase.SELECT + csdef.Round.ROURTH:	// ラウンド４
					m_lastDateCount = csdef.LastDate.FOURTH_PHASE;
					break;
				case csdef.Phase.SELECT + csdef.Round.FIFTH:	// ラウンド５
					m_lastDateCount = csdef.LastDate.FIFTH_PHASE;
					break;
			}

			// 初期化（起動時か、リザルトから来た場合）
			if ((param >= csdef.Phase.RESULT && param < csdef.Phase.RESULT + csdef.Phase.INTERVAL) || param == 0)
			{
				m_selectedChara.Clear();
				m_saveData.LastDateCount = m_lastDateCount;
			}
			CreatePlace();
			await CreateList();

			m_lastDateCountText.text = m_saveData.LastDateCount.ToString();
		}

		void CreatePlace()
		{
			m_placeObj.gameObject.SetActive(false);
			for (int i = 0; i < m_placePos.childCount; i++)
			{
				var child = m_placePos.GetChild(i);
				if (!child.gameObject.activeSelf)
					continue;

				Destroy(child.gameObject);
			}

			for(int i = 0; i < m_lastDateCount; i++)
			{
				var obj = Instantiate(m_placeObj, m_placePos);
				var place = obj.GetComponent<SelectViewPlace>();
				place.gameObject.SetActive(true);
				place.SetParam(i + 1, true);
			}
		}

		public void ClickPlace(SelectViewPlace place)
		{
			if (m_selectedPlace)
				m_selectedPlace.SetSelectedOff();
			place.SetSelected();
			m_saveData.PlaceId = place.PlaceId;
			m_selectedPlace = place;
		}

		/// <summary>キャラクターリスト作成</summary>
		async UniTask CreateList()
		{
			m_charaItemPrefab.gameObject.SetActive(false);
			for (int i = 0; i < m_charaItemPos.childCount; i++)
			{
				var child = m_charaItemPos.GetChild(i);
				if (!child.gameObject.activeSelf)
					continue;

				Destroy(child.gameObject);
			}

			bool isFirst = true;
			m_masterList = await CharacterMaster.Instance.LoadData();
			for (int i = 0; i < m_masterList.Count; i++)
			{
				var data = m_masterList[i];
				var chara = Instantiate(m_charaItemPrefab, m_charaItemPos);
				chara.gameObject.SetActive(true);
				var charaName = chara.GetComponent<SelectViewName>();
				bool isActive = m_saveData.LastCharaIdList.Contains(data.MasterId) && !m_selectedChara.Contains(data.MasterId);
				charaName.SetParam(data, isActive);
				// 最初に表示するキャラクター
				if (isActive && isFirst)
				{
					isFirst = false;
					SetCharacter(charaName);
				}
			}
		}

		public void SetCharacter(SelectViewName charaName)
		{
			SetCharacterSprite(charaName);
			SetCharacterProfile();
			if(m_selectedPlace)
				m_selectedPlace.SetSelectedOff();
			m_selectedPlace = null;
		}

		/// <summary>キャラクター画像セット</summary>
		void SetCharacterSprite(SelectViewName charaName)
		{
			m_currentChara = charaName;
			CharacterMaster.Instance.SetSprite(charaName.CharaId, (sp) =>
			{
				if (sp != null)
				{
					m_characterImage.enabled = true;
					m_characterImage.sprite = sp;
				}
			});
		}

		/// <summary>キャラクタープロフィールセット</summary>
		void SetCharacterProfile()
		{
			// プロフィールテキスト
			var charaMaster = m_masterList[m_currentChara.CharaId - 1];
			var charaData = m_saveData.CharacteDataList[m_currentChara.CharaId - 1];
			for (int i = 0; i < m_characterDetailText.Count; i++)
			{
				var text = m_characterDetailText[i];
				var profileText = string.Empty;
				//if (charaData.OpenProfile.Contains(i))
				//	profileText = charaMaster.Details[i].Text;
				//else
				//	profileText = "???";
				text.text = profileText;
			}
		}

		/// <summary>キャラクター確認ポップアップ</summary>
		public void ComfirmCharacterPopup()
		{
			var name = m_masterList[m_currentChara.CharaId - 1].Name;
			m_saveData.CharaId = m_currentChara.CharaId;
			Scene.CommonPopupManager.ShowSelectPopup(name + "を選びますか？" , async () =>
			{
				m_selectedChara.Add(m_saveData.CharaId);
				m_saveData.Phase += 10000;
				SaveData.Instance.Save(m_saveData);
				//await Scene.ChangeView(ViewName.Story, csdef.Phase.SELECT);
			});
		}
	}
}
