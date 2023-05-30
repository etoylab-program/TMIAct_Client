using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObject : MonoBehaviour {
    public enum eTYPE
    {
        COSTUME = 0,
        HAIR,
        FACE,
        BACK,
        WEAPON_R,
        WEAPON_L,
    }

    public string           kBoneName;
    public eTYPE            kSlotType;
    public bool             kInitTrans;
    public List<GameObject> kEffectList;
    public int              attachedEffId   = 0;
    public bool             bActiveEffect   = false;

    public bool AlwaysHideEffect { get; set; } = true;

    private List<SkinnedMeshRenderer>   m_listSkinnedMeshRenderer   = new List<SkinnedMeshRenderer>();
    private List<MeshRenderer>          m_listMeshRenderer          = new List<MeshRenderer>();


    private void Awake()
    {
        m_listSkinnedMeshRenderer.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
        for (int i = 0; i < m_listSkinnedMeshRenderer.Count; i++)
            m_listSkinnedMeshRenderer[i].allowOcclusionWhenDynamic = false;

        m_listMeshRenderer.AddRange(GetComponentsInChildren<MeshRenderer>());
        for (int i = 0; i < m_listMeshRenderer.Count; i++)
            m_listMeshRenderer[i].allowOcclusionWhenDynamic = false;

        if ( gameObject.layer != ( int )eLayer.Enemy ) {
            Utility.SetLayer( gameObject, ( int )eLayer.Player, true, ( int )eLayer.TransparentFX );
        }
    }

    //  해당 오브젝트에 자식으로 이펙트가 붙어있을시
    //  활성화/비활성화 처리
    public void ActiveEffect( bool b )
    {
        if (kEffectList == null || kEffectList.Count == 0)
            return;

        for (int i = 0; i < kEffectList.Count; i++)
        {
            if(kEffectList[i] != null)
                kEffectList[i].SetActive(b);
        }

        bActiveEffect = b;
    }

    //  이펙트 활성화(외부 Invoke용)
    public void ShowEffect()
    {
        ActiveEffect(true);
    }

    //  이펙트 비활성화(외부 Invoke용)
    public void HideEffect()
    {
        ActiveEffect(false);
    }

    public string GetModelName()
    {
        string strName = this.gameObject.name;
        strName = strName.Replace("(Clone)", "");
        strName = strName.Replace("prf_", "");

        return strName;
    }
}
