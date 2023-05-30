using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Holoville.HOTween;

public class UIHPGaugeUnit : FUnit
{

    public UISprite kBGSpr;
    public UISprite kGaugeRemainSpr;
    public UISprite kGaugeSpr;
    public UISprite kSubSpr;
    public UISprite kAddSpr;
    public UILabel kLabel;
    public bool kMax;
    public float kUpSpeed = 1.0f;
    public float kDownSpeed = 1.0f;
    private int _nowValue = 0;
    private int _maxValue = 0;

    private UISlider kSlider;
    private bool bSubAction = false;
    //private Tweener mGageExpGainTween = null;
    private Coroutine m_cr = null;
    public int kGageAllGainExp = -1;
    private int _curhp = -1;
    public int Now { get { return _nowValue; } }
    public int Max { get { return _maxValue; } }

    public bool _isAccGauge = false;
    public int _accGaugeMax = 0;          //  셋팅시 맥스 값(기본값 0)
    public int _accGaugeCount = 0;          //  현재 게이지의 갯수
    public int _accSubCount = 0;            //  차감할 게이지의 갯수(2000이상의 차감값 경우) 아직 안씀
    public int _remainValueRemember = 0;    //  차감할 체력의 값이 남아있는값보다 많은 경우 1000으로 돌아가서 차감된 값
    public int _remainValueCurrent = 0;     //  현재 값
    public int _remainValueBefore = 0;      //  차감되기 이전의 값


    public void InitUIGaugeUnit(int now, int max)
    {
        Set(now, max);
    }

    public void Set(int now, int max, bool isAcc = false)
    {
        _nowValue = now;
        _maxValue = max;
        _curhp = now;
        _isAccGauge = isAcc || _isAccGauge;

        //  누적 HP시
        if (_isAccGauge == true)
        {
            kSlider = kGaugeSpr.GetComponent<UISlider>();

            _remainValueBefore = _remainValueCurrent;

            _accGaugeCount = (_nowValue / _accGaugeMax) - 1;
            _remainValueCurrent = ( _nowValue % _accGaugeMax ) == 0 ? _accGaugeMax : _nowValue % _accGaugeMax;
        }
        else
        {
            if (kGaugeRemainSpr != null)
                kGaugeRemainSpr.gameObject.SetActive(false);
        }

        if (kSubSpr)
            kSubSpr.gameObject.SetActive(false);

        if (kAddSpr)
            kAddSpr.gameObject.SetActive(false);

        SetGaugeSprite(kGaugeSpr, _nowValue, _maxValue);
        SetGaugeLabel(kLabel, _nowValue, _maxValue, kMax);

        m_cr = null;

        kGageAllGainExp = _nowValue;
    }

    public void Add(int addValue)
    {
        if( addValue == 0 ) {
            return;
		}

        if (_maxValue < _nowValue + addValue)
            _nowValue = _maxValue;
        else
            _nowValue += addValue;

        //  누적 HP시
        if (_isAccGauge == true && addValue != 0)
        {
            _remainValueBefore = _remainValueCurrent;

            _accGaugeCount = _nowValue / _accGaugeMax;
            _remainValueCurrent = ( _nowValue % _accGaugeMax ) == 0 ? _accGaugeMax : _nowValue % _accGaugeMax;
        }

        if (kAddSpr)
        {
            SetGaugeSprite(kAddSpr, _nowValue, _maxValue);
            kAddSpr.gameObject.SetActive(true);
        }

        if (kSubSpr)
            kSubSpr.gameObject.SetActive(false);

        bSubAction = false;

        if (kUpSpeed != 0.0f)
        {
            Utility.StopCoroutine(this, ref m_cr);

            bool isActiveMainUI = false;
            if(World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
            {
                isActiveMainUI = World.Instance.UIPlay.gameObject.activeSelf;
            }
            else
            {
                isActiveMainUI = World.Instance.UIPVP.gameObject.activeSelf;
            }

            if (isActiveMainUI && gameObject.activeSelf)
                m_cr = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _nowValue, kDownSpeed));
        }
        else
        {
            m_cr = null;

            SetGaugeSprite(kGaugeSpr, _nowValue, _maxValue);
            SetGaugeLabel(kLabel, _nowValue, _maxValue, kMax);
        }

