using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Tarot.csdef;
using static UnityEditor.PlayerSettings;

namespace Tarot
{
	public class TarotPresenter
	{
		ITarotView m_view = null;
		TarotModel m_model = null;

		const string AI_SEND_MESSAGE = "あなたは有能な占い師です。" +
			"\n以下の条件に基づき、相談者に向けた占いの結果を「過去・現在・未来」の３つの流れと、最後にアドバイスとして簡潔に320文字以内でまとめてください。" +
			"\n【相談内容】{0}" +
			"\n【過去】{1}({2})" +
			"\n【現在】{3}({4})" +
			"\n【未来】{5}({6})" +
			"\nお願いします。";

		public TarotPresenter(ITarotView view, TarotModel model)
		{
			m_view = view;
			m_model = model;
		}

		/// <summary>初期化</summary>
		public void Init()
		{
			m_model.Init();
		}

		/// <summary>ジャンルセット</summary>
		public void SetGenre(int genre)
		{
			m_model.Genre = genre;
		}

		/// <summary>移動範囲セット</summary>
		public void SetMovingRange(CanvasScaler canvasScaler)
		{
			m_model.HalfWidth = canvasScaler.referenceResolution.x / 2f - 300;
			m_model.HalfHeight = canvasScaler.referenceResolution.y / 2f - 300;
		}

		/// <summary>カード生成</summary>
		public void CreateCards()
		{
			m_view.DestroyCards();

			List<GameObject> cardList = new List<GameObject>();
			// カード生成
			for (int i = 0; i < m_model.GetCardNum(); i++)
			{
				var card = m_view.InstantiateCard();
				card.GetComponent<Card>().CardIndex = i;

				float x = UnityEngine.Random.Range(-m_model.HalfWidth, m_model.HalfWidth);
				float y = UnityEngine.Random.Range(-m_model.HalfHeight, m_model.HalfHeight);

				// Z軸のランダムな角度（-180〜180度）
				float randomZ = UnityEngine.Random.Range(-180f, 180f);
				Quaternion randomRotation = Quaternion.Euler(0, 0, randomZ);

				var rect = card.GetComponent<RectTransform>();
				card.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(x, y, 1), randomRotation);
				card.SetActive(true);
				cardList.Add(card);
			}
			m_model.CardList = cardList;
		}

		/// <summary>カードシャッフル</summary>
		public async UniTask ShuffleCards()
		{
			List<UniTask> tasks = new List<UniTask>();

			// カード移動
			for (int i = 0; i < m_model.CardList.Count; i++)
			{
				var card = m_model.CardList[i];
				card.SetActive(true);
				// 終点を安全な範囲で設定
				float x = UnityEngine.Random.Range(-m_model.HalfWidth, m_model.HalfWidth);
				float y = UnityEngine.Random.Range(-m_model.HalfHeight, m_model.HalfHeight);
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
		}

		/// <summary>シャッフルしたカードをクリック</summary>
		public void OnClickCard(Card card)
		{
			bool isNormal = false;
			var rectTransform = card.GetComponent<RectTransform>();
			if (m_model.LeftCardIndex == -1)
			{
				m_model.LeftCardIndex = card.CardIndex;
				isNormal = (rectTransform.eulerAngles.z <= 90 && rectTransform.eulerAngles.z > -90) ||
					(rectTransform.eulerAngles.z >= 270 && rectTransform.eulerAngles.z < 450);
				m_model.LeftCardDirection = isNormal;
			}
			else if (m_model.CenterCardIndex == -1)
			{
				m_model.CenterCardIndex = card.CardIndex;
				isNormal = (rectTransform.eulerAngles.z <= 90 && rectTransform.eulerAngles.z > -90) ||
					(rectTransform.eulerAngles.z >= 270 && rectTransform.eulerAngles.z < 450);
				m_model.CenterCardDirection = isNormal;
			}
			else if (m_model.RightCardIndex == -1)
			{
				m_model.RightCardIndex = card.CardIndex;
				isNormal = (rectTransform.eulerAngles.z <= 90 && rectTransform.eulerAngles.z > -90) ||
					(rectTransform.eulerAngles.z >= 270 && rectTransform.eulerAngles.z < 450);
				m_model.RightCardDirection = isNormal;
			}
		}

		/// <summary>カードを３枚選んだか</summary>
		public bool SelectedThreeCards()
		{
			return m_model.LeftCardIndex != -1 && m_model.CenterCardIndex != -1 && m_model.RightCardIndex != -1;
		}

		/// <summary>ChatGPT送受信</summary>
		public async Task<string> SendingAndReceivingChatGPT()
		{
			var genre = CardName.GetGenre(m_model.Genre);
			var leftText = CardName.GetTarotName(m_model.LeftCardIndex);
			var leftDirection = CardName.GetDirection(m_model.LeftCardDirection);
			var centerText = CardName.GetTarotName(m_model.CenterCardIndex);
			var centerDirection = CardName.GetDirection(m_model.CenterCardDirection);
			var rightText = CardName.GetTarotName(m_model.RightCardIndex);
			var rightDirection = CardName.GetDirection(m_model.RightCardDirection);

			m_view.SetResultText(genre,
				$"過去:{leftText}({leftDirection})",
				$"現在:{centerText}({centerDirection})",
				$"未来:{rightText}({rightDirection})");

			var input = string.Format(AI_SEND_MESSAGE,
				genre,
				leftText,
				leftDirection,
				centerText,
				centerDirection,
				rightText,
				rightDirection);

			return await SendMessageToChatGPT(input);
		}

		/// <summary>ChatGPTにメッセージを送るAPI</summary>
		async Task<string> SendMessageToChatGPT(string msg)
		{
			var chatGPT = new ChatGPT();
			return await chatGPT.SendMessageToGPT(msg);
		}

		/// <summary>結果カードセット</summary>
		public void SetResultCardImage()
		{
			m_view.SetCardImage(m_model.LeftCardIndex,
				m_model.CenterCardIndex,
				m_model.RightCardIndex,
				m_model.LeftCardDirection,
				m_model.CenterCardDirection,
				m_model.RightCardDirection);
		}

		public Sprite LoadCardImage(int value)
		{
			return m_model.LoadCardImage(value);
		}

		public async UniTask ShareToX()
		{
			m_view.SetButtonInteractable(false);

			var share = new ShareToX();
			share.Share();

			await UniTask.Delay(TimeSpan.FromSeconds(0.5));

			m_view.SetButtonInteractable(true);
		}

		public async void ChangeView(SceneBase scene, CancellationToken token)
		{
			await scene.ChangeView(ViewName.Genre, 0, token);
		}
	}
}