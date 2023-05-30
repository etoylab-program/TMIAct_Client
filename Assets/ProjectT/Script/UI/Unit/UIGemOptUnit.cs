using UnityEngine;
using System.Collections;

public class UIGemOptUnit : FUnit
{
    public enum eGAUGESTATE
    {
        NORMAL = 0,
        MAX,
    }

    public UILabel kTextLabel;
	public UIGaugeUnit kGaugeUnit;
    public UISprite kLockSpr;

    public float kGageAllGainExp = -1;
    private Coroutine m_crGageExp = null;
    private int _optid;
    private int _optvalue;
    private GemData _gemdata;

    public void Lock()
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        kGageAllGainExp = -1;
        _gemdata = null;
        kLockSpr.gameObject.SetActive(true);
        kTextLabel.textlocalize = "";
        kGaugeUnit.InitGaugeUnit(0);
    }

    public void Open()
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        kGageAllGainExp = -1;
        _gemdata = null;
        kLockSpr.gameObject.SetActive(false);
        kTextLabel.textlocalize = "";
        kGaugeUnit.InitGaugeUnit(0);
    }

    public void Opt(GemData gemdata, int slot, bool bani = false)
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        kGageAllGainExp = -1;
        _gemdata = gemdata;
        _optid = gemdata.RandOptID[slot];
        _optvalue = gemdata.RandOptValue[slot];
        kLockSpr.gameObject.SetActive(false);
        kTextLabel.textlocalize = "";
        kGaugeUnit.InitGaugeUnit(0);

        var optdata = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == gemdata.TableData.RandOptGroup && x.ID == gemdata.RandOptID[slot]);
        if (optdata != null)
        {
            if (bani)
            {
                kGaugeUnit.InitGaugeUnit(0.0f);
                SetValueGainTween(_optvalue);
                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(optdata.Desc), 0 / (float)eCOUNT.MAX_RATE_VALUE * 100.0f);
            }
            else
            {
                int value = optdata.Min + (int)((float)gemdata.RandOptValue[slot] * optdata.Value);
                float v = value / (float)eCOUNT.MAX_RATE_VALUE;
                
                if( optdata.EffectType.Contains( "Penetrate" ) || optdata.EffectType.Contains( "Sufferance" ) ) {
                    v /= 10.0f;
				}

                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(optdata.Desc), v * 100.0f);
                float f = (float)gemdata.RandOptValue[slot] / (float)optdata.RndStep;
                kGaugeUnit.InitGaugeUnit(f);
                if (f >= 1.0f)
                    kGaugeUnit.SetColor((int)eGAUGESTATE.MAX);
                else
                    kGaugeUnit.SetColor((int)eGAUGESTATE.NORMAL);
            }
        }
    }

    public void Opt(GemData gemdata, int optid, int optvalue, bool bani = false)
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        kGageAllGainExp = -1;
        _gemdata = gemdata;
        _optid = optid;
        _optvalue = optvalue;
        kLockSpr.gameObject.SetActive(false);
        kTextLabel.textlocalize = "";
        kGaugeUnit.InitGaugeUnit(0);

        var optdata = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == gemdata.TableData.RandOptGroup && x.ID == optid);
        if (optdata != null)
        {
            if (bani)
            {
                kGaugeUnit.InitGaugeUnit(0.0f);
                SetValueGainTween(_optvalue);
                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(optdata.Desc), 0 / (float)eCOUNT.MAX_RATE_VALUE * 100.0f);
            }
            else
            {
                int value = optdata.Min + (int)((float)optvalue * optdata.Value);
                float v = value / (float)eCOUNT.MAX_RATE_VALUE;

                if( optdata.EffectType.Contains( "Penetrate" ) || optdata.EffectType.Contains( "Sufferance" ) ) {
                    v /= 10.0f;
                }

                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(optdata.Desc), v * 100.0f);
                float f = (float)optvalue / (float)optdata.RndStep;
                kGaugeUnit.InitGaugeUnit(f);
                if (f >= 1.0f)
                    kGaugeUnit.SetColor((int)eGAUGESTATE.MAX);
                else
                    kGaugeUnit.SetColor((int)eGAUGESTATE.NORMAL);
            }
        }
    }

    void FixedUpdate()
    {
        if (_gemdata == null)
            return;
        
        var optdata = GameInfo.Instance.GameTable.FindGemRandOpt(x => x.GroupID == _gemdata.TableData.RandOptGroup && x.ID == _optid);
        if (optdata == null)
            return;

        if (kGageAllGainExp == -1)
            return;

        float fnow = (float)kGageAllGainExp / (float)optdata.RndStep;
        float fmax = (float)_optvalue / (float)optdata.RndStep;

        int value = optdata.Min + (int)((float)kGageAllGainExp * optdata.Value);

        if (fnow >= fmax)
        {
            value = optdata.Min + (int)((float)_optvalue * optdata.Value );
            fnow = fmax;
        }

        float f = value;
        if( optdata.EffectType.Contains( "Penetrate" ) || optdata.EffectType.Contains( "Sufferance" ) ) {
            f /= 10.0f;
        }

        kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(optdata.Desc), f / (float)eCOUNT.MAX_RATE_VALUE * 100.0f);
        kGaugeUnit.InitGaugeUnit(fnow);
        if (fnow >= 1.0f)
            kGaugeUnit.SetColor((int)eGAUGESTATE.MAX);
        else
            kGaugeUnit.SetColor((int)eGAUGESTATE.NORMAL);
    }

    private void SetValueGainTween(int value)
    {
        Utility.StopCoroutine(this, ref m_crGageExp);
        m_crGageExp = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, value, 1.0f));
    }

    public void OnClick_Slot()
	{

	}
 
}
