using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Tarot
{
	public class GenrePresenter
	{
		IGenreView m_view = null;
		GenreModel m_model = null;

		public GenrePresenter(IGenreView view, GenreModel model)
		{
			m_view = view;
			m_model = model;
		}

		/// <summary>View切替</summary>
		public async UniTaskVoid MoveToTarot(int index, SceneBase scene, CancellationToken token)
		{
			try
			{
				await scene.ChangeView(ViewName.Tarot, index, token);
			}
			catch(Exception)
			{
				m_view.SetButtonInteractable(true);
			}
		}

		public void MoveToTitle(ViewBase view, CancellationToken token)
		{
			view.ChangeView(ViewName.Title, token, null);
		}
	}
}
