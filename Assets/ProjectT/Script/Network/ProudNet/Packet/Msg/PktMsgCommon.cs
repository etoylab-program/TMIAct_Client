using System.Collections.Generic;

using Nettention.Proud;


public class PktMsgType
{
    static public bool IsOnBitIdx_Func(ulong _targetVal, ulong _flagIdx)
    {
        ulong checkFlag = (ulong)((ulong)0x0000000000000001 << (int)_flagIdx);
        return (checkFlag == (_targetVal & checkFlag));
    }
    static public void DoOnBitIdx_Func(ref ulong _targetVal, ulong _flagIdx)
    {
        ulong checkFlag = (ulong)((ulong)0x0000000000000001 << (int)_flagIdx);
        _targetVal |= checkFlag;
    }
    static public void DoOffBitIdx_Func(ref ulong _targetVal, ulong _flagIdx)
    {
        ulong checkFlag = (ulong)(ulong)(0x0000000000000001 << (int)_flagIdx);
        _targetVal &= 0xFFFFFFFFFFFFFFFF ^ checkFlag;
    }
    public virtual bool Read(Message _s)
    {
        Debug.LogError("PktMsgType.Read() 함수를 재정의 하지 않았습니다. - Type:" + this.GetType().ToString());
        return false;
    }
    public virtual void Write(Message _s)
    {
        Debug.LogError("PktMsgType.Write() 함수를 재정의 하지 않았습니다. - Type:" + this.GetType().ToString());
    }

    protected bool _IsOnBit(ulong _targetVal, ulong _checkVal) {
        return (_checkVal == (_targetVal & _checkVal));
    }
    protected void _DoOnBit(ref ulong _targetVal, ulong _onVal) {
        _targetVal      |= _onVal;
    }
    protected void _DoOffBit(ref ulong _targetVal, ulong _offVal) {
        _targetVal      &= 0xFFFFFFFFFFFFFFFF ^ _offVal;
    }
    protected bool _IsOnBitIdx(ulong _targetVal, ulong _flagIdx) {
        return IsOnBitIdx_Func(_targetVal, _flagIdx);
    }
    protected void _DoOnBitIdx(ref ulong _targetVal, ulong _flagIdx) {
        DoOnBitIdx_Func(ref _targetVal, _flagIdx);
    }
    protected void _DoOffBitIdx(ref ulong _targetVal, ulong _flagIdx) {
        DoOffBitIdx_Func(ref _targetVal, _flagIdx);
    }

    protected bool _IsOnBit(uint _targetVal, uint _checkVal) {
        return (_checkVal == (_targetVal & _checkVal));
    }
    protected void _DoOnBit(ref uint _targetVal, uint _onVal) {
        _targetVal      |= _onVal;
    }
    protected void _DoOffBit(ref uint _targetVal, uint _offVal) {
        _targetVal      &= 0xFFFFFFFF ^ _offVal;
    }
    protected bool _IsOnBitIdx(uint _targetVal, uint _flagIdx) {
        uint checkFlag  = (uint)(0x00000001 << (int)_flagIdx);
        return (checkFlag == (_targetVal & checkFlag));
    }
    protected void _DoOnBitIdx(ref uint _targetVal, uint _flagIdx) {
        uint checkFlag  = (uint)(0x00000001 << (int)_flagIdx);
        _targetVal      |= checkFlag;
    }
    protected void _DoOffBitIdx(ref uint _targetVal, uint _flagIdx) {
        uint checkFlag = (uint)(0x00000001 << (int)_flagIdx);
        _targetVal      &= 0xFFFFFFFF ^ checkFlag;
    }

    protected bool _IsOnBitIdx(byte _targetVal, byte _flagIdx) {
        byte checkFlag  = (byte)(0x01 << (int)_flagIdx);
        return (checkFlag == (_targetVal & checkFlag));
    }
    protected void _DoOnBitIdx(ref byte _targetVal, byte _flagIdx) {
        byte checkFlag  = (byte)(0x01 << (int)_flagIdx);
        _targetVal      |= checkFlag;
    }
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out eGOODSTYPE _v)
    {
        System.Byte value = 0;
        _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eGOODSTYPE)value;
        return true;
    }
    public static void Write(Message _s, eGOODSTYPE _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eREWARDTYPE _v)
    {
        System.Byte value = 0;
        _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eREWARDTYPE)value;
        return true;
    }
    public static void Write(Message _s, eREWARDTYPE _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eBookGroup _v)
    {
        System.Byte value = 0;
        _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eBookGroup)value;
        return true;
    }
    public static void Write(Message _s, eBookGroup _v) { _s.Write((System.Byte)_v); }
    public static bool Read(Message _s, out eGrowState _v)
    {
        System.Byte value = 0;
        _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eGrowState)value;
        return true;
    }
    public static void Write(Message _s, eGrowState _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eContentsPosKind _v)
    {
        System.Byte value = 0;  _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eContentsPosKind)value;
        return true;
    }
    public static void Write(Message _s, eContentsPosKind _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eAccountType _v)
    {
        System.Byte value = 0; _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eAccountType)value;
        return true;
    }
    public static void Write(Message _s, eAccountType _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eInAppKind _v)
    {
        System.Byte value = 0; _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eInAppKind)value;
        return true;
    }
    public static void Write(Message _s, eInAppKind _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out ePlatformKind _v)
    {
        System.Byte value = 0; _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (ePlatformKind)value;
        return true;
    }
    public static void Write(Message _s, ePlatformKind _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eServerType _v)
    {
        System.Byte value = 0; _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eServerType)value;
        return true;
    }
    public static void Write(Message _s, eServerType _v) { _s.Write((System.Byte)_v); }

    public static bool Read(Message _s, out eSecurityKind _v)
    {
        System.Byte value = 0; _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eSecurityKind)value;
        return true;
    }
    public static void Write(Message _s, eSecurityKind _v) { _s.Write((System.UInt64)_v); }
	public static bool Read(Message _s, out ePresetKind _v)
	{
		System.Byte value = 0; _v = 0;
		if (false == _s.Read(out value)) return false;

		_v = (ePresetKind)value;
		return true;
	}
	public static void Write(Message _s, ePresetKind _v) { _s.Write((System.Byte)_v); }
	public static bool Read(Message _s, out eLANGUAGE _v)
	{
		System.Byte value = 0; _v = 0;
		if (false == _s.Read(out value)) return false;

		_v = (eLANGUAGE)value;
		return true;
	}
	public static void Write(Message _s, eLANGUAGE _v) { _s.Write((System.Byte)_v); }
	public static bool Read(Message _s, out eCircleAuthLevel _v)
	{
		System.Byte value = 0; _v = 0;
		if (false == _s.Read(out value)) return false;

		_v = (eCircleAuthLevel)value;
		return true;
	}
	public static void Write(Message _s, eCircleAuthLevel _v) { _s.Write((System.Byte)_v); }
	public static bool Read(Message _s, out eCircleMarkType _v)
	{
		System.Byte value = 0; _v = 0;
		if (false == _s.Read(out value)) return false;

		_v = (eCircleMarkType)value;
		return true;
	}
	public static void Write(Message _s, eCircleMarkType _v) { _s.Write((System.Byte)_v); }
	public static bool Read(Message _s, out eCircleNotiType _v)
	{
		System.Byte value = 0; _v = 0;
		if (false == _s.Read(out value)) return false;

		_v = (eCircleNotiType)value;
		return true;
	}
	public static void Write(Message _s, eCircleNotiType _v) { _s.Write((System.Byte)_v); }
	public static bool Read(Message _s, out eTMIErrNum _v)
    {
        System.UInt64 value = 0; _v = 0;
        if (false == _s.Read(out value)) return false;

        _v = (eTMIErrNum)value;
        return true;
    }
    public static void Write(Message _s, eTMIErrNum _v) { _s.Write((System.UInt64)_v); }
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool ReadList<T>(Message _s, out List<T> _v) where T : PktMsgType, new() {
        System.UInt16 count = 0;
        _s.Read(out count);

        _v = new List<T>(count);
        for (int i = 0; i < count; ++i) {
            T data = new T();
            if (false == data.Read(_s))
                return false;
            _v.Add(data);
        }
        return true;
    }
    public static bool Read(Message _s, out char[] _v) {
        System.UInt16 count = 0;
        _s.Read(out count);

        _v = new char[count];
        for (System.UInt64 i = 0; i < count; ++i) {
            System.Byte data = 0;
            if (false == _s.Read(out data))
                return false;
            _v[i]   = (char)data;
        }
        return true;
    }
    public static bool Read(Message _s, out List<System.Byte> _v) {
        System.UInt16 count = 0;
        _s.Read(out count);

        _v = new List<System.Byte>(count);
        for (int i = 0; i < count; ++i) {
            System.Byte data = 0;
            if (false == _s.Read(out data))
                return false;
            _v.Add(data);
        }
        return true;
    }
    public static bool Read(Message _s, out List<System.UInt32> _v) {
        System.UInt16 count = 0;
        _s.Read(out count);

        _v = new List<System.UInt32>(count);
        for (int i = 0; i < count; ++i) {
            System.UInt32 data = 0;
            if (false == _s.Read(out data))
                return false;
            _v.Add(data);
        }
        return true;
    }
    public static bool Read(Message _s, out List<System.UInt16> _v) {
        System.UInt16 count = 0;
        _s.Read(out count);

        _v = new List<System.UInt16>(count);
        for (int i = 0; i < count; ++i) {
            System.UInt16 data = 0;
            if (false == _s.Read(out data))
                return false;
            _v.Add(data);
        }
        return true;
    }
    public static bool Read(Message _s, out List<System.UInt64> _v) {
        System.UInt16 count = 0;
        _s.Read(out count);

        _v = new List<System.UInt64>(count);
        for (int i = 0; i < count; ++i) {
            System.UInt64 data = 0;
            if (false == _s.Read(out data))
                return false;
            _v.Add(data);
        }
        return true;
    }

    public static void WriteList<T>(Message _s, List<T> _v) where T : PktMsgType {
        System.UInt16 size = (System.UInt16)_v.Count;
        Write(_s, size);
        foreach (T data in _v)
            data.Write(_s);
    }
    public static void Write(Message _s, char[] _v) {
        System.UInt16 size = (System.UInt16)_v.Length;
        Write(_s, size);
        foreach (var data in _v)
            Write(_s, data);
    }
    public static void Write(Message _s, List<System.Byte> _v) {
        System.UInt16 size = (System.UInt16)_v.Count;
        Write(_s, size);
        foreach (var data in _v)
            Write(_s, data);
    }
    public static void Write(Message _s, List<System.UInt32> _v) {
        System.UInt16 size = (System.UInt16)_v.Count;
        Write(_s, size);
        foreach (var data in _v)
            Write(_s, data);
    }
    public static void Write(Message _s, List<System.UInt16> _v)
    {
        System.UInt16 size = (System.UInt16)_v.Count;
        Write(_s, size);
        foreach (var data in _v)
            Write(_s, data);
    }

    public static void Write(Message _s, List<System.UInt64> _v) {
        System.UInt16 size = (System.UInt16)_v.Count;
        Write(_s, size);
        foreach (var data in _v)
            Write(_s, data);
    }

    //public static void Read(Message _s, out Dictionary<int, float> _v)
    //{
    //    _v = new Dictionary<int, float>();

    //    int count = 0;
    //    _s.ReadScalar(ref count);

    //    for (int i = 0; i < count; ++i)
    //    {
    //        int key = 0;
    //        float elem = 0;
    //        _s.Read(out key);
    //        _s.Read(out elem);
    //        _v.Add(key, elem);
    //    }
    //}
    //public static void Write(Message _s, Dictionary<int, float> _v)
    //{
    //    int size = _v.Count;

    //    _s.WriteScalar(size);

    //    foreach (KeyValuePair<int, float> pair in _v)
    //    {
    //        _s.Write(pair.Key);
    //        _s.Write(pair.Value);
    //    }
    //}
}

