
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using UnityEngine;
using DynamicShadowProjector;


public class FigureUnit : Unit
{
    public enum eFigureType
    {
        Human = 0,
        Animal,
        Weapon,
    }

    public enum eEditableBoneType
    {
        None = 0,

        Pelvis,

        ThighLeft,
        KneeLeft,
        AnkleLeft,

        ThighRight,
        KneeRight,
        AnkleRight,
        
        Spine,
        Neck,

        ShoulderLeft,
        ElbowLeft,
        WristLeft,

        ShoulderRight,
        ElbowRight,
        WristRight,

        R_ThumbUpper,
        R_Thumb,

        R_IndexFingerUpper,
        R_IndexFinger,

        R_MiddleFingerUpper,
        R_MiddleFinger,

        R_RingFingerUpper,
        R_RingFinger,

        R_LittleFingerUpper,
        R_LittleFinger,

        L_ThumbUpper,
        L_Thumb,

        L_IndexFingerUpper,
        L_IndexFinger,

        L_MiddleFingerUpper,
        L_MiddleFinger,

        L_RingFingerUpper,
        L_RingFinger,

        L_LittleFingerUpper,
        L_LittleFinger,

        //L_Breast,
        //R_Breast,
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct sBoneSaveData
    {
        public float RotX;
        public float RotY;
        public float RotZ;

        public float Scale;


        public void Set(Vector3 rot, float scale)
        {
            RotX = rot.x;
            RotY = rot.y;
            RotZ = rot.z;

            Scale = scale;
        }

        public void SaveToBytes(BinaryWriter bw)
        {
            short value = (short)(RotX * 10.0f);
            bw.Write(value);

            value = (short)(RotY * 10.0f);
            bw.Write(value);

            value = (short)(RotZ * 10.0f);
            bw.Write(value);

            value = (short)(Scale * 10.0f);
            bw.Write(value);
        }

        public void LoadFromBytes(BinaryReader br)
        {
            short value = br.ReadInt16();
            RotX = (float)value / 10.0f;

            value = br.ReadInt16();
            RotY = (float)value / 10.0f;

            value = br.ReadInt16();
            RotZ = (float)value / 10.0f;

            value = br.ReadInt16();
            Scale = (float)value / 10.0f;
        }
    }

    public class sSaveData
    {
        public int                  Version;
        public string               Path;
        public Vector3              Pos;
        public Vector3              Rot;
        public List<sBoneSaveData>  ListBoneSaveData = new List<sBoneSaveData>();
        public bool                 UseDye;
        public Color                RPartsColor;
        public Color                GPartsColor;
        public Color                BPartsColor;


        public sSaveData()
        {
            Clear();
            ColorClear();
        }

        public sSaveData(sSaveData data)
        {
            if (data == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(data.Path))
            {
                Path = (string)data.Path.Clone();
            }

            Pos = data.Pos;
            Rot = data.Rot;

            ListBoneSaveData.Clear();
            for (int i = 0; i < data.ListBoneSaveData.Count; i++)
            {
                ListBoneSaveData.Add(data.ListBoneSaveData[i]);
            }

            data.UseDye = UseDye;
            RPartsColor = data.RPartsColor;
            GPartsColor = data.GPartsColor;
            BPartsColor = data.BPartsColor;
        }

        public void Clear()
        {
            Version = VERSION;

            Path = null;
            Pos = Vector3.zero;
            Rot = Vector3.zero;

            ListBoneSaveData.Clear();
        }
        
        public void ColorClear()
        {
            UseDye = false;
            RPartsColor = Color.white;
            GPartsColor = Color.white;
            BPartsColor = Color.white;
        }

        public byte[] ToBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            Version = VERSION;
            bw.Write(VERSION);

            bw.Write(Pos.x);
            bw.Write(Pos.y);
            bw.Write(Pos.z);

            if (VERSION < 20)
            {
                bw.Write(Rot.x);
                bw.Write(Rot.y);
                bw.Write(Rot.z);
            }
            else
            {
                short value = (short)(Rot.x * 10.0f);
                bw.Write(value);

                value = (short)(Rot.y * 10.0f);
                bw.Write(value);

                value = (short)(Rot.z * 10.0f);
                bw.Write(value);
            }

            int count = ListBoneSaveData.Count;
            bw.Write(count);

            if (VERSION < 20)
            {
                byte[] bytes = null;
                for (int i = 0; i < count; i++)
                {
                    int size = Marshal.SizeOf(ListBoneSaveData[i]);
                    bw.Write(size);

                    bytes = Utility.StructureToPtr(ListBoneSaveData[i]);
                    bw.Write(bytes);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    ListBoneSaveData[i].SaveToBytes(bw);
                }
            }

            if(VERSION >= 21)
            {
                bw.Write(UseDye);
                
                bw.Write((byte)(RPartsColor.r * 255.0f));
                bw.Write((byte)(RPartsColor.g * 255.0f));
                bw.Write((byte)(RPartsColor.b * 255.0f));

                bw.Write((byte)(GPartsColor.r * 255.0f));
                bw.Write((byte)(GPartsColor.g * 255.0f));
                bw.Write((byte)(GPartsColor.b * 255.0f));

                bw.Write((byte)(BPartsColor.r * 255.0f));
                bw.Write((byte)(BPartsColor.g * 255.0f));
                bw.Write((byte)(BPartsColor.b * 255.0f));
            }

            bw.Close();
            return ms.ToArray();

            //return Compress(ms.ToArray());
        }

        /*
        public byte[] Compress(byte[] bytes)
        {
            byte[] compressedBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
                {
                    ds.Write(bytes, 0, bytes.Length);
                }

                compressedBytes = ms.ToArray();
            }

            return compressedBytes;
        }
        */

