using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class PageNationItem : MonoBehaviour
	{
		public int Index = 0;
		[SerializeField] GameObject m_onObj = null;

		void Start()
		{
			var isDefault = Index == 1;
			m_onObj.SetActive(isDefault);
		}

		public void SetActive(bool flg)
		{
			m_onObj.SetActive(flg);
		}
	}
}
