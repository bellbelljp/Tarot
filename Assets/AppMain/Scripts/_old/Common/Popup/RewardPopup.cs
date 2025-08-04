using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public class RewardPopup : CommonPopup
	{
		[Header("スチル1")]
		[SerializeField] Button m_picButton = null;
		[SerializeField] Image m_pictureIcon = null;
		[SerializeField] GameObject m_openPictureObj = null;
		[SerializeField] GameObject m_lookedPictureObj = null;

		[Header("べると菌ちゃんのスチル")]
		[SerializeField] Button m_bellKinButton = null;
		[SerializeField] Image m_bellKinIcon = null;
		[SerializeField] GameObject m_openBellKinObj = null;
		[SerializeField] GameObject m_lookedBellKinObj = null;

		[Header("トロフィー")]
		[SerializeField] RewardSelectButton m_obj = null;
		[SerializeField] Transform m_objPos = null;

		[SerializeField] TextMeshProUGUI m_titleText = null;
		[SerializeField] TextMeshProUGUI m_messageText = null;
		[SerializeField] GameObject m_picture = null;
		[SerializeField] Image m_picImage = null;
		[SerializeField] UITransition m_picTransition = null;

		List<RewardMaster.Data> m_dataList = new List<RewardMaster.Data>();

		SaveData.Data m_saveData = new SaveData.Data();
		int m_charaId = 0;

		public async void SetParam(int charaId)
		{
			m_charaId = charaId;
			m_pictureIcon.enabled = false;
			var isBellKin = charaId == 1 || charaId == 2;
			m_bellKinButton.gameObject.SetActive(isBellKin);

			try
			{
				m_titleText.text = string.Empty;
				m_messageText.text = string.Empty;
				for (int i = 0; i < m_objPos.childCount; i++)
				{
					var child = m_objPos.GetChild(i);
					if (!child.gameObject.activeSelf)
						continue;
					Destroy(child.gameObject);
				}

				m_saveData =　await SaveData.Instance.Load();
				//---------------------------------------------------------------
				// FIXME
				//await SaveData.Instance.GetAllReward();
				//SaveData.Instance.SaveCommonData();
				//---------------------------------------------------------------
				m_dataList = await RewardMaster.Instance.GetDataByChara(charaId);
				foreach (var data in m_dataList)
				{
					//SetPicture(m_pictureIcon, m_charaId);
					//if (data.IsPicture)
					//{
					//	var hasReward = SaveData.Instance.HasReward(data);
					//	m_openPictureObj.SetActive(hasReward);
					//	m_lookedPictureObj.SetActive(!hasReward);
					//	m_picButton.enabled = hasReward;
					//}
					//else
					//{
					//	var obj = Instantiate(m_obj, m_objPos);
					//	obj.SetParam(m_saveData, data, OnAnyButtonClicked);
					//}
				}

				// べる菌スチル
				if (isBellKin)
				{
					SetPicture(m_bellKinIcon, 0102);
					var hasBellKinPic = await RewardMaster.Instance.HasBellKinPictureReward();
					m_openBellKinObj.SetActive(hasBellKinPic);
					m_lookedBellKinObj.SetActive(!hasBellKinPic);
					m_bellKinButton.enabled = hasBellKinPic;
				}
			}
			catch (OperationCanceledException e)
			{
				Debug.Log("報酬キャンセル：" + e);
				throw e;
			}
		}

		/// <summary>何かボタンを押されたら</summary>
		void OnAnyButtonClicked(int masterId)
		{
			var masterData = m_dataList.Find(it => it.MasterId == masterId);
			if(masterData.Para != 0)
				m_titleText.text = string.Format("{0} {1}", masterData.Name, masterData.Para);
			else
				m_titleText.text = masterData.Name;

			//if (SaveData.Instance.HasReward(masterData)
			//	|| !masterData.IsHide)
			//{
			//	m_messageText.text = masterData.Intro;
			//}
			//else
			//{
			//	m_messageText.text = "???";
			//}
		}

		/// <summary>報酬写真を見る</summary>
		public async void LookPicture()
		{
			SetPicture(m_picImage, m_charaId);
			m_picture.SetActive(true);
			await m_picTransition.TransitionInWait();
		}

		public async void LookBellKinPicture()
		{
			// 0102:ここだけで使用するべると菌のId
			SetPicture(m_picImage, 0102);
			m_picture.SetActive(true);
			await m_picTransition.TransitionInWait();
		}

		/// <summary>スチル画像をセット</summary>
		void SetPicture(Image image, int charaId)
		{
			image.enabled = false;
			//CharacterData.Instance.SetRewardPicture(charaId, sp =>
			//{
			//	image.sprite = sp;
			//	image.enabled = true;
			//});
		}

		/// <summary>報酬写真を閉じる</summary>
		public async void ClosePicture()
		{
			await m_picTransition.TransitionOutWait();
			m_picTransition.transform.parent.gameObject.SetActive(false);
		}
	}
}
