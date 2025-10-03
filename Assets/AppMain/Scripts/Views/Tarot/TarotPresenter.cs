using UnityEngine;

namespace Tarot
{
	public class TarotPresenter
	{
		ITarotView m_view = null;
		TarotModel m_model = null;

		public TarotPresenter(ITarotView view, TarotModel model)
		{
			m_view = view;
			m_model = model;
		}
	}
}