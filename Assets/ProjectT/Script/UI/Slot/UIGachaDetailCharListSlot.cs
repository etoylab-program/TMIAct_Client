
using UnityEngine;
using System.Collections;


public class UIGachaDetailCharListSlot : FSlot {
	[SerializeField] private UITexture _TexChar;


	public void UpdateSlot( int index, string charIconName )
	{
		_TexChar.SetActive( true );

		if( string.IsNullOrEmpty( charIconName ) ) {
			_TexChar.mainTexture = null;
		}
		else {
			_TexChar.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Set/Set_Chara_" + charIconName + ".png" );
		}
	}
}
