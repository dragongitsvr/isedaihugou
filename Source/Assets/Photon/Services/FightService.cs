using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Assets.Services
{
    public class FightService : MonoBehaviourPunCallbacks
    {
        [SerializeField] Text _lblFirstPlayerName;
        [SerializeField] Text _lblSecondPlayerName;
        [SerializeField] Text _lblThirdPlayerName;
        [SerializeField] Text _lblFourthPlayerName;
        [SerializeField] Image _imgFirstPlayerFrame;
        [SerializeField] Image _imgSecondPlayerFrame;
        [SerializeField] Image _imgThirdPlayerFrame;
        [SerializeField] Image _imgFourthPlayerFrame;
        [SerializeField] Button _btnPull;
        [SerializeField] Button _btnPass;
        [SerializeField] Button _btnSend;

        // カスタムプロパティの変数名
        private readonly string _isCompletedDecideOrder = "isCompletedDecideOrder";
        private readonly string _playerNames = "playerNames";

        public async UniTask Init()
        {
            var myName = PhotonNetwork.NickName;

            // 同期させるためマスタークライアントが諸々決める
            if (PhotonNetwork.IsMasterClient)
            {
                // 順番決め
                DecideOrder();
            }

            // マスタークライアントでの順番決めが完了するまで待機
            await new WaitUntil(() => IsCompletedDecideOrder());

            // プレイヤー名の表示
            ShowPlayerInfo();



        }
        
        /// <summary>
        /// 順番を決める
        /// </summary>
        private void DecideOrder()
        {
            // プレイヤーをシャッフル
            var playerList = PhotonNetwork.PlayerList.OrderBy(x => Guid.NewGuid()).ToList();

            var playerNames = new List<string>();
            for(var i = 0; i < playerList.Count(); i++)
            {
                playerNames.Add(playerList[i].NickName);
            }

            // カスタムプロパティ更新
            // リストはNGなので、配列に変更
            var hashTable = new Hashtable()
             {
                {_playerNames, playerNames.ToArray()}
                , {_isCompletedDecideOrder,true}
             };

            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

        }

        /// <summary>
        /// 順番決めが完了したかどうかの確認
        /// </summary>
        /// <returns></returns>
        private bool IsCompletedDecideOrder()
        {
            var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            // キーの存在チェック
            if (customProperties.TryGetValue(_isCompletedDecideOrder, out var outValue))
            {
                return Convert.ToBoolean(outValue);
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// プレイヤー情報の表示
        /// </summary>
        private void ShowPlayerInfo()
        {
            // カスタムプロパティから順番を取得
            var playerNames = ((string[])PhotonNetwork.CurrentRoom.CustomProperties[_playerNames]).ToList();
            var tmpRemainPlayerNames = new List<string>(playerNames);

            var myName = PhotonNetwork.NickName;

            var lblPlayerNames = new List<Text>()
            {
                _lblFirstPlayerName
                , _lblSecondPlayerName
                , _lblThirdPlayerName
                , _lblFourthPlayerName
            };

            var tmpRemainLblPlayerNames = new List<Text>()
            {
                _lblFirstPlayerName
                , _lblSecondPlayerName
                , _lblThirdPlayerName
                , _lblFourthPlayerName
            };

            var imgPlayerFrames = new List<Image>()
            {
                _imgFirstPlayerFrame
                ,_imgSecondPlayerFrame
                ,_imgThirdPlayerFrame
                ,_imgFourthPlayerFrame
            };

            var btns = new List<Button>()
            {
                _btnPull
                , _btnSend
            };

            var isFirstPlayer = true;
            var playerStartOrder = 0;
            // プレイヤー名を正面から順番に表示していく。
            for(var i = 0;i < playerNames.Count(); i++)
            {
                if (isFirstPlayer && playerNames[i] == myName)
                {
                    lblPlayerNames[playerStartOrder].text = playerNames[i];
                    tmpRemainLblPlayerNames.Remove(lblPlayerNames[i]);
                    tmpRemainPlayerNames.Remove(playerNames[i]);
                    isFirstPlayer = false;
                    playerStartOrder++;
                }
                else if (!isFirstPlayer)
                {
                    lblPlayerNames[playerStartOrder].text = playerNames[i];
                    tmpRemainLblPlayerNames.Remove(lblPlayerNames[i]);
                    tmpRemainPlayerNames.Remove(playerNames[i]);
                    playerStartOrder++;
                }
            }

            // 残ったプレイヤー名も同様に正面から表示していく。
            for(var i = 0;i < tmpRemainPlayerNames.Count(); i++)
            {
                lblPlayerNames[playerStartOrder].text = tmpRemainPlayerNames[i];
                playerStartOrder++;
            }

            // 最初のプレイヤーのみ「引く」「出す」ボタンを活性化
            if (lblPlayerNames[0].text == playerNames[0])
            {
                btns.ForEach(x => x.interactable = true);
            }

            // 赤枠の設定
            for(var i = 0;i < lblPlayerNames.Count(); i++)
            {
                if (lblPlayerNames[i].text == playerNames[0])
                {
                    imgPlayerFrames[i].enabled = true;
                    break;
                }
            }

        }

        // ルームプロパティが更新された時
        public override void OnRoomPropertiesUpdate(Hashtable hashTable)
        {
        }

    }
}
