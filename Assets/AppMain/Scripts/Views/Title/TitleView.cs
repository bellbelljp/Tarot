using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class TitleView : ViewBase
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

		/// <summary>オプションボタン押下</summary>
		public void OnClickOption()
		{
			if(m_isMoving)
				return;

			m_isMoving = true;
			ChangeView(ViewName.Option, m_cts.Token, () => m_isMoving = false);
		}

		/// <summary>ゲームスタートボタン押下</summary>
		public void OnClickStart()
		{
			if (m_isMoving)
				return;

			m_isMoving = true;
			ChangeScene(SceneName.InGame, m_cts.Token, () => m_isMoving = false);
		}
	}
}

