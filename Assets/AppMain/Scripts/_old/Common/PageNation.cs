using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tarot
{
	public class PageNation : MonoBehaviour
	{
		[SerializeField] PageNationItem[] m_pageNations = null;

		Action<int> m_callback = null;
		int m_nowIndex = 1;
		const int DEFAULT_INDEX = 1;

		public void SetParam(Action<int> callback)
		{
			m_callback = callback;
			ChangePage(DEFAULT_INDEX, false);
		}

		/// <summary>ページネーションボタンから切り替え</summary>
		public void ChangePage(PageNationItem item)
		{
			ChangePage(item.Index);
		}

		/// <summary>ページ切り替え</summary>
		void ChangePage(int index, bool calInvoke = true)
		{
			foreach (var page in m_pageNations)
			{
				var isOpen = page.Index == index;
				page.SetActive(isOpen);
			}
			if(calInvoke)
				m_callback.Invoke(index - 1);
		}

		/// <summary>右へ移動</summary>
		public void MoveToRight()
		{
			m_nowIndex++;
			if (m_nowIndex > m_pageNations.Length)
				m_nowIndex = DEFAULT_INDEX;
			ChangePage(m_nowIndex);
		}

		/// <summary>左へ移動</summary>
		public void MoveToLeft()
		{
			m_nowIndex--;
			if (m_nowIndex <= 0)
				m_nowIndex = m_pageNations.Length;
			ChangePage(m_nowIndex);
		}
	}
}
