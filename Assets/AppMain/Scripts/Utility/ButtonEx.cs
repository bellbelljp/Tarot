using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tarot
{
	public class ButtonEx : Button
	{
		[SerializeField] Image[] m_animImage = null;
		[SerializeField] SoundManager.SEType m_type = SoundManager.SEType.Decide;

		Vector3 m_pointerDown = new Vector3(0.8f, 0.8f, 0.8f);
		Vector3 m_pointerUp = new Vector3(1f, 1f, 1f);
		const float TRANSITION_TIME = 0.25f;

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
			}
			base.OnPointerUp(eventData);
			SoundManager.Instance.PlayClickSE(m_type);
		}
	}
}