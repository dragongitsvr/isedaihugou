using Assets.Photon.Argencies;
using Assets.Photon.Services;
using Assets.Services;
using Cysharp.Threading.Tasks;
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
        async void Start()
        {
            var userId = TitleLobbyArgency.UserId;
            var loadingService = gameObject.GetComponent<LoadingService>();

            try
            {
                loadingService.ShowLoading();

                // ログインユーザの表示
                _dspUserId.text = userId;

                // インスタンス※MonoBehaviourを継承している場合は、new禁止
                var resultService = gameObject.GetComponent<ResultService>();
                await resultService.Init(userId);

                loadingService.CloseLoading();

            }
            catch(Exception e) 
            {
                Debug.LogError(e.StackTrace);
            }   

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
