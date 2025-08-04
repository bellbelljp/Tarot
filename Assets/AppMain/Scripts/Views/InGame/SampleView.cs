using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class SampleView : ViewBase
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

		/// <summary>戻るボタン押下</summary>
		public void OnClickBack()
		{
			if (m_isMoving)
				return;

			m_isMoving = true;
			ChangeView(ViewName.InGame, m_cts.Token, () => m_isMoving = false);
		}
	}
}

