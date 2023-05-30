using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.InputMapping;

public class GPGSInputMgr : MonoSingleton<GPGSInputMgr>
{

    private bool bisInit = false;

    public void Init()
    {
        if (Instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        if (!AppMgr.Instance.IsAndroidPC())
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        if (!bisInit)
        {
            bisInit = true;
            InitInputMapping();
        }
    }

    private void InitInputMapping()
    {
        PlayInput.GetInputMappingClient().SetInputMappingProvider(new GPGSInputMappingProvider());
        Input.simulateMouseWithTouches = false;
    }
}

public class GPGSInputMappingProvider : PlayInputMappingProvider
{
    private enum eGPGSActionIds
    {
        NONE,

        //Movement
        Left,
        Right,
        Up,
        Down,
        Dash,
        CamRotation,

        //Action
        Attack,
        WeaponSkill,
        SupporterSkill,
        USkill,
        ChangeWeapon,
        Pause,
        _MAX_,
    }

    List<PlayInputGroup> _inputGroups = new List<PlayInputGroup>();
    PlayMouseSettings _mouseSettings;

    public GPGSInputMappingProvider()
    {
        OnProvideInputMap();
    }

    public PlayInputMap OnProvideInputMap()
    {
        
        SetMouseSettings();
        SetInputGroups();

        return PlayInputMap.Create(_inputGroups, _mouseSettings);
    }

