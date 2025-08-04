using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace JourneysOfRealPeople
{
	public class UISaveLoadViewBase : ViewBase
	{
		[SerializeField] PageNation m_pageNation = null;
		[SerializeField] SaveLoadSlot[] m_saveSlot = null;
		[SerializeField] protected CommonPopupManager m_popupManager = null;
		[SerializeField] protected ButtonEx m_backButton = null;

		List<StoryMaster.Data> m_bellStory = new List<StoryMaster.Data>();
		List<StoryMaster.Data> m_kinStory = new List<StoryMaster.Data>();
		List<StoryMaster.Data> m_caseyStory = new List<StoryMaster.Data>();

		string m_bellName = string.Empty;
		string m_kinName = string.Empty;
		string m_caseyName = string.Empty;

		const int START_NO = 0;
		const int PAGE_MAX_COUNT = 6;
		int m_nowPage = 0;

		async UniTask Load()
		{
			try
			{
				var storyTasks = new List<UniTask<List<StoryMaster.Data>>>
				{
					StoryMaster.Instance.LoadData(99, 0),
					StoryMaster.Instance.LoadData(1, 0),
					StoryMaster.Instance.LoadData(2, 0),
					StoryMaster.Instance.LoadData(3, 0),
				};
				var nameTasks = new List<UniTask<string>>
				{
					CharacterMaster.Instance.GetCharacterName(1),
					CharacterMaster.Instance.GetCharacterName(2),
					CharacterMaster.Instance.GetCharacterName(3)
				};

				// ストーリーデータの読み込みを並行して実行
				var storyResults = await UniTask.WhenAll(storyTasks);
				m_bellStory = storyResults[0];
				m_kinStory = storyResults[1];
				m_caseyStory = storyResults[2];

				// キャラクター名の取得を並行して実行
				var nameResults = await UniTask.WhenAll(nameTasks);
				m_bellName = nameResults[0];
				m_kinName = nameResults[1];
				m_caseyName = nameResults[2];
			}
			catch (OperationCanceledException e)
			{
				Debug.Log("セーブスロットキャンセル：" + e);
				throw e;
			}
		}

		protected virtual async void OnEnable()
		{
			//データ読み込み
			m_pageNation.SetParam(SetData);

			if (m_bellStory.Count == 0 || m_kinStory.Count == 0 || m_caseyStory.Count == 0 ||
				m_bellName == string.Empty || m_kinName == string.Empty || m_caseyName == string.Empty)
				await Load();

			SetData(START_NO);
		}

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();
			m_backButton.enabled = true;
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			await base.OnViewClosed();
		}

		async void SetData(int index)
		{
			m_nowPage = index;
			var saveDataList = await SaveData.Instance.LoadCommonData();
			var start = m_nowPage * PAGE_MAX_COUNT;
			var end = start + PAGE_MAX_COUNT;
			for (int i = start; i < end; i++)
			{
				var slotIndex = i % PAGE_MAX_COUNT;
				var slot = m_saveSlot[slotIndex];
				SaveData.Data save = null;
				save = saveDataList.DataList.Find(it => it.SaveSlotNo == i);
				var charaId = save != null ? save.CharaId : 0;
				charaId = 1; // tmp
				switch (charaId)
				{
					case 1:
						slot.SetParam(i, save, m_bellStory, m_bellName);
						break;
					case 2:
						slot.SetParam(i, save, m_kinStory, m_kinName);
						break;
					case 3:
						slot.SetParam(i, save, m_caseyStory, m_caseyName);
						break;
					default:
						slot.SetParamNull(i);
						break;
				}
			}
		}

		protected void Reload(SaveLoadSlot slot, SaveData.Data save)
		{
			var charaId = save != null ? save.CharaId : 0;
			charaId = 1; // tmp
			switch (charaId)
			{
				case 1:
					slot.SetParam(slot.Index, save, m_bellStory, m_bellName);
					break;
				case 2:
					slot.SetParam(slot.Index, save, m_kinStory, m_kinName);
					break;
				case 3:
					slot.SetParam(slot.Index, save, m_caseyStory, m_caseyName);
					break;
				default:
					slot.SetParamNull(slot.Index);
					break;
			}
		}

		//------------------------------------------
		// ページネーション
		//------------------------------------------
		/// <summary>右へ移動</summary>
		public void MoveToRight()
		{
			m_pageNation.MoveToRight();
		}

		/// <summary>左へ移動</summary>
		public void MoveToLeft()
		{
			m_pageNation.MoveToLeft();
		}

		//------------------------------------------
		public virtual void Close() { }
	}
}
