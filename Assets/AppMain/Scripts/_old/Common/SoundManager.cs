using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Tarot
{
	public class SoundManager : Singleton<SoundManager>
	{
		AudioSource m_bgmSource = null;
		AudioSource m_seSource = null;
		AudioSource m_voiceSource = null;

		[Header("SE")]
		AudioClip m_click = null;
		AudioClip m_decide = null;
		AudioClip m_cancel = null;

		float m_bgmVolume = 1.0f;
		public float BgmVolume { get { return m_bgmVolume; } }
		float m_seVolume = 1.0f;
		public float SeVolume { get { return m_seVolume; } }
		float m_voiceVolume = 1.0f;
		public float VoiceVolume { get { return m_voiceVolume; } }
		bool m_isMuted = false;
		CancellationTokenSource m_token;

		const float FADE_DURATION = 1f;
		const string BGM_KEY = "BGMVolume";
		const string SE_KEY = "SEVolume";
		const string VOICE_KEY = "VOICEVolume";
		const string BGM_PATH = "BGM/{0}";
		const string SE_PATH = "SE/{0}";
		const string SECRET_STORY_PATH = "Voice/After/{0:D2}{1}";

		public enum SEType
		{
			None,
			Click,
			Decide,
			Cancel,
		}

		private void Awake()
		{
			if (m_bgmSource == null)
				m_bgmSource = this.gameObject.AddComponent<AudioSource>();

			if (m_seSource == null)
				m_seSource = this.gameObject.AddComponent<AudioSource>();

			if (m_voiceSource == null)
				m_voiceSource = this.gameObject.AddComponent<AudioSource>();

			if (m_click == null)
				m_click = GetSE("Click");

			if (m_decide == null)
				m_decide = GetSE("Decide");

			if (m_cancel == null)
				m_cancel = GetSE("Cancel");

			gameObject.name = "SoundManager";
			var bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 1.0f);
			SetBGMVolume(bgmVolume);
			var seVolume = PlayerPrefs.GetFloat(SE_KEY, 1.0f);
			SetSEVolume(seVolume);
			var voiceVolume = PlayerPrefs.GetFloat(VOICE_KEY, 1.0f);
			SetVoiceVolume(voiceVolume);
		}

		/// <summary>BGM取得</summary>
		AudioClip GetBGM(string name)
		{
			var path = string.Format(BGM_PATH, name);
			AudioClip bgm = Resources.Load<AudioClip>(path);
			return bgm;
		}

		public void PlayBGM(AudioClip clip)
		{
			//if (m_bgmSource.clip == clip)
			//	return;

			// フェードアウトをキャンセル
			CancelFadeOut();

			m_bgmSource.clip = clip;
			m_bgmSource.volume = m_bgmVolume;
			m_bgmSource.loop = true;
			m_bgmSource.Play();
		}

		/// <summary>ファイル名を指定してBGMを再生</summary>
		public async void PlayBGM(string name)
		{
			if(name == "None")
			{
				await StopBGMWithFadeOut();
				return;
			}
			var bgm = GetBGM(name);
			PlayBGM(bgm);
		}

		public void StopBGM()
		{
			m_bgmSource.Stop();
		}

		public async UniTask StopBGMWithFadeOut()
		{
			CancelFadeOut();
			m_token = new CancellationTokenSource();
			await FadeOutBGM(m_token.Token);
		}

		async UniTask FadeOutBGM(CancellationToken token)
		{
			var startVolume = m_bgmVolume;
			float time = 0;

			while (time < FADE_DURATION)
			{
				if (token.IsCancellationRequested)
					return;

				time += Time.deltaTime;
				m_bgmSource.volume = Mathf.Lerp(startVolume, 0, time / FADE_DURATION);
				await UniTask.Yield(PlayerLoopTiming.Update, token);
			}
			if (token.IsCancellationRequested)
				return;
			m_bgmSource.Stop();
			m_token = null;
			//m_bgmSource.clip = null;
			m_bgmSource.volume = m_bgmVolume;
		}

		void CancelFadeOut()
		{
			if(m_token != null)
			{
				m_token.Cancel();
				m_token.Dispose();
				m_token = null;
			}
		}

		/// <summary>SE取得</summary>
		AudioClip GetSE(string name)
		{
			var path = string.Format(SE_PATH, name);
			AudioClip se = Resources.Load<AudioClip>(path);
			return se;
		}

		public void PlaySE(AudioClip clip)
		{
			StopSE();
			//m_seSource.PlayOneShot(clip, m_seVolume);
		}

		public void PlaySE(string name)
		{
			var se = GetSE(name);
			PlaySE(se);
		}

		/// <summary>ファイル名を指定してSEを再生</summary>
		public void PlayClickSE(SEType type)
		{
			switch (type)
			{
				case SEType.None:
					return;
				case SEType.Click:
					PlaySE(m_click);
					break;
				case SEType.Decide:
					PlaySE(m_decide);
					break;
				case SEType.Cancel:
					PlaySE(m_cancel);
					break;
			}
		}

		public void StopSE()
		{
			m_seSource.Stop();
		}

		/// <summary>SEが終了するのを待つ</summary>
		public async UniTask<bool> WaitFinishSE()
		{
			await UniTask.WaitUntil(() => !m_seSource.isPlaying);
			return true;
		}

		/// <summary>ボイスオーディオを取得</summary>
		public AudioSource GetVoiceAudio()
		{
			return m_voiceSource;
		}

		public void PlayVoice(AudioClip clip)
		{
			StopVoice();
			m_voiceSource.PlayOneShot(clip, m_voiceVolume);
		}

		public void StopVoice()
		{
			m_voiceSource.Stop();
		}

		/// <summary>ボイスが終了するのを待つ</summary>
		public async UniTask<bool> WaitFinishVoice()
		{
			await UniTask.WaitUntil(() => !m_voiceSource.isPlaying);
			// 鳴り終わってから0.5s待つ
			await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: this.gameObject.GetCancellationTokenOnDestroy());
			return true;
		}

		public void PlaySecretStory(int index, string lang)
		{
			string path = string.Empty;
			if (lang != "ja")
				lang = "en";
			path = string.Format(SECRET_STORY_PATH, index, lang);
			var clip = Resources.Load<AudioClip>(path);

			StopVoice();
			m_voiceSource.PlayOneShot(clip, m_voiceVolume);
		}

		public void SetBGMVolume(float volume)
		{
			m_bgmVolume = volume;
			m_bgmSource.volume = m_isMuted ? 0 : m_bgmVolume;
			PlayerPrefs.SetFloat(BGM_KEY, m_bgmVolume);
		}

		public void SetSEVolume(float volume)
		{
			m_seVolume = volume;
			m_seSource.volume = m_isMuted ? 0 : m_seVolume;
			PlayerPrefs.SetFloat(SE_KEY, m_seVolume);
		}

		public void SetVoiceVolume(float volume)
		{
			m_voiceVolume = volume;
			m_voiceSource.volume = m_isMuted ? 0 : m_voiceVolume;
			PlayerPrefs.SetFloat(VOICE_KEY, m_voiceVolume);
		}

		/// <summary>ボリュームを改めてPlayerPrefsから取り直す</summary>
		public void ResetVolume()
		{
			var bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 1.0f);
			SetBGMVolume(bgmVolume);
			var seVolume = PlayerPrefs.GetFloat(SE_KEY, 1.0f);
			SetSEVolume(seVolume);
			var voiceVolume = PlayerPrefs.GetFloat(VOICE_KEY, 1.0f);
			SetVoiceVolume(voiceVolume);
		}

		public void Mute(bool mute)
		{
			m_isMuted = mute;
			m_bgmSource.volume = m_isMuted ? 0 : m_bgmVolume;
			m_seSource.volume = m_isMuted ? 0 : m_seVolume;
			m_voiceSource.volume = m_isMuted ? 0 : m_voiceVolume;
		}
	}
}
