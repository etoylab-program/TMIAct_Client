using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CostumeBody : MonoBehaviour
{
    [System.Serializable]
    public class CostumeResMaterial
    {
        public SkinnedMeshRenderer kMesh;
        public int kMaterialIndex;
        public List<Material> kMaterialList;
    }

    public List<CostumeResMaterial> kCostumeResMaterialList;

    public List<GameObject> kAttachList = new List<GameObject>();
    public GameObject kBaseHair;
    public List<DynamicBoneCollider> kColliderList;
    public Material MtrlEyeNormal;
    public Material MtrlEyeSurprise;

    [Header("[Other Parts]")]
    public GameObject BaseOtherParts   = null;
    public GameObject AttachOtherParts = null;

    [Header("Parts Color Texture")]
    public Texture TexPartsColor;

    public GameObject CostumeHair { get { return _costumehair; } }
    public GameTable.Costume.Param TableData { get; private set; } = null;

    private GameObject _costumehair;

    private Renderer    mFaceMesh       = null;
    private Material[]  mFaceMtrls      = null;
    private Material[]  mNewFaceMtrls   = null;
    private int         mEyeMtrlIndex   = 0;
    private bool        mSettingCostume = false;

	public void Start() {
        if ( mSettingCostume || kCostumeResMaterialList == null || kCostumeResMaterialList.Count <= 0 ) {
            return;
        }

		string usePartsColorStr = "_UsePartsColor";
		string partsColorMaskStr = "_PartsColorMask";

		for ( int i = 0; i < kCostumeResMaterialList.Count; i++ ) {
            if ( kCostumeResMaterialList[i] == null || kCostumeResMaterialList[i].kMesh == null || kCostumeResMaterialList[i].kMesh.materials == null ) {
                continue;
			}

			for ( int m = 0; m < kCostumeResMaterialList[i].kMesh.materials.Length; m++ ) {
				Material mtrl = kCostumeResMaterialList[i].kMesh.materials[m];
                if ( mtrl == null ) {
                    continue;
                }

                if ( !mtrl.HasProperty( usePartsColorStr ) ) {
                    continue;
                }

				float bUse = mtrl.GetFloat( usePartsColorStr );
                if ( bUse < 1 ) {
                    continue;
                }

				mtrl.SetTexture( partsColorMaskStr, null );
			}
		}
	}

	public void InitCostumeBody(CharData chardata)
    {
        SetCostumeBody(chardata.EquipCostumeID, chardata.CostumeColor, chardata.CostumeStateFlag, chardata.DyeingData);
    }

    public void InitCostumeBody(FigureData data)
    {
        if (data.saveData.Version < FigureUnit.VERSION)
        {
            SetCostumeBody(data.tableData.ContentsIndex, data.CostumeColor, data.CostumeStateFlag, null);
        }
        else
        {
            SetCostumeBody(data.tableData.ContentsIndex, data.CostumeColor, data.CostumeStateFlag, data.saveData.UseDye,
                           data.saveData.RPartsColor, data.saveData.GPartsColor, data.saveData.BPartsColor);
        }

        for(int i = 0; i < kAttachList.Count; i++)
        {
            Utility.SetLayer(kAttachList[i].gameObject, (int)eLayer.Figure, true);
        }

        if(kBaseHair)
        {
            Utility.SetLayer(kBaseHair, (int)eLayer.Figure, true);
        }

        if(_costumehair)
        {
            Utility.SetLayer(_costumehair, (int)eLayer.Figure, true);
        }

        if(BaseOtherParts)
        {
            Utility.SetLayer(BaseOtherParts, (int)eLayer.Figure, true);
        }

        if (AttachOtherParts)
        {
            Utility.SetLayer(AttachOtherParts, (int)eLayer.Figure, true);
        }

        mFaceMtrls = null;
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].gameObject.name.Contains("face") || renderers[i].gameObject.name.Contains("Face"))
            {
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    if (renderers[i].materials[j].name.Contains("eye"))
                    {
                        mFaceMesh = renderers[i];
                        mFaceMtrls = renderers[i].materials;
                        mEyeMtrlIndex = j;

                        break;
                    }
                }

                if(mFaceMtrls != null)
                {
                    break;
                }
            }
        }

        if(mFaceMtrls != null)
        {
            mNewFaceMtrls = new Material[mFaceMtrls.Length];

            for(int i = 0; i < mNewFaceMtrls.Length; i++)
            {
                mNewFaceMtrls[i] = mFaceMtrls[i];
            }
        }
    }

    private void SetCostumeDyeing(int costumeId, bool bDyeing, ref Material mtrl, DyeingData dyeingData)
    {
        string usePartsColorStr = "_UsePartsColor";
        if (!mtrl.HasProperty(usePartsColorStr))
        {
            return;
        }
        
        float bUse = mtrl.GetFloat(usePartsColorStr);
        if (bUse < 1)
        {
            return;
        }
        
        string partsColorMaskStr = "_PartsColorMask";
        if (!mtrl.HasProperty(partsColorMaskStr))
        {
            return;
        }
        
        mtrl.SetTexture(partsColorMaskStr, bDyeing ? TexPartsColor : null);

        if (!bDyeing)
        {
            return;
        }
        
        Texture texMask = mtrl.GetTexture(partsColorMaskStr);
        if (texMask == null)
        {
            return;
        }
        
        if (dyeingData == null)
        {
            MyDyeingData data = GameInfo.Instance.GetDyeingData(costumeId);
            if (data != null)
            {
                dyeingData = data.DyeingData;
            }
        }

        bool isInputTableColor = true;
        if (dyeingData != null)
        {
            isInputTableColor = dyeingData.IsFirstDyeing;
        }

        List<string> hasProperty = new List<string>() { "_PartsColorR", "_PartsColorG", "_PartsColorB" };
        for (int c = 0; c < hasProperty.Count; c++)
        {
            if (!mtrl.HasProperty(hasProperty[c]))
            {
                continue;
            }

            Color color = isInputTableColor ?
                FindCostumeBaseColor(costumeId, c) : dyeingData.PartsColorList[c];

            mtrl.SetColor(hasProperty[c], color);
        }
    }

    private void SetCostumeDyeing(int costumeId, bool bDyeing, ref Material mtrl, bool useDye, Color RPartsColor, Color GPartsColor, Color BPartsColor)
    {
        string usePartsColorStr = "_UsePartsColor";
        if (!mtrl.HasProperty(usePartsColorStr))
        {
            return;
        }

        float bUse = mtrl.GetFloat(usePartsColorStr);
        if (bUse < 1)
        {
            return;
        }

        string partsColorMaskStr = "_PartsColorMask";
        if (!mtrl.HasProperty(partsColorMaskStr))
        {
            return;
        }

        mtrl.SetTexture(partsColorMaskStr, bDyeing ? TexPartsColor : null);

        if (!bDyeing)
        {
            return;
        }

        Texture texMask = mtrl.GetTexture(partsColorMaskStr);
        if (texMask == null)
        {
            return;
        }

        if (mtrl.HasProperty("_PartsColorR"))
        {
            mtrl.SetColor("_PartsColorR", RPartsColor);
        }

        if (mtrl.HasProperty("_PartsColorG"))
        {
            mtrl.SetColor("_PartsColorG", GPartsColor);
        }

        if (mtrl.HasProperty("_PartsColorB"))
        {
            mtrl.SetColor("_PartsColorB", BPartsColor);
        }
    }

    public Color FindCostumeBaseColor(int tableId, int index)
    {
        Color color = Color.black;
        
        GameTable.Costume.Param costume = GameInfo.Instance.GameTable.FindCostume(tableId);
		
        string[] split = System.Array.Empty<string>();
        char separator = ',';
        switch (index)
        {
            case 0:
				split = Utility.Split(costume.BaseColor1, separator); //costume.BaseColor1.Split(separator);
				break;
            case 1:
                split = Utility.Split(costume.BaseColor2, separator); //costume.BaseColor2.Split(separator);
				break;
            case 2:
                split = Utility.Split(costume.BaseColor3, separator); //costume.BaseColor3.Split(separator);
				break;
        }

        if (3 <= split.Length)
        {
            color = new Color32(System.Convert.ToByte(split[0]), System.Convert.ToByte(split[1]), System.Convert.ToByte(split[2]), System.Byte.MaxValue);
        }
        
        return color;
    }
    
    public void SetCostumeBody(int costumeid, int costumecolor, int costumestateflag, DyeingData dyeingData)
    {
        TableData = GameInfo.Instance.GameTable.FindCostume(x => x.ID == costumeid);
        if (TableData == null)
            return;
        
        mSettingCostume = true;
        
        bool bDyeing = TableData.ColorCnt <= costumecolor;
        
        //텍스쳐 교체
        if( kCostumeResMaterialList.Count != 0 )
        {
            for (int i = 0; i < kCostumeResMaterialList.Count; i++)
            {
                var data = kCostumeResMaterialList[i];
                
                Material[] newMats = new Material[data.kMesh.materials.Length];
                for (int j = 0; j < newMats.Length; j++)
                {
                    newMats[j] = data.kMesh.materials[j];
                    int q = data.kMesh.materials[j].renderQueue;

                    newMats[j].shader = data.kMesh.materials[j].shader;
                    newMats[j].renderQueue = q;
                }

                if (0 <= data.kMaterialIndex && data.kMesh.materials.Length > data.kMaterialIndex)
                {
                    Material firstMat = data.kMaterialList.FirstOrDefault();
                    if (costumecolor < data.kMaterialList.Count)
                    {
                        firstMat = data.kMaterialList[costumecolor];
                    }

                    if (firstMat != null)
                    {
                        newMats[data.kMaterialIndex].CopyPropertiesFromMaterial(firstMat);
                        int q = newMats[data.kMaterialIndex].renderQueue;

                        newMats[data.kMaterialIndex].shader = firstMat.shader;
                        newMats[data.kMaterialIndex].renderQueue = q;
                    }
                }
                
                for (int m = 0; m < newMats.Length; m++)
                {
                    SetCostumeDyeing(costumeid, bDyeing, ref newMats[m], dyeingData);
                }
                
                data.kMesh.materials = newMats;
            }
        }
        

        //안경 온오프
        for (int i = 0; i < kAttachList.Count; i++)
        {
            var meshRenderer = kAttachList[i].GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                Material material = new Material(meshRenderer.material.shader);
                material.CopyPropertiesFromMaterial(meshRenderer.material);
                SetCostumeDyeing(costumeid, bDyeing, ref material, dyeingData);
                meshRenderer.material = material;
            }
            
            if( GameSupport._IsOnBitIdx( (uint)costumestateflag, (int)(eCostumeStateFlag.CSF_ATTACH_1) ) )
                kAttachList[i].SetActive(false);
            else
                kAttachList[i].SetActive(true);
        }

        //헤어 교체
        bool bcostumehair = false;
        if (TableData.HairModel != string.Empty || TableData.HairModel != "")
            bcostumehair = true;

        if (bcostumehair)
        {
            if (_costumehair == null)
                _costumehair = AttachHair(TableData.HairModel);


            if (GameSupport._IsOnBitIdx((uint)costumestateflag, (int)(eCostumeStateFlag.CSF_HAIR)))
            {
                kBaseHair.SetActive(true);
                if (_costumehair != null)
                    _costumehair.SetActive(false);
            }
            else
            {
                kBaseHair.SetActive(false);
                if (_costumehair != null)
                    _costumehair.SetActive(true);
            }
        }
        
        // 다른 파츠(아스카 팔다리 등)
        if(AttachOtherParts)
        {
            if(!GameSupport._IsOnBitIdx((uint)costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2))
            {
                BaseOtherParts.SetActive(true);
                AttachOtherParts.SetActive(false);
            }
            else
            {
                BaseOtherParts.SetActive(false);
                AttachOtherParts.SetActive(true);
            }
        }
    }

    public void SetCostumeBody(int costumeid, int costumecolor, int costumestateflag, bool useDye, Color RPartsColor, Color GPartsColor, Color BPartsColor)
    {
        TableData = GameInfo.Instance.GameTable.FindCostume(x => x.ID == costumeid);
        if (TableData == null)
            return;

        mSettingCostume = true;

        bool bDyeing = TableData.ColorCnt <= costumecolor;

        //텍스쳐 교체
        if (kCostumeResMaterialList.Count != 0)
        {
            for (int i = 0; i < kCostumeResMaterialList.Count; i++)
            {
                var data = kCostumeResMaterialList[i];

                Material[] newMats = new Material[data.kMesh.materials.Length];
                for (int j = 0; j < newMats.Length; j++)
                {
                    newMats[j] = data.kMesh.materials[j];
                    int q = data.kMesh.materials[j].renderQueue;
                    
                    newMats[j].shader = data.kMesh.materials[j].shader;
                    newMats[j].renderQueue = q;
                    
                }

                if (0 <= data.kMaterialIndex && data.kMesh.materials.Length > data.kMaterialIndex)
                {
                    Material firstMat = data.kMaterialList.FirstOrDefault();
                    if (costumecolor < data.kMaterialList.Count)
                    {
                        firstMat = data.kMaterialList[costumecolor];
                    }

                    if (firstMat != null)
                    {
                        newMats[data.kMaterialIndex].CopyPropertiesFromMaterial(firstMat);
                        int q = newMats[data.kMaterialIndex].renderQueue;
                        
                        newMats[data.kMaterialIndex].shader = firstMat.shader;
                        newMats[data.kMaterialIndex].renderQueue = q;
                    }
                }

                for (int m = 0; m < newMats.Length; m++)
                {
                    SetCostumeDyeing(costumeid, bDyeing, ref newMats[m], useDye, RPartsColor, GPartsColor, BPartsColor);
                }

                data.kMesh.materials = newMats;
            }
        }


        //안경 온오프
        for (int i = 0; i < kAttachList.Count; i++)
        {
            var meshRenderer = kAttachList[i].GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                Material material = new Material(meshRenderer.material.shader);
                material.CopyPropertiesFromMaterial(meshRenderer.material);
                SetCostumeDyeing(costumeid, bDyeing, ref material, useDye, RPartsColor, GPartsColor, BPartsColor);
                meshRenderer.material = material;
            }
            
            if (GameSupport._IsOnBitIdx((uint)costumestateflag, (int)(eCostumeStateFlag.CSF_ATTACH_1)))
                kAttachList[i].SetActive(false);
            else
                kAttachList[i].SetActive(true);
        }

        //헤어 교체
        bool bcostumehair = false;
        if (TableData.HairModel != string.Empty || TableData.HairModel != "")
            bcostumehair = true;

        if (bcostumehair)
        {
            if (_costumehair == null)
                _costumehair = AttachHair(TableData.HairModel);


            if (GameSupport._IsOnBitIdx((uint)costumestateflag, (int)(eCostumeStateFlag.CSF_HAIR)))
            {
                kBaseHair.SetActive(true);
                if (_costumehair != null)
                    _costumehair.SetActive(false);
            }
            else
            {
                kBaseHair.SetActive(false);
                if (_costumehair != null)
                    _costumehair.SetActive(true);
            }
        }

        // 다른 파츠 (e.g. 아스카 팔다리 등)
        if (AttachOtherParts)
        {
            if (!GameSupport._IsOnBitIdx((uint)costumestateflag, (int)eCostumeStateFlag.CSF_ATTACH_2))
            {
                BaseOtherParts.SetActive(true);
                AttachOtherParts.SetActive(false);
            }
            else
            {
                BaseOtherParts.SetActive(false);
                AttachOtherParts.SetActive(true);
            }
        }
    }

    public GameObject AttachHair(string strmodel)
    {
        GameObject attachres = null;
        if (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos)
            attachres = ResourceMgr.Instance.LoadFromAssetBundle("item", "Item/" + strmodel + "_aos.prefab") as GameObject;
        else
            attachres = ResourceMgr.Instance.LoadFromAssetBundle("item", "Item/" + strmodel + ".prefab") as GameObject;


        if (attachres == null)
            return null;
        
        Unit unit = this.gameObject.transform.parent.gameObject.GetComponent<Unit>();

        GameObject GOAttach = GameObject.Instantiate(attachres);

        Transform tf = null;
        if (unit.aniEvent == null)
        {
            FigureUnit figureUnit = unit as FigureUnit;
            if(figureUnit)
            {
                tf = figureUnit.GetBoneByNameOrNull("Socket_head");
                if(tf == null)
                {
                    tf = figureUnit.GetBoneByNameOrNull("socket_head");
                }
            }
        }
        else
        {
            tf = unit.aniEvent.GetBoneByName("Socket_head");
            if(tf == null)
            {
                tf = unit.aniEvent.GetBoneByName("socket_head");
            }
        }

        GOAttach.transform.parent = tf;

        GOAttach.transform.localPosition = Vector3.zero;
        GOAttach.transform.localRotation = Quaternion.identity;
        GOAttach.transform.localScale = Vector3.one;

        var dbonelist = GOAttach.GetComponents<DynamicBone>();
        for (int i = 0; i < dbonelist.Length; i++)
        {
            if (dbonelist[i].m_Colliders.Count == kColliderList.Count)
            {
                for( int j = 0; j < dbonelist[i].m_Colliders.Count; j++ )
                {
                    dbonelist[i].m_Colliders[j] = kColliderList[j];
                }
            }
        }
        //다이나믹본 셋팅 구동
        return GOAttach;
    }

    public void SetNormalEye()
    {
        if(mFaceMtrls == null || MtrlEyeNormal == null || MtrlEyeSurprise == null)
        {
            return;
        }

        mNewFaceMtrls[mEyeMtrlIndex] = MtrlEyeNormal;
        mFaceMesh.materials = mNewFaceMtrls;
    }

    public void SetSurpriseEye()
    {
        if (mFaceMtrls == null || MtrlEyeNormal == null || MtrlEyeSurprise == null)
        {
            return;
        }

        mNewFaceMtrls[mEyeMtrlIndex] = MtrlEyeSurprise;
        mFaceMesh.materials = mNewFaceMtrls;
    }

    public bool HasHair()
    {
        if(_costumehair)
        {
            return true;
        }

        return false;
    }

    public bool HasAccessory()
    {
        if(kAttachList.Count > 0)
        {
            return true;
        }

        return false;
    }

    public bool HasOtherParts()
    {
        return AttachOtherParts != null;
    }

    public bool HasColor(int tableId, ref int colorCount)
    {
        GameTable.Costume.Param param = GameInfo.Instance.GameTable.FindCostume(x => x.ID == tableId);
        if (param == null)
        {
            return false;
        }

        colorCount = param.ColorCnt;

        if (colorCount == 1 && param.UseDyeing == 1)
        {
            return true;
        }
        
        if(colorCount > 1)
        {
            return true;
        }

        return false;
    }
}
