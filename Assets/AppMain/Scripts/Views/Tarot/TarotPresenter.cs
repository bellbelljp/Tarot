using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public class TarotPresenter
	{
		ITarotView m_view = null;
		TarotModel m_model = null;

		public TarotPresenter(ITarotView view, TarotModel model)
		{
			m_view = view;
			m_model = model;
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

		/// <summary>ChatGPTにメッセージを送るAPI</summary>
		public async Task<string> SendMessageToChatGPT(string msg)
		{
			var chatGPT = new ChatGPT();
			return await chatGPT.SendMessageToGPT(msg);
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
	}
}