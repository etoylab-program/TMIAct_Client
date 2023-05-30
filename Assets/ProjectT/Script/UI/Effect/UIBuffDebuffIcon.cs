
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIBuffDebuffIcon : MonoBehaviour
{
    public class sActiveIconInfo
    {
        public eBuffIconType buffIconType;
        public GameObject sprite;
        public float duration;
        public float checkDuration;
        public bool buffIconFlash;
        public Coroutine cr;


        public void Set(eBuffIconType buffIconType, GameObject sprite, float duration, bool buffIconFlash)
        {
            this.buffIconType = buffIconType;
            this.sprite = sprite;
            this.duration = duration;
            this.buffIconFlash = buffIconFlash;
            //this.duration = duration == -2 ? 1000.0f : duration;
            checkDuration = 0.0f;

            cr = null;
            //sprite.SetActive(true);
        }

        public void ResetCheckDuration()
        {
            checkDuration = 0.0f;

            Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            UITexture tex = sprite.GetComponent<UITexture>();
            if (tex)
            {
                tex.color = color;
            }
            else
            {
                SpriteRenderer sprRenderer = sprite.GetComponent<SpriteRenderer>();
                sprRenderer.color = color;
            }
        }
    }


    public UITexture[] sprBuffs;
    public UITexture[] sprDebuffs;

    private Unit					m_parent			= null;
    private UIGrid					m_grid;
    private List<sActiveIconInfo>	m_listVisibleIcon	= new List<sActiveIconInfo>();
	private WaitForFixedUpdate		mWaitForFixedUpdate	= new WaitForFixedUpdate();


	private void Awake()
    {
        m_grid = GetComponentInChildren<UIGrid>();
    }

    public void Init(Unit parent)
    {
        m_parent = parent;
    }

    public sActiveIconInfo Add(eBuffIconType buffIconType, float duration, bool buffIconFlash)
    {
        if (buffIconType == eBuffIconType.None)
            return null;

        GameObject sprite = null;
        if ((int)buffIconType < 0) // Debuff
            sprite = sprDebuffs[Mathf.Abs((int)buffIconType) - 1].gameObject;
        else
            sprite = sprBuffs[(int)buffIconType - 1].gameObject;

        sActiveIconInfo iconInfo = new sActiveIconInfo();
        iconInfo.Set(buffIconType, sprite, duration, buffIconFlash);

        if (iconInfo.duration <= 0.0f)
            return null;

        sActiveIconInfo find = m_listVisibleIcon.Find(x => x.buffIconType == buffIconType);
        if(find != null)
        {
            float remainTime = find.duration - find.checkDuration;
            if (remainTime <= iconInfo.duration)
            {
                m_listVisibleIcon.Insert(0, iconInfo);
                if (find.cr != null)
                    Utility.StopCoroutine(World.Instance, ref find.cr);
                m_listVisibleIcon.Remove(find);
            }
            else
                return null;
        }
        else
            m_listVisibleIcon.Add(iconInfo);

        iconInfo.cr = World.Instance.StartCoroutine(Hide(iconInfo));

        m_grid.Reposition();
        return iconInfo;
    }

    private IEnumerator Hide(sActiveIconInfo activeIcon)
    {
        activeIcon.sprite.SetActive(true);
        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        UITexture tex = activeIcon.sprite.GetComponent<UITexture>();
        tex.color = color;

        bool add = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (activeIcon.checkDuration < activeIcon.duration)
        {
            if ((activeIcon.duration - activeIcon.checkDuration) <= GameInfo.Instance.BattleConfig.BuffDebuffIconFlashTime)
            {
                if (!add)
                {
                    color.a -= Time.fixedDeltaTime / GameInfo.Instance.BattleConfig.BuffDebuffIconFlashDuration;
                    if (color.a <= 0.0f)
                    {
                        color.a = 0.0f;
                        add = true;
                    }

                    tex.color = color;
                }
                else
                {
                    color.a += Time.fixedDeltaTime / GameInfo.Instance.BattleConfig.BuffDebuffIconFlashDuration;
                    if (color.a >= 1.0f)
                    {
                        color.a = 1.0f;
                        add = false;
                    }

                    tex.color = color;
                }
            }
            else
            {
                tex.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            activeIcon.checkDuration += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        activeIcon.sprite.SetActive(false);
        m_listVisibleIcon.Remove(activeIcon);

        m_grid.Reposition();
    }

    public void RestartCoroutine()
    {
        for(int i = 0; i < m_listVisibleIcon.Count; i++)
        {
            if (m_listVisibleIcon[i].cr != null)
            {
                Utility.StopCoroutine(World.Instance, ref m_listVisibleIcon[i].cr);
                m_listVisibleIcon[i].cr = World.Instance.StartCoroutine(Hide(m_listVisibleIcon[i]));
            }
        }
    }

    public void End(bool clear = true)
    {
        StopAllCoroutines();

        for (int i = 0; i < m_listVisibleIcon.Count; i++)
            m_listVisibleIcon[i].sprite.SetActive(false);

        if (clear)
        {
            m_listVisibleIcon.Clear();
        }
    }

    private void Update()
    {
        if (m_parent == null)
            return;

        if (m_parent.curHp <= 0.0f || !m_parent.IsActivate())
        {
            End();
            return;
        }

        Vector3 pos = Vector3.zero;
        Transform dummy = m_parent.aniEvent.GetBoneByName("hpbar");
        if (dummy)
        {
            if (m_parent.aniEvent.IsLyingAni())
                pos = dummy.transform.position + m_parent.addIconLyingPos;
            else
                pos = dummy.transform.position + m_parent.addIconPos;
        }
        else
        {
            pos = m_parent.transform.position;

            float height = 0.0f;
            if (m_parent.MainCollider.direction == (int)eAxis.Y)
                height = m_parent.MainCollider.height;
            else
                height = m_parent.MainCollider.radius * 2.0f;

            if (m_parent.aniEvent.IsLyingAni() == false)
                pos.y += height;
            else
                pos.y += height * 0.5f;
        }

        transform.position = pos;
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}