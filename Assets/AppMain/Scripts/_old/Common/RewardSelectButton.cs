using UnityEngine;
using UnityEngine.Events;

namespace Tarot
{
	public class RewardSelectButton : MonoBehaviour
	{
		[SerializeField] GameObject m_onTrophyImg = null;
		[SerializeField] GameObject m_offTrophyImg = null;

		UnityEvent<int> callback = new UnityEvent<int>();
		int rewardId = 0;

		public void SetParam(SaveData.Data saveData, RewardMaster.Data data, UnityAction<int> action)
		{
			rewardId = data.MasterId;

			// 隠すか
			//var hasReward = SaveData.Instance.HasReward(data);
			//m_onTrophyImg.SetActive(hasReward);
			//m_offTrophyImg.SetActive(!hasReward);

			callback.AddListener(action);
		}

		public void OnClick()
		{
			callback.Invoke(rewardId);
		}
	}
}
