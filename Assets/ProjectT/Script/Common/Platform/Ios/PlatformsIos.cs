/// <summary>
/// Platform 관련 Ios 관련 작업 구현 클래스
/// </summary>
namespace Platforms
{
    public class PlatformsIos : IBase
    {
#if UNITY_IOS
        public override System.String GetDeviceUniqueID() { return UnityEngine.iOS.Device.vendorIdentifier; }
#endif
    }
}