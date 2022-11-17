using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Assets.Photon.Argencies;
using System;
using Assets.Services;

namespace Assets.Photon.Controllers
{
    public class LobbyController : MonoBehaviourPunCallbacks
    {
        // 表示フィールド
        [SerializeField] public Text _dspUserId;

        /// <summary>
        /// 初期処理
        /// </summary>
        void Start()
        {
            try
            {
                // ログインユーザの表示
                _dspUserId.text = TitleLobbyArgency.UserId;
            }
            catch(Exception e)
            {
                Debug.LogError(e.StackTrace);
            }

        }

        /// <summary>
        /// 「ルーム作成」ボタン押下時の処理
        /// </summary>
        public void BtnCreateRoom_Clicked()
        {
            try
            {
                // インスタンス※MonoBehaviourを継承している場合は、new禁止
                var lobbyService = gameObject.AddComponent<LobbyService>();

                lobbyService.ConnectToPhotonServer(_dspUserId.text,true);

            }
            catch(Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

        /// <summary>
        /// 「ルーム入室」ボタン押下時の処理
        /// </summary>
        public void BtnJoinRoom_Clicked()
        {
            try
            {
                // インスタンス※MonoBehaviourを継承している場合は、new禁止
                var lobbyService = gameObject.AddComponent<LobbyService>();

                lobbyService.ConnectToPhotonServer(_dspUserId.text,false);

            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

    }
}