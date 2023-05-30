
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIHpBar : MonoBehaviour
{
    public SpriteRenderer sprBg;
    public SpriteRenderer sprHp;
    public SpriteRenderer sprShieldBg;
    public SpriteRenderer sprShield;
	//public UIHPGaugeUnit gauge;

	public Enemy Parent { get; private set; } = null;

    private Color m_color;
    private bool m_attached = false;


    public void Init(Unit parent)
    {
        /*
        if (!World.Instance.UIPlay.gameObject.activeSelf && !GameSupport.IsInGameTutorial())
        {
            return;
        }
        */

        Parent = parent as Enemy;
        transform.position = Vector3.zero;
        m_attached = false;

        transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);

        SetSprHp();
        Show(true);
    }

	public void Release()
	{
		Parent = null;
	}

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void AddHp(int addHp)
    {
        if (Parent.curHp >= Parent.maxHp)
            return;

        if (!Parent.isVisible)
            return;

        SetSprHp();
    }

    public void SubHp(int subHp)
    {
        if(Parent == null)
        {
            return;
        }

        if (Parent.curHp <= 0.0f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!Parent.isVisible)
        {
            return;
        }

        SetSprHp();
    }

    public void UpdateShield()
    {
        if (Parent == null || /*Parent.curShield <= 0.0f ||*/ !Parent.isVisible)
        {
            return;
        }

        SetSprHp();
    }

    private void SetSprHp()
    {
        Vector3 scale = sprHp.transform.localScale;
        scale.x = Parent.curHp / Parent.maxHp;
        sprHp.transform.localScale = scale;

        if(Parent.maxShield > 0.0f)
        {
            sprShieldBg.gameObject.SetActive(true);
            sprShield.gameObject.SetActive(true);

            scale = sprShieldBg.transform.localScale;
            scale.x = Parent.curShield / Parent.maxShield;
            sprShield.transform.localScale = scale;
        }
        else
        {
            sprShieldBg.gameObject.SetActive(false);
            sprShield.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Parent == null)
        {
            return;
        }

        if (!Parent.isVisible)
        {
            sprBg.gameObject.SetActive(false);
            sprHp.gameObject.SetActive(false);

            sprShieldBg.gameObject.SetActive(false);
            sprShield.gameObject.SetActive(false);
        }
        else
        {
            if (!sprHp.gameObject.activeSelf)
            {
                sprBg.gameObject.SetActive(true);
                sprHp.gameObject.SetActive(true);

                SetSprHp();
            }

            if (!m_attached)
            {
                Vector3 pos = Vector3.zero;
                Transform dummy = Parent.aniEvent ? Parent.aniEvent.GetBoneByName("hpbar") : null;
                if (dummy)
                {
                    if (Parent.aniEvent.IsLyingAni())
                        pos = dummy.transform.position + Parent.addHpLyingPos;
                    else
                        pos = dummy.transform.position + Parent.addHpPos;
                }
                else
                {
                    pos = Parent.transform.position;

                    float height = 0.0f;
                    if (Parent.MainCollider.direction == (int)eAxis.Y)
                        height = Parent.MainCollider.height;
                    else
                        height = Parent.MainCollider.radius * 2.0f;

                    if (Parent.aniEvent && Parent.aniEvent.IsLyingAni())
                    {
                        pos.y += height;
                        pos += Parent.addHpLyingPos;
                    }
                    else
                    {
                        pos.y += height * 0.5f;
                        pos += Parent.addHpPos;
                    }
                }

                transform.position = pos;
            }

            transform.LookAt(Camera.main.transform.position, -Vector3.up);
        }
    }
}