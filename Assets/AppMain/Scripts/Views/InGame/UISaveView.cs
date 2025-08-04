using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

namespace Tarot
{
	public class UISaveView : UISaveLoadViewBase
	{
		SaveLoadSlot m_slot = null;

		protected override void OnEnable()
		{
			base.OnEnable();
			SoundManager.Instance.PlayBGM("BGM_Kakedasu");
		}

		/// <summary>セーブスロットをクリック</summary>
		public async void OnClickSlot(SaveLoadSlot slot)
		{
			if(slot.Index == 0)
			{
				SoundManager.Instance.PlaySE("Cancel");
				return;
			}
			SoundManager.Instance.PlaySE("Decide");
			m_slot = slot;
			if (slot.HasSaveData())
			{
				var text = await Language.Popup.SaveConfirm.PopupLocalize();
				m_popupManager.ShowSelectPopup(text, SaveSlot, null, true);
			}
			else
			{
				SaveSlot();
			}
		}

		/// <summary>セーブ</summary>
		async void SaveSlot()
		{
			var saveData = await SaveData.Instance.Load();
			saveData.CharaId = 1; // tmp
			SaveData.Instance.Save(saveData, m_slot.Index);
			Reload(m_slot, saveData);
		}

		//------------------------------------------
		public async override void Close()
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			m_backButton.enabled = false;

			await Scene.ChangeView(ViewName.Story, 0, cts.Token);
			//await Scene.ChangeScene(SceneName.InGame, cts.Token, true, ViewName.Story);
		}
	}
}
