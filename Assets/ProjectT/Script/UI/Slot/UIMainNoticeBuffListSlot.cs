using System;
using UnityEngine;
using System.Collections;

public class UIMainNoticeBuffListSlot : FSlot 
{
	public UILabel kBuffNameLabel;
	public UILabel kBuffEndTimeLabel;

	private BuffEffectData _buffEffectData = null;
	private System.DateTime _nowTime;

	public void UpdateSlot(BuffEffectData data) 	//Fill parameter if you need
	{
		_buffEffectData = data;

		if (_buffEffectData == null)
		{
			kBuffNameLabel.SetActive(false);
			kBuffEndTimeLabel.SetActive(false);
			return;
		}

		kBuffNameLabel.SetActive(true);
		kBuffEndTimeLabel.SetActive(true);

		kBuffNameLabel.textlocalize = FLocalizeString.Instance.GetText(_buffEffectData.TableData.Name);

		SetUserBuffEndTime();
	}

    private void Update()
    {
		SetUserBuffEndTime();
    }

	private void SetUserBuffEndTime()
	{
		if (_buffEffectData == null)
			return;

		_nowTime = GameSupport.GetCurrentServerTime();

		if (_nowTime.Ticks > _buffEffectData.BuffEndTime.Ticks)
		{
			GameInfo.Instance.UserBuffEffectData.BuffEffectDataList.Remove(_buffEffectData);
			_buffEffectData = null;

			LobbyUIManager.Instance.Renewal("MainPanel");
			return;
		}

		kBuffEndTimeLabel.textlocalize = GameSupport.GetRemainTimeString_DayAndHours(_buffEffectData.BuffEndTime, _nowTime);
		kBuffEndTimeLabel.gameObject.SetActive(_buffEffectData.BuffEndTime != DateTime.MaxValue);
	}

    public void OnClick_Slot()
	{
	}
 
}
