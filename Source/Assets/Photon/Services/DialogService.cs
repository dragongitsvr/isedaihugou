using UnityEditor;

namespace Photon.Services
{
    public class DialogService
    {
        /// <summary>
        /// 「OK」ボタンのダイアログを表示
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        public void OpenOkDialog(string title, string message)
        {
            #if UNITY_EDITOR
                EditorUtility.DisplayDialog(title, message, "OK");
            #endif
        }
    }
}
