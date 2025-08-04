using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

namespace JourneysOfRealPeople
{
	public class StoryView : ViewBase
	{
		[SerializeField] TalkWindow m_talkWindow;
		[SerializeField] StoryDataManager m_storyDataManager = null;

		// GamerStoriesのUI
		//{
		[SerializeField] Image m_bgImage = null;
		[SerializeField] CommonPopupManager m_popupManager = null;
		[SerializeField] GameObject m_settingPopup = null;
		[Header("隠すUI")]
		[SerializeField] GameObject[] m_hideUIList = null;
		[SerializeField] GameObject m_uiHideOffButton = null;
		[Header("ボタン")]
		[SerializeField] ButtonEx m_saveButton = null;
		[SerializeField] ButtonEx m_loadButton = null;
		[Header("設定")]
		[SerializeField] GameObject m_shakeOn = null;
		//}

		int m_currentChapter = 0;
		int m_charaId;
		CancellationTokenSource m_token;

		int m_lastDateCount = 0;
		int m_resultCharaIndex = 0;

		SaveData.Data m_saveData;

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();

			m_saveButton.enabled = true;
			m_loadButton.enabled = true;
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			await base.OnViewClosed();
		}

		/// <param name="sceneFrom">どこから来たのか</param>
		public override async void SetParam(int sceneFrom)
		{
			m_saveData = await SaveData.Instance.Load();
			//Debug.Log("べる：" + m_saveData.CharacteDataList[0].LoveParam);
			m_saveData.CharaId = 99;	// tmp
			m_charaId = m_saveData.CharaId;
			switch (sceneFrom)
			{
				//case int n when n >= csdef.Phase.SELECT && n < csdef.Phase.SELECT + csdef.Phase.INTERVAL:
				//	await StartStory(StoryMaster.StoryType.CommonStory);
				//	break;
				default:
					await StartStory();
					//await StartStoryFromChapter(savedata.CharaId, savedata.ChapterId);
					break;
			}
		}

		/// <summary>ストーリー開始</summary>
		async UniTask StartStory()
		{
			//m_uiHideOffButton.SetActive(false);
			//// セーブデータ読み込み
			var savedata = await SaveData.Instance.Load();
			try
			{
				//FIXME:初期チャプターはたくさんあるから、共通か、キャラクターかを判別させる必要がある
				//キャラを始めて開始する場合、初期チャプターを保存する
				if ((savedata.Phase == 10))
				{
					var chapter = await ChapterMaster.Instance.GetFirstChapter(m_charaId);
					var hasChapter = SaveData.Instance.CommonData.ChapterList.Find(it => it.ChapterId == chapter.ChapterId && it.CharaId == m_charaId);
					if (hasChapter == null)
					{
						SaveData.Instance.CommonData.ChapterList.Add(chapter);
						SaveData.Instance.SaveCommonData();
					}
				}
				await StartStoryFromPhase(m_charaId, m_saveData.Phase);
			}
			catch (OperationCanceledException e)
			{
				Debug.Log("会話キャンセル：" + e);
			}
			catch (Exception e)
			{
				Debug.LogError("エラーが発生しました。:" + e);
			}
		}

		/// <summary>チャプターからストーリー再生</summary>
		async public UniTask StartStoryFromChapter(int charaId, int chapterId)
		{
			m_token = new CancellationTokenSource();
			m_charaId = charaId;
			try
			{
				var storyData = await StoryMaster.Instance.LoadData(charaId, 0, chapterId);
				//// チャプター取り出し
				//var chapterData = m_storyDataManager.LoadStoryDataFromChapter(storyData, chapterId);
				await StartStoryFromData(storyData);
			}
			catch (OperationCanceledException)
			{
				Debug.Log("操作がキャンセルされました。");
			}
		}

		/// <summary>指定したPhaseから会話スタート</summary>
		public async UniTask StartStoryFromPhase(int charaId, int serifId)
		{
			m_token = new CancellationTokenSource();
			m_charaId = charaId;
			try
			{
				// 途中からストーリー取り出し
				var storyData = await StoryMaster.Instance.LoadData(charaId, serifId);
				await StartStoryFromData(storyData);
			}
			catch (OperationCanceledException)
			{
				Debug.Log("操作がキャンセルされました。");
			}
		}

