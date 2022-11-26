using Assets.Photon.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Services
{
    public class DialogService : MonoBehaviour
    {
        // Canvasの表示判定
        public Canvas dialogCanvasDelete = null;

        // ダイアログの文言
        public Text _title;
        public Text _message;

        /// <summary>
        /// 初期処理
        /// </summary>
        public void Init()
        {
            DeleteDialog();
        }

        /// <summary>
        /// 「OK」ボタンのダイアログを表示
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        public void OpenOkDialog(string title, string message)
        {
            // 値の設定
            _title.text = title;
            _message.text = message;

            dialogCanvasDelete.sortingOrder = 2;
            dialogCanvasDelete.enabled = true;

        }

        /// <summary>
        /// 「OK」ボタン押下時の処理
        /// </summary>
        public void BtnOk_Clicked()
        {
            DeleteDialog();
        }

        /// <summary>
        /// ダイアログを削除
        /// </summary>
        private void DeleteDialog()
        {
            var loadingService = gameObject.GetComponent<LoadingService>();
            loadingService.CloseLoading();
            dialogCanvasDelete.sortingOrder = 0;
            dialogCanvasDelete.enabled = false;
        }

    }
}
