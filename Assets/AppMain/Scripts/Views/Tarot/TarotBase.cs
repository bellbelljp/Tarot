using UnityEngine;

namespace Tarot
{
	public class TarotBase : MonoBehaviour
	{
		[SerializeField] TarotView m_view = null;

		private void Awake()
		{
			var model = new TarotModel();
			var presenter = new TarotPresenter(m_view, model);
			m_view.SetPresenter(presenter);
		}
	}
}