///////////////////////////////////////////////////////////////////////
/// <summary>
// 문자열 패킷 메시지
public class PktInfoStr : PktMsgType
{
    public System.String str_;
    public PktInfoStr()
    {
        str_ = "";
    }
};
// 8바이트 문자열 패킷 메시지
public class PktInfoStr8 : PktMsgType
{
    public char[] str_;

    public System.String GetStr() {
        System.String str = new System.String(str_);
        return str;
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoStr _v) {
        _v = new PktInfoStr();
        if (false == _s.Read(out _v.str_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStr _v) {
        Write(_s, _v.str_);
    }
    public static bool Read(Message _s, out PktInfoStr8 _v) {
        _v = new PktInfoStr8();
        if (false == Read(_s, out _v.str_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStr8 _v) {
        Write(_s, _v.str_);
    }
}
// 고유 아이디(UID) 목록 패킷 메시지
public class PktInfoUIDList : PktMsgType
{
    public List<System.UInt64> uids_;   // 고유 ID 목록
    public System.String GetStr() {
        System.String str = string.Format("UID[-");
        foreach (var tid in uids_)
            str += string.Format("{0}-", tid);
        str += string.Format("]");
        return str;
    }
};
// 테이블 아이디(TID) 목록 패킷 메시지
public class PktInfoTIDList : PktMsgType
{
    public List<System.UInt32> tids_;   // 테이블 ID 목록
    public System.String GetStr() {
        System.String str = string.Format("TID[-");
        foreach (var tid in tids_)
            str += string.Format("{0}-", tid);
        str += string.Format("]");
        return str;
    }
};
// uint8_t 크기 값 목록 패킷 메시지
public class PktInfoUInt8List : PktMsgType
{
    public List<System.Byte> vals_;     // 값 목록
    public System.String GetStr() {
        System.String str = string.Format("[-");
        foreach (var val in vals_)
            str += string.Format("{0}-", val);
        str += string.Format("]");
        return str;
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUIDList _v) {
        _v = new PktInfoUIDList();
        if (false == Read(_s, out _v.uids_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUIDList _v) {
        Write(_s, _v.uids_);
    }
    public static bool Read(Message _s, out PktInfoTIDList _v) {
        _v = new PktInfoTIDList();
        if (false == Read(_s, out _v.tids_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoTIDList _v) {
        Write(_s, _v.tids_);
    }
    public static bool Read(Message _s, out PktInfoUInt8List _v) {
        _v = new PktInfoUInt8List();
        if (false == Read(_s, out _v.vals_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUInt8List _v) {
        Write(_s, _v.vals_);
    }
}
// UID와 값을 묶어 놓은 패킷 메시지
public class PktInfoUIDValue : PktMsgType {
    public class Piece : PktMsgType {
        public System.UInt64 uid_;      // 고유 ID
        public System.UInt64 val_;      // 값
        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            return _s.Read(out val_);
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            _s.Write(val_);
        }
    };
    public List<Piece> infos_;
};
// TID와 값을 묶어 놓은 패킷 메시지
public class PktInfoTIDValue : PktMsgType {
    public class Piece : PktMsgType {
        public System.UInt32 tid_;      // 테이블 ID
        public System.UInt32 val_;      // 값
        public override bool Read(Message _s) {
            if (false == _s.Read(out tid_)) return false;
            return _s.Read(out val_);
        }
        public override void Write(Message _s) {
            _s.Write(tid_);
            _s.Write(val_);
        }
    };
    public List<Piece> infos_;
};
// UID On/Off 패킷 메시지
public class PktInfoUIDOnOff : PktMsgType {
    public class Piece : PktMsgType {
        public System.UInt64 uid_;      // 고유 ID
        public System.Boolean on_;      // on - off 여부
        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            return _s.Read(out on_);
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            _s.Write(on_);
        }
    };
    public List<Piece> infos_;
};
// TID On/Off 패킷 메시지
public class PktInfoTIDOnOff : PktMsgType {
    public class Piece : PktMsgType {
        public System.UInt32 tid_;      // 테이블 ID
        public System.Boolean on_;      // on - off 여부
        public override bool Read(Message _s) {
            if (false == _s.Read(out tid_)) return false;
            return _s.Read(out on_);
        }
        public override void Write(Message _s) {
            _s.Write(tid_);
            _s.Write(on_);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler {
    // UID와 값을 묶어 놓은 패킷 메시지
    public static bool Read(Message _s, out PktInfoUIDValue.Piece _v) {
        _v = new PktInfoUIDValue.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoUIDValue.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoUIDValue _v) {
        _v = new PktInfoUIDValue();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoUIDValue _v) {
        WriteList(_s, _v.infos_);
    }
    // TID와 값을 묶어 놓은 패킷 메시지
    public static bool Read(Message _s, out PktInfoTIDValue.Piece _v) {
        _v = new PktInfoTIDValue.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTIDValue.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTIDValue _v) {
        _v = new PktInfoTIDValue();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTIDValue _v) {
        WriteList(_s, _v.infos_);
    }
    // UID On/Off 패킷 메시지
    public static bool Read(Message _s, out PktInfoUIDOnOff.Piece _v) {
        _v = new PktInfoUIDOnOff.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoUIDOnOff.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoUIDOnOff _v) {
        _v = new PktInfoUIDOnOff();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoUIDOnOff _v) {
        WriteList(_s, _v.infos_);
    }
    // TID On/Off 패킷 메시지
    public static bool Read(Message _s, out PktInfoTIDOnOff.Piece _v) {
        _v = new PktInfoTIDOnOff.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTIDOnOff.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTIDOnOff _v) {
        _v = new PktInfoTIDOnOff();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTIDOnOff _v) {
        WriteList(_s, _v.infos_);
    }
}
// 데이타 스트림 용 패킷 메시지
public class PktInfoStream : PktMsgType
{
    public List<System.Byte> stm_;      // 데이타 스트림
    public System.String GetStr() {
        return string.Format("Size[{0}]", stm_.Count);
    }
    public void DoAdd(System.Byte[] _data) {
        stm_.InsertRange(stm_.Count, _data);
    }
    public bool IsEmpty() { return (0 == stm_.Count) ? true : false; }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoStream _v) {
        _v = new PktInfoStream();
        if (false == Read(_s, out _v.stm_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStream _v) {
        Write(_s, _v.stm_);
    }
}
// 버전 정보 패킷 메시지
public class PktInfoVersion : PktMsgType
{
    public System.UInt32 verMain_;      // 서버 버전 메인
    public System.UInt32 verSub_;       // 서버 버전 서브
    public System.String GetStr() {
        return string.Format("Ver[{0}.{1}]", verMain_, verSub_);
    }
    public void DoSet(System.UInt32 _main, System.UInt32 _sub) { verMain_ = _main; verMain_ = _sub; }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoVersion _v) {
        _v = new PktInfoVersion();
        if (false == _s.Read(out _v.verMain_)) return false;
        if (false == _s.Read(out _v.verSub_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoVersion _v) {
        _s.Write(_v.verMain_);
        _s.Write(_v.verSub_);
    }
}
// 공용 보상 획득 요청 패킷 메시지
public class PktInfoComRwd : PktMsgType
{
    public PktInfoUInt8List idxs_;      // 획득을 요청할 미션 인덱스
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoComRwd _v) {
        _v = new PktInfoComRwd();
        return Read(_s, out _v.idxs_);
    }
    public static void Write(Message _s, PktInfoComRwd _v) {
        Write(_s, _v.idxs_);
    }
}
// 제품 정보 패킷 메시지
public class PktInfoProductOne : PktMsgType
{
    public System.UInt32 index_;        // 제품 인덱스 - 보통은 제품 관련 데이타 테이블 ID로 사용됨
    public System.UInt32 value_;        // 제품 관련 값 - 보통은 제품의 개수로 사용됨
    public eREWARDTYPE type_;           // 제품 타입 - eREWARDTYPE

    public System.String GetStr() { return string.Format("[Product-TP:{0}, Idx:{1}, Val:{2}]", type_, index_, value_); }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoProductOne _v) {
        _v = new PktInfoProductOne();
        if (false == _s.Read(out _v.index_)) return false;
        if (false == _s.Read(out _v.value_)) return false;
        if (false == Read(_s, out _v.type_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoProductOne _v) {
        _s.Write(_v.index_);
        _s.Write(_v.value_);
        Write(_s, _v.type_);
    }
}
// 개별 재화 패킷 메시지
public class PktInfoGoods : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 value_;        // 재화 수치 값
        public eGOODSTYPE type_;            // 재화 타입
        public override bool Read(Message _s) {
            if (false == _s.Read(out value_)) return false;
            return PN_MarshalerEx.Read(_s, out type_);
        }
        public override void Write(Message _s) {
            _s.Write(value_);
            PN_MarshalerEx.Write(_s, type_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 nowHardCash_;      // 현재 하드 캐시 값
    public System.Boolean hardCashFlag_;    // 하드 캐시 변경 여부
};
// 재화 패킷 클래스
public class PktInfoGoodsAll : PktMsgType
{
    public System.UInt64[] goodsValues_ = new System.UInt64[(int)eGOODSTYPE.COUNT];
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 개별 재화 패킷 메시지
    public static bool Read(Message _s, out PktInfoGoods.Piece _v) {
        _v = new PktInfoGoods.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoGoods.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoGoods _v) {
        _v = new PktInfoGoods();
        if (false == ReadList(_s, out _v.infos_)) return false;
        if (false == _s.Read(out _v.nowHardCash_)) return false;
        if (false == _s.Read(out _v.hardCashFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoGoods _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.nowHardCash_);
        _s.Write(_v.hardCashFlag_);
    }
    // 재화 패킷 클래스
    public static bool Read(Message _s, out PktInfoGoodsAll _v) {
        _v = new PktInfoGoodsAll();
        for (int loop = 0; loop < (int)eGOODSTYPE.COUNT; ++loop) {
            if (false == _s.Read(out _v.goodsValues_[loop]))
                return false;
        }
        return true;
    }
    public static void Write(Message _s, PktInfoGoodsAll _v) {
        for (int loop = 0; loop < (int)eGOODSTYPE.COUNT; ++loop)
            _s.Write(_v.goodsValues_[loop]);
    }
}
// 아이템 패킷 메시지
public class PktInfoItem : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 itemUID_;
        public System.UInt32 tableID_;
        public System.UInt32 cnt_;
        public override bool Read(Message _s) {
            if (false == _s.Read(out itemUID_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out cnt_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(itemUID_);
            _s.Write(tableID_);
            _s.Write(cnt_);
        }
    };
    public List<Piece> infos_;
};
// 추가 아이템 패킷 메시지(보통 로그나 뷰어용으로 사용합니다.)
public class PktInfoAddItem : PktMsgType
{
    public class Piece : PktMsgType {
        public System.UInt32 tid_;                  // 획득한 아이템 테이블 ID
        public System.UInt32 retCnt_;               // 기존 아이템 수량과 획득된 수량을 더한 최종 수량(로그용)
        public System.UInt32 addCnt_;               // 회됙한 아이템 개수
        public bool newAddFlag_;                    // 획득한 아이템이 새로 추가된 것인지 확인합니다.
        public override bool Read(Message _s) {
            if (false == _s.Read(out tid_)) return false;
            if (false == _s.Read(out retCnt_)) return false;
            if (false == _s.Read(out addCnt_)) return false;
            if (false == _s.Read(out newAddFlag_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tid_);
            _s.Write(retCnt_);
            _s.Write(addCnt_);
            _s.Write(newAddFlag_);
        }
    };
    public List<Piece> infos_;          // 재료 무기 ID 목록
};
// 보상 아이템 패킷 메시지
public class PktInfoRwdItem : PktMsgType
{
    public PktInfoItem rets_;           // 보상 결과가 적용된 현재 아이템 정보
    public PktInfoAddItem adds_;        // 추가된 아이템 정보(로그용)
};
// 소모 아이템 패킷 메시지
public class PktInfoConsumeItem : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 uid_;         // 아이템 고유 ID
        public System.UInt32 remainCnt_;   // 아이템 남은 수량
        public System.UInt16 delCnt_;      // 아이템 차감 수량
        public System.Byte delFlag_;       // 삭제 여부
        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            if (false == _s.Read(out remainCnt_)) return false;
            if (false == _s.Read(out delCnt_)) return false;
            if (false == _s.Read(out delFlag_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            _s.Write(remainCnt_);
            _s.Write(delCnt_);
            _s.Write(delFlag_);
        }
    };
    public List<Piece> infos_;          // 재료 무기 ID 목록
};
// 아이템 및 재화 소모 적용용 패킷 메시지
public class PktInfoConsumeItemAndGoods : PktMsgType
{
    public PktInfoConsumeItem items_;   // 소모 아이템 정보 목록
    public PktInfoGoods goods_;         // 소모된 비용이 적용된 현재 재화
};
// 아이템 획득 및 재화 정보 패킷 메시지
public class PktInfoRwdItemGoods : PktMsgType
{
    public PktInfoRwdItem items_;       // 획득한 아이템 정보
    public PktInfoGoods goods_;         // 결과 재화 정보
};
// 아이템 수량 패킷 메시지
public class PktInfoItemCnt : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 uid_;      // 아이템 고유 ID
        public System.UInt16 cnt_;      // 아이템 수량
        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            if (false == _s.Read(out cnt_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            _s.Write(cnt_);
        }
        public System.String GetStr() {
            return string.Format("UID:{0} cnt:{1} ", uid_, cnt_);
        }
    };
    public List<Piece> infos_;         // 아이템 수량 정보 목록
    public System.String GetStr() {
        System.String log = string.Format("[ ");
        foreach (var info in infos_)
            log += info.GetStr();
        log += string.Format("]");
        return log;
    }
};
// 아이템 TID 수량 패킷 메시지
public class PktInfoItemTIDCnt : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tid_;      // 아이템 테이블 ID
        public System.UInt16 cnt_;      // 아이템 수량
        public override bool Read(Message _s) {
            if (false == _s.Read(out tid_)) return false;
            if (false == _s.Read(out cnt_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tid_);
            _s.Write(cnt_);
        }
        public System.String GetStr() {
            return string.Format("UID:{0} cnt:{1} ", tid_, cnt_);
        }
    };
    public List<Piece> infos_;         // 아이템 수량 정보 목록
    public System.String GetStr() {
        System.String log = string.Format("[ ");
        foreach (var info in infos_)
            log += info.GetStr();
        log += string.Format("]");
        return log;
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 아이템 패킷 메시지
    public static bool Read(Message _s, out PktInfoItem.Piece _v) {
        _v = new PktInfoItem.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoItem.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoItem _v) {
        _v = new PktInfoItem();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoItem _v) {
        WriteList(_s, _v.infos_);
    }
    // 추가 아이템 패킷 메시지(보통 로그나 뷰어용으로 사용합니다.)
    public static bool Read(Message _s, out PktInfoAddItem _v) {
        _v = new PktInfoAddItem();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAddItem _v) {
        WriteList(_s, _v.infos_);
    }
    // 아이템 및 재화 소모 적용용 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdItem _v) {
        _v = new PktInfoRwdItem();
        if (false == Read(_s, out _v.rets_)) return false;
        return Read(_s, out _v.adds_);
    }
    public static void Write(Message _s, PktInfoRwdItem _v) {
        Write(_s, _v.rets_);
        Write(_s, _v.adds_);
    }
    // 소모 아이템 패킷 메시지
    public static bool Read(Message _s, out PktInfoConsumeItem _v) {
        _v = new PktInfoConsumeItem();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoConsumeItem _v) {
        WriteList(_s, _v.infos_);
    }
    // 아이템 및 재화 소모 적용용 패킷 메시지
    public static bool Read(Message _s, out PktInfoConsumeItemAndGoods _v) {
        _v = new PktInfoConsumeItemAndGoods();
        if (false == Read(_s, out _v.items_)) return false;
        return Read(_s, out _v.goods_);
    }
    public static void Write(Message _s, PktInfoConsumeItemAndGoods _v) {
        Write(_s, _v.items_);
        Write(_s, _v.goods_);
    }
    // 아이템 획득 및 재화 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdItemGoods _v) {
        _v = new PktInfoRwdItemGoods();
        if (false == Read(_s, out _v.items_)) return false;
        return Read(_s, out _v.goods_);
    }
    public static void Write(Message _s, PktInfoRwdItemGoods _v) {
        Write(_s, _v.items_);
        Write(_s, _v.goods_);
    }
    // 아이템 수량 패킷 메시지
    public static bool Read(Message _s, out PktInfoItemCnt.Piece _v) {
        _v = new PktInfoItemCnt.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoItemCnt.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoItemCnt _v) {
        _v = new PktInfoItemCnt();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoItemCnt _v) {
        WriteList(_s, _v.infos_);
    }
    // 아이템 TID 수량 패킷 메시지
    public static bool Read(Message _s, out PktInfoItemTIDCnt.Piece _v) {
        _v = new PktInfoItemTIDCnt.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoItemTIDCnt.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoItemTIDCnt _v) {
        _v = new PktInfoItemTIDCnt();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoItemTIDCnt _v) {
        WriteList(_s, _v.infos_);
    }
}
// 시간 패킷 메시지
public class PktInfoTime : PktMsgType
{
    public System.UInt64 time_;     // 년월일시분초를 연속된 숫자로 표현합니다. 예: (2019년 1월 1일 6시 30분 15초) -> 20190101063015000

    public System.Int32 Year()     { return (System.Int32)((time_ / ((System.UInt64)10000000000 * 1000))); }
    public System.Int32 Month()    { return (System.Int32)((time_ / ((System.UInt64)100000000   * 1000)) % 100); }
    public System.Int32 MDay()     { return (System.Int32)((time_ / ((System.UInt64)1000000     * 1000)) % 100); }
    public System.Int32 Hour()     { return (System.Int32)((time_ / ((System.UInt64)10000       * 1000)) % 100); }
    public System.Int32 Minute()   { return (System.Int32)((time_ / ((System.UInt64)100         * 1000)) % 100); }
    public System.Int32 Second()   { return (System.Int32)((time_ / ((System.UInt64)1           * 1000)) % 100); }
    public System.Int32 MilliSec() { return (System.Int32)((time_ / ((System.UInt64)1           * 1))    % 1000); }
    public System.DateTime GetTime()
    {
		if (time_ == 0)
			return new System.DateTime();
		else
		{
			int year = Year();
			int month = Month();
			int day = MDay();

			int daysInMonth = System.DateTime.DaysInMonth(year, month);
			if (day > daysInMonth)
			{
				day = daysInMonth;
			}

			return new System.DateTime(year, month, day, Hour(), Minute(), Second(), MilliSec(), System.DateTimeKind.Local);
		}
    }
    public System.String GetStr(bool format = false) {
        if (true == format)
        {
            System.DateTime dateTime = this.GetTime();
            System.String nowTime = dateTime.ToLongDateString() + dateTime.ToLongTimeString();
            return string.Format("TM:{0}", nowTime);
        }
        return string.Format("TM:{0}", time_);
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoTime _v) {
        _v = new PktInfoTime();
        if (false == _s.Read(out _v.time_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoTime _v) {
        _s.Write(_v.time_);
    }
}

// 경험치 레벨 패킷 메시지
public class PktInfoExpLv : PktMsgType
{
    public System.UInt32 exp_;  // 경험치
    public System.Byte lv_;     // 레벨
    public bool changeLv_;      // 레벨 변경 여부
    public bool changeFlag_;    // 값이 변경됐는지 여부
    public bool maxFlag_;       // 최대 레벨 여부

    public string GetExpLvStr() { return string.Format("[Lv:{0},Exp:{1}]", lv_, exp_); }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoExpLv _v) {
        _v = new PktInfoExpLv();
        if (false == _s.Read(out _v.exp_)) return false;
        if (false == _s.Read(out _v.lv_)) return false;
        if (false == _s.Read(out _v.changeLv_)) return false;
        if (false == _s.Read(out _v.changeFlag_)) return false;
        if (false == _s.Read(out _v.maxFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoExpLv _v) {
        _s.Write(_v.exp_);
        _s.Write(_v.lv_);
        _s.Write(_v.changeLv_);
        _s.Write(_v.changeFlag_);
        _s.Write(_v.maxFlag_);
    }
}

// 플래그 갱신 패킷 메시지
public class PktInfoFlagUpdate : PktMsgType
{
    public System.UInt32 flagIdx_;      // 변경될 플레그 인덱스
    public bool onFlag_;                // 플레그 켜고 끄는지 여부(true:켜기, false: 끄기)

    public string GetExpLvStr() { return string.Format("[FlagIdx:{0},On:{1}]", flagIdx_, onFlag_); }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoFlagUpdate _v) {
        _v = new PktInfoFlagUpdate();
        if (false == _s.Read(out _v.flagIdx_)) return false;
        if (false == _s.Read(out _v.onFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFlagUpdate _v) {
        _s.Write(_v.flagIdx_);
        _s.Write(_v.onFlag_);
    }
}

// 공용 시간과 테이블 ID 패킷 메시지
public class PktInfoComTimeAndTID : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime tm_;             // 시간
        public System.UInt32 tid_;          // 아이템 남은 수량
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out tm_)) return false;
            return _s.Read(out tid_);
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, tm_);
            _s.Write(tid_);
        }
    };
    public List<Piece> infos_;          // 시간 과 TID 목록
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 공용 시간과 테이블 ID 패킷 메시지
    public static bool Read(Message _s, out PktInfoComTimeAndTID.Piece _v) {
        _v = new PktInfoComTimeAndTID.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoComTimeAndTID.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoComTimeAndTID _v) {
        _v = new PktInfoComTimeAndTID();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoComTimeAndTID _v) {
        WriteList(_s, _v.infos_);
    }
}

// 주간 미션 기본 정보 패킷 메시지
public class PktInfoComWeekMission : PktMsgType
{
    public PktInfoTime resetTime_;  // 주간 미션 Set ID 초기화 예정 시간
    public System.UInt32 tableID_;  // 주간 미션 발송 우편 타입 ID

    public System.String GetStr() {
        return string.Format("SetID:{0} {1}", tableID_, resetTime_.GetStr());
    }
};
// 패스 정보 패킷 메시지
public class PktInfoPass : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime rwdSEndTM_;  // 특별 보상 구매 종료 시간
        public System.UInt32 tid_;      // 패스 테이블 ID
        public System.Byte stepN_;      // 패스 일반 보상 획득 단계
        public System.Byte stepS_;      // 패스 특별 보상 획득 단계
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out rwdSEndTM_)) return false;
            if (false == _s.Read(out tid_)) return false;
            if (false == _s.Read(out stepN_)) return false;
            return _s.Read(out stepS_);
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, rwdSEndTM_);
            _s.Write(tid_);
            _s.Write(stepN_);
            _s.Write(stepS_);
        }
    };
    public List<Piece> infos_;          // 시간 과 TID 목록
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 주간 미션 기본 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoComWeekMission _v) {
        _v = new PktInfoComWeekMission();
        if (false == Read(_s, out _v.resetTime_)) return false;
        if (false == _s.Read(out _v.tableID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoComWeekMission _v) {
        Write(_s, _v.resetTime_);
        _s.Write(_v.tableID_);
    }
    // 패스 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoPass.Piece _v) {
        _v = new PktInfoPass.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoPass.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoPass _v) {
        _v = new PktInfoPass();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoPass _v) {
        WriteList(_s, _v.infos_);
    }
}

// PVP 모드 시즌 시간 패킷 메시지
public class PktInfoArenaSeasonTime : PktMsgType
{
    enum STATE
    {
        START = 0,      // 시즌 진행 중
        END,            // 시즌 종료

        _MAX_
    };

    public PktInfoTime endTime_;        // 현재 시즌 종료 시간
    public PktInfoTime nextStartTime_;  // 다음 시즌 시작 시간
    public System.Byte seasonState_;    // 현재 시즌 진행상황

    public System.String GetStr() {
        return string.Format("SetTime:{0} {1}", endTime_.GetStr(), nextStartTime_.GetStr());
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoArenaSeasonTime _v) {
        _v = new PktInfoArenaSeasonTime();
        if (false == Read(_s, out _v.endTime_)) return false;
        if (false == Read(_s, out _v.nextStartTime_)) return false;
        if (false == _s.Read(out _v.seasonState_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaSeasonTime _v) {
        Write(_s, _v.endTime_);
        Write(_s, _v.nextStartTime_);
        _s.Write(_v.seasonState_);
    }
}


// RAID 모드 시즌 시간 패킷 메시지
public class PktInfoRaidSeasonTime : PktMsgType
{
    enum STATE
    {
        START = 0,      // 시즌 진행 중
        END,            // 시즌 종료

        _MAX_
    };

    public PktInfoTime endTime_;        // 현재 시즌 종료 시간
    public PktInfoTime nextStartTime_;  // 다음 시즌 시작 시간
    public System.Byte seasonState_;    // 현재 시즌 진행상황

    public System.String GetStr()
    {
        return string.Format("SetTime:{0} {1}", endTime_.GetStr(), nextStartTime_.GetStr());
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoRaidSeasonTime _v)
    {
        _v = new PktInfoRaidSeasonTime();
        if (false == Read(_s, out _v.endTime_)) return false;
        if (false == Read(_s, out _v.nextStartTime_)) return false;
        if (false == _s.Read(out _v.seasonState_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidSeasonTime _v)
    {
        Write(_s, _v.endTime_);
        Write(_s, _v.nextStartTime_);
        _s.Write(_v.seasonState_);
    }
}


// 제품 공용 성장 요청 패킷 메시지
public class PktInfoProductComGrowReq : PktMsgType
{
    public PktInfoUIDList maters_;          // 재료 제품 UID 목록 - (클라이언트에서 채워줘야 할 값)
    public PktInfoItemCnt materItems_;      // 재료 아이템 정보 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 targetUID_;        // 성장의 대상이 되는 제품의 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 tutoVal_;          // 튜토리얼 값 - (클라이언트에서 채워줘야 할 값)

    public System.String GetStr() {
        return string.Format("tgtUID:{0} maters:{1} materItems:{2}", targetUID_, maters_.GetStr(), maters_.GetStr());
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoProductComGrowReq _v) {
        _v = new PktInfoProductComGrowReq();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == Read(_s, out _v.materItems_)) return false;
        if (false == _s.Read(out _v.targetUID_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoProductComGrowReq _v) {
        Write(_s, _v.maters_);
        Write(_s, _v.materItems_);
        _s.Write(_v.targetUID_);
        _s.Write(_v.tutoVal_);
    }
}

// 제품 공용 성장 응답 패킷 메시지
public class PktInfoProductComGrowAck : PktMsgType
{
    public PktInfoUIDList maters_;          // 재료 제품 UID 목록
    public PktInfoConsumeItem materItems_;  // 재료 아이템 결과 정보 목록
    public PktInfoExpLv expLv_;             // 최종 경험치 레벨
    public System.UInt64 targetUID_;        // 성장의 대상이 되는 제품의 고유 ID
    public System.UInt64 userGold_;         // 성장 비용이 차감된 현재 유저의 금화

    public System.String GetStr() {
        return string.Format("tgtUID:{0} userGold:{1} {2} maters:{3} materItems:{4}", targetUID_, userGold_, expLv_.GetExpLvStr(), maters_.GetStr(), maters_.GetStr());
    }
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoProductComGrowAck _v) {
        _v = new PktInfoProductComGrowAck();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == Read(_s, out _v.materItems_)) return false;
        if (false == Read(_s, out _v.expLv_)) return false;
        if (false == _s.Read(out _v.targetUID_)) return false;
        if (false == _s.Read(out _v.userGold_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoProductComGrowAck _v) {
        Write(_s, _v.maters_);
        Write(_s, _v.materItems_);
        Write(_s, _v.expLv_);
        _s.Write(_v.targetUID_);
        _s.Write(_v.userGold_);
    }
}

// 카드(서포터) 도감 패킷 메시지
public class PktInfoCardBook : PktMsgType
{
    public class Piece : PktMsgType
    {
        public enum STATE : System.UInt32
        {
            NEW_CHK     = 0,                // 신규 획득 확인 여부
            MAX_WAKE_AND_LV,                // 최대 각성 및 레벨 달성 여부
            MAX_FAVOR_LV,                   // 최대 호감도 달성 여부

            _START_FAVOR_RWD_    = 11,
            FAVOR_RWD_GET_1  = _START_FAVOR_RWD_,   // 호감도 보상1 획득 여부
            FAVOR_RWD_GET_2,                        // 호감도 보상2 획득 여부
            FAVOR_RWD_GET_3,                        // 호감도 보상3 획득 여부
            FAVOR_RWD_GET_4,                        // 호감도 보상4 획득 여부
            FAVOR_RWD_GET_5,                        // 호감도 보상5 획득 여부
            _END_FAVOR_RWD_,
            
            _MAX_
        };

        public System.UInt32 tableID_;
        public System.UInt32 stateFlag_;    // 도감의 상태 플레그(신규, 최대 래벨 달성, 보상 획득 등)
        public System.UInt32 favorExp_;     // 호감도 경험치
        public System.Byte favorLv_;        // 호감도 레벨

        // 원하는 부분을 활성화 했는지 확인합니다.
        public bool IsOnFlag(int _flagIdx /*PktInfoCardBook.Piece.STATE*/)
        {
            if (32 <= _flagIdx) return false;
            return _IsOnBitIdx(stateFlag_, (uint)_flagIdx);
        }
        // 원하는 부분을 활성화 했는지 확인합니다.
        public void DoOnFlag(int _flagIdx /*PktInfoCardBook.Piece.STATE*/)
        {
            if (32 <= _flagIdx) return;
            _DoOnBitIdx(ref stateFlag_, (uint)_flagIdx);
        }
        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out stateFlag_)) return false;
            if (false == _s.Read(out favorExp_)) return false;
            if (false == _s.Read(out favorLv_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(stateFlag_);
            _s.Write(favorExp_);
            _s.Write(favorLv_);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 카드 도감 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardBook.Piece _v) {
        _v = new PktInfoCardBook.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoCardBook.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoCardBook _v) {
        _v = new PktInfoCardBook();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoCardBook _v) {
        WriteList(_s, _v.infos_);
    }
}

// 무기 도감 패킷 메시지
public class PktInfoWeaponBook : PktMsgType
{
    public class Piece : PktMsgType
    {
        public enum STATE : System.UInt32 {
            NEW_CHK     = 0,            // 신규 획득 확인 여부
            MAX_WAKE_AND_LV,
            
            _MAX_
        };

        public System.UInt32 tableID_;
        public System.UInt32 stateFlag_;    // 도감의 상태 플레그(신규, 최대 래벨 달성 등)

        // 원하는 부분을 활성화 했는지 확인합니다.
        public bool IsOnFlag(int _flagIdx /*PktInfoWeaponBook.Piece.STATE*/)
        {
            if (32 <= _flagIdx)
                return false;
            return _IsOnBitIdx(stateFlag_, (uint)_flagIdx);
        }
        // 원하는 부분을 활성화 했는지 확인합니다.
        public void DoOnFlag(int _flagIdx /*PktInfoWeaponBook.Piece.STATE*/)
        {
            if (32 <= _flagIdx) return;
            _DoOnBitIdx(ref stateFlag_, (uint)_flagIdx);
        }

        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out stateFlag_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(stateFlag_);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 무기 도감 패킷 메시지
    public static bool Read(Message _s, out PktInfoWeaponBook.Piece _v) {
        _v = new PktInfoWeaponBook.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWeaponBook.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWeaponBook _v) {
        _v = new PktInfoWeaponBook();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoWeaponBook _v) {
        WriteList(_s, _v.infos_);
    }
}

// 카드(서포터) 도감 호감도 레벨 보상 요청 패킷 메시지
public class PktInfoBookOnStateReq : PktMsgType
{
    public PktInfoComRwd idxs_;         // 보상을 수령할 인덱스 목록
    public System.UInt32 tid_;          // 해당 도감 테이블 ID
};
// 카드(서포터) 도감 호감도 레벨 보상 응답 패킷 메시지
public class PktInfoBookChangeState : PktMsgType
{
    public System.UInt32 tid_;          // 해당 도감 테이블 ID
    public System.UInt32 stateFlag_;    // 변경된 도감 상태 플레그
};

// 스테이지 패킷 메시지
public class PktInfoStage : PktMsgType
{
    public class Special : PktMsgType {
     
        public PktInfoTime nextTime_;
        public System.UInt32 tableID_;      // 보상 획득 여부

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out nextTime_)) return false;
            return _s.Read(out tableID_);
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, nextTime_);
            _s.Write(tableID_);
        }
    };
    public Special special_;
    public System.Byte arenaProc_;      // 아레나 실행값
};


// 레이드에 대한 정보 저장 패킷 메시지,  개별 스테이지가아니라 공용 정보
public class PktInfoRaidData : PktMsgType
{
    public PktInfoTime lastPlaySeasonEndTime_;          // 마지막 플레이한 시즌 종료시간
    public System.UInt16 OpenLevel_;                    // 레이드 으픈된 최고 단계(레벨)
};

// 시크릿 퀘스트 옵션 패킷 메시지
public class PktInfoSecretQuestOpt : PktMsgType
{
    public class Piece : PktMsgType {
     
        public System.UInt32 groupID_;      // 그룹 ID
        public System.UInt16 boSet_;        // 배틀 옵션 셋
        public System.Byte lvNo_;           // 레벨 번호

        public override bool Read(Message _s) {
            if (false == _s.Read(out groupID_)) return false;
            if (false == _s.Read(out boSet_)) return false;
            return _s.Read(out lvNo_);
        }
        public override void Write(Message _s) {
            _s.Write(groupID_);
            _s.Write(boSet_);
            _s.Write(lvNo_);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{

    // 스테이지 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidData _v)
    {
        _v = new PktInfoRaidData();
        if (false == PN_MarshalerEx.Read(_s, out _v.lastPlaySeasonEndTime_)) return false;
        if (false == _s.Read(out _v.OpenLevel_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidData _v)
    {
        PN_MarshalerEx.Write(_s, _v.lastPlaySeasonEndTime_);
        _s.Write(_v.OpenLevel_);
    }

    // 스테이지 패킷 메시지
    public static bool Read(Message _s, out PktInfoStage.Special _v) {
        _v = new PktInfoStage.Special();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoStage.Special _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoStage _v) {
        _v = new PktInfoStage();
        if (false == Read(_s, out _v.special_)) return false;
        if (false == _s.Read(out _v.arenaProc_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStage _v) {
        Write(_s, _v.special_);
        _s.Write(_v.arenaProc_);
    }
    // 시크릿 퀘스트 옵션 패킷 메시지
    public static bool Read(Message _s, out PktInfoSecretQuestOpt.Piece _v) {
        _v = new PktInfoSecretQuestOpt.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoSecretQuestOpt.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoSecretQuestOpt _v) {
        _v = new PktInfoSecretQuestOpt();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoSecretQuestOpt _v) {
        WriteList(_s, _v.infos_);
    }
}

// 콘텐츠 슬롯 위치 정보 패킷 메시지
public class PktInfoContentsSlotPos : PktMsgType
{
    public class Piece : PktMsgType {
     
        public System.UInt64 uid_;      // 위치 적용이될 대상의 고유 ID
        public System.UInt64 value_;    // 적용될 콘텐츠의 값
        public eContentsPosKind kind_;  // 적용될 콘텐츠 분류
        public System.Byte slotNum_;    // 적용될 슬롯 번호

        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            if (false == _s.Read(out value_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out kind_)) return false;
            return _s.Read(out slotNum_);
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            _s.Write(value_);
            PN_MarshalerEx.Write(_s, kind_);
            _s.Write(slotNum_);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 콘텐츠 슬롯 위치 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoContentsSlotPos.Piece _v) {
        _v = new PktInfoContentsSlotPos.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoContentsSlotPos.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoContentsSlotPos _v) {
        _v = new PktInfoContentsSlotPos();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoContentsSlotPos _v) {
        WriteList(_s, _v.infos_);
    }
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 카드(서포터) 도감 호감도 레벨 보상 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoBookOnStateReq _v) {
        _v = new PktInfoBookOnStateReq();
        if (false == Read(_s, out _v.idxs_)) return false;
        if (false == _s.Read(out _v.tid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBookOnStateReq _v) {
        Write(_s, _v.idxs_);
        _s.Write(_v.tid_);
    }
    // 카드(서포터) 도감 호감도 레벨 보상 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoBookChangeState _v) {
        _v = new PktInfoBookChangeState();
        if (false == _s.Read(out _v.tid_)) return false;
        if (false == _s.Read(out _v.stateFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBookChangeState _v) {
        _s.Write(_v.tid_);
        _s.Write(_v.stateFlag_);
    }
}

// 운영 테이블 배너 정보 패킷
public class PktInfoTblSvrStoreSale : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 TableID;               // 상점 테이블 ID
        public System.UInt32 LimitValue;            // 제한 타입 값
        public System.Byte DiscountRate;            // 할인률 (1=1%) (할인률이 0이 아닌 경우 제한에 해당해도 할인만 안될 뿐 구매 가능)
        public System.Byte LimitType;               // 구조체 바이트라인 (64비트 주소에 맞춥니다.)
        public override bool Read(Message _s) {
            if (false == _s.Read(out TableID)) return false;
            if (false == _s.Read(out LimitValue)) return false;
            if (false == _s.Read(out DiscountRate)) return false;
            if (false == _s.Read(out LimitType)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(TableID);
            _s.Write(LimitValue);
            _s.Write(DiscountRate);
            _s.Write(LimitType);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoTblSvrStoreSale.Piece _v) {
        _v = new PktInfoTblSvrStoreSale.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTblSvrStoreSale.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTblSvrStoreSale _v) {
        _v = new PktInfoTblSvrStoreSale();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTblSvrStoreSale _v) {
        WriteList(_s, _v.infos_);
    }
}
// 운영 테이블 배너 정보 패킷
public class PktInfoTblSvrBanner : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoStr8 URL;             // 이미지 경로
        public PktInfoStr8 URL_Add1;        // 추가 이미지 경로1
        public PktInfoStr8 URL_Add2;        // 추가 이미지 경로2
        public PktInfoStr8 FuncTP;          // eUIMOVEFUNCTIONTYPE = UIPANEL,UIPOPUP,WEBVIEW 스트링으로 입력
        public PktInfoStr8 FuncVal1;        // 바로가기 위치 입력(카테고리)
        public PktInfoStr8 FuncVal2;        // 바로가기 위치 입력(세부 항목)
        public PktInfoStr8 FuncVal3;        // GameClient.Acquisition.Value3 으로 사용
        public PktInfoStr8 TagMark;         // 태그 마크 종류 입력
        public PktInfoStr8 Local;           // 로컬 대응 여부
        public PktInfoTime StartDate;       // 시작 시간
        public PktInfoTime EndDate;         // 종료 시간
        public System.UInt32 Name;          // 이름 라벨
        public System.UInt32 Desc;          // 혜택이나 추가 설명
        public System.UInt32 TypeValue;     // 배너 타입별 값
        public System.Byte Type;            // 배너 타입
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out StartDate)) return false;
            if (false == PN_MarshalerEx.Read(_s, out EndDate)) return false;
            if (false == _s.Read(out Name)) return false;
            if (false == _s.Read(out Desc)) return false;
            if (false == _s.Read(out TypeValue)) return false;
            if (false == _s.Read(out Type)) return false;
            if (false == PN_MarshalerEx.Read(_s, out URL)) return false;
            if (false == PN_MarshalerEx.Read(_s, out URL_Add1)) return false;
            if (false == PN_MarshalerEx.Read(_s, out URL_Add2)) return false;
            if (false == PN_MarshalerEx.Read(_s, out FuncTP)) return false;
            if (false == PN_MarshalerEx.Read(_s, out FuncVal1)) return false;
            if (false == PN_MarshalerEx.Read(_s, out FuncVal2)) return false;
            if (false == PN_MarshalerEx.Read(_s, out FuncVal3)) return false;
            if (false == PN_MarshalerEx.Read(_s, out TagMark)) return false;
            if (false == PN_MarshalerEx.Read(_s, out Local)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, StartDate);
            PN_MarshalerEx.Write(_s, EndDate);
            _s.Write(Name);
            _s.Write(Desc);
            _s.Write(TypeValue);
            _s.Write(Type);
            PN_MarshalerEx.Write(_s, URL);
            PN_MarshalerEx.Write(_s, URL_Add1);
            PN_MarshalerEx.Write(_s, URL_Add2);
            PN_MarshalerEx.Write(_s, FuncTP);
            PN_MarshalerEx.Write(_s, FuncVal1);
            PN_MarshalerEx.Write(_s, FuncVal2);
            PN_MarshalerEx.Write(_s, FuncVal3);
            PN_MarshalerEx.Write(_s, TagMark);
            PN_MarshalerEx.Write(_s, Local);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoTblSvrBanner.Piece _v) {
        _v = new PktInfoTblSvrBanner.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTblSvrBanner.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTblSvrBanner _v) {
        _v = new PktInfoTblSvrBanner();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTblSvrBanner _v) {
        WriteList(_s, _v.infos_);
    }
}
// 운영 테이블 게릴라 캠페인 상점 정보 패킷
public class PktInfoTblSvrGuerrillaCamp : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoStr8 Type;            // 타입
        public PktInfoTime StartDate;       // 시작 시간
        public PktInfoTime EndDate;         // 종료 시간
        public System.UInt32 Condition;     // 조건 값
        public System.UInt32 EffectValue;   // 효과 값
        public System.UInt32 Title;         // 제목
        public System.UInt32 Desc;          // 설명
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out Type)) return false;
            if (false == PN_MarshalerEx.Read(_s, out StartDate)) return false;
            if (false == PN_MarshalerEx.Read(_s, out EndDate)) return false;
            if (false == _s.Read(out Condition)) return false;
            if (false == _s.Read(out EffectValue)) return false;
            if (false == _s.Read(out Title)) return false;
            if (false == _s.Read(out Desc)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, Type);
            PN_MarshalerEx.Write(_s, StartDate);
            PN_MarshalerEx.Write(_s, EndDate);
            _s.Write(Condition);
            _s.Write(EffectValue);
            _s.Write(Title);
            _s.Write(Desc);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoTblSvrGuerrillaCamp.Piece _v) {
        _v = new PktInfoTblSvrGuerrillaCamp.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTblSvrGuerrillaCamp.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTblSvrGuerrillaCamp _v) {
        _v = new PktInfoTblSvrGuerrillaCamp();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTblSvrGuerrillaCamp _v) {
        WriteList(_s, _v.infos_);
    }
}
// 운영 테이블 상점 정보 패킷
public class PktInfoTblSvrGuerrillaStore : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoStr8 Type;            // eUIMOVEFUNCTIONTYPE = UIPANEL,UIPOPUP,WEBVIEW 스트링으로 입력
        public PktInfoStr8 UrlBtnImage;     // 탭버튼에 사용할 텍스쳐
        public PktInfoStr8 UrlBGImage;      // BG에 사용될 텍스쳐
        public PktInfoStr8 UrlAddImage;     // 이미지를 추가할 수 있게 적용
        public PktInfoStr8 Local;           // 로컬 대응 여부
        public PktInfoStr8 Val1;            // 태그 마크 종류 입력
        public PktInfoTime StartDate;       // 시작 시간
        public PktInfoTime EndDate;         // 종료 시간
        public System.UInt32 StoreID1;      // 상점 ID 1
        public System.UInt32 StoreID2;      // 상점 ID 2
        public System.UInt32 StoreID3;      // 상점 ID 3
        public System.UInt32 StoreID4;      // 상점 ID 4
        public System.UInt32 Name;          // 이름 라벨
        public System.UInt32 Desc;          // 혜택이나 추가 설명
        public System.UInt32 Text;                  
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out StartDate)) return false;
            if (false == PN_MarshalerEx.Read(_s, out EndDate)) return false;
            if (false == _s.Read(out StoreID1)) return false;
            if (false == _s.Read(out StoreID2)) return false;
            if (false == _s.Read(out StoreID3)) return false;
            if (false == _s.Read(out StoreID4)) return false;
            if (false == _s.Read(out Desc)) return false;
            if (false == _s.Read(out Text)) return false;
            if (false == _s.Read(out Name)) return false;
            if (false == PN_MarshalerEx.Read(_s, out Type)) return false;
            if (false == PN_MarshalerEx.Read(_s, out UrlBtnImage)) return false;
            if (false == PN_MarshalerEx.Read(_s, out UrlBGImage)) return false;
            if (false == PN_MarshalerEx.Read(_s, out UrlAddImage)) return false;
            if (false == PN_MarshalerEx.Read(_s, out Local)) return false;
            if (false == PN_MarshalerEx.Read(_s, out Val1)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, StartDate);
            PN_MarshalerEx.Write(_s, EndDate);
            _s.Write(StoreID1);
            _s.Write(StoreID2);
            _s.Write(StoreID3);
            _s.Write(StoreID4);
            _s.Write(Name);
            _s.Write(Desc);
            _s.Write(Text);
            PN_MarshalerEx.Write(_s, Type);
            PN_MarshalerEx.Write(_s, UrlBtnImage);
            PN_MarshalerEx.Write(_s, UrlBGImage);
            PN_MarshalerEx.Write(_s, UrlAddImage);
            PN_MarshalerEx.Write(_s, Local);
            PN_MarshalerEx.Write(_s, Val1);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoTblSvrGuerrillaStore.Piece _v) {
        _v = new PktInfoTblSvrGuerrillaStore.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTblSvrGuerrillaStore.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTblSvrGuerrillaStore _v) {
        _v = new PktInfoTblSvrGuerrillaStore();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTblSvrGuerrillaStore _v) {
        WriteList(_s, _v.infos_);
    }
}

// 운영 테이블 게릴라 미션 패킷
public class PktInfoTblSvrGuerrillaMission : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoStr8 Type;              // eGuerrillaMissionType
        public PktInfoTime StartDate;           // 시작 시간
        public PktInfoTime EndDate;             // 종료 시간
        public System.UInt32 Condition;         // 타입별 조건
        public System.UInt32 Count;             // 필요 횟수
        public System.UInt32 Name;              // 제목
        public System.UInt32 Desc;              // 설명
        public System.UInt32 GroupID;           // 그룹ID
        public System.UInt32 GroupOrder;        // 그룹 내 순서
        public System.UInt32 RewardType;        // 보상 타입
        public System.UInt32 RewardIndex;       // 보상 인덱스
        public System.UInt32 RewardValue;       // 보상 수량
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out Type)) return false;
            if (false == PN_MarshalerEx.Read(_s, out StartDate)) return false;
            if (false == PN_MarshalerEx.Read(_s, out EndDate)) return false;
            if (false == _s.Read(out Condition)) return false;
            if (false == _s.Read(out Count)) return false;
            if (false == _s.Read(out Name)) return false;
            if (false == _s.Read(out Desc)) return false;
            if (false == _s.Read(out GroupID)) return false;
            if (false == _s.Read(out GroupOrder)) return false;
            if (false == _s.Read(out RewardType)) return false;
            if (false == _s.Read(out RewardIndex)) return false;
            if (false == _s.Read(out RewardValue)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, Type);
            PN_MarshalerEx.Write(_s, StartDate);
            PN_MarshalerEx.Write(_s, EndDate);
            _s.Write(Condition);
            _s.Write(Count);
            _s.Write(Name);
            _s.Write(Desc);
            _s.Write(GroupID);
            _s.Write(GroupOrder);
            _s.Write(RewardType);
            _s.Write(RewardIndex);
            _s.Write(RewardValue);
        }
    };
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoTblSvrGuerrillaMission.Piece _v) {
        _v = new PktInfoTblSvrGuerrillaMission.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTblSvrGuerrillaMission.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTblSvrGuerrillaMission _v) {
        _v = new PktInfoTblSvrGuerrillaMission();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTblSvrGuerrillaMission _v) {
        WriteList(_s, _v.infos_);
    }
}

