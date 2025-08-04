using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Tarot
{
	public class ChapterSelectButton : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI m_text = null;
		[SerializeField] GameObject m_trophyOn = null;
		[SerializeField] GameObject m_trophyOff = null;

		UnityEvent<int> callback = new UnityEvent<int>();

		int m_chapterId = 0;
		bool m_isHide = false;

		public async void SetParam(ChapterMaster.Data data, bool isHide, UnityAction<int> action)
		{
			m_chapterId = data.ChapterId;
			m_isHide = isHide;
			if (data.ChapterId == 1)
				m_isHide = false;
			//var chapterName = m_isHide ? "???" : data.ChapterName;
			m_isHide = false; // tmp
			var chapterName = m_isHide ? "???" : data.JpName;
			m_text.text = string.Format("{0} : {1}", m_chapterId, chapterName);
			callback.AddListener(action);

			// Reward
			if (data.ChapterId == 1)
			{
				m_trophyOn.SetActive(false);
				m_trophyOff.SetActive(false);
			}
			else
			{
				//var reward = await RewardData.Instance.GetDataFromChapter(data.CharaId, data.ChapterId);
				//var hasReward = SaveData.Instance.HasReward(reward);
				//m_trophyOn.SetActive(hasReward);
				//m_trophyOff.SetActive(!hasReward);
			}
		}

		public void SetCurrentChapter()
		{

		}

		public void OnClick()
		{
			if (m_isHide)
			{
				SoundManager.Instance.PlaySE("Cancel");
				return;
			}

			SoundManager.Instance.PlaySE("Decide");
			callback.Invoke(m_chapterId);
		}
	}
}
