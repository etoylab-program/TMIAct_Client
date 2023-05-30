
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeviceBatteryAndDate : MonoBehaviour
{
    public UILabel		kTimeLabel;
    public UIGaugeUnit	kBatteryGauge;
    public UISprite		kChargingSprite;

	private float mTime = 30.0f;


    private void Update()
    {
		mTime += Time.deltaTime;
		if (mTime >= 30.0f)
		{
			if (kTimeLabel != null)
			{
				kTimeLabel.text = string.Format("{0:D2}:{1:D2}", System.DateTime.Now.Hour, System.DateTime.Now.Minute);
			}

			if (kBatteryGauge != null)
			{
				kBatteryGauge.InitGaugeUnit(SystemInfo.batteryLevel);

				if (SystemInfo.batteryStatus == BatteryStatus.Charging)
				{
					kBatteryGauge.kGaugeSpr.color = Color.green;
					kChargingSprite.gameObject.SetActive(true);
				}
				else
				{
					kBatteryGauge.kGaugeSpr.color = Color.white;
					kChargingSprite.gameObject.SetActive(false);
				}
			}

			mTime = 0.0f;
		}
    }
}
