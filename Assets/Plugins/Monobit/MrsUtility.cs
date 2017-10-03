using System;

namespace mrs {
public class Utility {
    public static void LoadScene( String name ){
        #if UNITY_5_3_OR_NEWER || UNITY_5_3
            UnityEngine.SceneManagement.SceneManager.LoadScene( name );
        #else
            UnityEngine.Application.LoadLevel( name );
        #endif
    }
    
    public static String ToHex( byte[] data, UInt32 data_len ){
        System.Text.StringBuilder string_builder = new System.Text.StringBuilder( (int)( data_len * 2 ) );
        for ( UInt32 i = 0; i < data_len; ++i ){
            string_builder.Append( String.Format( "{0:X2}", data[ i ] ) );
        }
        return string_builder.ToString();
    }
    
    public static String ToHex( byte[] data ){
        return ToHex( data, (UInt32)data.Length );
    }
}
}
