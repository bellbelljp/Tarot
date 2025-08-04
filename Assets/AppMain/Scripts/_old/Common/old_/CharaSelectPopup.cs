//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//namespace GamersStories
//{
//	public class CharaSelectPopup : CommonPopup
//	{
//		[SerializeField] CharaSelectButton m_charaButton = null;
//		[SerializeField] Transform m_charaButtonParent = null;

//		Action<int> m_callback = null;

//		public async void SetParam(Action<int> callback)
//		{
//			for(int i = 0; i < m_charaButtonParent.childCount; i++)
//			{
//				var child = m_charaButtonParent.GetChild(i);
//				if (!child.gameObject.activeSelf)
//				{
//					continue;
//				}

//				Destroy(child.gameObject);
//			}

//			m_callback = null;
//			m_callback = callback;
//			var charaList = await CharacterData.Instance.GetAllExceptHide();

//			foreach(var chara in charaList)
//			{
//				var obj = Instantiate(m_charaButton, m_charaButtonParent);
//				var charaSelectButton = obj.GetComponent<CharaSelectButton>();
//				charaSelectButton.SetParam(chara.MasterId, ClickChara);
//			}
//		}

//		void ClickChara(int charaId)
//		{
//			m_callback.Invoke(charaId);
//		}
//	}
//}