		/// <summary>受け取ったストーリーデータで開始</summary>
		async UniTask StartStoryFromData(List<StoryMaster.Data> storyList)
		{
			try
			{
				List<UniTask> tasks = new List<UniTask>();

				// エフェクトオフ
				m_talkWindow.EffectOff();

				// 背景セット
				//tasks.Add(m_talkWindow.SetBG(storyList[0].Place, true));

				// 会話開始
				tasks.Add(m_talkWindow.Open());

				await UniTask.WhenAll(tasks);

				var response = await m_talkWindow.TalkStart(storyList, m_token.Token, m_saveData);

				// 会話遷移・選択肢遷移
				while (response != null && response.Count >= 1)
				{
					// バッドエンド
					//if (response[0] > 900)
					//{
					//	await Scene.ChangeView(ViewName.Title, response[0], m_token.Token);
					//}

					int charaId = 0;
					// 100@XXXなど、100以上の場合は、チャプター遷移
					if (response[0] >= 100)
					{
						charaId = int.Parse(response[0].ToString()[0].ToString());
						if (charaId == 9) charaId = 99;
						storyList = await StoryMaster.Instance.LoadData(charaId, 0, response[0]);
						response = await m_talkWindow.TalkStart(storyList, m_token.Token, m_saveData);
					}
					else
					{
						var paramData = m_storyDataManager.LoadStoryDataFromParam(storyList, response[0]);
						response = await m_talkWindow.TalkStart(paramData, m_token.Token, m_saveData);
					}
				}

				await m_talkWindow.Close();
			}
			catch (OperationCanceledException)
			{
				Debug.Log("ストーリー再生がキャンセルされました。");
			}
			catch (Exception e)
			{
				Debug.Log("ストーリー再生中にエラーが発生しました:" + e);
			}
		}

		#region GamerStoriesの機能
		/// <summary>チャプター選択をクリック</summary>
		public void ClickChapter()
		{
			m_talkWindow.OffAutoSkip();
			m_popupManager.ShowChapterPopup(m_currentChapter, chapterId =>
			{
				CancelTalk();

				// chapterIdからcharaIdを取得
				int charaId = int.Parse(chapterId.ToString()[0].ToString());
				if(charaId == 9) charaId = 99;
				StartStoryFromChapter(charaId, chapterId).Forget();
			}, m_token.Token);
		}

		/// <summary>トロフィーをクリック</summary>
		public void ClickReward()
		{
			m_talkWindow.OffAutoSkip();
			//m_popupManager.ShowCharaPopup();
		}

		/// <summary>ログをクリック</summary>
		public void ClickLog()
		{
			m_talkWindow.ShowLog();
		}

		/// <summary>セーブ画面へ遷移</summary>
		public async void ClickSave()
		{
			m_saveButton.enabled = false;
			m_token.Cancel();
			await m_talkWindow.SaveScreenShot();
			await Scene.ChangeView(ViewName.Save, 0, m_token.Token);
			//m_talkWindow.BGOff();
		}

		/// <summary>ロード画面へ遷移</summary>
		public async void ClickLoad()
		{
			m_loadButton.enabled = false;
			m_token.Cancel();
			await m_talkWindow.SaveScreenShot();
			await Scene.ChangeView(ViewName.Load, (int)UILoadView.ViewFrom.InGame, m_token.Token);
			//m_talkWindow.BGOff();
		}

		public void CancelTalk()
		{
			m_token.Cancel();
			m_talkWindow.ResetCharacter();
		}

		/// <summary>UIの表示切り替え</summary>
		public void ActiveUI(bool isActive)
		{
			foreach (var ui in m_hideUIList)
			{
				ui.gameObject.SetActive(isActive);
			}
			m_uiHideOffButton.SetActive(!isActive);
		}

		public void ClickSetting()
		{
			m_settingPopup.SetActive(true);
			var popup = m_settingPopup.GetComponent<CommonPopup>();
			popup.Open();
		}

		public void CloseSetting()
		{
			var popup = m_settingPopup.GetComponent<CommonPopup>();
			popup.SetActiveFalse();
		}

		/// <summary>ホームに戻る</summary>
		public async void OnBackHomeButton()
		{
			var text = await Language.Popup.HomeConfirm.PopupLocalize();
			m_popupManager.ShowSelectPopup(text, BackHome);
		}

		void BackHome()
		{
			//SaveData.Instance.Logs.Clear();
			//Scene.ChangeScene(SceneName.Home).Forget();
		}

		/// <summary>サウンド設定表示</summary>
		public void ClickSoundSetting()
		{
			//m_popupManager.ShowSoundPopup();
		}

		/// <summary>タイトルに戻る</summary>
		public async void OnClickBackTitleButton()
		{
			var text = await Language.Popup.TitleConfirm.PopupLocalize();
			m_popupManager.ShowSelectPopup(text, BackToTitle);
		}

		/// <summary>タイトルに戻る</summary>
		async void BackToTitle()
		{
			try
			{
				await Scene.ChangeScene(SceneName.Title,　m_token.Token, true);

				SoundManager.Instance.ResetVolume();
			}
			catch (OperationCanceledException)
			{
				Debug.Log("キャンセルされました。");
			}
		}

		/// <summary>振動エフェクトのアクティブ設定</summary>
		public void SetShakeActive()
		{
			m_talkWindow.IsShake = !m_talkWindow.IsShake;
			var value = m_talkWindow.IsShake == true ? 1 : 0;
			//PlayerPrefs.SetInt(SHAKE_KEY, value);
			m_shakeOn.SetActive(m_talkWindow.IsShake);
		}
		#endregion
	}
}
