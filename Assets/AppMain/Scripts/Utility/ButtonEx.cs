using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tarot
{
	public class ButtonEx : Button
	{
		[SerializeField] bool m_isPinterDownText = false;
		[SerializeField] RectTransform m_text = null;
		[SerializeField] Image[] m_animImage = null;
		[SerializeField] SoundManager.SEType m_type = SoundManager.SEType.Decide;

		Vector3 m_pointerDown = new Vector3(0.8f, 0.8f, 0.8f);
		Vector3 m_pointerUp = new Vector3(1f, 1f, 1f);
		const float TRANSITION_TIME = 0.25f;
		const int TEXT_UP = 8;
		Vector2 m_textVector = new Vector2();

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
		}

		public override void OnSelect(BaseEventData eventData)
		{
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if(transition != Transition.None && m_animImage != null)
			{
				var sequence = DOTween.Sequence();
				foreach(var image in m_animImage)
				{
					sequence.Join(
						image.transform.DOScale(m_pointerDown, TRANSITION_TIME).SetLink(gameObject));
				}

				// 文字も下げる
				if (m_isPinterDownText && this.interactable)
				{
					m_textVector = m_text.anchoredPosition;
					m_text.anchoredPosition = new Vector2(m_text.anchoredPosition.x, m_text.anchoredPosition.y - TEXT_UP);
				}
			}
			base.OnPointerDown(eventData);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if(transition != Transition.None && m_animImage != null)
			{
				var sequence = DOTween.Sequence();
				foreach(var image in m_animImage)
				{
					sequence.Join(
						image.transform.DOScale(m_pointerUp, TRANSITION_TIME).SetLink(gameObject));
				}

				// 文字を上げる
				if (m_isPinterDownText && this.interactable)
					m_text.anchoredPosition = m_textVector;
			}
			base.OnPointerUp(eventData);
			SoundManager.Instance.PlayClickSE(m_type);
		}
	}
}