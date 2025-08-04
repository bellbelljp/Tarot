//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Bachelor
//{
//	public class UIGameView : ViewBase
//	{
//		[SerializeField] TalkWindow m_talkWindow = null;
//		[SerializeField] StoryDataManager m_storyDataManager = null;
//		[SerializeField] Image m_bgImage = null;
//		[SerializeField] CommonPopupManager m_popupManager = null;
//		[SerializeField] GameObject m_settingPopup = null;
//		[Header("隠すUI")]
//		[SerializeField] GameObject[] m_hideUIList = null;
//		[SerializeField] GameObject m_uiHideOffButton = null;
//		[Header("ボタン")]
//		[SerializeField] ButtonEx m_saveButton = null;
//		[SerializeField] ButtonEx m_loadButton = null;
//		[Header("設定")]
//		[SerializeField] GameObject m_shakeOn = null;

//		int m_currentChapter = 0;
//		CancellationTokenSource m_token;
//		int m_charaId = 0;

//		const string SHAKE_KEY = "IsShake";

//		public enum Param
//		{
//			StartStory = 1,
//			FromEnding = 2,
//		}

//		private void Awake()
//		{
//			m_bgImage.enabled = false;
//		}

//		private void OnEnable()
//		{
//			m_talkWindow.IsShake = PlayerPrefs.GetInt(SHAKE_KEY, 1) == 1 ? true : false;
//			m_shakeOn.SetActive(m_talkWindow.IsShake);
//		}

//		/// <summary>ビューオープン時</summary>
//		public override void OnViewOpened()
//		{
//			base.OnViewOpened();
//			m_saveButton.enabled = true;
//			m_loadButton.enabled = true;
//		}

//		/// <summary>ビュークローズ時</summary>
//		public async override UniTask OnViewClosed()
//		{
//			await base.OnViewClosed();
//		}

//		public override async void SetParam(int parameter)
//		{
//			switch (parameter)
//			{
//				case (int)Param.StartStory:
//				default:
//					await StartStory();
//					break;
//				case (int)Param.FromEnding:
//					// セーブデータ読み込み
//					var savedata = SaveData.Instance.Load();
//					await StartStoryFromChapter(savedata.CharaId, savedata.ChapterId);
//					break;
//			}
//		}

//		/// <summary>ストーリー開始</summary>
//		async UniTask StartStory()
//		{
//			m_uiHideOffButton.SetActive(false);
//			// セーブデータ読み込み
//			var savedata = SaveData.Instance.Load();
//			try
//			{
//				m_charaId = savedata.CharaId;
//				// キャラを始めて開始する場合、初期チャプターを保存する
//				if((savedata.TalkId == 10))
//				{
//					var chapter = await ChapterData.Instance.GetFirstChapter(m_charaId);
//					var hasChapter = SaveData.Instance.CommonData.ChapterList.Find(it => it.ChapterId == chapter.ChapterId && it.CharaId == m_charaId);
//					if(hasChapter == null)
//					{
//						SaveData.Instance.CommonData.ChapterList.Add(chapter);
//						SaveData.Instance.SaveCommonData();
//					}
//				}
//				await StartStoryFromTalkId(savedata.CharaId, savedata.TalkId);
//			}
//			catch (OperationCanceledException e)
//			{
//				Debug.Log("会話キャンセル：" + e);
//			}
//			catch(Exception e)
//			{
//				Debug.LogError("エラーが発生しました。:" + e);
//			}
//		}

//		/// <summary>指定したTalkIdから会話スタート</summary>
//		public async UniTask StartStoryFromTalkId(int charaId, int talkId)
//		{
//			m_token = new CancellationTokenSource();
//			m_charaId = charaId;
//			try
//			{
//				// 途中からストーリー取り出し
//				var storyData = await StoryData.Instance.LoadData(charaId, talkId);
//				await StartStoryFromData(storyData);
//			}
//			catch (OperationCanceledException)
//			{
//				Debug.Log("操作がキャンセルされました。");
//			}
//		}

//		/// <summary>チャプターからストーリー再生</summary>
//		async public UniTask StartStoryFromChapter(int charaId, int chapterId)
//		{
//			m_token = new CancellationTokenSource();
//			m_charaId = charaId;
//			try
//			{
//				var storyData = await StoryData.Instance.LoadData(charaId);
//				// チャプター取り出し
//				var chapterData = m_storyDataManager.LoadStoryDataFromChapter(storyData, chapterId);
//				await StartStoryFromData(chapterData);
//			}
//			catch (OperationCanceledException)
//			{
//				Debug.Log("操作がキャンセルされました。");
//			}
//		}

//		/// <summary>受け取ったストーリーデータで開始</summary>
//		async UniTask StartStoryFromData(List<StoryData.Data> storyList)
//		{
//			try
//			{
//				List<UniTask> tasks = new List<UniTask>();

//				// エフェクトオフ
//				m_talkWindow.EffectOff();

