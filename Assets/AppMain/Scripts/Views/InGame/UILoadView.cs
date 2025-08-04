using System;
using System.Threading;
using UnityEngine;

namespace Tarot
{
	public class UILoadView : UISaveLoadViewBase
	{
		SaveLoadSlot m_slot = null;
		public enum ViewFrom
		{
			Home = 1,
			InGame = 2,
		}
		int m_viewFrom = (int)ViewFrom.Home;

		CancellationTokenSource m_cts = new CancellationTokenSource();

		public override void SetParam(int param)
		{
			m_viewFrom = param;
			// tmp
			m_viewFrom = 1;
			SetBGM();
		}

		public async void StartGame(SaveLoadSlot slot)
		{
			if (!slot.HasSaveData())
			{
				SoundManager.Instance.PlaySE("Cancel");
				return;
			}

			m_slot = slot;
			SoundManager.Instance.PlaySE("Decide");
			var text = await Language.Popup.LoadConfirm.PopupLocalize();
			m_popupManager.ShowSelectPopup(text, LoadGame);
		}

		async void LoadGame()
		{
			try
			{
				//セーブデータロードしてから
				m_slot.Load();
				//UIGameView.Param.StartStory:0
				//UIGameView.Param.FromEnding:1
				await Scene.ChangeView(ViewName.Title, /*(int)UIGameView.Param.StartStory*/ 0, m_cts.Token);
			}
			catch (OperationCanceledException e)
			{
				Debug.Log("StartGameキャンセル：" + e);
				throw e;
			}
		}

		/// <summary>BGMセット</summary>
		void SetBGM()
		{
			SoundManager.Instance.PlayBGM("BGM_Kakedasu");
		}

		//------------------------------------------
		public async override void Close()
		{
			m_backButton.enabled = false;
			switch (m_viewFrom)
			{
				case (int)ViewFrom.Home:
					await Scene.ChangeScene(SceneName.InGame, m_cts.Token);
					break;
				case (int)ViewFrom.InGame:
					//await Scene.ChangeScene(SceneName.InGame, m_cts.Token, true, ViewName.Story);
					break;
			}
		}
	}
}
