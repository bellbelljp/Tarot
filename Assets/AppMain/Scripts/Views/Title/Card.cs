using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tarot
{
	public class Card : MonoBehaviour
	{
		[SerializeField] UITransition m_uiTransition = null;

		int m_cardIndex = -1;
		public int CardIndex { set { m_cardIndex = value; } get { return m_cardIndex; } }

		public async UniTask FadeOut()
		{
			await m_uiTransition.TransitionOutWait();
		}
	}
}
