//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System;

//namespace GamersStories
//{
//	public class CharaSelectButton : MonoBehaviour
//	{
//		[SerializeField] Image m_charaImage = null;

//		Action<int> m_callback = null;
//		int m_charaId = 0;

//		public void SetParam(int charaId, Action<int> callback)
//		{
//			m_callback = null;
//			m_callback = callback;
//			m_charaId = charaId;
//			m_charaImage.enabled = false;
//			CharacterData.Instance.SetDefaultSprite(charaId, sp =>
//			{
//				if (sp != null)
//				{
//					m_charaImage.sprite = sp;
//					m_charaImage.enabled = true;
//				}
//			});
//		}

//		public void ClickButton()
//		{
//			m_callback.Invoke(m_charaId);
//		}
//	}
//}
