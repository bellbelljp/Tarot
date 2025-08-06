using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Tarot
{
	public class GenreView : ViewBase
	{
		[SerializeField] ButtonEx[] m_buttons = null;
		CancellationTokenSource m_cts = null;

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();

			m_cts = new CancellationTokenSource();
			foreach (var btn in m_buttons)
				btn.interactable = true;
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			await base.OnViewClosed();
		}

		/// <summary>ジャンルクリック</summary>
		public void ClickGenre(int index)
		{
			foreach (var btn in m_buttons)
				btn.interactable = false;

			ChangeView(index);
		}

		/// <summary>View切替</summary>
		async void ChangeView(int index)
		{
			await Scene.ChangeView(ViewName.Tarot, index, m_cts.Token);
		}

		/// <summary>戻るボタン押下</summary>
		public void OnClickBack()
		{
			foreach (var btn in m_buttons)
				btn.interactable = false;

			ChangeView(ViewName.Title, m_cts.Token, null);
		}
	}
}