// 서버에서 갱신될 수 있는 공용 정보
public class PktInfoUserReflash : PktMsgType
{
    public PktInfoTblSvrBanner banner_;         // 운영 배너 정보 목록
    public PktInfoTblSvrGuerrillaCamp campaign_;    // 운영 게릴라 캠페인 정보 목록
    public PktInfoTblSvrGuerrillaStore store_;  // 운영 상점 정보 목록
    public PktInfoTblSvrGuerrillaMission mission_;  // 운영 미션 정보 목록
    public PktInfoTblSvrStoreSale sale_;        // 상점 세일 정보 목록
    public override bool Read(Message _s) {
        if (false == PN_MarshalerEx.Read(_s, out banner_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out campaign_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out store_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out mission_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out sale_)) return false;
        return true;
    }
    public override void Write(Message _s) {
        PN_MarshalerEx.Write(_s, banner_);
        PN_MarshalerEx.Write(_s, campaign_);
        PN_MarshalerEx.Write(_s, store_);
        PN_MarshalerEx.Write(_s, mission_);
        PN_MarshalerEx.Write(_s, sale_);
    }
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUserReflash _v) {
        _v = new PktInfoUserReflash();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoUserReflash _v) {
        _v.Write(_s);
    }
}

public class PktInfoBlackList : PktMsgType
{
	public System.Byte	level_;
	public PktInfoTime	blockEndTime_;

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out level_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out blockEndTime_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(level_);
		PN_MarshalerEx.Write(_s, blockEndTime_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoBlackList _v)
	{
		_v = new PktInfoBlackList();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoBlackList _v)
	{
		_v.Write(_s);
	}
}
///
///////////////////////////////////////////////////////////////////////