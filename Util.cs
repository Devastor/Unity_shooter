﻿using UnityEngine;

public class Util : MonoBehaviour 
{
	public static void SetLayerRecursively (GameObject _obj, int _newLayer)
	{
		if (_obj == null)
			return;
		
		_obj.layer = _newLayer;
		foreach (Transform child in _obj.transform) 
		{			
			if (child == null)
				continue;
			
			SetLayerRecursively (child.gameObject, _newLayer);
		}
	}
}
