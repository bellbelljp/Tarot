using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEditor.Overlays;
using UnityEditor.Localization.Plugins.XLIFF.V12;

namespace Tarot
{
	public class TalkWindow : MonoBehaviour
	{
		[Tooltip("名前テキスト"), SerializeField] TextMeshProUGUI m_nameText = null;
		[Tooltip("台詞テキスト"), SerializeField] TextMeshProUGUI m_talkText = null;
		[Tooltip("テキストWindowトランジション"), SerializeField] UITransition[] m_transitions = null;
		[Tooltip("矢印"), SerializeField] GameObject m_nextArrow = null;
		[Tooltip("選択肢"), SerializeField] SelectButtonDialog m_selectButtonDialog = null;

		[Header("キャラ画像")]
		[Tooltip("左キャラ画像"), SerializeField] GameViewCharacter m_leftChara = null;
		[Tooltip("中央キャラ画像"), SerializeField] GameViewCharacter m_centerChara = null;
		[Tooltip("右キャラ画像"), SerializeField] GameViewCharacter m_rightChara = null;

		[Header("キャラトランジション")]
		[Tooltip("左キャラFade"), SerializeField] UITransition m_leftCharaFade = null;
		[Tooltip("中央キャラFade"), SerializeField] UITransition m_centerCharaFade = null;
		[Tooltip("右キャラFade"), SerializeField] UITransition m_rightCharaFade = null;
		[Tooltip("左キャラ中央から移動"), SerializeField] UITransition m_leftCharaMoveL = null;
		[Tooltip("右キャラ中央から移動"), SerializeField] UITransition m_rightCharaMoveR = null;
		[Tooltip("中央キャラ左から移動"), SerializeField] UITransition m_centerCharaMoveLtoC = null;
		[Tooltip("中央キャラ右から移動"), SerializeField] UITransition m_centerCharaMoveRtoC = null;

		[Header("背景")]
		[Tooltip("背景画像"), SerializeField] Image m_bgImage = null;
		[Tooltip("背景トランジション"), SerializeField] UITransition m_bgTransition = null;

		[Header("ボタン")]
		[SerializeField] GameObject m_autoButtonImage = null;
		[SerializeField] GameObject m_skipButtonImage = null;

		[Header("ログ")]
		[SerializeField] UIGameViewLogWindow m_logWindow = null;

		[Header("エフェクト")]
		[SerializeField] UITransition m_lightEffect = null;
		[SerializeField] Transform[] m_shake = null;
		[SerializeField] UITransition m_fireEffect = null;
		[SerializeField] UITransition m_waterEffect = null;
		[SerializeField] UITransition m_blackEffect = null;

		[Header("キャラ情報")]
		string m_currentLeft = string.Empty;
		string m_currentCenter = string.Empty;
		string m_currentRight = string.Empty;

		// 次へフラグ
		bool m_goToNextPage = false;
		// 次へいけるフラグ
		bool m_currentPageCompleted = false;
		// 全文表示フラグ
		bool m_isAllText = false;
		bool m_isInTag = false;
		// スキップフラグ
		bool m_isSkip = false;
		// オートフラグ
		bool m_isAuto = false;
		string m_tagStrings = string.Empty;
		int m_currentChapter = 0;
		string m_currentBGM = string.Empty;
		bool isQuitting = false;
		bool m_isFirstSentence = true;
		string m_lang = string.Empty;

		// シェイクの強度と時間
		bool m_isShake = true;
		public bool IsShake { get { return m_isShake; } set { m_isShake = value; } }
		const float SHAKE_DURATION = 1f;
		const float SHAKE_STRENGTH = 30f;

		SaveData.Data m_saveData = null;
		CancellationToken m_token;

		/// <summary>アプリ終了時</summary>
		void OnApplicationQuit()
		{
			isQuitting = true;
		}