//				// 背景セット
//				tasks.Add(m_talkWindow.SetBG(storyList[0].Place, true));

//				// 会話開始
//				tasks.Add(m_talkWindow.Open());

//				await UniTask.WhenAll(tasks);

//				var response = await m_talkWindow.TalkStart(storyList, m_token.Token);

//				// 会話遷移・選択肢遷移
//				while (response != null && response.Count >= 1)
//				{
//					// バッドエンド
//					if (response[0] > 900)
//					{
//						await Scene.ChangeView(ViewName.Ending, response[0]);
//					}
//					var paramData = m_storyDataManager.LoadStoryDataFromParam(storyList, response[0]);
//					response = await m_talkWindow.TalkStart(paramData, m_token.Token);
//				}
//				await m_talkWindow.Close();
//			}
//			catch (OperationCanceledException)
//			{
//				Debug.Log("ストーリー再生がキャンセルされました。");
//			}
//			catch(Exception e)
//			{
//				Debug.Log("ストーリー再生中にエラーが発生しました:" + e);
//			}
//		}

//		/// <summary>チャプター選択をクリック</summary>
//		public void ClickChapter()
//		{
//			m_talkWindow.OffAutoSkip();
//			m_popupManager.ShowChapterPopup(m_charaId, m_currentChapter, parameter =>
//			{
//				CancelTalk();
//				StartStoryFromChapter(m_charaId, parameter).Forget();
//			}, m_token.Token);
//		}

//		/// <summary>トロフィーをクリック</summary>
//		public void ClickReward()
//		{
//			m_talkWindow.OffAutoSkip();
//			m_popupManager.ShowCharaPopup();
//		}

//		/// <summary>ログをクリック</summary>
//		public void ClickLog()
//		{
//			m_talkWindow.ShowLog();
//		}

//		/// <summary>セーブ画面へ遷移</summary>
//		public async void ClickSave()
//		{
//			m_saveButton.enabled = false;
//			m_token.Cancel();
//			await m_talkWindow.SaveScreenShot();
//			await Scene.ChangeView(ViewName.Save, 0);
//			m_talkWindow.BGOff();
//		}

//		/// <summary>ロード画面へ遷移</summary>
//		public async void ClickLoad()
//		{
//			m_loadButton.enabled = false;
//			m_token.Cancel();
//			await m_talkWindow.SaveScreenShot();
//			await Scene.ChangeView(ViewName.Load, (int)UILoadView.ViewFrom.InGame);
//			m_talkWindow.BGOff();
//		}

//		public void CancelTalk()
//		{
//			m_token.Cancel();
//			m_talkWindow.ResetCharacter();
//		}

//		/// <summary>UIの表示切り替え</summary>
//		public void ActiveUI(bool isActive)
//		{
//			foreach(var ui in m_hideUIList)
//			{
//				ui.gameObject.SetActive(isActive);
//			}
//			m_uiHideOffButton.SetActive(!isActive);
//		}

//		public void ClickSetting()
//		{
//			m_settingPopup.SetActive(true);
//			var popup = m_settingPopup.GetComponent<CommonPopup>();
//			popup.Open();
//		}

//		public void CloseSetting()
//		{
//			var popup = m_settingPopup.GetComponent<CommonPopup>();
//			popup.SetActiveFalse();
//		}

//		/// <summary>ホームに戻る</summary>
//		public async void OnBackHomeButton()
//		{
//			var text = await Language.Popup.HomeConfirm.PopupLocalize();
//			m_popupManager.ShowSelectPopup(text, BackHome);
//		}

//		void BackHome()
//		{
//			SaveData.Instance.Logs.Clear();
//			Scene.ChangeScene(SceneName.Home).Forget();
//		}

//		/// <summary>サウンド設定表示</summary>
//		public void ClickSoundSetting()
//		{
//			m_popupManager.ShowSoundPopup();
//		}

//		/// <summary>タイトルに戻る</summary>
//		public async void OnClickBackTitleButton()
//		{
//			var text = await Language.Popup.TitleConfirm.PopupLocalize();
//			m_popupManager.ShowSelectPopup(text, BackToTitle);
//		}

//		/// <summary>タイトルに戻る</summary>
//		async void BackToTitle()
//		{
//			try
//			{
//				await Scene.ChangeScene(SceneName.Title, true);

//				SoundManager.Instance.ResetVolume();
//			}
//			catch (OperationCanceledException)
//			{
//				Debug.Log("キャンセルされました。");
//			}
//		}

//		/// <summary>振動エフェクトのアクティブ設定</summary>
//		public void SetShakeActive()
//		{
//			m_talkWindow.IsShake = !m_talkWindow.IsShake;
//			var value = m_talkWindow.IsShake == true ? 1 : 0;
//			PlayerPrefs.SetInt(SHAKE_KEY, value);
//			m_shakeOn.SetActive(m_talkWindow.IsShake);
//		}
//	}
//}
