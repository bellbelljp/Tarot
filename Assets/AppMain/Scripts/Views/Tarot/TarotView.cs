using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
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
		public void SetResultText(string genre, string past, string now, string futer);
		public void SetCardImage(int leftIndex, int centerIndex, int rightIndex, bool leftDir, bool centerDir, bool rightDir);
	}

	public class TarotView : ViewBase, ITarotView
	{
		[SerializeField] UITransition m_explainText = null;
		[SerializeField] ChatGPT m_chatGPT = null;
		[SerializeField] GameObject m_blockTap = null;
		[SerializeField] UITransition m_startBtnTransition = null;
		[SerializeField] UITransition m_subExplainText = null;
		[SerializeField] UITransition m_tarotBG = null;

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

		CancellationTokenSource m_cts = new CancellationTokenSource();
		TarotPresenter m_presenter = null;

		public void SetPresenter(TarotPresenter presenter)
		{
			m_presenter = presenter;
			m_presenter.Init();
		}

		async void OnEnable()
		{
			SetButtonInteractable(true);
			m_startBtn.interactable = true;
			BlockTapActive(true);
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
		}

		private void OnDisable()
		{
			foreach (var tra in m_resultTransition)
				tra.Canvas.alpha = 0;
		}

		public override void SetParam(int genre)
		{
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

			BlockTapActive(false);
		}

		/// <summary>シャッフルしたカードをクリック</summary>
		public void OnClickCard(Card card)
		{
			BlockTapActive(true);

			m_presenter.OnClickCard(card);

			AfterSelectCard(card).Forget();
		}

		/// <summary>カードを選んだあとの処理</summary>
		async UniTask AfterSelectCard(Card card)
		{
			await FadeOutCard(card);
			
			// ３枚選んだら
			if (m_presenter.SelectedThreeCards())
			{
				await Result();
			}
		}

		async UniTask Result()
		{
			List<UniTask> tasks = new List<UniTask>();
			tasks.Add(SendingAndReceivingChatGPT());
			tasks.Add(m_subExplainText.TransitionOutWait());
			tasks.Add(m_cardTransition.TransitionOutWait());
			await UniTask.WhenAll(tasks);

			await ShowResult();
		}

		/// <summary>ChatGPTに送受信</summary>
		public async UniTask SendingAndReceivingChatGPT()
		{
			// 非同期処理
			m_resultText.text = await m_presenter.SendingAndReceivingChatGPT();
		}

		/// <summary>リザルト</summary>
		async UniTask ShowResult()
		{
			// カード画像セット
			m_presenter.SetResultCardImage();

			// カード表示演出
			await m_leftResultTransition.TransitionInWait();
			await m_centerResultTransition.TransitionInWait();
			await m_rightResultTransition.TransitionInWait();

			// リザルト表示
			List<UniTask> tasks = new List<UniTask>();
			foreach (var tra in m_resultTransition)
				tasks.Add(tra.TransitionInWait());

			BlockTapActive(true);

			await UniTask.WhenAll(tasks);
		}

		/// <summary>結果テキストセット</summary>
		public void SetResultText(string genre, string past, string now, string futer)
		{
			m_genreText.text = genre;
			m_leftText.text = past;
			m_centerText.text = now;
			m_rightText.text = futer;
		}

		/// <summary>カードをフェードアウトさせる</summary>
		async UniTask FadeOutCard(Card card)
		{
			await card.FadeOut();
			if(!m_presenter.SelectedThreeCards())
				BlockTapActive(false);
			card.gameObject.SetActive(false);
		}

		/// <summary>カード削除</summary>
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

		/// <summary>カード生成</summary>
		public GameObject InstantiateCard()
		{
			return Instantiate(m_cardObj, m_cardPos);
		}

		/// <summary>カード画像セット</summary>
		public void SetCardImage(int leftIndex, int centerIndex, int rightIndex, bool leftDir, bool centerDir, bool rightDir)
		{
			LoadCardImage(leftIndex, m_leftResultCard, leftDir);
			LoadCardImage(centerIndex, m_centerResultCard, centerDir);
			LoadCardImage(rightIndex, m_rightResultCard, rightDir);
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
			m_presenter.ChangeView(Scene, m_cts.Token);

			m_leftResultTransition.TransitionOut();
			m_centerResultTransition.TransitionOut();
			m_rightResultTransition.TransitionOut(); 
		}

		public void SetButtonInteractable(bool flg)
		{
			m_closeBtn.interactable = flg;
			m_shareBtn.interactable = flg;
		}

		public void BlockTapActive(bool flg)
		{
			m_blockTap.SetActive(flg);
		}
	}
}
