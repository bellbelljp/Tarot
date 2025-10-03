using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public interface ITitleView
	{
		void OnClickStart();
	}

	public class TitleView : ViewBase, ITitleView
	{
		[SerializeField] Slider m_slider = null;
		CancellationTokenSource m_cts = null;
		TitlePresenter m_presenter = null;

		public void SetPresenter(TitlePresenter presenter)
		{
			m_presenter = presenter;
		}

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();

			m_cts = new CancellationTokenSource();
			m_slider.value =  m_presenter.GetVolume();
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			await base.OnViewClosed();
		}

		void OnDisable()
		{
			// CancellationTokenSourceを適切に破棄
			m_cts?.Cancel();
			m_cts?.Dispose();
			m_cts = null;
		}

		/// <summary>ゲームスタートボタン押下</summary>
		public void OnClickStart()
		{
			m_presenter.StartGame(Scene, m_cts.Token);
		}

		public void ChangeBGMVolume(Slider slider)
		{
			m_presenter.ChangeBGMVolume(slider.value);
		}
	}
}

