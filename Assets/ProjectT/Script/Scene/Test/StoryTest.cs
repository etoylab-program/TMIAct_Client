using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTest : MonoBehaviour
{
    public UIBookCardCinemaPopup bookCardCinema;
    public UIStroyPopup stroyPopup;
    public UIStoryCommunicationPopup uIStoryCommunicationPopup;
    private GameTable.Card.Param _cardtabledata;
    public int kCardIdx = 1;
    public int kScenarioGroupID = 1;
    public int kScenarioCommunicationGroupID = 1;

    public GameObject kVolumes;
    private UISprite[] m_Volumes;
    public AudioSource kAudioSource;
    public float maxHeight = 300;
    public float[] samples = new float[128]; //샘플//샘플수는 64가 최소 2의 거듭제곱으로 해야함

    public UITexture circleEff;
    Coroutine circleEffectCor;

    public UITexture textTex;
    public SpriteRenderer image;
    public bool kJpnTalk = false;


    private void Awake()
    {
        FManager[] find = FindObjectsOfType<FManager>();
        for(int i = 0; i < find.Length; i++)
        {
            DestroyImmediate(find[i]);
        }
    }

    void Start()
    {
        GameInfo.Instance.DoInitGame();

        m_Volumes = kVolumes.GetComponentsInChildren<UISprite>();

        if(kJpnTalk)
        {
            ChangeLanguage(eLANGUAGE.JPN);
        }
        else
        {
            ChangeLanguage(eLANGUAGE.KOR);
        }

        if(circleEff != null)
        {
            circleEff.width = 1280;// (int)(Screen.width * 1.2);
            circleEff.height = 720;// (int)(Screen.height * 1.2f);
            if(circleEff.material == null)
            {
                Shader shader = (Shader)ResourceMgr.Instance.LoadFromAssetBundle("shader","Shader/Circle.shader");
                Material mat = new Material(shader);
                circleEff.material = mat;
                
            }
            circleEff.material.SetFloat("_Hardness", 10f);
            //circleEff.drawCall.dynamicMaterial.SetFloat("_Hardness", 0);
        }
        //UIValue.Instance.SetValue(UIValue.EParamType.StroyID, kScenarioGroupID);
        //ScenarioMgr.Instance.Open(kScenarioGroupID);
        //stroyPopup.gameObject.SetActive(true);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F3))
        {
            _cardtabledata = GameInfo.Instance.GameTable.FindCard(kCardIdx);
            if (_cardtabledata != null)
            {
                //  카드 ID 저장
                int scenarioGroupID = _cardtabledata.ScenarioGroupID;
                string scenariobg = _cardtabledata.Icon + "_0";
                string scenariobgSprite = _cardtabledata.SpriteIcon;
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioGroupID, scenarioGroupID);
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGStr, scenariobg);
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGSprite, scenariobgSprite);
                ScenarioMgr.Instance.Open(scenarioGroupID);
                bookCardCinema.gameObject.SetActive(true);
                return;
            }
            else
            {
                //var chardata = GameInfo.Instance.CharList.Find(x => x.TableData.ScenarioGroupID == kCardIdx);
                var charTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ScenarioGroupID == kCardIdx);
                int scenarioGroupID = charTableData.ScenarioGroupID;
                string scenariobg = charTableData.Icon;
                string scenariobgSprite = charTableData.SpriteIcon;
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioGroupID, scenarioGroupID);
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGStr, scenariobg);
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioFavorBGSprite, scenariobgSprite);
                ScenarioMgr.Instance.Open(scenarioGroupID);
                bookCardCinema.gameObject.SetActive(true);
                return;
            }
        }

        if(Input.GetKeyDown(KeyCode.F2))
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StroyID, kScenarioGroupID);
            ScenarioMgr.Instance.Open(kScenarioGroupID);
            if(ScenarioMgr.Instance.GetScenarioParam(0).Pos == 0)
                stroyPopup.gameObject.SetActive(true);
            if (ScenarioMgr.Instance.GetScenarioParam(0).Pos == 2)
                bookCardCinema.gameObject.SetActive(true);
            //UIStroyPopup
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            UIValue.Instance.SetValue(UIValue.EParamType.StroyID, kScenarioCommunicationGroupID);
            ScenarioMgr.Instance.Open(kScenarioCommunicationGroupID);
            uIStoryCommunicationPopup.gameObject.SetActive(true);
            //ShowUI("StoryCommunicationPopup", true);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            kAudioSource.Play();
        }
        if(Input.GetKeyDown(KeyCode.F4))
        {
            LoadSpriteAtlasWithTexture();
        }

        AudioSpectrumData();
    }

    void LoadSpriteAtlasWithTexture()
    {
        Object[] sprite = ResourceMgr.Instance.LoadFromAssetBundleWithSubObject("favor", "Favor/card.png");


        for (int i = 0; i < sprite.Length; i++)
            Debug.Log(sprite[i].name);

        Sprite sp = sprite[2] as Sprite;

        image.sprite = sp;
        textTex.mainTexture = ConvertSpriteToTexture(sp);
        //textTex.mainTexture = ((Sprite)sprite[0]).texture;
    }
    
    IEnumerator CircleEffect(float duration, bool reverse = true)
    {
        float EndTime = Time.time + duration;
        float lerpValue = 0f;
        //circleEff.gameObject.SetActive(true);
        while (this.gameObject.activeSelf)
        {
            //clampValue = Mathf.Clamp((EndTime - Time.time) / duration, 0f, 1f);
            lerpValue = Mathf.Lerp(0f, 1f, (EndTime - Time.time) / duration);


            circleEff.drawCall.dynamicMaterial.SetFloat("_Hardness", (reverse ? (1f - lerpValue) : lerpValue) * 5f);

            if(EndTime < Time.time)
            {
                break;
            }
            yield return null;
        }
        //circleEff.gameObject.SetActive(false);
    }
    
    void AudioSpectrumData()
    {
        if (m_Volumes == null)
            return;
        kAudioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);

        //정방향
        //for(int i = 0; i < m_Volumes.Length; i++)
        //{
        //    m_Volumes[i].width = 8;
        //    m_Volumes[i].height = (int)(samples[i] * maxHeight) + 10;
        //}

        //역방향
        //for (int i = 0; i < m_Volumes.Length; i++)
        //{
        //    m_Volumes[i].width = 8;
        //    m_Volumes[i].height = (int)(samples[m_Volumes.Length - i] * maxHeight) + 10;
        //}
        m_Volumes[0].height = (int)(Mathf.Lerp(m_Volumes[0].height, Mathf.Clamp(samples[1] * maxHeight, 0, 100), 0.05f));
        //m_Volumes[0].height = (int)Mathf.Clamp(samples[0] * maxHeight, 0, 100);

        int i = 1;
        while (i < samples.Length - 1)
        {
            Debug.DrawLine(new Vector3(i - 1, samples[i] + 10, 0), new Vector3(i, samples[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(samples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(samples[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), samples[i - 1] - 10, 1), new Vector3(Mathf.Log(i), samples[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(samples[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(samples[i]), 3), Color.yellow);
            i++;
        }
    }

    public void ChangeLanguage(eLANGUAGE e)
    {
        FLocalizeString.Instance.InitLocalize(e);
        FLocalizeString.Instance.SaveLanguage();

        var root = GameObject.Find("UIRoot");
        if (root == null)
            return;
        var labellist = root.GetComponentsInChildren<FLocalizeLabel>(true);
        for (int i = 0; i < labellist.Length; i++)
            labellist[i].OnLocalize();

        //LobbyUIManager.Instance.Renewal();
    }

    Texture2D ConvertSpriteToTexture(Sprite sprite)
    {
        var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        return croppedTexture;
    }
}
