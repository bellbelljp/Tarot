using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Tarot
{
	public class ViewBase : MonoBehaviour
	{
		UITransition m_transition = null;
		[SerializeField] AudioClip m_bgm = null;

		public UITransition Transition
		{
			get
			{
				if (m_transition == null) m_transition = GetComponent<UITransition>();
				return m_transition;
			}
		}

		public SceneBase Scene = null;

		/// <summary>ビューオープン時</summary>
		public virtual void OnViewOpened()
		{
			if (m_bgm != null)
				SoundManager.Instance.PlayBGM(m_bgm);
		}

		/// <summary>ビュークローズ時</summary>
		public virtual async UniTask OnViewClosed()
		{
			//await SoundManager.Instance.StopBGMWithFadeOut();
		}

		public virtual void SetParam(int parameter)
		{

		}

		/// <summary>Viewを変更する</summary>
		public async void ChangeView(string viewName, CancellationToken token, Action callback)
		{
			try
			{
				await Scene.ChangeView(viewName, 0, token);
			}
			catch (OperationCanceledException e)
			{
				// ユーザーがキャンセルしただけだから軽めに扱う
				Debug.LogError($"キャンセル: {e.Message}");
			}
			catch (Exception e)
			{
				// それ以外の問題はガチのバグの可能性
				Debug.LogError($"例外: {e.Message}");
			}

			callback?.Invoke();
		}

		/// <summary>Sceneを変更する</summary>
		protected async void ChangeScene(string sceneName, CancellationToken token, Action callback)
		{
			try
			{
				await Scene.ChangeScene(sceneName, token, true);
			}
			catch (OperationCanceledException e)
			{
				// ユーザーがキャンセルしただけだから軽めに扱う
				Debug.LogError($"キャンセル: {e.Message}");
			}
			catch (Exception e)
			{
				// それ以外の問題はガチのバグの可能性
				Debug.LogError($"例外: {e.Message}");
			}

			callback.Invoke();
		}

		/// <summary>以前のCTSを安全にキャンセル/破棄し、新しいものを生成します。</summary>
		protected void CancelPreviousIfAny(ref CancellationTokenSource cts)
		{
			if (cts != null)
			{
				if (!cts.IsCancellationRequested)
				{
					cts.Cancel();
				}
				cts.Dispose();
			}
			cts = new CancellationTokenSource();
		}
	}
}
