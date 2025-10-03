using UnityEngine;

namespace Tarot
{
	public class Title : MonoBehaviour
	{
		[SerializeField] TitleView m_view = null;
		private void Awake()
		{
			var model = new TitleModel();
			var presenter = new TitlePresenter(m_view, model);
			m_view.SetPresenter(presenter);
		}
	}
}
