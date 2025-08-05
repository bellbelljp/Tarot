using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO:キャンセル処理やる
namespace Tarot
{
	public class SceneBase : MonoBehaviour
	{
		[SerializeField] AudioClip m_bgm = null;
		// 初期ビューIndex
		[SerializeField] protected int m_initialViewIndex = 0;
		// フェードインアウトを行うか
		[SerializeField] protected bool m_isFadeInOut = true;
		[SerializeField] protected List<ViewBase> m_viewList = new List<ViewBase>();
		// ロードアイコン
		[SerializeField] protected GameObject m_loadIcon = null;

		[SerializeField] CommonPopupManager m_commonPopupManager;
		public CommonPopupManager CommonPopupManager { get { return m_commonPopupManager; } }

		CancellationTokenSource m_cts = null;

		// 現在のビュー
		protected ViewBase m_currentView = null;

		private void Awake()
		{
			if (m_loadIcon != null)
				m_loadIcon.SetActive(false);

			foreach (var view in m_viewList)
			{
				view.gameObject.SetActive(false);
			}

			m_cts = new CancellationTokenSource();

			if (m_bgm != null)
				SoundManager.Instance.PlayBGM(m_bgm);
		}

		protected virtual void Start()
		{
			if (m_initialViewIndex >= 0)
			{
				foreach(var view in m_viewList)
				{
					view.Scene = this;
					// 最初に表示するビューか
					if (m_viewList.IndexOf(view) == m_initialViewIndex)
					{
						// フェードインするか
						if (view.Transition != null && m_isFadeInOut == true)
						{
							view.Transition.Canvas.alpha = 0;
							view.gameObject.SetActive(true);
							view.OnViewOpened();
							view.Transition.TransitionIn();
						}
						else
						{
							view.OnViewOpened();
							view.gameObject.SetActive(true);
						}
						view.SetParam(0);
						m_currentView = view;
					}
					else
					{
						view.gameObject.SetActive(false);
					}
				}
			}
		}

		/// <summary>ビュー遷移</summary>
		public virtual async UniTask ChangeView(string viewName, int parameter, CancellationToken token )
		{
			if(m_currentView != null)
			{
				m_currentView.OnViewClosed().Forget();

				try
				{
					if (m_currentView.Transition != null)
						await m_currentView.Transition.TransitionOutWait(token);
				}
				catch (OperationCanceledException e)
				{
					Debug.Log("キャンセル：" + e);
					m_currentView.Transition.TransitionOutWait(token).Forget();
					//throw e;
				}
				catch (Exception e)
				{
					Debug.LogWarning($"演出中にエラー: {e.Message}");
					m_currentView.Transition.TransitionOutWait(token).Forget();
				}
			}

			foreach(var view in m_viewList)
			{
				if(view.name == viewName)
				{
					// オープン処理
					// フェードインするか
					if (view.Transition != null && m_isFadeInOut == true)
					{
						view.Transition.Canvas.alpha = 0;
						view.gameObject.SetActive(true);
						view.OnViewOpened();
						view.Transition.TransitionIn();
					}
					else
					{
						view.OnViewOpened();
						view.gameObject.SetActive(true);
					}
					view.SetParam(parameter);

					m_currentView = view;
				}
				else
				{
					view.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>シーン遷移</summary>
		public virtual async UniTask ChangeScene(string sceneName, CancellationToken token, bool fadeIn = false, string viewName = "", int parameter = 0)
		{
			if (m_currentView != null)
			{
				m_currentView.OnViewClosed().Forget();
				try
				{
					if (m_currentView.Transition != null)
						await m_currentView.Transition.TransitionOutWait(token);
				}
				catch (OperationCanceledException e)
				{
					Debug.Log("キャンセル：" + e);
					m_currentView.Transition.TransitionOutWait(token).Forget();
					//throw e;
				}
				catch (Exception e)
				{
					Debug.LogWarning($"演出中にエラー: {e.Message}");
					m_currentView.Transition.TransitionOutWait(token).Forget();
				}
			}

			m_isFadeInOut = fadeIn;
			if (m_loadIcon != null)
				m_loadIcon.SetActive(true);

			try
			{
				AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
				await asyncLoad;

				if (viewName != "")
				{
					var sceneBase = await SceneLoader.Load<SceneBase>(sceneName);
					await sceneBase.ChangeView(viewName, parameter, token);
				}
			}
			catch (OperationCanceledException e)
			{
				Debug.LogWarning($"シーン遷移キャンセル: {e.Message}");
			}
			catch (Exception e)
			{
				Debug.LogError($"シーン遷移中にエラー: {e.Message}");
			}

			if (m_loadIcon != null)
				m_loadIcon.SetActive(false);

			await SoundManager.Instance.StopBGMWithFadeOut();
		}
	}
}
