using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Tarot
{
	public class ChatGPT
	{
		private string apiKey = Secret.AI_API_KEY; // OpenAIのAPIキー
		private string model = "gpt-3.5-turbo";

		private const string endpoint = "https://api.openai.com/v1/chat/completions";

		public async Task<string> SendMessageToGPT(string userMessage)
		{
			var messages = new List<Dictionary<string, string>>
			{
				new() { { "role", "user" }, { "content", userMessage } }
			};

			var requestData = new
			{
				model = model,
				messages = messages
			};

			var json = JsonConvert.SerializeObject(requestData);

			using UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
			byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

			var asyncOp = request.SendWebRequest();

			while (!asyncOp.isDone)
				await Task.Yield();

			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError($"Error: {request.error}");
				return "エラーが発生しました";
			}

			var result = request.downloadHandler.text;
			var response = JsonConvert.DeserializeObject<ChatGPTResponse>(result);

			return response.choices[0].message.content.Trim();
		}

		// ChatGPTのレスポンスクラス
		public class ChatGPTResponse
		{
			public List<Choice> choices;

			public class Choice
			{
				public Message message;
			}

			public class Message
			{
				public string role;
				public string content;
			}
		}
	}
}
