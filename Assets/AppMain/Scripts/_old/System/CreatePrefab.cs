// Copyright © 2021 BellMemo All rights reserved.

using UnityEngine;

public class CreatePrefab : MonoBehaviour
{
	//[ToolTips("プレハブ"), SerializeField]
	public GameObject m_prefab = null;

	//[ToolTips("ロケーター"), SerializeField]
	public Transform m_pos = null;

	// Start is called before the first frame update
	void Start()
	{
		var pre = Instantiate(m_prefab, m_pos);
	}
}
