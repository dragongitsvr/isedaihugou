using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Photon.Dtos;
using Photon.Commons;
using Newtonsoft.Json;
using System.Xml.Linq;
using Photon.Realtime;

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
        [SerializeField] Text _lblRemainingNumber;
        [SerializeField] RawImage _myFirstCard;
        [SerializeField] RawImage _secondBackCard;
        [SerializeField] RawImage _thirdBackCard;
        [SerializeField] RawImage _fourthBackCard;
        [SerializeField] GameObject _firstPlayerHand;
        [SerializeField] GameObject _secondPlayerHand;
        [SerializeField] GameObject _thirdPlayerHand;
        [SerializeField] GameObject _fourthPlayerHand;

        // カスタムプロパティの変数名
        private readonly string _isCompletedDecideOrder = "isCompletedDecideOrder";
        private readonly string _playerNames = "playerNames";
        private readonly string _isCompletedDecideFirstHand = "isCompletedDecideFirstHand";
        private readonly string _playerHand = "playerHand";
        private readonly string _deckCards = "deckCards";
        private readonly string _playerSendCards = "playerSendCards";
        private readonly string _allCards = "allCards";

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

            // 同期させるためマスタークライアントが諸々決める
            if (PhotonNetwork.IsMasterClient)
            {
                // 初期手札決め
                DecideFirstHand();
            }

            await new WaitUntil(() => IsCompletedDecideHand());

            // カードの表示
            ShowCards();

        }
        
        /// <summary>
        /// 順番を決める
        /// </summary>
        private void DecideOrder()
        {
            var playerList = new List<Player>();
            if (Const.IS_TEST)
            {
                // プレイヤーをシャッフル(テスト)
                playerList = PhotonNetwork.PlayerList.ToList();
            }
            else
            {
                // プレイヤーをシャッフル(本番)
                playerList = PhotonNetwork.PlayerList.OrderBy(x => Guid.NewGuid()).ToList();
            }

            var playerNames = new List<string>();
            for(var i = 0; i < playerList.Count(); i++)
            {
                playerNames.Add(playerList[i].NickName);
            }

            // カスタムプロパティ更新
            // リストはNGなので、配列に変更
            var hashTable = new ExitGames.Client.Photon.Hashtable()
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
        /// 初期手札決めが完了したかどうかの確認
        /// </summary>
        /// <returns></returns>
        private bool IsCompletedDecideHand()
        {
            var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            // キーの存在チェック
            if (customProperties.TryGetValue(_isCompletedDecideFirstHand, out var outValue))
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
            // カスタムプロパティから名前を取得
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

        /// <summary>
        /// 初期手札決め
        /// </summary>
        private void DecideFirstHand()
        {
            // カードを作成
            var cards = MakeCards();

            // シャッフル
            var shuffledCards = cards.OrderBy(x => Guid.NewGuid()).ToList();

            // カスタムプロパティからプレイヤー名を取得
            var playerNames = ((string[])PhotonNetwork.CurrentRoom.CustomProperties[_playerNames]).ToList();

            // 各プレイヤーの初期手札を決める
            var nowCardsNum = 0;
            var hashTable = new ExitGames.Client.Photon.Hashtable();
            foreach(var playerName in playerNames)
            {
                var playerHands = new List<CardDto>();
                for (var i = 0;i < Const.FIRST_HAND_NUMBER; i++)
                {
                    playerHands.Add(shuffledCards[nowCardsNum]);
                    nowCardsNum++;
                }
                hashTable.Add($"{_playerHand}{playerName}", JsonConvert.SerializeObject(playerHands));
            }
            var deckCards = shuffledCards.GetRange(playerNames.Count() * Const.FIRST_HAND_NUMBER,
                shuffledCards.Count() - playerNames.Count() * Const.FIRST_HAND_NUMBER);
            hashTable.Add(_allCards, JsonConvert.SerializeObject(cards));
            hashTable.Add(_deckCards, JsonConvert.SerializeObject(deckCards));
            hashTable.Add(_isCompletedDecideFirstHand, true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

        }

        /// <summary>
        /// カード情報の作成
        /// </summary>
        /// <returns></returns>
        private List<CardDto> MakeCards()
        {
            var cards = new List<CardDto>();

            var cardMarks = new List<string>()
            {
                Const.CARD_MARK_SPADE
                , Const.CARD_MARK_CLUB
                , Const.CARD_MARK_DIAMOND
                , Const.CARD_MARK_HEART
            };

            var cardId = 1;
            for (var j = 3; j <= 15; j++)            
            {
                for (var i = 0; i < cardMarks.Count(); i++)
                {
                    // 数字の1と2は13を引く
                    var number = j;
                    if(j >= 14)
                    {
                        number = j - 13;
                    }
                    var card = new CardDto()
                    {
                        Id = cardId
                        , IsJoker = false
                        , Mark = cardMarks[i]
                        , Number = number
                    };
                    cards.Add(card);
                    cardId++;
                }
            }

            // ジョーカーを作成
            for(var i = 0;i < Const.JOKER_NUMBER; i++)
            {
                var card = new CardDto()
                {
                    Id = cardId
                    , IsJoker = true
                };
                cards.Add(card);
                cardId++;
            }

            return cards;

        }

        private void ShowCards()
        {
            // カード情報を取得
            var myName = PhotonNetwork.NickName;
            var deckCards = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[_deckCards].ToString());
            var handCards = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[$"{_playerHand }{myName}"].ToString());

            // 残り枚数を表示
            _lblRemainingNumber.text = $"{ Const.RESULT_LBL_REMAINING_NUMBER }{ deckCards.Count }";

            // 並び替え
            var orderByIdHandCards = handCards.OrderBy(x => x.Id).ToList();

            // 自分の手札を表示
            var i = 0;
            foreach(var orderByIdHandCard in orderByIdHandCards)
            {
                var cardName = $"{orderByIdHandCard.Mark}{orderByIdHandCard.Number:00}";
                // ジョーカーの場合
                if (orderByIdHandCard.IsJoker)
                {
                    cardName = Const.JOKER_DICTIONARY.First(x => x.Key == orderByIdHandCard.Id).Value;
                }

                // 1枚目は画像の変更のみ
                if(i == 0)
                {
                    _myFirstCard.enabled = true;
                    _myFirstCard.texture = Resources.Load<Texture2D> ($"{ Const.CARD_IMG_PASS }{ cardName }");
                    _myFirstCard.name = cardName;
                    i++;
                    continue;
                }

                // 2枚目以降は複製
                var clone = Instantiate(_myFirstCard.transform.gameObject);
                clone.GetComponent<RawImage>().texture = Resources.Load<Texture2D>($"{ Const.CARD_IMG_PASS }{ cardName }");
                clone.GetComponent<RawImage>().name = cardName;
                clone.transform.SetParent(_firstPlayerHand.transform,false);

                i++;
            }

            // 相手の手札を表示
            var lblPlayerNames = new List<Text>()
            {
                _lblFirstPlayerName
                , _lblSecondPlayerName
                , _lblThirdPlayerName
                , _lblFourthPlayerName
            };

            var backCards = new List<RawImage>()
            {
                _secondBackCard
                , _thirdBackCard
                , _fourthBackCard
            };

            var playerBackHands = new List<GameObject>()
            {
                _secondPlayerHand
                , _thirdPlayerHand
                , _fourthPlayerHand
            };

            i = 0;
            foreach(var lblPlayerName in lblPlayerNames)
            {
                // 自分の手札は何もしない
                if(lblPlayerName.text == myName)
                {
                    continue;
                }
                var playerHands = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[$"{_playerHand}{lblPlayerName.text}"].ToString());

                foreach(var playerHand in playerHands)
                {
                    backCards[i].enabled = true;
                    var clone = Instantiate(backCards[i].transform.gameObject);
                    clone.transform.SetParent(playerBackHands[i].transform, false);
                }
                i++;

            }


        }

        // ルームプロパティが更新された時
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable hashTable)
        {
        }

        /// <summary>
        /// カードをクリックした時の処理
        /// </summary>
        public void OnCardClicked(RectTransform rectTransform)
        {
            var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            var playerSendCards = new List<CardDto>();
            var myName = PhotonNetwork.NickName;
            // キーの存在チェック
            if (customProperties.TryGetValue($"{_playerSendCards}{myName}", out var outValue))
            {
                playerSendCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[$"{_playerSendCards}{myName}"].ToString());
            }
            var allCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[_allCards].ToString());

            if(playerSendCards.Any(x => $"{x.Mark}{x.Id.ToString("00")}" == rectTransform.name))
            {
                // 既にクリックされている場合は元の位置に戻す
                rectTransform.position -= new Vector3(0, 30f, 0);
                playerSendCards.RemoveAll(x => $"{x.Mark}{x.Id.ToString("00")}" == rectTransform.name);
            }
            else
            {
                // まだクリックされていない場合は上に移動
                rectTransform.position += new Vector3(0, 30f, 0);
                playerSendCards.Add(allCards.First(x => $"{x.Mark}{x.Id.ToString("00")}" == rectTransform.name));
            }

            var hashTable = new ExitGames.Client.Photon.Hashtable();
            hashTable.Add($"{_playerSendCards}{myName}", JsonConvert.SerializeObject(playerSendCards));
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

        }

        /// <summary>
        /// カードを引く時の処理
        /// </summary>
        public void OnBtnPullClicked()
        {
            // 山札のカード
            var deckCards = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[_deckCards].ToString());

            // 引くカード
            var deckCard = deckCards.First();
            deckCards.Remove(deckCard);

            // 全てのカード
            var allCards = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[_allCards].ToString());

            // プレイヤーのカード
            var myName = PhotonNetwork.NickName;
            var handCards = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[$"{ _playerHand }{ myName }"].ToString());
            handCards.Add(deckCard);

            // 引いたカードの置く場所決め
            var deckCardId = deckCard.Id;
            var putCardIdx = 0;
            foreach (Transform childTransform in _firstPlayerHand.transform)
            {
                var name = childTransform.gameObject.name;
                var handCardId = 0;
                if (Const.JOKER_DICTIONARY.Any(x => x.Value.Contains(name)))
                {
                    handCardId = Const.JOKER_DICTIONARY.First(x => x.Value == name).Key;
                }
                else
                {
                    handCardId = allCards.First(x => $"{x.Mark}{x.Number.ToString("00")}" == childTransform.gameObject.name).Id;
                }

                if (handCardId < deckCardId)
                {
                    putCardIdx++;
                }
            }

            var clone = Instantiate(_myFirstCard.transform.gameObject);
            var cardName = $"{deckCard.Mark}{deckCard.Number.ToString("00")}";
            if(deckCard.IsJoker)
            {
                cardName = Const.JOKER_DICTIONARY.First(x => x.Key == deckCardId).Value;
            }
            clone.GetComponent<RawImage>().texture = Resources.Load<Texture2D>($"{Const.CARD_IMG_PASS}{cardName}");
            clone.GetComponent<RawImage>().name = cardName;
            clone.transform.SetParent(_firstPlayerHand.transform, false);
            clone.transform.SetSiblingIndex(putCardIdx);

            var hashTable = new ExitGames.Client.Photon.Hashtable
            {
                { _deckCards, JsonConvert.SerializeObject(deckCards) }
                , { $"{ _playerHand }{ myName }", JsonConvert.SerializeObject(handCards) }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

        }

    }
}