		private async void OnEnable()
		{
			m_saveData = await SaveData.Instance.Load();
			//Debug.Log("ロード");
			SetCharacter(null).Forget();
			foreach (var tra in m_transitions)
				tra.gameObject.SetActive(false);

			m_autoButtonImage.SetActive(false);
			m_skipButtonImage.SetActive(false);
			//m_logWindow.gameObject.SetActive(false);
			m_isFirstSentence = true;
			m_lang = PlayerPrefs.GetString("selected-locale", "ja");

			// エフェクトオフ
			EffectOff();
		}

		private void OnDisable()
		{
			m_isSkip = false;
			m_isAuto = false;
			m_currentBGM = string.Empty;
			if (!isQuitting)
				SoundManager.Instance.StopVoice();
		}

		/// <summary>会話開始</summary>
		public async UniTask<List<int>> TalkStart(List<StoryMaster.Data> talkList, CancellationToken token, SaveData.Data saveData, float wordInterval = 0.1f)
		{
			m_token = token;
			// すでにキャンセルされているなら例外を投げる
			m_token.ThrowIfCancellationRequested();
			if (m_lang == "en")
				wordInterval = 0.03f;

			try
			{
				List<int> responseList = new List<int>();
				foreach (var talk in talkList)
				{
					// すでにキャンセルされているなら例外を投げる
					m_token.ThrowIfCancellationRequested();

					// BGM再生
					if (talk.BGM != string.Empty && m_currentBGM != talk.BGM)
					{
						m_currentBGM = talk.BGM;
						SoundManager.Instance.PlayBGM(talk.BGM);
					}

					// SE再生
					if (talk.SE != string.Empty && !m_isSkip)
					{
						SoundManager.Instance.PlaySE(talk.SE);
					}

					// キャラセット
					SetCharacter(talk).Forget();

					// 背景セット
					if (talk.Place > 0)
					{
						// MEMO：このシーンだけどうしても背景待ちたい
						if (talk.Effect == "LightOff")
							await SetBG(talk.Place, false);
						else
							SetBG(talk.Place, false).Forget();
					}

					// エフェクト
					if (talk.Effect != string.Empty)
					{
						SetEffect(talk.Effect).Forget();
					}

					// ログ要素作成
					UIGameViewLogWindow.Log log = new UIGameViewLogWindow.Log();

					var nextSerif = 0;
					// 選択肢の場合
					if (talk.Talk.Count('@') > 1)
					{
						m_goToNextPage = false;
						m_currentPageCompleted = false;
						m_isAllText = false;
						m_nextArrow.SetActive(false);
						await SetCharacter(talk);
						AutoSave(talk);
						// セーブ後に名前変換
						talk.Talk = talk.Talk.ReplaceName();
						// ","で分割
						string[] arr = talk.Talk.Split(',');
						// res:1@XXX, 2@XXXの数値
						var (selectedId, buttonIndex) = await m_selectButtonDialog.CreateButtons(true, arr, m_token);

						responseList.Add(selectedId);

						m_goToNextPage = true;

						// すでにキャンセルされているなら例外を投げる
						m_token.ThrowIfCancellationRequested();

						// ログに選択肢をコピー
						var selectText = arr[buttonIndex];
						//var index = int.Parse(selectText.Substring(0, selectText.IndexOf("@")));
						selectText = selectText.Substring(selectText.IndexOf("@") + 1);
						log.LogText = selectText;
						//// ログに追加
						SaveData.Instance.Logs.Enqueue(log);
						if (SaveData.Instance.Logs.Count > UIGameViewLogWindow.MAX_LOG)
							_ = SaveData.Instance.Logs.Dequeue();

						// スキップをキャンセル
						m_isSkip = false;
						m_skipButtonImage.SetActive(false);

						return responseList;
					}
					else
					{
						var talkText = string.Empty;
						// 台詞遷移
						if (talk.Talk.Contains('＠'))
						{
							Debug.LogWarning("全角の＠が含まれてるよ");
						}
						else if (talk.Talk.Contains('@'))
						{
							talkText = talk.Talk.Substring(talk.Talk.IndexOf('@') + 1);
							nextSerif = int.Parse(talk.Talk.Substring(0, talk.Talk.IndexOf('@')));
							responseList.Add(nextSerif);
						}
						else
						{
							talkText = talk.Talk;
						}

						// 名前セット
						if (talk.Chara == csdef.YOUR_NAME_ID)
						{
							m_nameText.text = "ぼく"; /* SaveData.Instance.CommonData.YourName;*/
						}
						else
						{
							m_nameText.text = await CharacterMaster.Instance.GetCharacterName(talk.Chara);
						}
						m_talkText.text = string.Empty;
						m_goToNextPage = false;
						m_currentPageCompleted = false;
						m_isAllText = false;
						m_nextArrow.SetActive(false);
						await SetCharacter(talk);

						// パラメーター
						//saveData.CharacteDataList[saveData.CharaId - 1].LoveParam += talk.Param;
						//Debug.Log("べる：" + saveData.CharacteDataList[0].LoveParam);
						//Debug.Log("キンキン：" + saveData.CharacteDataList[1].LoveParam);
						//Debug.Log("真奈美：" + saveData.CharacteDataList[2].LoveParam);
						
						// リザルトフェーズで振られたら、残りキャラからRemoveする
						//var param = saveData.CharacteDataList[saveData.CharaId - 1].LoveParam;
						//if(talk.Conditoin == 0 && 
						//	saveData.Phase >= csdef.Phase.RESULT && saveData.Phase < csdef.Phase.RESULT + csdef.Phase.INTERVAL &&
						//	saveData.LastCharaIdList.Contains(saveData.CharaId))
						//	saveData.LastCharaIdList.Remove(saveData.CharaId);

						// 解放プロフィール
						if(talk.Release > 0)
						{
							if(!saveData.CharacteDataList[saveData.CharaId - 1].OpenProfile.Contains(talk.Release))
								saveData.CharacteDataList[saveData.CharaId - 1].OpenProfile.Add(talk.Release);
						}

						//AutoSave(/*talk*/saveData);
						AutoSave(talk);
						// セーブ後に名前変換
						talkText = talkText.ReplaceName();
						await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.gameObject.GetCancellationTokenOnDestroy());


						//// ボイス再生
						//var voice = StoryData.Instance.GetVoice(talk.Chara, talk.Id);
						//if (voice != null && !m_isSkip)
						//	SoundManager.Instance.PlayVoice(voice);

						// ログのQueueをListに変換して末尾の要素を取得
						string lastElement = string.Empty;
						if (m_isFirstSentence)
						{
							List<UIGameViewLogWindow.Log> list = new List<UIGameViewLogWindow.Log>(SaveData.Instance.Logs);
							if (list.Count != 0)
								lastElement = list[list.Count - 1].LogText;
							m_isFirstSentence = false;
						}
						// 最後の要素とテキストが異なっていればログに追加
						if (talkText != lastElement)
						{
							log.LogText = talkText;
							log.NameText = m_nameText.text;
							SaveData.Instance.Logs.Enqueue(log);
							if (SaveData.Instance.Logs.Count > UIGameViewLogWindow.MAX_LOG)
								_ = SaveData.Instance.Logs.Dequeue();
						}

						foreach (var word in talkText)
						{
							// すでにキャンセルされているなら例外を投げる
							m_token.ThrowIfCancellationRequested();

							if (m_isSkip == true)
							{
								m_talkText.text = talkText;
								break;
							}

							bool isCloseTag = false;
							if (word.ToString() == "<")
							{
								m_isInTag = true;
							}
							else if (word.ToString() == ">")
							{
								m_isInTag = false;
								isCloseTag = true;
							}

							if (m_isInTag == false && isCloseTag == false && string.IsNullOrEmpty(m_tagStrings) == false)
							{
								// タグ付きの場合
								var word2 = m_tagStrings + word;
								m_talkText.text += word2;
								m_tagStrings = string.Empty;
							}
							else if (m_isInTag == true || isCloseTag == true)
							{
								// タグ内
								m_tagStrings += word;
								continue;
							}
							else
							{
								m_talkText.text += word;
							}

							await UniTask.Delay(TimeSpan.FromSeconds(wordInterval), cancellationToken: this.gameObject.GetCancellationTokenOnDestroy());

							if (m_isAllText == true)
							{
								m_talkText.text = talkText;
								break;
							}
						}
					}

					m_currentPageCompleted = true;
					m_nextArrow.SetActive(true);
					// 複数のTokenを合成
					var tokens = CancellationTokenSource.CreateLinkedTokenSource(new[] { m_token, this.gameObject.GetCancellationTokenOnDestroy() });
					// オートの場合は少し待つ
					if (m_isAuto)
					{
						await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: tokens.Token);
						// ボイスが鳴り終わるのを待つ
						await SoundManager.Instance.WaitFinishVoice();
					}

					// スキップの場合は1フレーム待つ
					if (m_isSkip)
						await UniTask.DelayFrame(1, cancellationToken: tokens.Token);

					await UniTask.WaitUntil(() => m_isSkip || m_isAuto || m_goToNextPage, cancellationToken: tokens.Token);

					// クリック音
					if (!m_isAuto && !m_isSkip)
						SoundManager.Instance.PlayClickSE(SoundManager.SEType.Click);
					// SE停止
					StopSE(talk.SE);

					// キャラクターの口パク競合停止
					//m_leftChara.StopVoice();
					//m_centerChara.StopVoice();
					//m_rightChara.StopVoice();

					if (nextSerif != 0)
						return responseList;
				}

