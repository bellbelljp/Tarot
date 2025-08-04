using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JourneysOfRealPeople
{
	public class StoryDataManager : MonoBehaviour
	{
		/// <summary>Transition(遷移)を指定してデータを返す</summary>
		public List<StoryMaster.Data> LoadStoryDataFromTransition(List<StoryMaster.Data> storyData, int transition)
		{
			bool read = false;
			List<StoryMaster.Data> data = new List<StoryMaster.Data>();
			foreach (var story in storyData)
			{
				//// 一致しているパラメータ以下のみ読み込む
				//if (read)
				//{
				//	data.Add(story);
				//}
				//else if (story.Transition != transition)
				//{
				//	read = false;
				//}
				//else if (story.Transition == transition)
				//{
				//	data.Add(story);
				//	read = true;
				//}
			}
			return data;
		}


		/// <summary>Parameterを指定してデータを返す</summary>
		public List<StoryMaster.Data> LoadStoryDataFromParam(List<StoryMaster.Data> storyData, int parameter)
		{
			bool read = false;
			List<StoryMaster.Data> data = new List<StoryMaster.Data>();
			foreach (var story in storyData)
			{
				// 一致しているパラメータ以下のみ読み込む
				if (read)
				{
					data.Add(story);
				}
				else if (story.Param != parameter)
				{
					read = false;
				}
				else if (story.Param == parameter)
				{
					data.Add(story);
					read = true;
				}
			}
			return data;
		}

		/// <summary>Phaseを指定してデータを返す</summary>
		public List<StoryMaster.Data> LoadStoryDataFromChapter(List<StoryMaster.Data> storyData, int phase)
		{
			if (phase == 0)
				return storyData;

			bool read = false;
			List<StoryMaster.Data> data = new List<StoryMaster.Data>();
			foreach (var story in storyData)
			{
				// 一致しているパラメータ以下のみ読み込む
				if (read)
				{
					data.Add(story);
				}
				else if (story.Id != phase)
				{
					read = false;
				}
				else if (story.Id == phase)
				{
					data.Add(story);
					read = true;
				}
			}
			return data;
		}
	}
}