        public void FromBytes(byte[] bytes)
        {
            //bytes = Decompress(bytes);

            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader br = new BinaryReader(ms);

            Version = br.ReadInt32();

            Pos.x = br.ReadSingle();
            Pos.y = br.ReadSingle();
            Pos.z = br.ReadSingle();

            if (Version < 20)
            {
                Rot.x = br.ReadSingle();
                Rot.y = br.ReadSingle();
                Rot.z = br.ReadSingle();
            }
            else
            {
                short value = br.ReadInt16();
                Rot.x = (float)value / 10.0f;

                value = br.ReadInt16();
                Rot.y = (float)value / 10.0f;

                value = br.ReadInt16();
                Rot.z = (float)value / 10.0f;
            }

            ListBoneSaveData.Clear();
            int coreDataCount = br.ReadInt32();

            if (Version < 20)
            {
                for (int i = 0; i < coreDataCount; i++)
                {
                    int size = br.ReadInt32();
                    byte[] readBytes = br.ReadBytes(size);

                    sBoneSaveData boneSaveData = Utility.PtrToStructure<sBoneSaveData>(readBytes);
                    ListBoneSaveData.Add(boneSaveData);
                }
            }
            else
            {
                for(int i = 0; i < coreDataCount; i++)
                {
                    sBoneSaveData boneSaveData = new sBoneSaveData();
                    boneSaveData.LoadFromBytes(br);

                    ListBoneSaveData.Add(boneSaveData);
                }
            }

            if(Version >= 22)
            {
                UseDye = br.ReadBoolean();
                    
                RPartsColor = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), byte.MaxValue);
                GPartsColor = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), byte.MaxValue);
                BPartsColor = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), byte.MaxValue);
            }

            br.Close();
        }

        /*
        public byte[] Decompress(byte[] bytes)
        {
            MemoryStream rms = new MemoryStream();

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    ds.CopyTo(rms);
                    ds.Close();
                }
            }

            byte[] decompressedBytes = rms.ToArray();
            rms.Dispose();

            return decompressedBytes;
        }
        */
    }

    public class sEditableBoneData
    {
        public sEditableBoneData    Parent;
        public Transform            Bone;
        public GameObject           SelectObj;
        public RotateAxis           RotAxis;
        public Transform            Target;
        public sBoneSaveData        BoneSaveData;

        public Vector3              OriginalPos;
        public Quaternion           OriginalRot;

        private BoxCollider         mBoxCol;

        private float               mScale;
        private float               mDefaultAlpha;


        public sEditableBoneData()
        {
        }

        public sEditableBoneData(Transform root, Transform bone, sEditableBoneData parent, sBoxColInfo boxColInfo)
        {
            Bone = bone;
            Parent = parent;

            mScale = 0.7f;
            if(bone.name.Contains("Finger")) {
                mScale = 0.3f;
            }

            mDefaultAlpha = 0.3f;

            // Bone select object
            SelectObj = ResourceMgr.Instance.CreateFromAssetBundle("unit", "Unit/FigureSelectBone.prefab");
            SelectObj.name = string.Format("{0}_{1}", "SelectObject", bone.name);
            SelectObj.transform.parent = bone;
            Utility.InitTransform(SelectObj, Vector3.zero, Quaternion.identity, Vector3.one * mScale);

            MeshRenderer[] renderers = SelectObj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.renderQueue = 3002;
                renderers[i].material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, mDefaultAlpha));
            }

            SelectObj.SetActive(false);

            // Rotate axis object
            RotAxis = ResourceMgr.Instance.CreateFromAssetBundle<RotateAxis>("unit", "Unit/FigureRotAxis.prefab");
            RotAxis.name = string.Format("{0}_{1}", "RotateAxis", bone.name);
            RotAxis.transform.parent = bone;
            Utility.InitTransform(RotAxis.gameObject, Vector3.zero, Quaternion.identity, Vector3.one * mScale);
            RotAxis.Show(false);

            renderers = RotAxis.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.renderQueue = 3002;
            }

            // Target object
            Transform[] transforms = SelectObj.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].name.CompareTo("movement") == 0)
                {
                    Target = transforms[i];
                    Target.name = string.Format("{0}_{1}", "Target", bone.name);
                    Target.SetParent(root);

                    break;
                }
            }

            BoneSaveData.Set(Bone.rotation.eulerAngles, 1.0f);

            OriginalPos = Bone.position;
            OriginalRot = Bone.rotation;

            BoxCollider find = Bone.gameObject.GetComponent<BoxCollider>();
            if(find)
            {
                DestroyImmediate(find);
            }

            if (boxColInfo != null)
            {
                mBoxCol = Bone.gameObject.AddComponent<BoxCollider>();
                mBoxCol.center = boxColInfo.Center;
                mBoxCol.size = boxColInfo.Size;
            }
        }

        public void HideSelectObjects()
        {
            SelectObj.SetActive(false);
            RotAxis.gameObject.SetActive(false);
        }

        public void Select()
        {
            SelectObj.SetActive(true);
            SelectObj.transform.localScale = Vector3.one * mScale * 1.5f;

            Renderer[] renderers = SelectObj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, mDefaultAlpha * 0.5f));
            }

            RotAxis.Show(false);
            Target.SetPositionAndRotation(Bone.position, Bone.rotation);
        }

        public void Unselect()
        {
            SelectObj.SetActive(true);
            SelectObj.transform.localScale = Vector3.one * mScale;

            Renderer[] renderers = SelectObj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, mDefaultAlpha));
            }

            RotAxis.Show(false);
        }

        public void SelectRotAxis()
        {
            SelectObj.SetActive(false);
            RotAxis.Show(true);
        }

        public void UnselectRotAxis()
        {
            SelectObj.SetActive(true);
            RotAxis.Show(false);
        }

        public void UpdateSelectObj()
        {
            SelectObj.transform.LookAt(FigureRoomScene.Instance.RoomCamera.transform.position, -Vector3.up);
        }

        public void Reset()
        {
            Bone.rotation = OriginalRot;
            BoneSaveData.Set(OriginalRot.eulerAngles, 1.0f);

            Utility.InitTransform(RotAxis.gameObject);
        }
    }

    public class sBoxColInfo
    {
        public Vector3 Center;
        public Vector3 Size;


        public sBoxColInfo(Vector3 center, Vector3 size, float scale)
        {
            Center = center;
            Size = size * scale;
        }
    }

    public class sDefaultHumanBoxCols
    {
        public sBoxColInfo ThighLeft;
        public sBoxColInfo KneeLeft;
        public sBoxColInfo AnkleLeft;

        public sBoxColInfo ThighRight;
        public sBoxColInfo KneeRight;
        public sBoxColInfo AnkleRight;

        public sBoxColInfo Spine;
        public sBoxColInfo Neck;
        public sBoxColInfo Head;

        public sBoxColInfo ShoulderLeft;
        public sBoxColInfo ElbowLeft;
        public sBoxColInfo WristLeft;

        public sBoxColInfo ShoulderRight;
        public sBoxColInfo ElbowRight;
        public sBoxColInfo WristRight;


        public sDefaultHumanBoxCols(float scale)
        {
            ThighLeft = new sBoxColInfo(new Vector3(-0.1f, 0.0f, 0.02f), new Vector3(0.32f, 0.22f, 0.16f), scale);
            KneeLeft = new sBoxColInfo(new Vector3(-0.04f, -0.02f, 0.0f), new Vector3(0.53f, 0.2f, 0.16f), scale);
            AnkleLeft = new sBoxColInfo(new Vector3(0.0f, 0.02f, 0.0f), new Vector3(0.35f, 0.2f, 0.1f), scale);

            ThighRight = new sBoxColInfo(new Vector3(-0.1f, 0.0f, 0.02f), new Vector3(0.32f, 0.22f, 0.16f), scale);
            KneeRight = new sBoxColInfo(new Vector3(-0.04f, -0.02f, 0.0f), new Vector3(0.53f, 0.2f, 0.16f), scale);
            AnkleRight = new sBoxColInfo(new Vector3(0.0f, 0.02f, 0.0f), new Vector3(0.35f, 0.2f, 0.1f), scale);

            Spine = new sBoxColInfo(new Vector3(0.05f, 0.0f, 0.0f), new Vector3(0.48f, 0.36f, 0.25f), scale);
            Neck = new sBoxColInfo(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.1f, 0.1f, 0.1f), scale);
            Head = new sBoxColInfo(new Vector3(-0.08f, 0.0f, 0.0f), new Vector3(0.2f, 0.2f, 0.2f), scale);

            ShoulderLeft = new sBoxColInfo(new Vector3(-0.03f, 0.0f, 0.0f), new Vector3(0.23f, 0.1f, 0.1f), scale);
            ElbowLeft = new sBoxColInfo(new Vector3(-0.04f, 0.0f, 0.0f), new Vector3(0.3f, 0.1f, 0.1f), scale);
            WristLeft = new sBoxColInfo(new Vector3(-0.05f, 0.0f, 0.0f), new Vector3(0.18f, 0.1f, 0.1f), scale);

            ShoulderRight = new sBoxColInfo(new Vector3(-0.03f, 0.0f, 0.0f), new Vector3(0.23f, 0.1f, 0.1f), scale);
            ElbowRight = new sBoxColInfo(new Vector3(-0.04f, 0.0f, 0.0f), new Vector3(0.3f, 0.1f, 0.1f), scale);
            WristRight = new sBoxColInfo(new Vector3(-0.05f, 0.0f, 0.0f), new Vector3(0.18f, 0.1f, 0.1f), scale);
        }
    }

    public class sMtrlInfo
    {
        public Material Mtrl;
        public Color    OriginalColor           = Color.white;
        public Color    Original1stShadeColor   = Color.white;
        public Color    Original2ndShadeColor   = Color.white;
    }


    public static int VERSION = 22; // 1.1 : 손가락 본 추가
                                    // 2.0 : 데이터 압축 (본 회전 값을 float -> short로 수정)
                                    // 2.1 : 염색 정보 추가
                                    // 2.2 : 염색 정보 수정

    [Header("[Figure Property]")]
    public eFigureType              Type                    = eFigureType.Human;
    public float                    ColliderScale           = 1.0f;
    public List<eEditableBoneType>  ListExceptEditableBone;
	public string					FigureChangeWeaponName	= "";

    public bool                         IsDyeing        { get; set; }           = false;
    public GameTable.RoomFigure.Param   Data            { get; private set; }   = null;
    public sSaveData                    SaveData        { get; private set; }   = null;
    public RotateAxis                   BodyRotateAxis  { get; private set; }   = null;

    private sDefaultHumanBoxCols                                mDefaultHumanBoxCols    = null;
    private CostumeBody                                         mCostumeBody            = null;
    private Animator                                            mAniFace                = null;
    private List<string>                                        mListFaceAniType        = new List<string>();
    private List<sMtrlInfo>                                     mListMtrl               = new List<sMtrlInfo>();
    private GameObject                                          mShyObject              = null;

    private List<Transform>                                     mListTransform          = new List<Transform>();
    private Dictionary<eEditableBoneType, sEditableBoneData>    mDicEditableBone        = new Dictionary<eEditableBoneType, sEditableBoneData>();
    private Transform                                           mPelvis                 = null;
    private bool                                                mChangeCostumeFigure   = false;

    
    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        if (Type != eFigureType.Weapon)
        {
            base.Init(tableId, type, faceAniControllerPath);
            SetKinematicRigidBody();

            Utility.SetLayer(gameObject, (int)eLayer.Figure, true, (int)eLayer.Pick);

            if (GetComponentInChildren<AniEvent>() == null)
            {
                mAniFace = GetComponentInChildren<Animator>();
                if (mAniFace)
                {
                    mListFaceAniType.Clear();
                    for (int i = 0; i < (int)eFaceAnimation.Max; i++)
                    {
                        mListFaceAniType.Add(((eFaceAnimation)i).ToString());
                    }

                    mAniFace.SetLayerWeight(1, 0.0f);

                    Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        for (int j = 0; j < renderers[i].materials.Length; j++)
                        {
                            if (renderers[i].gameObject.name.Equals("emotion_shy"))
                            {
                                renderers[i].materials[j].renderQueue = 3001;
                            }

                            //mListMtrl.Add(renderers[i].materials[j]);
                        }

                        if (renderers[i].gameObject.name.Equals("emotion_shy"))
                        {
                            mShyObject = renderers[i].gameObject;
                            mShyObject.SetActive(false);
                        }
                    }
                }

                UnitCollider[] unitColliders = GetComponentsInChildren<UnitCollider>();
                if (unitColliders != null && unitColliders.Length > 0)
                {
                    for (int i = 0; i < unitColliders.Length; i++)
                    {
                        DestroyImmediate(unitColliders[i]);
                    }
                }

                BoxCollider[] boxCols = GetComponentsInChildren<BoxCollider>();
                if (boxCols != null && boxCols.Length > 0)
                {
                    for (int i = 0; i < boxCols.Length; i++)
                    {
                        DestroyImmediate(boxCols[i]);
                    }
                }

                CapsuleCollider[] capsuleCols = GetComponentsInChildren<CapsuleCollider>();
                if (capsuleCols != null && capsuleCols.Length > 0)
                {
                    for (int i = 0; i < capsuleCols.Length; i++)
                    {
                        DestroyImmediate(capsuleCols[i]);
                    }
                }
            }

            mListTransform.Clear();
            mListTransform.AddRange(GetComponentsInChildren<Transform>());

            if (mDefaultHumanBoxCols == null)
            {
                mDefaultHumanBoxCols = new sDefaultHumanBoxCols(ColliderScale);
            }

            //==
            // 추가 순서 바꾸면 안됨
            AddEditableBone(eEditableBoneType.Pelvis,               "Bip001 Pelvis",        null,                   null);

            AddEditableBone(eEditableBoneType.ThighLeft,            "Bip001 L Thigh",       null,                   mDefaultHumanBoxCols.ThighLeft);
            AddEditableBone(eEditableBoneType.KneeLeft,             "Bip001 L Calf",        "Bip001 L Thigh",       mDefaultHumanBoxCols.KneeLeft);
            AddEditableBone(eEditableBoneType.AnkleLeft,            "Bip001 L Foot",        "Bip001 L Calf",        mDefaultHumanBoxCols.AnkleLeft);

            AddEditableBone(eEditableBoneType.ThighRight,           "Bip001 R Thigh",       null,                   mDefaultHumanBoxCols.ThighRight);
            AddEditableBone(eEditableBoneType.KneeRight,            "Bip001 R Calf",        "Bip001 R Thigh",       mDefaultHumanBoxCols.KneeRight);
            AddEditableBone(eEditableBoneType.AnkleRight,           "Bip001 R Foot",        "Bip001 R Calf",        mDefaultHumanBoxCols.AnkleRight);
            
            AddEditableBone(eEditableBoneType.Spine,                "Bip001 Spine1",        null,                   mDefaultHumanBoxCols.Spine);

            if (Type == eFigureType.Human) {
                AddEditableBone(eEditableBoneType.Neck,             "Bip001 Neck",          null,                   mDefaultHumanBoxCols.Neck);
            }
            else if (Type == eFigureType.Animal) {
            }

            AddEditableBone(eEditableBoneType.ShoulderLeft,         "Bip001 L UpperArm",    null,                   mDefaultHumanBoxCols.ShoulderLeft);
            AddEditableBone(eEditableBoneType.ElbowLeft,            "Bip001 L Forearm",     "Bip001 L UpperArm",    mDefaultHumanBoxCols.ElbowLeft);
            AddEditableBone(eEditableBoneType.WristLeft,            "Bip001 L Hand",        "Bip001 L Forearm",     mDefaultHumanBoxCols.WristLeft);

            AddEditableBone(eEditableBoneType.ShoulderRight,        "Bip001 R UpperArm",    null,                   mDefaultHumanBoxCols.ShoulderRight);
            AddEditableBone(eEditableBoneType.ElbowRight,           "Bip001 R Forearm",     "Bip001 R UpperArm",    mDefaultHumanBoxCols.ElbowRight);
            AddEditableBone(eEditableBoneType.WristRight,           "Bip001 R Hand",        "Bip001 R Forearm",     mDefaultHumanBoxCols.WristRight);

            // 손가락 추가 됨
            AddEditableBone(eEditableBoneType.R_ThumbUpper,         "Bip001 R Finger0",     "Bip001 R Hand",        null);
            AddEditableBone(eEditableBoneType.R_Thumb,              "Bip001 R Finger01",    "Bip001 R Finger0",     null);

            AddEditableBone(eEditableBoneType.R_IndexFingerUpper,   "Bip001 R Finger1",     "Bip001 R Hand",        null);
            AddEditableBone(eEditableBoneType.R_IndexFinger,        "Bip001 R Finger11",    "Bip001 R Finger1",     null);

            AddEditableBone(eEditableBoneType.R_MiddleFingerUpper,  "Bip001 R Finger2",     "Bip001 R Hand",        null);
            AddEditableBone(eEditableBoneType.R_MiddleFinger,       "Bip001 R Finger21",    "Bip001 R Finger2",     null);

            AddEditableBone(eEditableBoneType.R_RingFingerUpper,    "Bip001 R Finger3",     "Bip001 R Hand",        null);
            AddEditableBone(eEditableBoneType.R_RingFinger,         "Bip001 R Finger31",    "Bip001 R Finger3",     null);

            AddEditableBone(eEditableBoneType.R_LittleFingerUpper,  "Bip001 R Finger4",     "Bip001 R Hand",        null);
            AddEditableBone(eEditableBoneType.R_LittleFinger,       "Bip001 R Finger41",    "Bip001 R Finger4",     null);

            AddEditableBone(eEditableBoneType.L_ThumbUpper,         "Bip001 L Finger0",     "Bip001 L Hand",        null);
            AddEditableBone(eEditableBoneType.L_Thumb,              "Bip001 L Finger01",    "Bip001 L Finger0",     null);

            AddEditableBone(eEditableBoneType.L_IndexFingerUpper,   "Bip001 L Finger1",     "Bip001 L Hand",        null);
            AddEditableBone(eEditableBoneType.L_IndexFinger,        "Bip001 L Finger11",    "Bip001 L Finger1",     null);

            AddEditableBone(eEditableBoneType.L_MiddleFingerUpper,  "Bip001 L Finger2",     "Bip001 L Hand",        null);
            AddEditableBone(eEditableBoneType.L_MiddleFinger,       "Bip001 L Finger21",    "Bip001 L Finger2",     null);

            AddEditableBone(eEditableBoneType.L_RingFingerUpper,    "Bip001 L Finger3",     "Bip001 L Hand",        null);
            AddEditableBone(eEditableBoneType.L_RingFinger,         "Bip001 L Finger31",    "Bip001 L Finger3",     null);

            AddEditableBone(eEditableBoneType.L_LittleFingerUpper,  "Bip001 L Finger4",     "Bip001 L Hand",        null);
            AddEditableBone(eEditableBoneType.L_LittleFinger,       "Bip001 L Finger41",    "Bip001 L Finger4",     null);

            //AddEditableBone(eEditableBoneType.L_Breast,             "Bone005",              "Bip001 Spine1",        null);
            //AddEditableBone(eEditableBoneType.R_Breast,             "Bone006",              "Bip001 Spine1",        null);
            //==

            Transform trHead = GetBoneByNameOrNull("Bip001 Head");
            if(trHead)
            {
                BoxCollider find = trHead.gameObject.GetComponent<BoxCollider>();
                if(find)
                {
                    Destroy(find);
                }

                BoxCollider boxCol = trHead.gameObject.AddComponent<BoxCollider>();
                boxCol.center = mDefaultHumanBoxCols.Head.Center;
                boxCol.size = mDefaultHumanBoxCols.Head.Size;
            }

            CreateBodyRotateAxis();
        }
        else
        {
            SetData(tableId);
            Utility.SetLayer(gameObject, (int)eLayer.Figure, true, (int)eLayer.Pick);
        }
    }

    public override void SetData(int tableId)
    {
        if (tableId <= 0)
        {
            return;
        }

        m_tableId = tableId;

        Data = GameInfo.Instance.GameTable.FindRoomFigure(tableId);
        m_folder = Utility.GetFolderFromPath(Data.Model);
    }

    protected override void SetShadow()
    {
        mShadowTexRenderer = GetComponentInChildren<DynamicShadowProjector.ShadowTextureRenderer>(true);
        if (mShadowTexRenderer)
        {
            mShadowTexRenderer.gameObject.SetActive(true);

            mShadowProjector = mShadowTexRenderer.GetComponent<Projector>();
            if (mShadowProjector)
            {
                mShadowProjector.ignoreLayers = (1 << (int)eLayer.IgnoreRaycast) |
                                                (1 << (int)eLayer.Player) |
                                                (1 << (int)eLayer.Enemy) |
                                                (1 << (int)eLayer.EnemyGate) |
                                                (1 << (int)eLayer.EnvObject) |
                                                (1 << (int)eLayer.Figure);

                mShadowProjector.material = (Material)ResourceMgr.Instance.LoadFromAssetBundle("etc", "Etc/Material/ShadowProjector.mat"); //(Material)Resources.Load("ShadowProjector", typeof(Material));
            }

            mDrawTargetObject = mShadowTexRenderer.GetComponent<DrawTargetObject>();
            if (mDrawTargetObject)
            {
                SetShadowPosByLight();
            }
        }

        if (FSaveData.Instance.Graphic <= 0)
        {
            ShowShadow(false);
        }
        else
        {
            ShowShadow(true);
        }
    }

	public void InitCostume( FigureData data ) {
		mChangeCostumeFigure = false;

		mCostumeBody = GetComponentInChildren<CostumeBody>();
		if ( mCostumeBody == null ) {
			mListMtrl.Clear();
			Renderer[] renderers = GetComponentsInChildren<Renderer>( true );

			for ( int i = 0; i < renderers.Length; i++ ) {
				for ( int j = 0; j < renderers[i].materials.Length; j++ ) {
					if ( renderers[i].materials[j].name.Contains( "selectbone" ) ) {
						continue;
					}

					sMtrlInfo mtrlInfo = new sMtrlInfo();
					mtrlInfo.Mtrl = renderers[i].materials[j];

					if ( mtrlInfo.Mtrl.HasProperty( "_BaseColor" ) ) {
						mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_BaseColor" );
					}
					else if ( mtrlInfo.Mtrl.HasProperty( "_Color" ) ) {
						mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_Color" );
					}

					if ( mtrlInfo.Mtrl.HasProperty( "_1st_ShadeColor" ) ) {
						mtrlInfo.Original1stShadeColor = mtrlInfo.Mtrl.GetColor( "_1st_ShadeColor" );
					}

					if ( mtrlInfo.Mtrl.HasProperty( "_2nd_ShadeColor" ) ) {
						mtrlInfo.Original2ndShadeColor = mtrlInfo.Mtrl.GetColor( "_2nd_ShadeColor" );
					}

					mListMtrl.Add( mtrlInfo );
				}
			}

			return;
		}

		mCostumeBody.InitCostumeBody( data );
		GetMtrls();
	}

	public override void Activate()
    {
        gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetFaceAnimatorController()
    {
		string[] split = Utility.Split(name, '_'); //name.Split('_');
        if(split.Length <= 0)
        {
            return;
        }

        string path = string.Format("Unit/Character/{0}_L/{0}_H.controller", split[0]);
        mAniFace.runtimeAnimatorController = (RuntimeAnimatorController)ResourceMgr.Instance.LoadFromAssetBundle("unit", path);
    }

    public void PlayFigureFaceAni(eFaceAnimation faceAni, int layer, float weight)
    {
        if (mAniFace == null || faceAni == eFaceAnimation.None)
        {
            return;
        }

        if (faceAni == eFaceAnimation.FaceShy || faceAni == eFaceAnimation.MouthShy)
        {
            if (mShyObject)
            {
                mShyObject.SetActive(true);
            }

            if (mCostumeBody)
            {
                mCostumeBody.SetNormalEye();
            }
        }
        else if(mCostumeBody && (faceAni == eFaceAnimation.FaceSurprise || faceAni == eFaceAnimation.MouthSurprise || 
            faceAni == eFaceAnimation.FaceSurprise02 || faceAni == eFaceAnimation.MouthSurprise02))
        {
            if (mShyObject)
            {
                mShyObject.SetActive(false);
            }

            mCostumeBody.SetSurpriseEye();
        }
        else
        {
            if (mShyObject)
            {
                mShyObject.SetActive(false);
            }

            if (mCostumeBody)
            {
                mCostumeBody.SetNormalEye();
            }
        }

        mAniFace.SetLayerWeight(1, weight);
        mAniFace.Play(mListFaceAniType[(int)faceAni], layer);
        mAniFace.speed = 0.0f;
    }

	public void SetColor( float value ) {
		for( int i = 0; i < mListMtrl.Count; i++ ) {
            string propertyName = "_BaseColor";

			if( !mListMtrl[i].Mtrl.HasProperty( propertyName ) ) {
                propertyName = "_Color";

                if( !mListMtrl[i].Mtrl.HasProperty( propertyName ) ) {
                    continue;
                }
			}

			Color c = Color.white;
			if( value < 1.0f ) {
				c = mListMtrl[i].Mtrl.GetColor( propertyName );
				c.r = mListMtrl[i].OriginalColor.r * value;
				c.g = mListMtrl[i].OriginalColor.g * value;
				c.b = mListMtrl[i].OriginalColor.b * value;
			}
			else {
				c = mListMtrl[i].OriginalColor;
			}

			mListMtrl[i].Mtrl.SetColor( propertyName, c );
		}

        SetColor( "_1st_ShadeColor", value );
        SetColor( "_2nd_ShadeColor", value );
    }

    public void SetColor( string propertyName, float value ) {
        for( int i = 0; i < mListMtrl.Count; i++ ) {
            if( mListMtrl[i].Mtrl.HasProperty( propertyName ) ) {
                Color c = Color.white;
                Color original = Color.white;

                if ( propertyName == "_1st_ShadeColor" ) {
                    original = mListMtrl[i].Original1stShadeColor;
                }
                else if ( propertyName == "_2nd_ShadeColor" ) {
                    original = mListMtrl[i].Original2ndShadeColor;
                }

                if ( value < 1.0f ) {
                    c = mListMtrl[i].Mtrl.GetColor( propertyName );

                    c.r = original.r * value;
                    c.g = original.g * value;
                    c.b = original.b * value;
                }
                else {
                    c = original;
                }

                mListMtrl[i].Mtrl.SetColor( propertyName, c );
            }
        }
    }

    public void ResetFigure()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        foreach(KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            kv.Value.Reset();
        }
    }

    public void ShowAllSelectObject(bool show)
    {
        UnselectBodyRotateAxis();

        foreach (KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            kv.Value.SelectObj.SetActive(show);
            kv.Value.RotAxis.gameObject.SetActive(false);
        }
    }

    public void UnselectAll()
    {
        UnselectBodyRotateAxis();

        foreach (KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            kv.Value.Unselect();
        }
    }

    public Transform GetBoneByNameOrNull(string name)
    {
        if(mListTransform.Count <= 0)
        {
            return null;
        }

        return mListTransform.Find(x => x.name.CompareTo(name) == 0);
    }

    public sEditableBoneData GetSelectedBoneDataOrNull(eEditableBoneType selectedBoneType)
    {
        if (!mDicEditableBone.ContainsKey(selectedBoneType))
        {
            return null;
        }

        return mDicEditableBone[selectedBoneType];
    }

    public eEditableBoneType FindBoneTypeByName(string name)
    {
        foreach (KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            if (kv.Value.Bone.name.CompareTo(name) == 0)
            {
                return kv.Key;
            }
        }

        return eEditableBoneType.None;
    }

    public void SelectBodyRotateAxis()
    {
        BodyRotateAxis.Show(true);
    }

    public void UnselectBodyRotateAxis()
    {
        BodyRotateAxis.Show(false);
    }

    public bool Save()
    {
        if (SaveData == null)
        {
            SaveData = new sSaveData();
        }
        else
        {
            SaveData.Clear();
        }

        SaveData.Pos = transform.position;
        SaveData.Rot = transform.rotation.eulerAngles;

        foreach(KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            kv.Value.BoneSaveData.Set(kv.Value.Bone.rotation.eulerAngles, 1.0f);
            SaveData.ListBoneSaveData.Add(kv.Value.BoneSaveData);
        }
        
        if (mChangeCostumeFigure && mCostumeBody)
        {
            MyDyeingData dyeingData = GameInfo.Instance.GetDyeingData(mCostumeBody.TableData.ID);
            if (dyeingData != null && !dyeingData.DyeingData.IsFirstDyeing)
            {
                SaveData.UseDye = IsDyeing;

                SaveData.RPartsColor = dyeingData.DyeingData.PartsColorList[0];
                SaveData.GPartsColor = dyeingData.DyeingData.PartsColorList[1];
                SaveData.BPartsColor = dyeingData.DyeingData.PartsColorList[2];
            }
        }

        return true;
    }

    public bool SaveToFile(string path)
    {
        if (!Save())
        {
            return false;
        }

        if (File.Exists(path) == true)
        {
            File.Delete(path);
        }

        FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);

        bw.Write(transform.position.x);
        bw.Write(transform.position.y);
        bw.Write(transform.position.z);

        bw.Write(transform.rotation.eulerAngles.x);
        bw.Write(transform.rotation.eulerAngles.y);
        bw.Write(transform.rotation.eulerAngles.z);

        int count = SaveData.ListBoneSaveData.Count;
        bw.Write(count);

        byte[] bytes = null;
        for (int i = 0; i < count; i++)
        {
            bw.Write(Marshal.SizeOf(SaveData.ListBoneSaveData[i]));

            bytes = Utility.StructureToPtr(SaveData.ListBoneSaveData[i]);
            bw.Write(bytes);
        }

        SaveData.Path = path;

        bw.Close();
        fs.Close();

        return true;
    }

    public bool Load(sSaveData saveData)
    {
        if (saveData == null)
        {
            return false;
        }

        transform.position = saveData.Pos;
        transform.rotation = Quaternion.Euler(saveData.Rot);

        int count = 0;
        foreach(KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            if(count >= saveData.ListBoneSaveData.Count)
            {
                break;
            }

            sBoneSaveData boneSaveData = saveData.ListBoneSaveData[count];
            kv.Value.BoneSaveData = boneSaveData;

            kv.Value.Bone.rotation = Quaternion.Euler(boneSaveData.RotX, boneSaveData.RotY, boneSaveData.RotZ);
            kv.Value.Bone.localScale = Vector3.one * boneSaveData.Scale;

            ++count;
        }

        IsDyeing = saveData.UseDye;
        return true;
    }

    public bool LoadFromFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);

        if (SaveData == null)
        {
            SaveData = new sSaveData();
        }
        else
        {
            SaveData.Clear();
        }

        SaveData.Pos.x = br.ReadSingle();
        SaveData.Pos.y = br.ReadSingle();
        SaveData.Pos.z = br.ReadSingle();

        SaveData.Rot.x = br.ReadSingle();
        SaveData.Rot.y = br.ReadSingle();
        SaveData.Rot.z = br.ReadSingle();

        int count = br.ReadInt32();
        byte[] bytes = null;
        for (int i = 0; i < count; i++)
        {
            int size = br.ReadInt32();
            bytes = br.ReadBytes(size);

            sBoneSaveData boneSaveData = Utility.PtrToStructure<sBoneSaveData>(bytes);
            SaveData.ListBoneSaveData.Add(boneSaveData);
        }

        Load(SaveData);

        br.Close();
        fs.Close();

        return true;
    }

    public bool GetAttachObjectsInfo(ref bool hasHair, ref bool hasAccessory, ref bool hasOtherParts, ref bool hasColor, ref int colorCount)
    {
        hasHair = false;
        hasAccessory = false;
        hasOtherParts = false;
        hasColor = false;

        if(mCostumeBody == null || Data == null)
        {
            return false;
        }

        hasHair = mCostumeBody.HasHair();
        hasAccessory = mCostumeBody.HasAccessory();
        hasOtherParts = mCostumeBody.HasOtherParts();
        hasColor = mCostumeBody.HasColor(Data.ContentsIndex, ref colorCount);

        if (mCostumeBody.TableData != null && mCostumeBody.TableData.UseDyeing > 0)
        {
            MyDyeingData dyeingData = GameInfo.Instance.GetDyeingData(mCostumeBody.TableData.ID);
            if (dyeingData != null && !dyeingData.DyeingData.IsFirstDyeing)
            {
                ++colorCount;
                return true;
            }
        }

        return false;
    }

	public void SetCostumeBody( int colorIndex, int flag ) {
		if( Data == null ) {
			return;
		}

		if( mCostumeBody == null ) {
            mListMtrl.Clear();
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

			for( int i = 0; i < renderers.Length; i++ ) {
				for( int j = 0; j < renderers[i].materials.Length; j++ ) {
					if( renderers[i].materials[j].name.Contains( "selectbone" ) ) {
						continue;
					}

					sMtrlInfo mtrlInfo = new sMtrlInfo();
					mtrlInfo.Mtrl = renderers[i].materials[j];

                    if( mtrlInfo.Mtrl ) {
                        if( mtrlInfo.Mtrl.HasProperty( "_Color" ) ) {
                            mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_Color" );
                        }
                        else if( mtrlInfo.Mtrl.HasProperty( "_BaseColor" ) ) {
                            mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_BaseColor" );
                        }

                        if ( mtrlInfo.Mtrl.HasProperty( "_1st_ShadeColor" ) ) {
                            mtrlInfo.Original1stShadeColor = mtrlInfo.Mtrl.GetColor( "_1st_ShadeColor" );
                        }

                        if ( mtrlInfo.Mtrl.HasProperty( "_2nd_ShadeColor" ) ) {
                            mtrlInfo.Original2ndShadeColor = mtrlInfo.Mtrl.GetColor( "_2nd_ShadeColor" );
                        }
                    }

					mListMtrl.Add( mtrlInfo );
				}
			}

			return;
		}

		Color r = Color.white;
		Color g = Color.white;
		Color b = Color.white;

		if( mCostumeBody && mCostumeBody.TableData != null ) {
			MyDyeingData dyeingData = GameInfo.Instance.GetDyeingData(mCostumeBody.TableData.ID);
			if( dyeingData != null && !dyeingData.DyeingData.IsFirstDyeing ) {
				r = dyeingData.DyeingData.PartsColorList[0];
				g = dyeingData.DyeingData.PartsColorList[1];
				b = dyeingData.DyeingData.PartsColorList[2];
			}
		}

		mChangeCostumeFigure = true;

		mCostumeBody.SetCostumeBody( Data.ContentsIndex, colorIndex, flag, IsDyeing, r, g, b );
		GetMtrls();
	}

	private void AddEditableBone(eEditableBoneType type, string boneName, string parentBoneName, sBoxColInfo boxColInfo)
    {
        if (mDicEditableBone.ContainsKey(type))
        {
            Debug.LogError("AddEditableBone::이미 등록된 본입니다.");
            return;
        }

        eEditableBoneType find = ListExceptEditableBone.Find(x => x == type);
        if(find != eEditableBoneType.None)
        {
            return;
        }

        Transform bone = mListTransform.Find(x => x.name.CompareTo(boneName) == 0);
        if (bone == null)
        {
            Debug.LogWarning("AddEditableBone::찾을 수 없는 본입니다. : " + boneName);
            return;
        }

        sEditableBoneData parentData = null;
        if (!string.IsNullOrEmpty(parentBoneName))
        {
            foreach (KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
            {
                if (kv.Value.Bone.name.CompareTo(parentBoneName) == 0)
                {
                    parentData = kv.Value;
                    break;
                }
            }

            if (parentData == null)
            {
                Debug.LogError("AddEditableBone::부모를 찾을 수 없습니다. : " + parentBoneName);
                return;
            }
        }

        mDicEditableBone.Add(type, new sEditableBoneData(transform, bone, parentData, boxColInfo));
    }

	private void GetMtrls() {
		mListMtrl.Clear();

		if( mAniFace ) {
			Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
			for( int i = 0; i < renderers.Length; i++ ) {
				for( int j = 0; j < renderers[i].materials.Length; j++ ) {
					if( renderers[i].materials[j].name.Contains( "selectbone" ) ) {
						continue;
					}

					sMtrlInfo mtrlInfo = new sMtrlInfo();
					mtrlInfo.Mtrl = renderers[i].materials[j];

                    if( mtrlInfo.Mtrl.HasProperty( "_BaseColor" ) ) {
                        mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_BaseColor" );
                    }
                    else if( mtrlInfo.Mtrl.HasProperty( "_Color" ) ) {
						mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_Color" );
					}

                    if ( mtrlInfo.Mtrl.HasProperty( "_1st_ShadeColor" ) ) {
                        mtrlInfo.Original1stShadeColor = mtrlInfo.Mtrl.GetColor( "_1st_ShadeColor" );
                    }

                    if ( mtrlInfo.Mtrl.HasProperty( "_2nd_ShadeColor" ) ) {
                        mtrlInfo.Original2ndShadeColor = mtrlInfo.Mtrl.GetColor( "_2nd_ShadeColor" );
                    }

                    mListMtrl.Add( mtrlInfo );
				}
			}
		}

		if( mCostumeBody.kCostumeResMaterialList.Count > 0 ) {
			for( int i = 0; i < mCostumeBody.kCostumeResMaterialList.Count; i++ ) {
				for( int j = 0; j < mCostumeBody.kCostumeResMaterialList[i].kMesh.materials.Length; j++ ) {
					if( mListMtrl.Find( x => x.Mtrl == mCostumeBody.kCostumeResMaterialList[i].kMesh.materials[j] ) != null ) {
						continue;
					}

					sMtrlInfo mtrlInfo = new sMtrlInfo();
					mtrlInfo.Mtrl = mCostumeBody.kCostumeResMaterialList[i].kMesh.materials[j];

                    if( mtrlInfo.Mtrl.HasProperty( "_BaseColor" ) ) {
                        mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_BaseColor" );
                    }
                    else if( mtrlInfo.Mtrl.HasProperty( "_Color" ) ) {
						mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_Color" );
					}

                    if ( mtrlInfo.Mtrl.HasProperty( "_1st_ShadeColor" ) ) {
                        mtrlInfo.Original1stShadeColor = mtrlInfo.Mtrl.GetColor( "_1st_ShadeColor" );
                    }

                    if ( mtrlInfo.Mtrl.HasProperty( "_2nd_ShadeColor" ) ) {
                        mtrlInfo.Original2ndShadeColor = mtrlInfo.Mtrl.GetColor( "_2nd_ShadeColor" );
                    }

                    mListMtrl.Add( mtrlInfo );
				}
			}
		}

		if( mCostumeBody.CostumeHair ) {
			Renderer[] renderers = mCostumeBody.CostumeHair.GetComponentsInChildren<Renderer>();
			for( int i = 0; i < renderers.Length; i++ ) {
				if( mListMtrl.Find( x => x.Mtrl == renderers[i].material ) != null ) {
					continue;
				}

				sMtrlInfo mtrlInfo = new sMtrlInfo();
				mtrlInfo.Mtrl = renderers[i].material;

                if( mtrlInfo.Mtrl.HasProperty( "_BaseColor" ) ) {
                    mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_BaseColor" );
                }
                else if( mtrlInfo.Mtrl.HasProperty( "_Color" ) == false ) {
					mtrlInfo.OriginalColor = mtrlInfo.Mtrl.GetColor( "_Color" );
				}

                if ( mtrlInfo.Mtrl.HasProperty( "_1st_ShadeColor" ) ) {
                    mtrlInfo.Original1stShadeColor = mtrlInfo.Mtrl.GetColor( "_1st_ShadeColor" );
                }

                if ( mtrlInfo.Mtrl.HasProperty( "_2nd_ShadeColor" ) ) {
                    mtrlInfo.Original2ndShadeColor = mtrlInfo.Mtrl.GetColor( "_2nd_ShadeColor" );
                }

                mListMtrl.Add( mtrlInfo );
			}
		}
	}

	private void CreateBodyRotateAxis()
    {
        mPelvis = mListTransform.Find(x => x.name.CompareTo("Bip001 Pelvis") == 0);
        if (mPelvis == null)
        {
            return;
        }
         
        BodyRotateAxis = ResourceMgr.Instance.CreateFromAssetBundle<RotateAxis>("unit", "Unit/FigureRotAxis.prefab");
        BodyRotateAxis.name = string.Format("RotateAxis_Body");
        BodyRotateAxis.transform.parent = transform;
        Utility.InitTransform(BodyRotateAxis.gameObject);
        BodyRotateAxis.Show(false);

        Renderer[] renderers = BodyRotateAxis.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.renderQueue = 3002;
        }
    }

    protected override void LateUpdate()
    {
        if(!withoutAniEvent)
        {
            return;
        }

        foreach (KeyValuePair<eEditableBoneType, sEditableBoneData> kv in mDicEditableBone)
        {
            kv.Value.UpdateSelectObj();
        }

        if (BodyRotateAxis)
        {
            BodyRotateAxis.transform.position = mPelvis.position;
        }
    }
}
