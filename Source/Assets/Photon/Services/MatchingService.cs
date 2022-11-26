using Assets.Photon.Argencies;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Commons;
using Photon.Messages;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Photon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Services
{
    public class MatchingService : MonoBehaviourPunCallbacks
    {
        bool _needCreatedRoomFlg;
        private readonly byte BtnReadyEvent = 1;
        public PhotonView _photonView;
        public Text _firstPlayer;
        public Text _secondPlayer;
        public Text _thirdPlayer;
        public Text _fourthPlayer;
        public Image _firstPlayerFlame;
        public Image _secondPlayerFlame;
        public Image _thirdPlayerFlame;
        public Image _fourthPlayerFlame;
        public string _userId;

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
        /// ルーム参加に失敗
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // ダイアログ表示
            DialogService dialogService = new();
            dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE, DialogMessage.ERR_MSG_JOIN_ROOM_FAILED);

        }

        /// <summary>
        /// Photonサーバーに接続
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="needCreatedRoomFlg">ルームを作成するかどうかのフラグ</param>
        public void ConnectToPhotonServer(string userId, bool needCreatedRoomFlg)
        {
            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var dialogService = gameObject.GetComponent<DialogService>();
            dialogService.Init();

            // ルーム作成の判定
            _needCreatedRoomFlg = needCreatedRoomFlg;

            // Photonに接続されているかどうか
            if (!PhotonNetwork.IsConnected)
            {
                // PhotonServerSettingsに設定した内容を使って
                // マスターサーバーへ接続する
                PhotonNetwork.ConnectUsingSettings();

                PhotonNetwork.NickName = userId;
            }
        }

        /// <summary>
        /// ロビーに入ったとき
        /// </summary>
        public override void OnJoinedLobby()
        {
            UnityEngine.Debug.Log("ロビーに入った");

            // ルーム入室の場合は部屋を作成しない
            if (!_needCreatedRoomFlg)
            {
                PhotonNetwork.JoinRoom(Const.ROOM_NAME);
            }
            else
            {
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
            //CleanUpList();
            Debug.Log("部屋に入室");
            UpdateMatching();
        }

        /// <summary>
        /// ルームから退出したときのコールバック関数
        /// </summary>
        public override void OnLeftRoom()
        {
            // ロビー画面に遷移
            LobbyMatchingArgency.UserId = _userId;
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(Const.SCENE_NAME_LOBBY);
        }

        /// <summary>
        /// 新規プレイヤーがルームに参加したときのコールバック関数
        /// </summary>
        /// <param name="newPlayer">新規プレイヤー</param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdateMatching();
        }

        /// <summary>
        /// 既存プレイヤーがルームか退出したときのコールバック関数
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdateMatching();
        }

        /// <summary>
        /// マスターが変更されたときのコールバック関数
        /// </summary>
        /// <param name="newMasterClient"></param>
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            UpdateMatching();
        }

        /// <summary>
        /// 待機画面の更新
        /// </summary>
        private void UpdateMatching()
        {
            // 入った人から上から順番に表示するためソート
            var playerList = PhotonNetwork.PlayerList.OrderBy(x => x.ActorNumber).ToList();

            var matchingTexts = new List<Text>()
            {
                _firstPlayer
                , _secondPlayer
                , _thirdPlayer
                , _fourthPlayer
            };

            var matchingFrames = new List<Image>()
            {
                _firstPlayerFlame
                , _secondPlayerFlame
                , _thirdPlayerFlame
                , _fourthPlayerFlame
            };

            // 名前を表示
            for (var i = 0; i < playerList.Count(); i++)
            {
                matchingTexts[i].text = playerList[i].NickName;
            }

            // プレイヤーが揃っていない場合
            var playerCount = playerList.Count;
            if (playerList.Count != matchingTexts.Count)
            {
                for (var i = playerList.Count + 1; i <= matchingTexts.Count; i++)
                {
                    matchingFrames[i - 1].enabled = false;
                    matchingTexts[i - 1].text = "待機中...";
                }
            }

        }

        /// <summary>
        /// 部屋作成に失敗したときのコールバック関数
        /// </summary>
        /// <param name="returnCode">エラーコード</param>
        /// <param name="message">エラーメッセージ</param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            PhotonNetwork.JoinRoom(Const.ROOM_NAME);
            //// 部屋作成に失敗
            //var dialogService = gameObject.GetComponent<DialogService>();           
            //dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE, DialogMessage.ERR_MSG_CREATE_ROOM_FAILED);
        }

        /// <summary>
        /// 「準備完了」ボタン押下時の処理
        /// </summary>
        public void OnBtnReadyClicked(string userId)
        {
            var properties = new Hashtable
            {
                { "btnReadyClickedPlayer", userId },
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable hashTable)
        {
            string userId = hashTable["btnReadyClickedPlayer"].ToString();

            // 自分が何番目にいるかを確認して、赤枠表示
            var matchingTexts = new List<Text>()
            {
                _firstPlayer
                , _secondPlayer
                , _thirdPlayer
                , _fourthPlayer
            };

            var matchingFrames = new List<Image>()
            {
                _firstPlayerFlame
                , _secondPlayerFlame
                , _thirdPlayerFlame
                , _fourthPlayerFlame
            };

            for (var i = 0; i < matchingTexts.Count(); i++)
            {
                if (matchingTexts[i].text == userId)
                {
                    matchingFrames[i].enabled = !matchingFrames[i].enabled;
                }
            }
        }

        /// <summary>
        /// 「退出」ボタン押下時の処理
        /// </summary>
        public void OnBtnLeaveClicked(string userId)
        {
            _userId = userId;
            PhotonNetwork.LeaveRoom();
        }

    }
}
