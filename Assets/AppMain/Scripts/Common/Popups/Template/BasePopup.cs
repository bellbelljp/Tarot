using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace JourneysOfRealPeople
{
	public class BasePopup : MonoBehaviour
	{
		[SerializeField] UITransition m_bgTransition = null;
		[SerializeField] UITransition m_windowTransition = null;
		[SerializeField] GameObject m_closeButton = null;
		[SerializeField] GameObject m_yesNoButton = null;
		[SerializeField] GameObject m_okButton = null;
		[SerializeField] GameObject m_yesButton = null;

		Action yesHandler = null;
		Action noHandler = null;

		/// <summary>YesNoボタンをセット</summary>
		public void SetYesNoButton(Action yes, Action no)
		{
			m_closeButton.SetActive(false);
			m_okButton.SetActive(false);
			m_yesNoButton.SetActive(true);
			yesHandler = null;
			yesHandler = yes;
			noHandler = null;
			noHandler = no;
		}

		/// <summary>OKボタンをセット</summary>
		public void SetOKButton(Action yes)
		{
			m_closeButton.SetActive(false);
			m_yesNoButton.SetActive(false);
			m_okButton.SetActive(true);
			yesHandler = null;
			yesHandler = yes;
		}

		public async UniTask Open()
		{
			List<UniTask> tasks = new List<UniTask>();
			// 背景を開く
			if (m_bgTransition.gameObject.activeSelf == true)
			{
				tasks.Add(m_bgTransition.TransitionInWait());
			}
			// Windowを開く
			if (m_windowTransition.gameObject.activeSelf == true)
			{
				tasks.Add(m_windowTransition.TransitionInWait());
			}
			await UniTask.WhenAll(tasks);
		}

		public async UniTask Close()
		{
			List<UniTask> tasks = new List<UniTask>();
			// 背景を閉じる
			if (m_bgTransition.gameObject.activeSelf == true)
			{
				tasks.Add(m_bgTransition.TransitionOutWait());
			}
			if (m_windowTransition.gameObject.activeSelf == true)
			{
				tasks.Add(m_windowTransition.TransitionOutWait());
			}
			await UniTask.WhenAll(tasks);
		}

		public void ClickYes()
		{
			yesHandler.Invoke();
		}

		public void ClickNo()
		{
			noHandler.Invoke();
		}

		/// <summary>ボタンの位置をYesとNoでチェンジ</summary>
		public void ChangeButtonPos()
		{
			m_yesButton.transform.SetSiblingIndex(0);
		}
	}
}
