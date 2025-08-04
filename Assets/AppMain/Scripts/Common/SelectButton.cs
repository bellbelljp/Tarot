using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace JourneysOfRealPeople
{
	/// <summary>選択肢ボタン</summary>
	public class SelectButton : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI m_buttonText = null;
		[SerializeField] UITransition m_transition = null;

		/// <summary>クリックイベントの定義</summary>
		//public class ClickEvent : UnityEvent<int> { };
		//public ClickEvent OnClicked = new ClickEvent();
		private Action<int, int> OnClicked;
		public int m_buttonSelectId = 0;
		int m_index = -1;

		/// <summary>作成時コール</summary>
		public async UniTask OnCreated(string text, int selectId, Action<int, int> onClick, int index, CancellationToken token)
		{
			m_transition.Canvas.alpha = 0;
			m_buttonText.text = text;
			m_buttonSelectId = selectId;
			OnClicked = onClick;
			m_index = index;

			await m_transition.TransitionInWait(token);
		}

		/// <summary>閉じる</summary>
		public async UniTask Close(CancellationTokenSource token)
		{
			await m_transition.TransitionOutWait();
			Destroy(gameObject);
		}

		/// <summary>ボタンクリック</summary>
		public void OnClickButton()
		{
			OnClicked.Invoke(m_buttonSelectId, m_index);
		}
	}
}
