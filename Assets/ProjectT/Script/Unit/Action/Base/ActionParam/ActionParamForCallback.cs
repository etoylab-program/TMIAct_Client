using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionParamForCallback : IActionBaseParam {
	public Action OnCallback { get; private set; } = null;

	public ActionParamForCallback( Action onCallback ) {
		OnCallback = onCallback;
	}
}
