using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Assets.Photon.Argencies;
using System;
using Photon.Services;
using Assets.Services;

namespace Assets.Photon.Controllers
{
    public class MatchingController : MonoBehaviourPunCallbacks
    {
        // 表示フィールド
        [SerializeField] public Text _dspUserId;

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        void Start()
        {
            var matchingService = gameObject.GetComponent<MatchingService>();

            try
            {
                // ログインユーザの表示
                _dspUserId.text = LobbyMatchingArgency.UserId;
                matchingService.ConnectToPhotonServer(LobbyMatchingArgency.UserId, LobbyMatchingArgency.NeedCreateRoomFlg);
            }
            catch(Exception e)
            {
                Debug.LogError(e.StackTrace);
            }

        }

        /// <summary>
        /// 「準備完了」ボタン押下時の処理
        /// </summary>
        public void BtnReady_Clicked()
        {
            var matchingService = gameObject.GetComponent<MatchingService>();

            try
            {
                matchingService.OnBtnReadyClicked(LobbyMatchingArgency.UserId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }

        }

        /// <summary>
        /// 「退出」ボタン押下時の処理
        /// </summary>
        public void BtnLeave_Clicked()
        {
            var matchingService = gameObject.GetComponent<MatchingService>();

            try
            {
                matchingService.OnBtnLeaveClicked(LobbyMatchingArgency.UserId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }

        }

    }
}