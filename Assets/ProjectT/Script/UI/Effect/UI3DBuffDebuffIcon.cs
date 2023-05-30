
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI3DBuffDebuffIcon : MonoBehaviour
{
    public SpriteRenderer[] sprBuffs;
    public SpriteRenderer[] sprDebuffs;
    public int widthPerIcon;
    public int heightPerIcon;
    public int numWidth; 

    public Unit Parent { get; private set; } = null;

    private List<UIBuffDebuffIcon.sActiveIconInfo>	m_listVisibleIcon	= new List<UIBuffDebuffIcon.sActiveIconInfo>();
    private float									m_width = 0.0f;
	private WaitForFixedUpdate						mWaitForFixedUpdate	= new WaitForFixedUpdate();


	private void Awake()
    {
    }

    public void Init(Unit parent)
    {
        Parent = parent;
		m_width = (float)widthPerIcon / 2.0f;
	}

    public void Show(bool show)
    {
        for(int i = 0; i < sprBuffs.Length; i++)
            sprBuffs[i].enabled = show;

        for(int i = 0; i < sprDebuffs.Length; i++)
            sprDebuffs[i].enabled = show;
    }

    private void Reposition()
    {
        float firstX = -m_width * (Mathf.Min(numWidth, m_listVisibleIcon.Count) - 1);
        float yIndex = 0;
        int widthIndex = 0;
        Vector3 pos = Vector3.zero;

        for(int i = 0; i < m_listVisibleIcon.Count; i++, widthIndex++)
        {
            if (widthIndex >= numWidth)
            {
                widthIndex = 0;
                ++yIndex;
            }

            pos.x = (firstX + ((float)widthPerIcon * widthIndex)) / 100.0f;
            pos.y = (-(float)heightPerIcon * yIndex) / 100.0f;
            pos.z = 0.0f;

            m_listVisibleIcon[i].sprite.transform.localPosition = pos;
        }
    }

    public UIBuffDebuffIcon.sActiveIconInfo Add(eBuffIconType buffIconType, float duration, bool buffIconFlash)
    {
        if (buffIconType == eBuffIconType.None)
            return null;

        GameObject sprite = null;
        if ((int)buffIconType < 0) // Debuff
            sprite = sprDebuffs[Mathf.Abs((int)buffIconType) - 1].gameObject;
        else
            sprite = sprBuffs[(int)buffIconType - 1].gameObject;

        UIBuffDebuffIcon.sActiveIconInfo iconInfo = new UIBuffDebuffIcon.sActiveIconInfo();
        iconInfo.Set(buffIconType, sprite, duration, buffIconFlash);

        UIBuffDebuffIcon.sActiveIconInfo find = null;
        if (iconInfo.duration <= 0.0f)
        {
#if UNITY_EDITOR
            find = m_listVisibleIcon.Find(x => x.buffIconType == buffIconType);
            if (find != null)
            {
                m_listVisibleIcon.Insert(0, iconInfo);
                m_listVisibleIcon.Remove(find);
            }
            else
            {
                m_listVisibleIcon.Add(iconInfo);
            }

            ShowActiveIcon(iconInfo);
#else
            return null;
#endif
        }
        else
        {
            find = m_listVisibleIcon.Find(x => x.buffIconType == buffIconType);
            if (find != null)
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
        }

        Reposition();
        return iconInfo;
    }

    public void ShowActiveIcon(UIBuffDebuffIcon.sActiveIconInfo activeIcon)
    {
        activeIcon.sprite.SetActive(true);

        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SpriteRenderer sprRenderer = activeIcon.sprite.GetComponent<SpriteRenderer>();
        sprRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void HideActiveIcon(UIBuffDebuffIcon.sActiveIconInfo activeIcon)
    {
		if(activeIcon == null || activeIcon.sprite == null)
		{
			return;
		}

		activeIcon.sprite.SetActive(false);
        m_listVisibleIcon.Remove(activeIcon);

        Reposition();
    }

    private IEnumerator Hide(UIBuffDebuffIcon.sActiveIconInfo activeIcon)
    {
        ShowActiveIcon(activeIcon);

        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        SpriteRenderer sprRenderer = activeIcon.sprite.GetComponent<SpriteRenderer>();
        sprRenderer.color = color;
        bool add = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (activeIcon.checkDuration < activeIcon.duration)
        {
            if (activeIcon.buffIconFlash)
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

                        sprRenderer.color = color;
                    }
                    else
                    {
                        color.a += Time.fixedDeltaTime / GameInfo.Instance.BattleConfig.BuffDebuffIconFlashDuration;
                        if (color.a >= 1.0f)
                        {
                            color.a = 1.0f;
                            add = false;
                        }

                        sprRenderer.color = color;
                    }
                }
                else
                {
                    sprRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }

            activeIcon.checkDuration += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        HideActiveIcon(activeIcon);
    }

    public void End()
    {
        StopAllCoroutines();

        for (int i = 0; i < m_listVisibleIcon.Count; i++)
            m_listVisibleIcon[i].sprite.SetActive(false);
        m_listVisibleIcon.Clear();

        Parent = null;
    }

    private void Update()
    {
		if (Parent == null || Camera.main == null || !Camera.main.enabled || transform == null)
		{
			return;
		}

        if (Parent.curHp <= 0.0f)
        {
            End();
            return;
        }

        Vector3 pos = Vector3.zero;
        Transform dummy = Parent.aniEvent != null ? Parent.aniEvent.GetBoneByName("hpbar") : null;
        if (dummy)
        {
            if (Parent.aniEvent.IsLyingAni())
                pos = dummy.transform.position + Parent.addIconLyingPos;
            else
                pos = dummy.transform.position + Parent.addIconPos;
        }
        else
        {
            pos = Parent.transform.position;

            float height = 0.0f;

			if (Parent.MainCollider)
			{
				if (Parent.MainCollider.direction == (int)eAxis.Y)
					height = Parent.MainCollider.height;
				else
					height = Parent.MainCollider.radius * 2.0f;
			}
			else
			{
				height = Parent.GetHeadPos(1.0f).y;
			}

            if (Parent.aniEvent && Parent.aniEvent.IsLyingAni() == false)
                pos.y += height;
            else
                pos.y += height * 0.5f;
        }

        transform.position = pos;
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}