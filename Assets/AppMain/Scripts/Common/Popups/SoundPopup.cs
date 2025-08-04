using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public class SoundPopup : CommonPopup
	{
		[SerializeField] Slider m_bgmSlider = null;
		[SerializeField] Slider m_voiceSlider = null;
		[SerializeField] Slider m_seSlider = null;

		bool m_isInit = false;

		override protected void Awake()
		{
			base.Awake();

			m_bgmSlider.value = SoundManager.Instance.BgmVolume * 10;
			m_voiceSlider.value = SoundManager.Instance.VoiceVolume * 10;
			m_seSlider.value = SoundManager.Instance.SeVolume * 10;
			m_isInit = true;
		}

		public void SetBGMVolume(Slider slider)
		{
			if (!m_isInit)
				return;
			var volume = slider.value * 0.1f;
			SoundManager.Instance.SetBGMVolume(volume);
		}

		public void SetVoiceVolume(Slider slider)
		{
			if (!m_isInit)
				return;
			var volume = slider.value * 0.1f;
			SoundManager.Instance.SetVoiceVolume(volume);
		}

		public void SetSEVolume(Slider slider)
		{
			if (!m_isInit)
				return;
			var volume = slider.value * 0.1f;
			SoundManager.Instance.SetSEVolume(volume);
		}
	}
}
