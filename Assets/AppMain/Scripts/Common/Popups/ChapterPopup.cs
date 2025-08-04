using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace JourneysOfRealPeople
{
	public class ChapterPopup : CommonPopup
	{
		[SerializeField] ChapterSelectButton m_obj = null;
		[SerializeField] Transform m_objPos = null;

		List<ChapterMaster.Data> m_chapterDataList = new List<ChapterMaster.Data>();
		//int m_charaId = 0;
		Action<int> m_callback = null;

		public async void SetParam(int currentChapter, Action<int> callback, CancellationToken token)
		{
			try
			{
				//m_charaId = charaId;
				for (int i = 0; i < m_objPos.childCount; i++)
				{
					var child = m_objPos.GetChild(i);
					if (!child.gameObject.activeSelf)
						continue;
					Destroy(child.gameObject);
				}

				m_callback = null;
				m_callback = callback;
				m_chapterDataList = await ChapterMaster.Instance.LoadData();
				//m_chapterDataList = await ChapterMaster.Instance.LoadDataByChara(m_charaId);
				foreach (var chapter in m_chapterDataList)
				{
					var obj = Instantiate(m_obj, m_objPos);
					var hasChapter = SaveData.Instance.CommonData.ChapterList.Find(it => it.ChapterId == chapter.ChapterId && it.CharaId == chapter.CharaId);
					obj.SetParam(chapter, hasChapter == null, OnAnyButtonClicked);
					if (currentChapter == chapter.ChapterId)
						obj.SetCurrentChapter();
				}
			}
			catch(OperationCanceledException e)
			{
				Debug.Log("チャプターキャンセル：" + e);
				throw e;
			}
		}

		void OnAnyButtonClicked(int chapterId)
		{
			m_callback.Invoke(chapterId);
			SoundManager.Instance.StopVoice();
			Close();
		}
	}
}
