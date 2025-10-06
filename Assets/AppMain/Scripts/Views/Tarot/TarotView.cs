using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public interface ITarotView
	{
		public void SetButtonInteractable(bool flg);
		public void DestroyCards();
		public GameObject InstantiateCard();
	}

	public class TarotView : ViewBase, ITarotView
	{
		[SerializeField] UITransition m_explainText = null;
		[SerializeField] ChatGPT m_chatGPT = null;
		[SerializeField] GameObject m_blockTap = null;
		[SerializeField] UITransition m_startBtnTransition = null;
		[SerializeField] UITransition m_subExplainText = null;
		[SerializeField] UITransition m_tarotBG = null;
		//[SerializeField] ShareToX m_shareToX = null;

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
		[SerializeField] ButtonEx m_startBtn = null;

		List<GameObject> m_cardList = new List<GameObject>();
		const int CARD_NUM = 22;
		//const string CARD_PATH = "Cards/{0:d2}";
		CancellationTokenSource m_cts = new CancellationTokenSource();
		//float m_halfWidth = 0;
		//float m_halfHeight = 0;
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

		TarotPresenter m_presenter = null;

		public void SetPresenter(TarotPresenter presenter)
		{
			m_presenter = presenter;
		}

		async void OnEnable()
		{
			SetButtonInteractable(true);
			m_startBtn.interactable = true;
			m_leftCardIndex =
			m_centerCardIndex = 
			m_rightCardIndex = -1;
			m_blockTap.SetActive(true);
			m_cardTransition.Canvas.alpha = 0;
			m_startBtnTransition.gameObject.SetActive(false);
			m_subExplainText.gameObject.SetActive(false);
			m_tarotBG.gameObject.SetActive(false);
			m_resultText.text = "ロード中";

			foreach (var tra in m_resultTransition)
				tra.Canvas.alpha = 0;

			m_explainText.gameObject.SetActive(true);
			await m_explainText.TransitionInWait();

			await UniTask.Delay(TimeSpan.FromSeconds(1));

			// スタートボタン表示
			m_startBtnTransition.gameObject.SetActive(true);
			await m_startBtnTransition.TransitionInWait();

			// 移動範囲
			CanvasScaler canvasRect = Scene.GetComponent<CanvasScaler>();
			m_presenter.SetMovingRange(canvasRect);

			// カード作成
			m_presenter.CreateCards();
			//CreateCards();
		}

		private void OnDisable()
		{
			foreach (var tra in m_resultTransition)
				tra.Canvas.alpha = 0;
		}

		public override void SetParam(int genre)
		{
			//FIXME
			m_genre = genre;
			m_presenter.SetGenre(genre);
		}

		/// <summary>スタートボタン押下</summary>
		public void OnClickStartShaffle()
		{
			m_startBtn.interactable = false;
			StartShaffle().Forget();
		}

		async UniTask StartShaffle()
		{
			List<UniTask> tasks = new List<UniTask>();

			tasks.Add(m_explainText.TransitionOutWait());
			tasks.Add(m_startBtnTransition.TransitionOutWait());

			await UniTask.WhenAll(tasks);
			m_explainText.gameObject.SetActive(false);
			m_startBtnTransition.gameObject.SetActive(false);

			tasks = new List<UniTask>();
			// 背景表示
			m_tarotBG.gameObject.SetActive(true);
			tasks.Add(m_tarotBG.TransitionInWait());
			// カード表示
			tasks.Add(m_cardTransition.TransitionInWait());

			await UniTask.WhenAll(tasks);

			// カードシャッフル
			await m_presenter.ShuffleCards();

			m_subExplainText.gameObject.SetActive(true);
			await m_subExplainText.TransitionInWait();

			m_blockTap.SetActive(false);
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
			//await SendChatGPT();

			List<UniTask> tasks = new List<UniTask>();
			tasks.Add(SendChatGPT());
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

			m_blockTap.SetActive(true);

			await UniTask.WhenAll(tasks);
		}


		public async UniTask SendChatGPT()
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

			// 非同期処理
			m_resultText.text = await m_presenter.SendMessageToChatGPT(input);
		}

		/// <summary>カードをフェードアウトさせる</summary>
		async UniTask FadeOutCard(Card card)
		{
			await card.FadeOut();
			if(!SelectedThreeCards())
				m_blockTap.SetActive(false);
			card.gameObject.SetActive(false);
		}

		/// <summary>カードを３枚選んだか</summary>
		bool SelectedThreeCards()
		{
			return m_leftCardIndex != -1 && m_centerCardIndex != -1 && m_rightCardIndex != -1;
		}

		public void DestroyCards()
		{
			for (int i = 0; i < m_cardPos.childCount; i++)
			{
				var child = m_cardPos.GetChild(i);
				if (!child.gameObject.activeSelf)
					continue;

				Destroy(child.gameObject);
			}
		}

		public GameObject InstantiateCard()
		{
			return Instantiate(m_cardObj, m_cardPos);
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
			var sp = m_presenter.LoadCardImage(value);
			if (sp != null)
			{
				image.sprite = sp;
				image.enabled = true;
			}
			if (direction)
				image.transform.rotation = Quaternion.Euler(0, 0, 0);
			else
				image.transform.rotation = Quaternion.Euler(0, 0, 180f);
		}

		/// <summary>シェアボタン</summary>
		public void ClickToShare() => ShareToX().Forget();
		async UniTask ShareToX()
		{
			await m_presenter.ShareToX();
		}

		public void Close()
		{
			SetButtonInteractable(false);
			ChangeView();

			m_leftResultTransition.TransitionOut();
			m_centerResultTransition.TransitionOut();
			m_rightResultTransition.TransitionOut(); 
		}

		public void SetButtonInteractable(bool flg)
		{
			m_closeBtn.interactable = flg;
			m_shareBtn.interactable = flg;
		}

		async void ChangeView()
		{
			await Scene.ChangeView(ViewName.Genre, 0, m_cts.Token);
		}
	}
}
