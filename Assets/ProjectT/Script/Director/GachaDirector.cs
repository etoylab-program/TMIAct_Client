
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GachaDirector : Director
{
    [Header("Property")]
    public float changeItemDuration = 5.0f;

    [Header("Object")]
    public GameObject psTabToNext;
    public GameObject psChangeGrade;

    private List<GachaItem> m_listItem = new List<GachaItem>();
    private List<int> m_listGrade = new List<int>();
    private int m_changeGradeItemIndex = -1;
    private float m_stopLoopTime = 0.0f;
    private bool m_showTablToNext = false;

    public List<int> listGrade { get { return m_listGrade; } }


    private void Awake()
    {
        m_listItem.Clear();
        m_listItem.AddRange(GetComponentsInChildren<GachaItem>(true));
    }

    public override void Init(Unit owner)
    {
        base.Init(owner);

        m_changeGradeItemIndex = -1;

        m_listGrade.Clear();
        for (int i = 0; i < m_listItem.Count; i++)
        {
            if (GameInfo.Instance.RewardList[i].ChangeGrade)
                m_changeGradeItemIndex = i;

            m_listItem[i].Init(GameInfo.Instance.RewardList[i]);
            m_listGrade.Add(m_listItem[i].grade);
        }

        m_stopLoopTime = 0.0f;
    }

    public override void Play( bool startPosIsZero = false )
    {
        base.Play( startPosIsZero );

        m_showTablToNext = false;
        psTabToNext.gameObject.SetActive(false);
        psChangeGrade.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (AppMgr.Instance.CustomInput.GetButtonUp(BaseCustomInput.eKeyKind.Select))
        {
            if (m_checkTime >= clickStartTime)
            {
                if (m_changeGradeItemIndex == -1)
                {
                    if (m_loopSection)
                    {
                        psTabToNext.gameObject.SetActive(false);

                        m_playableDirector.time = m_checkTime = loopEndTime;
                        m_loopSection = false;
                    }
                }
                else if (m_stopLoopTime == 0.0f)
                {
                    /*
                    UIGachaPanel gachaPanel = LobbyUIManager.Instance.GetUI<UIGachaPanel>("GachaPanel");
                    SoundManager.Instance.PlayFxSnd(gachaPanel.audios[0], FSaveData.Instance.GetSEVolume());
                    SoundManager.Instance.PlayDelayedFxSnd(gachaPanel.audios[0].length, gachaPanel.audios[1], FSaveData.Instance.GetSEVolume());
                    */
                    SoundManager.sSoundInfo soundInfo = SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 100, FSaveData.Instance.GetSEVolume());
                    if(soundInfo != null)
                        SoundManager.Instance.PlayDelayedSnd(SoundManager.eSoundType.FX, soundInfo.clip.length, 101, FSaveData.Instance.GetSEVolume());

                    psTabToNext.gameObject.SetActive(false);
                    psChangeGrade.gameObject.SetActive(true);

                    m_listItem[m_changeGradeItemIndex].ChangeGrade(changeItemDuration);

                    m_stopLoopTime = changeItemDuration;
                    StartCoroutine("DelayedStopLoop");
                }
            }

            /*if (forceQuit)
            {
                if (m_doubleClickCheckTime == TimeSpan.Zero)
                    m_doubleClickCheckTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                else if ((TimeSpan.FromTicks(DateTime.Now.Ticks) - m_doubleClickCheckTime).TotalSeconds <= 0.2f)
                {
                    m_playableDirector.Stop();

                    StopCoroutine("EndDirector");
                    End();
                }
                else
                    m_doubleClickCheckTime = TimeSpan.Zero;
            }*/
        }
    }

    private IEnumerator DelayedStopLoop()
    {
        //m_playableDirector.Pause();
        yield return new WaitForSeconds(m_stopLoopTime);

        //m_playableDirector.Resume();
        m_playableDirector.time = m_checkTime = loopEndTime;
        m_loopSection = false;
    }

    protected override IEnumerator EndDirector()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_checkTime < m_endTime)
        {
            m_checkTime += Time.fixedDeltaTime;
            if (!m_showTablToNext && m_checkTime >= loopStartTime)
            {
                m_showTablToNext = true;
                psTabToNext.gameObject.SetActive(true);
            }

            if (m_loopSection)
            {
                if (m_checkTime >= loopEndTime)
                {
                    m_playableDirector.time = loopStartTime;
                    m_checkTime = loopStartTime;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        if (isEnd == false)
            End();
    }
}