    private void SetInputGroups()
    {
        _inputGroups.Clear();

        //Movement
        List<PlayInputAction> movementGrupList = new List<PlayInputAction>();
        SetInputGroupField(ref movementGrupList, BaseCustomInput.eKeyKind.Left, eGPGSActionIds.Left);
        SetInputGroupField(ref movementGrupList, BaseCustomInput.eKeyKind.Right, eGPGSActionIds.Right);
        SetInputGroupField(ref movementGrupList, BaseCustomInput.eKeyKind.Up, eGPGSActionIds.Up);
        SetInputGroupField(ref movementGrupList, BaseCustomInput.eKeyKind.Down, eGPGSActionIds.Down);
        movementGrupList.Add(PlayInputAction.Create(GetInputKeyStr(575), (int)eGPGSActionIds.CamRotation, PlayInputControls.Create(null, GetMouseAction(PlayMouseAction.MouseMovement))));
        PlayInputGroup movementGroup = PlayInputGroup.Create("Movement", movementGrupList);

        List<PlayInputAction> actionGrupList = new List<PlayInputAction>();
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.Pause, eGPGSActionIds.Pause);
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.Attack, eGPGSActionIds.Attack);
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.Dash, eGPGSActionIds.Dash);
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.WeaponSkill, eGPGSActionIds.WeaponSkill);
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.SupporterSkill, eGPGSActionIds.SupporterSkill);
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.USkill, eGPGSActionIds.USkill);
        SetInputGroupField(ref actionGrupList, BaseCustomInput.eKeyKind.ChangeWeapon, eGPGSActionIds.ChangeWeapon);

        PlayInputGroup actionGroup = PlayInputGroup.Create("Action", actionGrupList);

        _inputGroups.Add(movementGroup);
        _inputGroups.Add(actionGroup);
    }

    private void SetInputGroupField(ref List<PlayInputAction> grouplist, BaseCustomInput.eKeyKind kind, eGPGSActionIds actionkind)
    {
        grouplist.Add(PlayInputAction.Create(GetInputKeyStr(AppMgr.Instance.CustomInput.GetTextID(kind)), (int)actionkind, PlayInputControls.Create(GetAndroidKeyCodes(AppMgr.Instance.CustomInput.GetKeyMapping(kind).GetPCKey()), GetMouseAction(AppMgr.Instance.CustomInput.GetKeyMapping(kind).GetPCKey()))));
    }

    private void SetMouseSettings()
    {
        if (_mouseSettings == null)
        {
            _mouseSettings = PlayMouseSettings.Create(false, false);
        }
    }

    private List<int> GetAndroidKeyCodes(params KeyCode[] keycode)
    {
        if (null == keycode || keycode.Length <= (int)eCOUNT.NONE)
            return null;

        if (keycode[(int)eCOUNT.NONE] == KeyCode.Mouse0 || keycode[(int)eCOUNT.NONE] == KeyCode.Mouse1)
            return null;

        List<int> result = new List<int>();
        for (int i = 0; i < keycode.Length; i++)
            result.Add(GetKeyCode(keycode[i]));

        return result;
    }

    private List<PlayMouseAction> GetMouseAction(params KeyCode[] keycode)
    {
        if (null == keycode || keycode.Length <= (int)eCOUNT.NONE)
            return null;

        List<PlayMouseAction> result = new List<PlayMouseAction>();

        if (keycode[(int)eCOUNT.NONE] == KeyCode.Mouse0)
        {
            result.Add(PlayMouseAction.MouseLeftClick);
        }
        else if (keycode[(int)eCOUNT.NONE] == KeyCode.Mouse1)
        {
            result.Add(PlayMouseAction.MouseRightClick);
        }
        else
        {
            return null;
        }
            
        return result;
    }

    private List<PlayMouseAction> GetMouseAction(params PlayMouseAction[] args)
    {
        if (null == args || args.Length <= (int)eCOUNT.NONE)
            return null;

        List<PlayMouseAction> result = new List<PlayMouseAction>();
        for (int i = 0; i < args.Length; i++)
            result.Add(args[i]);

        return result;
    }



    private string GetInputKeyStr(int key)
    {
        return FLocalizeString.Instance.GetText(key);
    }

    private int GetKeyCode(KeyCode keycode)
    {
        switch (keycode)
        {
            case KeyCode.Escape:
                return AndroidKeyCode.KEYCODE_ESCAPE;
            case KeyCode.F1:
                return AndroidKeyCode.KEYCODE_F1;
            case KeyCode.F2:
                return AndroidKeyCode.KEYCODE_F2;
            case KeyCode.F3:
                return AndroidKeyCode.KEYCODE_F3;
            case KeyCode.F4:
                return AndroidKeyCode.KEYCODE_F4;
            case KeyCode.F5:
                return AndroidKeyCode.KEYCODE_F5;
            case KeyCode.F6:
                return AndroidKeyCode.KEYCODE_F6;
            case KeyCode.F7:
                return AndroidKeyCode.KEYCODE_F7;
            case KeyCode.F8:
                return AndroidKeyCode.KEYCODE_F8;
            case KeyCode.F9:
                return AndroidKeyCode.KEYCODE_F9;
            case KeyCode.F10:
                return AndroidKeyCode.KEYCODE_F10;
            case KeyCode.F11:
                return AndroidKeyCode.KEYCODE_F11;
            case KeyCode.F12:
                return AndroidKeyCode.KEYCODE_F12;

            case KeyCode.BackQuote:
                return AndroidKeyCode.KEYCODE_GRAVE;
            case KeyCode.Keypad0:
                return AndroidKeyCode.KEYCODE_0;
            case KeyCode.Keypad1:
                return AndroidKeyCode.KEYCODE_1;
            case KeyCode.Keypad2:
                return AndroidKeyCode.KEYCODE_2;
            case KeyCode.Keypad3:
                return AndroidKeyCode.KEYCODE_3;
            case KeyCode.Keypad4:
                return AndroidKeyCode.KEYCODE_4;
            case KeyCode.Keypad5:
                return AndroidKeyCode.KEYCODE_5;
            case KeyCode.Keypad6:
                return AndroidKeyCode.KEYCODE_6;
            case KeyCode.Keypad7:
                return AndroidKeyCode.KEYCODE_7;
            case KeyCode.Keypad8:
                return AndroidKeyCode.KEYCODE_8;
            case KeyCode.Keypad9:
                return AndroidKeyCode.KEYCODE_9;
            case KeyCode.Minus:
                return AndroidKeyCode.KEYCODE_MINUS;
            case KeyCode.Equals:
                return AndroidKeyCode.KEYCODE_EQUALS;
            case KeyCode.Backspace:
                return AndroidKeyCode.KEYCODE_DEL;

            case KeyCode.Tab:
                return AndroidKeyCode.KEYCODE_TAB;
            case KeyCode.Q:
                return AndroidKeyCode.KEYCODE_Q;
            case KeyCode.W:
                return AndroidKeyCode.KEYCODE_W;
            case KeyCode.E:
                return AndroidKeyCode.KEYCODE_E;
            case KeyCode.R:
                return AndroidKeyCode.KEYCODE_R;
            case KeyCode.T:
                return AndroidKeyCode.KEYCODE_T;
            case KeyCode.Y:
                return AndroidKeyCode.KEYCODE_Y;
            case KeyCode.U:
                return AndroidKeyCode.KEYCODE_U;
            case KeyCode.I:
                return AndroidKeyCode.KEYCODE_I;
            case KeyCode.O:
                return AndroidKeyCode.KEYCODE_O;
            case KeyCode.P:
                return AndroidKeyCode.KEYCODE_P;
            case KeyCode.LeftBracket:
                return AndroidKeyCode.KEYCODE_LEFT_BRACKET;
            case KeyCode.RightBracket:
                return AndroidKeyCode.KEYCODE_RIGHT_BRACKET;
            case KeyCode.Backslash:
                return AndroidKeyCode.KEYCODE_BACKSLASH;
            
            case KeyCode.CapsLock:
                return AndroidKeyCode.KEYCODE_CAPS_LOCK;
            case KeyCode.A:
                return AndroidKeyCode.KEYCODE_A;
            case KeyCode.S:
                return AndroidKeyCode.KEYCODE_S;
            case KeyCode.D:
                return AndroidKeyCode.KEYCODE_D;
            case KeyCode.F:
                return AndroidKeyCode.KEYCODE_F;
            case KeyCode.G:
                return AndroidKeyCode.KEYCODE_G;
            case KeyCode.H:
                return AndroidKeyCode.KEYCODE_H;
            case KeyCode.J:
                return AndroidKeyCode.KEYCODE_J;
            case KeyCode.K:
                return AndroidKeyCode.KEYCODE_K;
            case KeyCode.L:
                return AndroidKeyCode.KEYCODE_L;
            case KeyCode.Semicolon:
                return AndroidKeyCode.KEYCODE_SEMICOLON;
            case KeyCode.Return:
                return AndroidKeyCode.KEYCODE_ENTER;

            case KeyCode.LeftShift:
                return AndroidKeyCode.KEYCODE_SHIFT_LEFT;
            case KeyCode.Z:
                return AndroidKeyCode.KEYCODE_Z;
            case KeyCode.X:
                return AndroidKeyCode.KEYCODE_X;
            case KeyCode.C:
                return AndroidKeyCode.KEYCODE_C;
            case KeyCode.V:
                return AndroidKeyCode.KEYCODE_V;
            case KeyCode.B:
                return AndroidKeyCode.KEYCODE_B;
            case KeyCode.N:
                return AndroidKeyCode.KEYCODE_N;
            case KeyCode.M:
                return AndroidKeyCode.KEYCODE_M;
            case KeyCode.Comma:
                return AndroidKeyCode.KEYCODE_COMMA;
            case KeyCode.Period:
                return AndroidKeyCode.KEYCODE_PERIOD;
            case KeyCode.Slash:
                return AndroidKeyCode.KEYCODE_SLASH;
            case KeyCode.RightShift:
                return AndroidKeyCode.KEYCODE_SHIFT_RIGHT;

            case KeyCode.LeftControl:
                return AndroidKeyCode.KEYCODE_CTRL_LEFT;
            case KeyCode.LeftAlt:
                return AndroidKeyCode.KEYCODE_ALT_LEFT;
            case KeyCode.Space:
                return AndroidKeyCode.KEYCODE_SPACE;
            case KeyCode.RightAlt:
                return AndroidKeyCode.KEYCODE_ALT_RIGHT;
            case KeyCode.RightControl:
                return AndroidKeyCode.KEYCODE_CTRL_RIGHT;

            case KeyCode.LeftArrow:
                return AndroidKeyCode.KEYCODE_DPAD_LEFT;
            case KeyCode.RightArrow:
                return AndroidKeyCode.KEYCODE_DPAD_RIGHT;
            case KeyCode.UpArrow:
                return AndroidKeyCode.KEYCODE_DPAD_UP;
            case KeyCode.DownArrow:
                return AndroidKeyCode.KEYCODE_DPAD_DOWN;

        }
        return AndroidKeyCode.KEYCODE_UNKNOWN;
    }
}

#endif