        if (kLabel)
            GameSupport.TweenerPlay(kLabel.gameObject);
    }

    public void Sub(int subValue)
    {
        if (subValue == 0)
            return;

        if (kSubSpr)
        {
            kSubSpr.fillAmount = kGaugeSpr.fillAmount;
            kSubSpr.gameObject.SetActive(true);
        }

        if (kAddSpr)
            kAddSpr.gameObject.SetActive(false);

        _nowValue -= subValue;

        //  누적 HP시
        if (_isAccGauge == true && subValue != 0)
        {
            //  차감되기전 HP 저장
            _remainValueBefore = _remainValueCurrent;
            //  현재 체력의 게이지 갯수
            _accGaugeCount = _nowValue / _accGaugeMax;
            //  현재 체력의 값 계산
            _remainValueCurrent = ( _nowValue % _accGaugeMax ) == 0 ? _accGaugeMax : _nowValue % _accGaugeMax;

            //  실 체력 게이지
            SetGaugeSlider(kSlider, _remainValueCurrent, _maxValue);
        }
        else
        {
            SetGaugeSprite(kGaugeSpr, _nowValue, _maxValue);
        }

        bSubAction = true;

        if (kDownSpeed != 0.0f)
        {
            Utility.StopCoroutine(this, ref m_cr);
            
            //  게이지가 꺼진 상태일 경우 안켜질 가능성을 고려
            //  강제 활성화 시켜 줍니다.
            kGaugeSpr.gameObject.SetActive(true);

            bool isActiveMainUI = false;
            if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
            {
                isActiveMainUI = World.Instance.UIPlay.gameObject.activeSelf;
            }
            else
            {
                isActiveMainUI = World.Instance.UIPVP.gameObject.activeSelf;
            }

            if (_isAccGauge == false)
            {
                if (isActiveMainUI && gameObject.activeSelf)
                    m_cr = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _nowValue, kDownSpeed));
            }
            else
            {
                int destination = 0;
                if (_remainValueBefore < subValue)
                {
                    //  남은 값이 차감 값보다 작은 경우
                    //  목표값은 0으로 맞춥니다.
                    _remainValueRemember = _accGaugeMax - ((Mathf.Abs(_remainValueBefore - subValue)) % _accGaugeMax);
                    destination = 0;

                    kGaugeSpr.gameObject.SetActive(false);
                }
                else
                {
                    //  남은 값이 큰경우는
                    //  남은 값을 목표치로 맞춥니다.
                    destination = _remainValueCurrent;
                }

                if (isActiveMainUI && gameObject.activeSelf)
                    m_cr = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, destination, kDownSpeed));
            }
        }
        else
        {
            m_cr = null;

            SetGaugeSprite(kSubSpr, _nowValue, _maxValue);
            SetGaugeLabel(kLabel, _nowValue, _maxValue, kMax);
        }

        if (kLabel)
            GameSupport.TweenerPlay(kLabel.gameObject);
    }

    void FixedUpdate()
    {
        if (kGageAllGainExp == -1)
            return;
        //if (mGageExpGainTween == null)
        //    return;
        if (m_cr == null)
            return;

        if (bSubAction)
        {
            SetGaugeSprite(kSubSpr, kGageAllGainExp, _maxValue);
        }
        else
        {
            SetGaugeSprite(kGaugeSpr, kGageAllGainExp, _maxValue);
        }

        _curhp = kGageAllGainExp;

        if (_isAccGauge == true)
        {
            //  남은 게이지가 있다는 표시
            bool isRemainGaugeActive = ( _accGaugeCount > 0 );
            kGaugeRemainSpr.gameObject.SetActive(isRemainGaugeActive);

            //  연출 게이지 값이 0일시
            if (kGageAllGainExp == 0)
            {
                StopCoroutine(m_cr);

                //  현재 값을 다시 맥스 값으로 맞추고
                //  기억한 값까지 진행합니다.
                kGageAllGainExp = _accGaugeMax;

                bool isActiveMainUI = false;
                if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
                {
                    isActiveMainUI = World.Instance.UIPlay.gameObject.activeSelf;
                }
                else
                {
                    isActiveMainUI = World.Instance.UIPVP.gameObject.activeSelf;
                }

                if (isActiveMainUI && gameObject.activeSelf)
                    m_cr = StartCoroutine(Utility.UpdateCoroutineValue((x) => kGageAllGainExp = (int)x, kGageAllGainExp, _remainValueRemember, kDownSpeed));

                kGaugeSpr.gameObject.SetActive(true);
            }
            else
            {
                //  값이 같지 않은 경우(게이지 차감이 진행 되는 경우)
                //  슬라이드 끝 이미지 활성화
                bool isblinkActive = ( kGageAllGainExp != _remainValueCurrent );
                kSlider.thumb.gameObject.SetActive(isblinkActive);
            }
        }

        SetGaugeLabel(kLabel, kGageAllGainExp, _maxValue, kMax);
    }

    /// <summary>
    ///  체력바에 슬라이더가 붙어 있는 경우
    /// </summary>
    private void SetAccGauge()
    {
    }

    /// <summary>
    ///  현재 / 맥스 게이지 표시
    /// </summary>
    private void SetGaugeSprite(UISprite sprite, int nowValue, int maxValue)
    {
        int max = maxValue;
        if (_isAccGauge == true)
        {
            max = _accGaugeMax;
        }

        if (sprite != null)
        {
            sprite.fillAmount = nowValue / (float)max;
        }
    }

    /// <summary>
    ///  현재 / 맥스 슬라이더 표시
    /// </summary>
    private void SetGaugeSlider(UISlider slider, int nowValue, int maxValue)
    {
        int max = maxValue;
        if (_isAccGauge == true)
        {
            max = _accGaugeMax;
        }

        if (slider != null)
        {
            slider.value = nowValue / (float)max;
        }
    }

    /// <summary>
    ///  현재 / 맥스 텍스트 표시
    /// </summary>
    private void SetGaugeLabel(UILabel label, int nowValue, int maxValue, bool isMax)
    {
        if (label != null)
        {
            if (_isAccGauge == true)
            {
                label.textlocalize = string.Format("x {0}", _accGaugeCount + 1);

                bool isActive = _accGaugeCount > 0;
                label.gameObject.SetActive(isActive);
            }
            else
            {
                if (isMax == true)
                    label.textlocalize = string.Format("{0} / {1}", nowValue, maxValue); //nowValue.ToString() + " / " + maxValue.ToString();
                else
                    label.textlocalize = nowValue.ToString();
            }
        }
    }
}
