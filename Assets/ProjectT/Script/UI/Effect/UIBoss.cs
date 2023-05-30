
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIBoss : MonoBehaviour
{
    public SpriteRenderer spr;

	public Enemy Parent { get; private set; } = null;

    private bool mAttached = false;


    public void Init(Unit parent)
    {
        Parent = parent as Enemy;
        transform.position = Vector3.zero;
        mAttached = false;

        transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        gameObject.SetActive(true);
    }

	public void Release()
	{
		Parent = null;

		if (spr)
		{
			spr.gameObject.SetActive(false);
		}
	}

    private void Update()
    {
		if(spr == null)
		{
			return;
		}

		if (Parent == null)
		{
			if (spr)
			{
				spr.gameObject.SetActive(false);
			}

			return;
		}

        if (!Parent.isVisible || !Parent.IsShowMesh || Director.IsPlaying)
        {
			if (spr)
			{
				spr.gameObject.SetActive(false);
			}
        }
        else if(Camera.main && Camera.main.transform)
		{
            if (spr && !spr.gameObject.activeSelf)
            {
                spr.gameObject.SetActive(true);
            }

            if (!mAttached)
            {
                Vector3 pos = Vector3.zero;
                Transform dummy = Parent.aniEvent != null ? Parent.aniEvent.GetBoneByName("hpbar") : null;
                if (dummy)
                {
                    if (Parent.aniEvent.IsLyingAni())
                    {
                        pos = dummy.transform.position + Parent.addHpLyingPos;
                    }
                    else
                    {
                        pos = dummy.transform.position + Parent.addHpPos;
                    }
                }
                else
                {
                    pos = Parent.transform.position;

                    float height = 0.0f;
                    if (Parent.MainCollider.direction == (int)eAxis.Y)
                    {
                        height = Parent.MainCollider.height;
                    }
                    else
                    {
                        height = Parent.MainCollider.radius * 2.0f;
                    }

                    if (Parent.aniEvent.IsLyingAni() == false)
                    {
                        pos.y += height;
                    }
                    else
                    {
                        pos.y += height * 0.5f;
                    }
                }

                transform.position = pos;
            }

			transform.LookAt(Camera.main.transform.position, -Vector3.up);
        }
    }
}