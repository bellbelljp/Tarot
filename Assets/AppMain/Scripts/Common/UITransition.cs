using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace Tarot
{
	[RequireComponent(typeof(CanvasGroup))]
	public class UITransition : MonoBehaviour
	{
		// RectTransform取得
		public RectTransform Rect
		{
			get
			{
				if (m_rect == null) m_rect = GetComponent<RectTransform>();
				return m_rect;
			}
		}

		RectTransform m_rect = null;

		[System.Serializable]
		public class TransitionParam
		{
			public bool IsActive = true;
			public Vector2 In = new Vector2(0, 1f);
			public Vector2 Out = new Vector2(1f, 0);
			public Vector2 Now = new Vector2(0, 0);
		}

		// フェードの設定値
		[SerializeField] TransitionParam m_fade = new TransitionParam();
		// スケールの設定値
		[SerializeField] TransitionParam m_scale = new TransitionParam() { IsActive = false, In = Vector2.zero, Out = Vector2.zero};
		// スライドの設定値
		[SerializeField] TransitionParam m_slideX = new TransitionParam() { IsActive = false, In = Vector2.zero, Out = Vector2.zero };
		// スライドの設定値
		[SerializeField] TransitionParam m_slideY = new TransitionParam() { IsActive = false, In = Vector2.zero, Out = Vector2.zero };
		// 遷移時間
		[SerializeField] float m_duration = 0.5f;

		// inのシークエンス
		Sequence m_inSequence = null;
		// outのシークエンス
		Sequence m_outSequence = null;

		// inのキャンセルトークン
		CancellationTokenSource m_inCts = null;
		// outのキャンセルトークン
		CancellationTokenSource m_outCts = null;

		public CanvasGroup Canvas
		{
			get
			{
				if (m_canvas == null) m_canvas = GetComponent<CanvasGroup>();
				return m_canvas;
			}
		}

		CanvasGroup m_canvas = null;

		private void OnDestroy()
		{
			if(m_inCts != null)
			{
				m_inCts.Cancel();
			}
			if(m_outCts != null)
			{
				m_outCts.Cancel();
			}
		}

		/// <summary>トランジションIn</summary>
		public void TransitionIn(UnityAction onCompleted = null)
		{
			if (this.gameObject.name == "Cards")
				Debug.Log("フェードイン");

			if (m_inSequence != null)
			{
				m_inSequence.Kill();
				m_inSequence = null;
			}
			m_inSequence = DOTween.Sequence();

			if(m_fade.IsActive == true && Canvas != null)
			{
				// alphaを0にセット
				Canvas.alpha = m_fade.In.x;

				// SetLink:このgameObjectが削除されたらTweenを停止する
				// フェードイン
				m_inSequence.Join(
					Canvas.DOFade(m_fade.In.y, m_duration)
					.SetLink(gameObject)
				);
			}

			if(m_scale.IsActive == true)
			{
				var current = Rect.transform.localScale;
				Rect.transform.localScale = new Vector3(m_scale.In.x, m_scale.In.y, current.z);

				m_inSequence.Join(
					Rect.DOScale(current, m_duration).SetLink(gameObject));
			}

			if(m_slideX.IsActive == true)
			{
				Canvas.alpha = 1;
				var current = new Vector3(m_slideX.Now.x, m_slideX.Now.y);
				Rect.anchoredPosition = new Vector3(m_slideX.In.x, m_slideX.In.y);

				m_inSequence.Join(
					Rect.DOAnchorPosX(current.x, m_duration, true).SetLink(gameObject));
			}

			if (m_slideY.IsActive == true)
			{
				Canvas.alpha = 1;
				var current = new Vector3(m_slideY.Now.x, m_slideY.Now.y);
				Rect.anchoredPosition = new Vector3(m_slideY.In.x, m_slideY.In.y);

				m_inSequence.Join(
					Rect.DOAnchorPosY(current.y, m_duration, true).SetLink(gameObject));
			}

			m_inSequence
				.SetLink(gameObject)
				.OnComplete(() => onCompleted?.Invoke());
		}

		/// <summary>トランジションOut</summary>
		public void TransitionOut(UnityAction onCompleted = null)
		{
			if (m_outSequence != null)
			{
				m_outSequence.Kill();
				m_outSequence = null;
			}
			m_outSequence = DOTween.Sequence();

			if (m_fade.IsActive == true && Canvas != null)
			{
				// alphaを1にセット
				Canvas.alpha = m_fade.Out.x;

				// SetLink:このgameObjectが削除されたらTweenを停止する
				// フェードアウト
				m_outSequence.Join(
					Canvas.DOFade(m_fade.Out.y, m_duration)
					.SetLink(gameObject)
				);
			}

			if (m_scale.IsActive == true)
			{
				var current = Rect.transform.localScale;
				m_outSequence.Join(
					Rect.DOScale(new Vector3(m_scale.Out.x, m_scale.Out.y, current.z), m_duration)
					.SetLink(gameObject)
					.OnComplete(() => Rect.transform.localScale = current));
			}

			if (m_slideX.IsActive == true)
			{
				Canvas.alpha = 1;
				var current = new Vector3(m_slideX.Now.x, m_slideX.Now.y);
				m_inSequence.Join(
					Rect.DOAnchorPosX(m_slideX.Out.x, m_duration, true)
					.SetLink(gameObject))
					.OnComplete(() => Rect.anchoredPosition = current);
			}

			if (m_slideY.IsActive == true)
			{
				Canvas.alpha = 1;
				var current = new Vector3(m_slideY.Now.x, m_slideY.Now.y);
				m_inSequence.Join(
					Rect.DOAnchorPosY(m_slideY.Out.y, m_duration, true)
					.SetLink(gameObject))
					.OnComplete(() => Rect.anchoredPosition = current);
			}

			m_outSequence
				.SetLink(gameObject)
				.OnComplete(() => onCompleted?.Invoke());
		}

		/// <summary>トランジションIn終了待機</summary>
		public async UniTask TransitionInWait(CancellationToken token = default)
		{
			bool isDone = false;
			if(m_inCts != null)
			{
				m_inCts.Cancel();
			}
			m_inCts = new CancellationTokenSource();

			TransitionIn(() =>
			{
				isDone = true;
			});

			try
			{
				// すでにキャンセルされているなら例外を投げる
				token.ThrowIfCancellationRequested();
				await UniTask.WaitUntil(() => isDone == true, PlayerLoopTiming.Update, m_inCts.Token);
			}
			catch(System.OperationCanceledException e)
			{
				Debug.Log("キャンセルされました:" + e);
				throw e;
			}
		}

		/// <summary>トランジションOut終了待機</summary>
		public async UniTask TransitionOutWait(CancellationToken token = default)
		{
			bool isDone = false;
			if (m_outCts != null)
			{
				m_outCts.Cancel();
			}
			m_outCts = new CancellationTokenSource();

			TransitionOut(() =>
			{
				isDone = true;
			});

			try
			{
				// すでにキャンセルされているなら例外を投げる
				token.ThrowIfCancellationRequested();
				await UniTask.WaitUntil(() => isDone == true, PlayerLoopTiming.Update, m_outCts.Token);
			}
			catch (System.OperationCanceledException e)
			{
				Debug.Log("キャンセルされました:" + e);
				throw e;
			}
		}
	}
}
