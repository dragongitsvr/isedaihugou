using Assets.Photon.Argencies;
using Photon.Commons;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

namespace Assets.Services
{
    public class LobbyService : MonoBehaviourPunCallbacks
    {
        // グローバル変数
        private string _userId;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            // マスタークライアントのsceneと同じsceneを部屋に入室した人もロードする
            // マスタークライアントはルームを作ったクライアント
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// Photonサーバーに接続
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void ConnectToPhotonServer(string userId)
        {
            // 「マッチング画面」に渡す
            _userId = userId;

            // Photonに接続されているかどうか
            if (!PhotonNetwork.IsConnected)
            {
                // PhotonServerSettingsに設定した内容を使って
                // マスターサーバーへ接続する
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        /// <summary>
        /// Photonに接続した時
        /// </summary>
        public override void OnConnected()
        {
            UnityEngine.Debug.Log("Photonに接続");
        }

        /// <summary>
        /// マスターサーバーへ接続した時
        /// </summary>
        public override void OnConnectedToMaster()
        {
            UnityEngine.Debug.Log("マスターサーバーへ接続");

            //ロビーに入る
            PhotonNetwork.JoinLobby();

        }

        /// <summary>
        /// ロビーに入ったとき
        /// </summary>
        public override void OnJoinedLobby()
        {
            UnityEngine.Debug.Log("ロビーに入った");
            var roomOptions = new RoomOptions
            {
                MaxPlayers = Convert.ToByte(Const.MAX_PLAYERES),
                IsOpen = true,
                IsVisible = false,
                PublishUserId = true
            };

            // 部屋を作成
            PhotonNetwork.CreateRoom(Const.ROOM_NAME, roomOptions, null);

        }

        /// <summary>
        /// 部屋を作成したとき
        /// </summary>
        public override void OnCreatedRoom()
        {
            UnityEngine.Debug.Log("部屋を作成");
        }

        /// <summary>
        /// ルームに参加したときのコールバック関数
        /// </summary>
        public override void OnJoinedRoom()
        {
            // マッチング画面に遷移
            TitleLobbyArgency.UserId = _userId;
            SceneManager.LoadScene(Const.SCENE_NAME_MATCHING);

        }

    }
}
