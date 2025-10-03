using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
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

		/// <summary>ChatGPTにメッセージを送るAPI</summary>
		public async Task<string> SendMessageToChatGPT(string msg)
		{
			var chatGPT = new ChatGPT();
			return await chatGPT.SendMessageToGPT(msg);
		}

		public Sprite LoadCardImage(int value)
		{
			return m_model.LoadCardImage(value);
		}

		public async UniTask ShareToX()
		{
			m_view.SetButtonInteractable(false);

			var share = new ShareToX();
			share.Share();

			await UniTask.Delay(TimeSpan.FromSeconds(0.5));

			m_view.SetButtonInteractable(true);
		}
	}
}