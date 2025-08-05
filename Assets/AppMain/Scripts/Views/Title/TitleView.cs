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
			await base.OnViewClosed();
		}

		/// <summary>ゲームスタートボタン押下</summary>
		public void OnClickStart()
		{
			if (m_isMoving)
				return;

			m_isMoving = true;
			ChangeView();
		}

		public void SetBGMVolume(Slider slider)
		{
			SoundManager.Instance.SetBGMVolume(slider.value);
		}

		async void ChangeView()
		{
			await Scene.ChangeView(ViewName.Genre, 0, m_cts.Token);

			m_isMoving = false;
		}
	}
}

