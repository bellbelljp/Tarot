using UnityEngine;

namespace Tarot
{
	public class Genre : MonoBehaviour
	{
		[SerializeField] GenreView m_view = null;

		private void Awake()
		{
			var model = new GenreModel();
			var presenter = new GenrePresenter(m_view, model);
			m_view.SetPresenter(presenter);
		}
	}
}
