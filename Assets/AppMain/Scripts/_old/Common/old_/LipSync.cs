using UnityEngine;
using UnityEngine.UI;

namespace Tarot
{
	public class LipSync : MonoBehaviour
	{
		[SerializeField] Image m_charaImage;
		[SerializeField] Sprite[] m_normal = null;
		[SerializeField] Sprite[] m_smile = null;
		[SerializeField] Sprite[] m_angry = null;
		[SerializeField] Sprite[] m_sad = null;
		[SerializeField] Sprite[] m_shy = null;
		[SerializeField] Sprite[] m_another = null;
		AudioSource m_audioSource;
		bool m_isQuietly = false;
		public bool IsQuietly { get { return m_isQuietly; } set { m_isQuietly = value; } }
		public float m_updateInterval = 0.1f;
		public float m_sensitivity = 1000f; // 感度調整用（ボイスボリュームによって調整する必要あり）

		private float[] spectrum = new float[256];
		private float timer;
		public enum Expression
		{
			None,
			Normal,
			Smile,
			Angry,
			Sad,
			Shy,
		}
		string m_expression = "Normal";

		private void Start()
		{
			m_audioSource = SoundManager.Instance.GetVoiceAudio();
		}

		public void SetExpression(string expression)
		{
			m_expression = expression;
			m_sensitivity = 1000 * (1.1f - SoundManager.Instance.VoiceVolume);
			if (SoundManager.Instance.VoiceVolume == 0.1f)
				m_sensitivity = 10000;
		}

		void Update()
		{
			timer += Time.deltaTime;
			if (timer >= m_updateInterval)
			{
				timer = 0f;
				switch (m_expression)
				{
					default:
					case "Normal":
						UpdateMouth(m_normal);
						break;
					case "Smile":
					case "SmileSec":
						UpdateMouth(m_smile);
						break;
					case "Angry":
					case "AngrySec":
						UpdateMouth(m_angry);
						break;
					case "Sad":
						UpdateMouth(m_sad);
						break;
					case "Shy":
						UpdateMouth(m_shy);
						break;
					case "ShySec":
					case "Sup":
						UpdateMouth(m_another);
						break;
				}
			}
		}

		void UpdateMouth(Sprite[] sprites)
		{
			if (!m_audioSource.isPlaying || m_isQuietly)
			{
				if(m_charaImage.sprite != sprites[0])
					m_charaImage.sprite = sprites[0];
				return;
			}

			m_audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

			float maxValue = 0f;
			for (int i = 0; i < 8; i++) // 低周波数帯域を使用
			{
				if (spectrum[i] > maxValue)
					maxValue = spectrum[i];
			}

			float normalizedValue = maxValue * m_sensitivity;
			int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedValue * (sprites.Length - 1)), 0, sprites.Length - 1);

			//Debug.Log($"Max Value: {maxValue}, Normalized: {normalizedValue}, Sprite Index: {spriteIndex}");

			m_charaImage.sprite = sprites[spriteIndex];
		}
	}
}
