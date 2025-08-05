using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Tarot
{
	public class GenreView : ViewBase
	{
		bool m_isMoving = false;
		CancellationTokenSource m_cts = null;

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();

			m_isMoving = false;
			m_cts = new CancellationTokenSource();
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			await base.OnViewClosed();
		}

		/// <summary>ジャンルクリック</summary>
		public void ClickGenre(int index)
		{
			if (m_isMoving)
				return;

			m_isMoving = true;

			ChangeView(index);
		}

		/// <summary>View切替</summary>
		async void ChangeView(int index)
		{
			await Scene.ChangeView(ViewName.Tarot, index, m_cts.Token);

			m_isMoving = false;
		}

		/// <summary>戻るボタン押下</summary>
		public void OnClickBack()
		{
			if (m_isMoving)
				return;

			m_isMoving = true;
			ChangeView(ViewName.Title, m_cts.Token, () => m_isMoving = false);
		}
	}
}

