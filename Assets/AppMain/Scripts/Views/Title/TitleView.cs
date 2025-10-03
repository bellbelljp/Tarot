using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public class TitleView : ViewBase
	{
		[SerializeField] Slider m_slider = null;
		bool m_isMoving = false;
		CancellationTokenSource m_cts = null;

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();

			m_isMoving = false;
			m_cts = new CancellationTokenSource();
			m_slider.value = SoundManager.Instance.BgmVolume;
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			// CancellationTokenSourceを適切に破棄
			m_cts?.Cancel();
			m_cts?.Dispose();
			m_cts = null;
			
			await base.OnViewClosed();
		}

		/// <summary>ゲームスタートボタン押下</summary>
		public void OnClickStart()
		{
			if (m_isMoving)
				return;

			m_isMoving = true;
			ChangeViewAsync();
		}

		public void SetBGMVolume(Slider slider)
		{
			SoundManager.Instance.SetBGMVolume(slider.value);
		}

		async void ChangeViewAsync()
		{
			try
			{
				await Scene.ChangeView(ViewName.Genre, 0, m_cts.Token);
			}
			catch (OperationCanceledException)
			{
				// キャンセルされた場合は何もしない
			}
			catch (Exception ex)
			{
				Debug.LogError($"ビュー変更中にエラーが発生しました: {ex.Message}");
			}
			finally
			{
				m_isMoving = false;
			}
		}
	}
}

