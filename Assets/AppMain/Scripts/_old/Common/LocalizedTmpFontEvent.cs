using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Tarot
{
	/// <summary>TmpFontAsset用のLocalizedAssetEvent</summary>
	[AddComponentMenu("Localization/Asset/" + nameof(LocalizedTmpFontEvent))]
	public class LocalizedTmpFontEvent : LocalizedAssetEvent<TMP_FontAsset, LocalizedTmpFont, UnityEventTmpFont> { }

	/// <summary>TmpFontAssetを引数とするUnityEvent</summary>
	[Serializable]
	public class UnityEventTmpFont : UnityEvent<TMP_FontAsset> { }
}