				return responseList;
			}
			catch (OperationCanceledException e)
			{
				Debug.Log("TalkStartキャンセル：" + e);
				throw e;
			}
		}

		async void StopSE(string talkSE)
		{
			// FIXME:フェードアウト
			if (m_isSkip)
			{
				SoundManager.Instance.StopSE();
			}
			else if (talkSE == string.Empty)
			{
				await SoundManager.Instance.WaitFinishSE();
				SoundManager.Instance.StopSE();
			}
		}

		/// <summary>オートセーブ（キャラの描画終わってから）</summary>
		async void AutoSave(StoryMaster.Data talk)
		{
			// チャプターが異なったらチャプター追加
			//if (m_currentChapter != 0 && m_currentChapter != talk.Chapter && talk.Chapter != 0)
			//{
			//	// 結果をセーブ
			//	var chapter = await ChapterData.Instance.GetData(m_saveData.CharaId, talk.Chapter);
			//	var hasChapter = SaveData.Instance.CommonData.ChapterList.Find(it => it.ChapterId == chapter.ChapterId && it.CharaId == chapter.CharaId);
			//	if (hasChapter == null)
			//		SaveData.Instance.CommonData.ChapterList.Add(chapter);
			//}
			// 常にオートセーブ
			m_saveData.Phase = talk.Id;
			Debug.Log("オートセーブ：" + m_saveData.Phase);
			//m_saveData.ChapterId = talk.Chapter;
			if (!m_isSkip)
				m_saveData.ScreenshotName = await SaveData.Instance.GetScreenShot();

			SaveData.Instance.Save(m_saveData);
			//m_currentChapter = talk.Chapter;
		}

		public async UniTask SaveScreenShot()
		{
			if (m_saveData == null)
				return;

			m_saveData.ScreenshotName = await SaveData.Instance.GetScreenShot();
			SaveData.Instance.Save(m_saveData);
		}

		/// <summary>次へボタンクリック</summary>
		public void OnNextButtonClicked()
		{
			if (m_currentPageCompleted == true)
				m_goToNextPage = true;
			else
				m_isAllText = true;
		}

		/// <summary>ウィンドウを開く</summary>
		public async UniTask Open(string initName = "", string initText = "")
		{
			SetCharacter(null).Forget();
			m_nameText.text = initName;
			m_talkText.text = initText;
			m_nextArrow.SetActive(false);
			List<UniTask> task = new List<UniTask>();
			foreach (var tra in m_transitions)
			{
				tra.gameObject.SetActive(true);
				task.Add(tra.TransitionInWait());
			}
			await task;
		}

		/// <summary>ウィンドウを閉じる</summary>
		public async UniTask Close()
		{
			List<UniTask> task = new List<UniTask>();
			foreach (var tra in m_transitions)
			{
				task.Add(tra.TransitionOutWait());
			}
			await task;
			foreach (var tra in m_transitions)
			{
				tra.gameObject.SetActive(false);
			}
		}

		/// <summary>オートクリック</summary>
		public void OnClickAuto()
		{
			m_isAuto = !m_isAuto;
			m_autoButtonImage.SetActive(m_isAuto);

			// オートとスキップは共存しない
			m_isSkip = false;
			m_skipButtonImage.SetActive(false);
		}

		/// <summary>スキップクリック</summary>
		public void OnClickSkip()
		{
			m_isSkip = !m_isSkip;
			m_skipButtonImage.SetActive(m_isSkip);

			// オートとスキップは共存しない
			m_isAuto = false;
			m_autoButtonImage.SetActive(false);
		}

		/// <summary>キャラクターセット</summary>
		async UniTask SetCharacter(StoryMaster.Data storyData)
		{
			// nullなら全て消す
			if (storyData == null)
			{
				m_leftChara.gameObject.SetActive(false);
				m_centerChara.gameObject.SetActive(false);
				m_rightChara.gameObject.SetActive(false);
				m_currentLeft = string.Empty;
				m_currentCenter = string.Empty;
				m_currentRight = string.Empty;
				return;
			}

			try
			{
				var tasks = new List<UniTask>();
				bool hideLeft = false;
				bool hideCenter = false;
				bool hideRight = false;

				// 左キャラ
				var currentLeft = string.IsNullOrEmpty(m_currentLeft) ? 0 : int.Parse(Regex.Replace(m_currentLeft, @"[^0-9]", ""));
				var nextLeft = string.IsNullOrEmpty(storyData.Left) ? 0 : int.Parse(Regex.Replace(storyData.Left, @"[^0-9]", ""));

				// 右キャラ
				var currentRight = string.IsNullOrEmpty(m_currentRight) ? 0 : int.Parse(Regex.Replace(m_currentRight, @"[^0-9]", ""));
				var nextRight = string.IsNullOrEmpty(storyData.Right) ? 0 : int.Parse(Regex.Replace(storyData.Right, @"[^0-9]", ""));

				// 中央キャラ
				var currentCenter = string.IsNullOrEmpty(m_currentCenter) ? 0 : int.Parse(Regex.Replace(m_currentCenter, @"[^0-9]", ""));
				var nextCenter = string.IsNullOrEmpty(storyData.Center) ? 0 : int.Parse(Regex.Replace(storyData.Center, @"[^0-9]", ""));

				// 左キャラ設定
				{
					// nullの時
					if (string.IsNullOrEmpty(storyData.Left) == true)
					{
						if (!string.IsNullOrEmpty(m_currentLeft))
							tasks.Add(m_leftCharaFade.TransitionOutWait());
						hideLeft = true;
						m_currentLeft = string.Empty;
					}
					else if (m_currentLeft != storyData.Left)
					{
						m_leftChara.SetCharacter(storyData.Left, storyData.Chara);
						m_leftChara.gameObject.SetActive(true);
						//CharacterData.Instance.SetSprite(storyData.Left, (sp) =>
						//{
						//	if(sp != null)
						//	{
						//		m_leftChara.enabled = true;
						//	}
						//});
						// キャラが変更されていれば待機する
						// 中央から左の場合はスライド
						if (currentLeft == 0 && nextLeft == currentCenter)
						{
							// 左スライド
							tasks.Add(m_leftCharaMoveL.TransitionInWait());
							// 中央はフェードアウトせずに普通に消す
							hideCenter = true;
							m_centerChara.gameObject.SetActive(false);
						}
						// その他はフェード
						else if (currentLeft != nextLeft)
						{
							tasks.Add(m_leftCharaFade.TransitionInWait());
						}

						m_currentLeft = storyData.Left;
					}
					else if (m_currentLeft == storyData.Left)
					{
						var showCharaId = int.Parse(Regex.Replace(storyData.Left, @"[^0-9]", ""));
						// 立ち絵と喋り手が異なる場合
						if (showCharaId != storyData.Chara)
						{
							m_leftChara.SetExpression(showCharaId, storyData.Chara);
						}
					}
				}

				// 右キャラ設定
				{
					// nullの時
					if (string.IsNullOrEmpty(storyData.Right) == true)
					{
						if (!string.IsNullOrEmpty(m_currentRight))
							tasks.Add(m_rightCharaFade.TransitionOutWait());
						hideRight = true;
						m_currentRight = string.Empty;
					}
					else if (m_currentRight != storyData.Right)
					{
						m_rightChara.SetCharacter(storyData.Right, storyData.Chara);
						m_rightChara.gameObject.SetActive(true);
						//CharacterData.Instance.SetSprite(storyData.Right, (sp) =>
						//{
						//	if (sp != null)
						//	{
						//		m_rightChara.enabled = true;
						//		m_rightChara.sprite = sp;
						//	}
						//});

						// キャラが変更されていれば待機する
						// 中央から右の場合はスライド
						if (currentRight == 0 && nextRight == currentCenter)
						{
							// 右スライド
							tasks.Add(m_rightCharaMoveR.TransitionInWait());
							// 中央はフェードアウトせずに普通に消す
							hideCenter = true;
							m_centerChara.gameObject.SetActive(false);
						}
						// その他はフェード
						else if (currentRight != nextRight)
						{
							tasks.Add(m_rightCharaFade.TransitionInWait());
						}

						m_currentRight = storyData.Right;
					}
					else if (m_currentRight == storyData.Right)
					{
						var showCharaId = int.Parse(Regex.Replace(storyData.Right, @"[^0-9]", ""));
						// 立ち絵と喋り手が異なる場合
						if (showCharaId != storyData.Chara)
						{
							m_rightChara.SetExpression(showCharaId, storyData.Chara);
						}
					}
				}

				// 中央キャラ設定
				{
					// nullの時
					if (string.IsNullOrEmpty(storyData.Center) == true)
					{
						if (!string.IsNullOrEmpty(m_currentCenter))
							tasks.Add(m_centerCharaFade.TransitionOutWait());
						hideCenter = true;
						m_currentCenter = string.Empty;
					}
					else if (m_currentCenter != storyData.Center)
					{
						m_centerChara.SetCharacter(storyData.Center, storyData.Chara);
						m_centerChara.gameObject.SetActive(true);
						//CharacterMaster.Instance.SetSprite(storyData.Center, (sp) =>
						//{
						//	if (sp != null)
						//	{
						//		m_centerChara.enabled = true;
						//		m_centerChara.sprite = sp;
						//	}
						//});

						// キャラが変更されていれば待機する
						// 左から中央の場合はスライド
						if (currentCenter == 0 && nextCenter == currentLeft)
						{
							// 中央スライド
							tasks.Add(m_centerCharaMoveLtoC.TransitionInWait());
							// 左はフェードアウトせずに普通に消す
							hideLeft = true;
							m_leftChara.gameObject.SetActive(false);
						}
						// 右から中央の場合はスライド
						else if (currentCenter == 0 && nextCenter == currentRight)
						{
							// 中央スライド
							tasks.Add(m_centerCharaMoveRtoC.TransitionInWait());
							// 右はフェードアウトせずに普通に消す
							hideRight = true;
							m_rightChara.gameObject.SetActive(false);
						}
						// その他はフェード
						else if (currentCenter != nextCenter)
						{
							tasks.Add(m_centerCharaFade.TransitionInWait());
						}

						m_currentCenter = storyData.Center;
					}
					else if (m_currentCenter == storyData.Center)
					{
						var showCharaId = int.Parse(Regex.Replace(storyData.Center, @"[^0-9]", ""));
						// 立ち絵と喋り手が異なる場合
						if (showCharaId != storyData.Chara)
						{
							m_centerChara.SetExpression(showCharaId, storyData.Chara);
						}
					}
				}

				// 待機
				await UniTask.WhenAll(tasks);

				// 消したいキャラを消す
				if (hideLeft == true) m_leftChara.gameObject.SetActive(false);
				if (hideCenter == true) m_centerChara.gameObject.SetActive(false);
				if (hideRight == true) m_rightChara.gameObject.SetActive(false);
			}
			catch (OperationCanceledException e)
			{
				m_currentLeft = string.Empty;
				m_currentCenter = string.Empty;
				m_currentRight = string.Empty;
				Debug.Log("SetCharacterキャンセル：" + e);
				throw e;
			}
		}

		/// <summary>キャラクターリセット</summary>
		public void ResetCharacter()
		{
			m_currentLeft = string.Empty;
			m_currentCenter = string.Empty;
			m_currentRight = string.Empty;
		}

		/// <summary>背景セット</summary>
		public async UniTask SetBG(int placeId, bool isImmediate)
		{
			m_bgTransition.gameObject.SetActive(true);
			if (placeId == 0 || ( m_bgImage.sprite != null && m_bgImage.sprite.name == string.Format("BG{0}", placeId)))
			{
				// 同じ背景なのでReturn
				m_bgImage.enabled = true;
				return;
			}

			if (isImmediate == false && !m_isSkip)
			{
				try
				{
					await m_bgTransition.TransitionOutWait();
					SetBG();
					await m_bgTransition.TransitionInWait();
				}
				catch (OperationCanceledException e)
				{
					Debug.Log("背景セットキャンセル：" + e);
					SetBG();
					throw e;
				}
			}
			else
			{
				SetBG();
			}

			void SetBG()
			{
				m_bgImage.enabled = false;
				BackgroundMaster.Instance.SetSprite(placeId, (sp) =>
				{
					if (sp != null)
					{
						m_bgImage.sprite = sp;
						m_bgImage.enabled = true;
					}
				});
			}
		}

		/// <summary>BGを消す</summary>
		public void BGOff()
		{
			m_bgImage.enabled = false;
		}

		/// <summary>エフェクトをセット</summary>
		public async UniTask SetEffect(string effect)
		{
			switch (effect)
			{
				case "BlackOut": // 暗転
					if (m_blackEffect.gameObject.activeSelf) break;
					m_blackEffect.gameObject.SetActive(true);
					await m_blackEffect.TransitionInWait();
					m_blackEffect.gameObject.SetActive(false);
					break;
				case "LightOn":
					m_lightEffect.gameObject.SetActive(true);
					await m_lightEffect.TransitionInWait();
					break;
				case "LightOff":
					await m_lightEffect.TransitionOutWait();
					m_lightEffect.gameObject.SetActive(false);
					break;
				case "Shake":
					if (!m_isShake) break;
					foreach (var obj in m_shake)
					{
						_ = obj.DOShakePosition(SHAKE_DURATION, SHAKE_STRENGTH);
					}
					break;
				case "Fire":
					if (m_fireEffect.gameObject.activeSelf) break;
					m_fireEffect.gameObject.SetActive(true);
					await m_fireEffect.TransitionInWait();
					break;
				case "Water":
					if (m_waterEffect.gameObject.activeSelf) break;
					m_waterEffect.gameObject.SetActive(true);
					await m_waterEffect.TransitionInWait();
					break;
			}
		}

		/// <summary>エフェクトオフ</summary>
		public void EffectOff()
		{
			m_lightEffect.gameObject.SetActive(false);
			m_fireEffect.gameObject.SetActive(false);
			m_waterEffect.gameObject.SetActive(false);
			m_blackEffect.gameObject.SetActive(false);
		}

		/// <summary>AutoとSkipを取り消す</summary>
		public void OffAutoSkip()
		{
			if (m_isSkip)
				OnClickSkip();
			if (m_isAuto)
				OnClickAuto();
		}

		/// <summary>ログを表示</summary>
		public void ShowLog()
		{
			OffAutoSkip();
			m_logWindow.ShowLogs();
			m_logWindow.gameObject.SetActive(true);
		}
	}
}
