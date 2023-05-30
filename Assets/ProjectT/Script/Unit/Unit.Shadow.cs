
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using DynamicShadowProjector;
using taecg.tools.mobileFastShadow;
using DG.Tweening;


public abstract partial class Unit : MonoBehaviour
{
    protected ShadowTextureRenderer mShadowTexRenderer  = null;
    protected Projector             mShadowProjector    = null;
    protected DrawTargetObject      mDrawTargetObject   = null;


    public void ShowShadow(bool show)
    {
        if (mShadowTexRenderer == null)
        {
            return;
        }

        if (FSaveData.Instance.Graphic <= 0)
        {
            if (mShadowTexRenderer.gameObject.activeSelf)
            {
                mShadowTexRenderer.gameObject.SetActive(false);
            }

            return;
        }

        mShadowTexRenderer.gameObject.SetActive(show);
    }

    public void SetShadowCommandBufferDirty()
    {
        if ( mDrawTargetObject == null ) {
            return;
        }

        mDrawTargetObject.SetCommandBufferDirty();
    }

    public void SetShadowPosByLightInPrivateRoom()
    {
        /*
        Vector3 v = (mShadowTexRenderer.transform.parent.up * 1.5f) - (mShadowTexRenderer.transform.parent.right * 0.5f);
        mShadowTexRenderer.transform.position = (mShadowTexRenderer.transform.parent.position + v);
        */

        if ( mShadowTexRenderer == null || mDrawTargetObject == null ) {
            return;
		}

        float fixedValue = 2.5f;

        if (FigureRoomScene.Instance.LightRotate == Vector3.zero)
        {
            Vector3 v = (mShadowTexRenderer.transform.parent.up * fixedValue) - (mShadowTexRenderer.transform.parent.right * 0.5f);
            mShadowTexRenderer.transform.position = (mShadowTexRenderer.transform.parent.position + v);
        }
        else
        {
            mShadowTexRenderer.transform.rotation = Quaternion.Euler(FigureRoomScene.Instance.LightRotate);
            mShadowTexRenderer.transform.position = -mShadowTexRenderer.transform.forward * fixedValue;

            mDrawTargetObject.SetOriginalRotation(FigureRoomScene.Instance.LightRotate);
        }
    }

    public void SetShadowPosByLight()
    {
		if ( AppMgr.Instance.SceneType != AppMgr.eSceneType.Stage || World.Instance.LightRotate == Vector3.zero || 
            mShadowTexRenderer == null || mDrawTargetObject == null ) {
			return;
		}

		mShadowTexRenderer.transform.rotation = Quaternion.Euler(World.Instance.LightRotate);
        mShadowTexRenderer.transform.position = -mShadowTexRenderer.transform.forward * 5.0f;

        mDrawTargetObject.SetOriginalRotation(World.Instance.LightRotate);
    }

	public void RemoveShadow()
	{
		if(mShadowTexRenderer == null)
		{
			mShadowTexRenderer = GetComponentInChildren<ShadowTextureRenderer>(true);
			if(mShadowTexRenderer)
			{
				DestroyImmediate(mShadowTexRenderer.gameObject);
			}
		}
		else
		{
			DestroyImmediate(mShadowTexRenderer.gameObject);
		}
	}

    protected virtual void SetShadow()
    {
		mShadowTexRenderer = GetComponentInChildren<ShadowTextureRenderer>(true);
		if(mShadowTexRenderer == null)
		{
			return;
		}

		mShadowTexRenderer.gameObject.SetActive(true);

		mShadowProjector = mShadowTexRenderer.GetComponent<Projector>();
		if (mShadowProjector)
		{
			mShadowProjector.ignoreLayers = (1 << (int)eLayer.IgnoreRaycast) |
											(1 << (int)eLayer.Player) |
											(1 << (int)eLayer.Enemy) |
											(1 << (int)eLayer.EnemyGate) |
											(1 << (int)eLayer.EnvObject) | 
                                            (1 << (int)eLayer.Director);

			if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
				mShadowProjector.ignoreLayers |= ( 1 << (int)eLayer.RenderTarget );
			}

			mShadowProjector.material = (Material)ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/Material/ShadowProjector.mat"); //(Material)Resources.Load("ShadowProjector", typeof(Material));
		}

		mDrawTargetObject = mShadowTexRenderer.GetComponent<DrawTargetObject>();
		if (mDrawTargetObject)
		{
			SetShadowPosByLight();
		}

		ShowShadow(true);
    }
}
