using UnityEngine;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace JourneysOfRealPeople
{
	public class CommonPopup : MonoBehaviour
	{
		[SerializeField] BasePopup m_basePopup = null;
		[SerializeField] ButtonEx m_bgButton = null;
		[Tooltip("背景タップで隠すか"), SerializeField] bool m_isHideByBg = true;
		[Tooltip("背景タップで消すか"), SerializeField] bool m_isDeleteByBg = true;

		protected virtual void Awake()
		{
			//背景タップで隠すか
			if (m_isHideByBg && m_bgButton != null)
			{
				m_bgButton.onClick.AddListener(() => Hide());
			}
			//背景タップで消すか
			if (m_isDeleteByBg && m_bgButton != null)
			{
				m_bgButton.onClick.AddListener(() => Close());
			}
		}

		public async void Open()
		{
			m_bgButton.enabled = false;
			await m_basePopup.Open();
			m_bgButton.enabled = true;
		}

		public async virtual void Close()
		{
			m_bgButton.enabled = false;
			try
			{
				await m_basePopup.Close();
				Destroy(gameObject);
			}
			catch(OperationCanceledException e)
			{
				Debug.Log("Closeキャンセル" + e);
			}
		}

		public async void Hide()
		{
			m_bgButton.enabled = false;
			try
			{
				await m_basePopup.Close();
				gameObject.SetActive(false);
			}
			catch (OperationCanceledException e)
			{
				Debug.Log("Closeキャンセル" + e);
			}
		}

		public async void SetActiveFalse()
		{
			await m_basePopup.Close();
			gameObject.SetActive(false);
		}

		public void SetYesNoButton(Action yes, Action no)
		{
			yes += () => Close();
			no += () => Close();
			m_basePopup.SetYesNoButton(yes, no);
		}

		public void SetOKButton(Action yes)
		{
			yes += () => Close();
			m_basePopup.SetOKButton(yes);
		}

		/// <summary>ボタンの位置をYesとNoでチェンジ</summary>
		public void ChangeButtonPos()
		{
			m_basePopup.ChangeButtonPos();
		}
	}
}
