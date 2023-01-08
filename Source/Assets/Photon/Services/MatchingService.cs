using Assets.Photon.Argencies;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Commons;
using Photon.Messages;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Photon.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Services
{
    public class MatchingService : MonoBehaviourPunCallbacks
    {
        bool _needCreatedRoomFlg;
        public Text _firstPlayer;
        public Text _secondPlayer;
        public Text _thirdPlayer;
        public Text _fourthPlayer;
        public Image _firstPlayerFlame;
        public Image _secondPlayerFlame;
        public Image _thirdPlayerFlame;
        public Image _fourthPlayerFlame;
        private string _userId;
        public Button _btnReady;

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
            DialogService dialogService = gameObject.GetComponent<DialogService>();
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

            for (var i = 0; i < matchingTexts.Count; i++)
            {
                if (playerList.ElementAtOrDefault(i) == null)
                {
                    matchingFrames[i].enabled = false;
                    matchingTexts[i].text = "待機中...";
                }
                else
                {
                    var hashKeyReadyPlayer = $"isReadyPlayer" + playerList[i].NickName;
                    var isReadyPlayer = Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[hashKeyReadyPlayer] ?? false);
                    matchingFrames[i].enabled = isReadyPlayer;
                    matchingTexts[i].text = playerList[i].NickName;
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
        }

        /// <summary>
        /// 「準備完了」ボタン押下時の処理
        /// </summary>
        public void OnBtnReadyClicked(string userId)
        {
            var hashKeyReadyPlayer = $"isReadyPlayer" + userId;
            var isReadyPlayer = Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties[hashKeyReadyPlayer] ?? false);
            var properties = new ExitGames.Client.Photon.Hashtable
            {
                { "btnReadyClickedPlayer", userId },
                { hashKeyReadyPlayer, !isReadyPlayer },
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable hashTable)
        {
            if (hashTable.Count() == 1) return;
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

            // 人数が揃っていない場合はゲームを開始しない
            if (PhotonNetwork.CurrentRoom.PlayerCount != Const.MAX_PLAYERES) return;

            var hashTables = PhotonNetwork.CurrentRoom.CustomProperties;
            var readyCount = 0;
            foreach (var hash in hashTables)
            {
                if (hash.Key.ToString().Contains("isReadyPlayer"))
                {
                    if (Convert.ToBoolean(hash.Value))
                    {
                        readyCount++;
                    }
                }
            }

            // 4人全員が準備完了状態になった場合
            if (readyCount == Const.MAX_PLAYERES)
            {
                StartCoroutine(nameof(LoadFight));//コルーチンを実行
            }
        }

        /// <summary>
        /// 「退出」ボタン押下時の処理
        /// </summary>
        public void OnBtnLeaveClicked(string userId)
        {
            _userId = userId;
            var hashKeyReadyPlayer = $"isReadyPlayer" + userId;
            var properties = new ExitGames.Client.Photon.Hashtable
            {
                { hashKeyReadyPlayer,false },
            };
            // 部屋が作成されていない場合はロビー画面に遷移
            if(PhotonNetwork.CurrentRoom == null)
            {
                LobbyMatchingArgency.UserId = _userId;
                SceneManager.LoadScene(Const.SCENE_NAME_LOBBY);
                return;
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 対戦画面に遷移
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadFight()
        {
            _btnReady.enabled = false;
            yield return new WaitForSeconds(2.0f);
            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var dialogService = gameObject.GetComponent<DialogService>();
            dialogService.OpenOkDialog(DialogMessage.SUCCESS_MSG_TITLE, DialogMessage.INF_MSG_GAME_START);
            yield return new WaitForSeconds(4.0f);
            SceneManager.LoadScene(Const.SCENE_NAME_FIGHT);
        }

    }
}
