using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace Tarot
{
	public class TarotView : ViewBase
	{
		[SerializeField] UITransition m_explainText = null;
		[SerializeField] ChatGPT m_chatGPT = null;
		[SerializeField] GameObject m_blockTap = null;
		[SerializeField] UITransition m_startBtn = null;
		[SerializeField] UITransition m_subExplainText = null;
		[SerializeField] UITransition m_tarotBG = null;
		[SerializeField] ShareToX m_shareToX = null;

		[Header("シャッフルカード")]
		[SerializeField] GameObject m_cardObj = null;
		[SerializeField] Transform m_cardPos = null;
		[SerializeField] UITransition m_cardTransition = null;

		[Header("結果カード")]
		[SerializeField] UITransition m_leftResultTransition = null;
		[SerializeField] UITransition m_centerResultTransition = null;
		[SerializeField] UITransition m_rightResultTransition = null;
		[SerializeField] Image m_leftResultCard = null;
		[SerializeField] Image m_centerResultCard = null;
		[SerializeField] Image m_rightResultCard = null;

		[Header("リザルト")]
		[SerializeField] UITransition[] m_resultTransition = null;
		[SerializeField] TextMeshProUGUI m_resultText = null;
		[SerializeField] TextMeshProUGUI m_genreText = null;
		[SerializeField] TextMeshProUGUI m_leftText = null;
		[SerializeField] TextMeshProUGUI m_centerText = null;
		[SerializeField] TextMeshProUGUI m_rightText = null;

		// FIXME：二十押ししないように
		[Header("ボタン")]
		[SerializeField] ButtonEx m_closeBtn = null;
		[SerializeField] ButtonEx m_shareBtn = null;

		List<GameObject> m_cardList = new List<GameObject>();
		const int CARD_NUM = 22;
		const string CARD_PATH = "Cards/{0:d2}";
		CancellationTokenSource m_cts = new CancellationTokenSource();
		float m_halfWidth = 0;
		float m_halfHeight = 0;
		int m_leftCardIndex = -1;
		int m_centerCardIndex = -1;
		int m_rightCardIndex = -1;
		bool m_leftCardDirection = true;
		bool m_centerCardDirection = true;
		bool m_rightCardDirection = true;

		int m_genre = -1;

		//bool m_isSelection = false;

		const string AI_SEND_MESSAGE = "あなたは有能な占い師です。" +
			"\n以下の条件に基づき、相談者に向けた占いの結果を「過去・現在・未来」の３つの流れと、最後にアドバイスとして簡潔に320文字以内でまとめてください。" +
			"\n【相談内容】{0}" +
			"\n【過去】{1}({2})" +
			"\n【現在】{3}({4})" +
			"\n【未来】{5}({6})" +
			"\nお願いします。";
		async void OnEnable()
		{
			m_leftCardIndex =
			m_centerCardIndex = 
			m_rightCardIndex = -1;
			m_blockTap.SetActive(true);
			m_cardTransition.Canvas.alpha = 0;
			m_startBtn.gameObject.SetActive(false);
			m_subExplainText.gameObject.SetActive(false);
			m_tarotBG.gameObject.SetActive(false);
			m_resultText.text = "ロード中";

			foreach (var tra in m_resultTransition)
				tra.Canvas.alpha = 0;

			m_explainText.gameObject.SetActive(true);
			await m_explainText.TransitionInWait();

			await UniTask.Delay(TimeSpan.FromSeconds(1));

			// スタートボタン表示
			m_startBtn.gameObject.SetActive(true);
			await m_startBtn.TransitionInWait();

			// 移動範囲
			CanvasScaler canvasRect = Scene.GetComponent<CanvasScaler>();
			m_halfWidth = canvasRect.referenceResolution.x / 2f - 300;
			m_halfHeight = canvasRect.referenceResolution.y / 2f - 300;

			// カード作成
			CreateCards();
		}

		private void OnDisable()
		{
			foreach (var tra in m_resultTransition)
				tra.Canvas.alpha = 0;
		}

		public override void SetParam(int genre)
		{
			m_genre = genre;
		}

		/// <summary>スタートボタン押下</summary>
		public void OnClickStartShaffle()
		{
			StartShaffle();
		}

		async void StartShaffle()
		{
			List<UniTask> tasks = new List<UniTask>();

			tasks.Add(m_explainText.TransitionOutWait());
			tasks.Add(m_startBtn.TransitionOutWait());

			await UniTask.WhenAll(tasks);
			m_explainText.gameObject.SetActive(false);
			m_startBtn.gameObject.SetActive(false);

			tasks = new List<UniTask>();
			// 背景表示
			m_tarotBG.gameObject.SetActive(true);
			tasks.Add(m_tarotBG.TransitionInWait());
			// カード表示
			tasks.Add(m_cardTransition.TransitionInWait());

			await UniTask.WhenAll(tasks);

			// カードシャッフル
			await ShuffleCards();
		}

		/// <summary>シャッフルしたカードをクリック</summary>
		public void OnClickCard(Card card)
		{
			m_blockTap.SetActive(true);

			bool isNormal = false;
			var rectTransform = card.GetComponent<RectTransform>();
			if (m_leftCardIndex == -1)
			{
				m_leftCardIndex = card.CardIndex;
				isNormal = (rectTransform.eulerAngles.z <= 90 && rectTransform.eulerAngles.z > -90) ||
					(rectTransform.eulerAngles.z >= 270 && rectTransform.eulerAngles.z < 450);
				m_leftCardDirection = isNormal;
			}
			else if(m_centerCardIndex == -1)
			{
				m_centerCardIndex = card.CardIndex;
				isNormal = (rectTransform.eulerAngles.z <= 90 && rectTransform.eulerAngles.z > -90) ||
					(rectTransform.eulerAngles.z >= 270 && rectTransform.eulerAngles.z < 450);
				m_centerCardDirection = isNormal;
			}
			else if(m_rightCardIndex == -1)
			{
				m_rightCardIndex = card.CardIndex;
				isNormal = (rectTransform.eulerAngles.z <= 90 && rectTransform.eulerAngles.z > -90) ||
					(rectTransform.eulerAngles.z >= 270 && rectTransform.eulerAngles.z < 450);
				m_rightCardDirection = isNormal;
			}

			AfterSelectCard(card);
		}

		/// <summary>カードを選んだあとの処理</summary>
		async void AfterSelectCard(Card card)
		{
			await FadeOutCard(card);
			
			// ３枚選んだら
			if (SelectedThreeCards())
			{
				Result();
			}
		}

		async void Result()
		{
			// 先にAIに送っておく
			SendChatGPT();

			List<UniTask> tasks = new List<UniTask>();
			tasks.Add(m_subExplainText.TransitionOutWait());
			tasks.Add(m_cardTransition.TransitionOutWait());
			await UniTask.WhenAll(tasks);

			await ShowResult();
		}

		/// <summary>リザルト</summary>
		async UniTask ShowResult()
		{
			// カード画像セット
			SetCardImage();

			// カード表示演出
			await m_leftResultTransition.TransitionInWait();
			await m_centerResultTransition.TransitionInWait();
			await m_rightResultTransition.TransitionInWait();

			// リザルト表示
			List<UniTask> tasks = new List<UniTask>();
			foreach (var tra in m_resultTransition)
				tasks.Add(tra.TransitionInWait());

			await UniTask.WhenAll(tasks);
		}


		public void SendChatGPT()
		{
			var genre = CardName.GetGenre(m_genre);
			var leftText = CardName.GetTarotName(m_leftCardIndex);
			var leftDirection = CardName.GetDirection(m_leftCardDirection);
			var centerText = CardName.GetTarotName(m_centerCardIndex);
			var centerDirection = CardName.GetDirection(m_centerCardDirection);
			var rightText = CardName.GetTarotName(m_rightCardIndex);
			var rughtDirection = CardName.GetDirection(m_rightCardDirection);

			m_genreText.text = genre;
			m_leftText.text = string.Format("過去:{0}({1})", leftText, leftDirection);
			m_centerText.text = string.Format("現在:{0}({1})", centerText, centerDirection);
			m_rightText.text = string.Format("未来:{0}({1})", rightText, rughtDirection);

			var input = string.Format(AI_SEND_MESSAGE,
				genre,
				leftText,
				leftDirection,
				centerText,
				centerDirection,
				rightText,
				rughtDirection);
			Debug.Log(input);

			// 非同期処理
			_ = SendMessageToChatGPT(input);
		}

		/// <summary>ChatGPTにメッセージを送るAPI</summary>
		async Task SendMessageToChatGPT(string msg)
		{
			string reply = await m_chatGPT.SendMessageToGPT(msg);
			m_resultText.text = reply;
		}

		/// <summary>カードをフェードアウトさせる</summary>
		async UniTask FadeOutCard(Card card)
		{
			await card.FadeOut();
			m_blockTap.SetActive(false);
			card.gameObject.SetActive(false);
		}

		/// <summary>カードを３枚選んだか</summary>
		bool SelectedThreeCards()
		{
			return m_leftCardIndex != -1 && m_centerCardIndex != -1 && m_rightCardIndex != -1;
		}

		/// <summary>カード生成</summary>
		void CreateCards()
		{
			for (int i = 0; i < m_cardPos.childCount; i++)
			{
				var child = m_cardPos.GetChild(i);
				if (!child.gameObject.activeSelf)
					continue;

				Destroy(child.gameObject);
			}
			Debug.Log($"m_halfWidth: {m_halfWidth}, m_halfHeight: {m_halfHeight}");

			m_cardList = new List<GameObject>();
			// カード生成
			for (int i = 0; i < CARD_NUM; i++)
			{
				var card = Instantiate(m_cardObj, m_cardPos);
				card.GetComponent<Card>().CardIndex = i;

				float x = UnityEngine.Random.Range(-m_halfWidth, m_halfWidth);
				float y = UnityEngine.Random.Range(-m_halfHeight, m_halfHeight);

				// Z軸のランダムな角度（-180〜180度）
				float randomZ = UnityEngine.Random.Range(-180f, 180f);
				Quaternion randomRotation = Quaternion.Euler(0, 0, randomZ);

				var rect = card.GetComponent<RectTransform>();
				card.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(x, y, 1), randomRotation);
				card.SetActive(true);
				m_cardList.Add(card);
			}
		}

		/// <summary>カードをシャッフル</summary>
		async UniTask ShuffleCards()
		{
			List<UniTask> tasks = new List<UniTask>();

			// カード移動
			for (int i = 0; i < m_cardList.Count; i++)
			{
				var card = m_cardList[i];
				card.SetActive(true);
				// 終点を安全な範囲で設定
				float x = UnityEngine.Random.Range(-m_halfWidth, m_halfWidth);
				float y = UnityEngine.Random.Range(-m_halfHeight, m_halfHeight);
				Vector3 endPos = new Vector3(x, y, 0);

				// 中継点も安全範囲内でズレを出す
				Vector3 startPos = card.transform.localPosition;
				Vector3 curve1 = startPos + new Vector3(UnityEngine.Random.Range(-80f, 80f), UnityEngine.Random.Range(-80f, 80f));
				Vector3 curve2 = endPos + new Vector3(UnityEngine.Random.Range(-80f, 80f), UnityEngine.Random.Range(-80f, 80f));

				var tween = card.transform
					.DOLocalPath(new Vector3[] { curve1, curve2, endPos }, 1.5f, PathType.CatmullRom)
					.SetEase(Ease.OutSine)
					.ToUniTask();

				tasks.Add(tween);
			}
			await UniTask.WhenAll(tasks);

			tasks = new List<UniTask>();
			m_subExplainText.gameObject.SetActive(true);
			tasks.Add(m_subExplainText.TransitionInWait());
			await UniTask.WhenAll(tasks);

			m_blockTap.SetActive(false);
		}

		/// <summary>カード画像セット</summary>
		void SetCardImage()
		{
			LoadCardImage(m_leftCardIndex, m_leftResultCard, m_leftCardDirection);
			LoadCardImage(m_centerCardIndex, m_centerResultCard, m_centerCardDirection);
			LoadCardImage(m_rightCardIndex, m_rightResultCard, m_rightCardDirection);
		}

		/// <summary>カードのイメージをロード</summary>
		void LoadCardImage(int value, Image image, bool direction)
		{
			image.enabled = false;

			var path = string.Format(CARD_PATH, value);
			Sprite sp = Resources.Load<Sprite>(path);
			if (sp != null)
			{
				image.sprite = sp;
				image.enabled = true;
			}
			if (direction)
				image.transform.rotation = Quaternion.Euler(0, 0, 0);
			else
				image.transform.rotation = Quaternion.Euler(0, 0, 180f);

			Debug.Log(string.Format("direction:{0}", direction));
		}

		/// <summary>シェアボタン</summary>
		public void Share()
		{
			m_shareToX.Share();
		}

		public void Close()
		{
			ChangeView();

			m_leftResultTransition.TransitionOut();
			m_centerResultTransition.TransitionOut();
			m_rightResultTransition.TransitionOut(); 
		}

		async void ChangeView()
		{
			await Scene.ChangeView(ViewName.Genre, 0, m_cts.Token);
		}
	}
}
