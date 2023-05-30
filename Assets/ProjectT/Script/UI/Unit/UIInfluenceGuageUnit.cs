using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfluenceGuageUnit : FUnit
{
	[SerializeField] private GameObject kMeObj;
	[SerializeField] private UILabel kTitle;
	[SerializeField] private UIGaugeUnit kGuage;


	public void SetData(string title, float gaugeValue, bool IsMe)
    {
		kMeObj.SetActive(IsMe);
		kTitle.textlocalize = title;
		kGuage.InitGaugeUnit(Mathf.Clamp01(gaugeValue));
		kGuage.SetText(string.Format("{0}%", Mathf.Floor(Mathf.Clamp01(gaugeValue) * 1000f) * 0.1f));
    }
}
