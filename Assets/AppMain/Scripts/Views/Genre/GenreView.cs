using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Tarot
{
	public interface IGenreView
	{
		void ClickGenre(int index);
		void OnClickBack();
		void SetButtonInteractable(bool flg);
	}

	public class GenreView : ViewBase, IGenreView
	{
		[SerializeField] ButtonEx[] m_buttons = null;
		CancellationTokenSource m_cts = null;

		GenrePresenter m_presenter = null;

		public void SetPresenter(GenrePresenter presenter)
		{
			m_presenter = presenter;
		}

		/// <summary>ビューオープン時</summary>
		public override void OnViewOpened()
		{
			base.OnViewOpened();

			m_cts = new CancellationTokenSource();

			SetButtonInteractable(true);
		}

		/// <summary>ビュークローズ時</summary>
		public async override UniTask OnViewClosed()
		{
			await base.OnViewClosed();
		}

		/// <summary>ジャンルクリック</summary>
		public void ClickGenre(int index)
		{
			SetButtonInteractable(false);

			_ = m_presenter.MoveToTarot(index, Scene, m_cts.Token);
		}

		/// <summary>戻るボタン押下</summary>
		public void OnClickBack()
		{
			SetButtonInteractable(false);

			m_presenter.MoveToTitle(this, m_cts.Token);
		}

		public void SetButtonInteractable(bool flg)
		{
			foreach (var btn in m_buttons)
				btn.interactable = flg;
		}
	}
}

