using Assets.Photon.Argencies;
using Assets.Services;
using Photon.Services;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Photon.Controllers
{
    internal class ResultController : MonoBehaviour
    {
        // 表示フィールド
        [SerializeField] public Text _dspUserId;

        /// <summary>
        /// 初期処理
        /// </summary>
        void Start()
        {
            var userId = TitleLobbyArgency.UserId;

            // ログインユーザの表示
            _dspUserId.text = userId;

            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var resultService = gameObject.GetComponent<ResultService>();
            resultService.Init(userId);

        }

        /// <summary>
        /// 「戻る」ボタン押下時の処理
        /// </summary>
        public void BtnBack_Clicked()
        {
            try
            {
                // インスタンス※MonoBehaviourを継承している場合は、new禁止
                var resultService = gameObject.GetComponent<ResultService>();

                resultService.LoadLobby();

            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

    }
}
