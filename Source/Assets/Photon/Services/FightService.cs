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
using ExitGames.Client.Photon;
using WDT;
using System.Diagnostics;
using UnityEditor;
using System.Collections;
using Photon.Services;
using Photon.Messages;
using DG.Tweening;

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
        [SerializeField] PhotonView _field;

        // カスタムプロパティの変数名
        private readonly string _isCompletedDecideOrder = "isCompletedDecideOrder";
        private readonly string _playerNames = "playerNames";
        private readonly string _isCompletedDecideFirstHand = "isCompletedDecideFirstHand";
        private readonly string _playerHand = "playerHand";
        private readonly string _deckCards = "deckCards";
        private readonly string _playerSendCards = "playerSendCards";
        private readonly string _allCards = "allCards";
        private readonly string _passCnt = "passCnt";
        private readonly string _nowPlayer = "nowPlayer";
        private readonly string _isFinished = "isFinished";
        private readonly string _fieldCards = "fieldCards";
        private readonly string _isCompletedInit = "isCompletedInit";

        private enum EnumBtnPullNextPlayer
        {
            _playerName
            , _deckCardCnt
        };
        private enum EnumBtnPassNextPlayer
        {
            _nextPlayer
            , _passCnt
            , _deckCardsCnt
            , _isNotFinished
        };

        // イベント番号
        private readonly byte _moveBtnPullNextPlayer = 0; // カードを引いた時の相手側の処理
        private readonly byte _moveBtnPassNextPlayer = 1; // パスした時の相手側の処理
        private readonly byte _moveBtnSendNextPlayer = 2; // カードを出した時の相手側の処理

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
            var hashTable = new ExitGames.Client.Photon.Hashtable();
            for (var i = 0; i < playerList.Count(); i++)
            {
                playerNames.Add(playerList[i].NickName);
                hashTable.Add($"{_isFinished}{playerList[i].NickName}", false);
            }

            // カスタムプロパティ更新
            // リストはNGなので、配列に変更
            hashTable.Add(_playerNames, playerNames.ToArray());
            hashTable.Add(_isCompletedDecideOrder, true);

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

            var isFirstPlayer = true;
            var playerStartOrder = 0;
            // プレイヤー名を正面から順番に表示していく。
            for (var i = 0; i < playerNames.Count(); i++)
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
            for (var i = 0; i < tmpRemainPlayerNames.Count(); i++)
            {
                lblPlayerNames[playerStartOrder].text = tmpRemainPlayerNames[i];
                playerStartOrder++;
            }

            // 最初のプレイヤーのみ「引く」ボタンを活性化
            if (lblPlayerNames[0].text == playerNames[0])
            {
                _btnPull.interactable = true;
                if (Const.IS_TEST)
                {
                    // パスボタンの動作確認
                    _btnPass.interactable = true;
                }
            }

            // 赤枠の設定
            for (var i = 0; i < lblPlayerNames.Count(); i++)
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
            foreach (var playerName in playerNames)
            {
                var playerHands = new List<CardDto>();
                for (var i = 0; i < Const.FIRST_HAND_NUMBER; i++)
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
            var fieldCards = new SortedList<string, List<CardDto>>();
            hashTable.Add(_fieldCards, JsonConvert.SerializeObject(fieldCards));
            hashTable.Add(_isCompletedDecideFirstHand, true);
            hashTable.Add(_passCnt, 0);
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
                    if (j >= 14)
                    {
                        number = j - 13;
                    }
                    var card = new CardDto()
                    {
                        Id = cardId
                        ,
                        IsJoker = false
                        ,
                        Mark = cardMarks[i]
                        ,
                        Number = number
                        , Name = $"{cardMarks[i] }{ number:00}"
                    };
                    cards.Add(card);
                    cardId++;
                }
            }

            // ジョーカーを作成
            for (var i = 0; i < Const.JOKER_NUMBER; i++)
            {
                var card = new CardDto()
                {
                    Id = cardId
                    ,
                    IsJoker = true
                    , Name = Const.JOKER_DICTIONARY[cardId].ToString()
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
            var handCards = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[$"{_playerHand}{myName}"].ToString());

            // カスタムプロパティから名前を取得
            var playerNames = ((string[])PhotonNetwork.CurrentRoom.CustomProperties[_playerNames]).ToList();

            // 残り枚数を表示
            _lblRemainingNumber.text = $"{Const.RESULT_LBL_REMAINING_NUMBER}{deckCards.Count}";

            // 並び替え
            var orderByIdHandCards = handCards.OrderBy(x => x.Id).ToList();

            // 自分の手札を表示
            var i = 0;
            foreach (var orderByIdHandCard in orderByIdHandCards)
            {
                var cardName = $"{orderByIdHandCard.Mark}{orderByIdHandCard.Number:00}";
                // ジョーカーの場合
                if (orderByIdHandCard.IsJoker)
                {
                    cardName = Const.JOKER_DICTIONARY.First(x => x.Key == orderByIdHandCard.Id).Value;
                }

                // 1枚目は画像の変更のみ
                if (i == 0)
                {
                    _myFirstCard.enabled = true;
                    _myFirstCard.texture = Resources.Load<Texture2D>($"{Const.CARD_IMG_PASS}{cardName}");
                    _myFirstCard.name = cardName;
                    i++;
                    // 自分が初手の場合、クリックイベントを追加
                    if(playerNames.First() == myName)
                    {
                        _myFirstCard.transform.gameObject.GetComponent<Button>().onClick.AddListener(() => { OnCardClicked(_myFirstCard.transform.gameObject.GetComponent<RectTransform>()); });
                    }
                    continue;
                }

                // 2枚目以降は複製
                var clone = Instantiate(_myFirstCard.transform.gameObject);
                clone.GetComponent<RawImage>().texture = Resources.Load<Texture2D>($"{Const.CARD_IMG_PASS}{cardName}");
                clone.GetComponent<RawImage>().name = cardName;
                clone.transform.SetParent(_firstPlayerHand.transform, false);
                // 自分が初手の場合、クリックイベントを追加
                if (playerNames.First() == myName)
                {
                    clone.transform.gameObject.GetComponent<Button>().onClick.AddListener(() => { OnCardClicked(clone.transform.gameObject.GetComponent<RectTransform>()); });
                }
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
            foreach (var lblPlayerName in lblPlayerNames)
            {
                // 自分の手札は何もしない
                if (lblPlayerName.text == myName)
                {
                    continue;
                }
                var playerHands = JsonConvert.DeserializeObject<List<CardDto>>(PhotonNetwork.CurrentRoom.CustomProperties[$"{_playerHand}{lblPlayerName.text}"].ToString());

                foreach (var playerHand in playerHands)
                {
                    // 土台を用意しているので、最後は複製しない
                    if (playerHands.Last().Equals(playerHand))
                    {
                        break;
                    }
                    backCards[i].enabled = true;
                    var clone = Instantiate(backCards[i].transform.gameObject);
                    clone.transform.SetParent(playerBackHands[i].transform, false);
                }
                i++;

            }

            var hashTable = new ExitGames.Client.Photon.Hashtable()
            {
                { _isCompletedInit ,true }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

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
            if(rectTransform.anchoredPosition.y == Const.MY_CARD_Y)
            {
                // まだクリックされていない場合は上に移動
                rectTransform.position += Vector3.up * 30f;
            }
            else
            {
                // 既にクリックされている場合は元の位置に戻す
                rectTransform.position -= Vector3.up * 30f;
            }

        }

        /// <summary>
        /// カードを引く時の処理
        /// </summary>
        public async void OnBtnPullClicked()
        {
            // カスタムプロパティが取得できるまで待機
            var customProperties = await WaitUntilGetCustomProperties();

            // 山札のカード
            var deckCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[_deckCards].ToString());

            // 引くカード
            var deckCard = deckCards.First();
            deckCards.Remove(deckCard);

            // 全てのカード
            var allCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[_allCards].ToString());

            // プレイヤーのカード
            var myName = PhotonNetwork.NickName;
            var handCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[$"{_playerHand}{myName}"].ToString());
            handCards.Add(deckCard);

            // 場のカード
            var fieldCards = JsonConvert.DeserializeObject<SortedList<int, List<CardDto>>>(customProperties[_fieldCards].ToString());
            if (fieldCards.Count() > 0)
            {
                // 場にカードが出ている場合のみ、「パス」ボタンが使用可能
                _btnPass.interactable = true;
            }

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

            // カードの複製＆並び位置を設定
            var clone = Instantiate(_myFirstCard.transform.gameObject);
            var cardName = $"{deckCard.Mark}{deckCard.Number.ToString("00")}";
            if (deckCard.IsJoker)
            {
                cardName = Const.JOKER_DICTIONARY.First(x => x.Key == deckCardId).Value;
            }
            clone.GetComponent<RawImage>().texture = Resources.Load<Texture2D>($"{Const.CARD_IMG_PASS}{cardName}");
            clone.GetComponent<RawImage>().name = cardName;
            clone.transform.SetParent(_firstPlayerHand.transform, false);
            clone.transform.SetSiblingIndex(putCardIdx);
            clone.transform.gameObject.GetComponent<Button>().onClick.AddListener(() => { OnCardClicked(clone.transform.gameObject.GetComponent<RectTransform>()); });

            _lblRemainingNumber.text = $"{Const.RESULT_LBL_REMAINING_NUMBER}{deckCards.Count}";
            _btnSend.interactable = true;

            var hashTable = new ExitGames.Client.Photon.Hashtable
            {
                { _deckCards, JsonConvert.SerializeObject(deckCards) }
                , { $"{ _playerHand }{ myName }", JsonConvert.SerializeObject(handCards) }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache,
            };

            var objectDatas = new object[]
            {
                myName
                , deckCards.Count
            };

            PhotonNetwork.RaiseEvent(_moveBtnPullNextPlayer, objectDatas, raiseEventOptions, SendOptions.SendReliable);

        }

        /// <summary>
        /// 「パス」ボタン押下時の処理
        /// </summary>
        public async void OnBtnPassClicked()
        {
            // カスタムプロパティが取得できるまで待機
            var customProperties = await WaitUntilGetCustomProperties();

            // 現在のパス回数を取得
            var passCnt = Int32.Parse(customProperties[_passCnt].ToString());

            // 次のプレイヤーを決定
            var nextPlayer = GetNextPlayer(customProperties);

            // 山札のカード
            var deckCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[_deckCards].ToString());

            // 自分を除く上がっていないプレイヤー数
            var isNotFinishedPlayerCnt = GetIsNotFinishedCnt(customProperties);

            // フレーム非表示＆ボタンの非活性
            _imgFirstPlayerFrame.enabled = false;
            _btnPass.interactable = false;
            _btnPull.interactable = false;
            _btnSend.interactable = false;

            // イベント削除
            foreach (Transform childTransForm in _firstPlayerHand.transform)
            {
                childTransForm.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            // 赤枠の移動
            MoveFrame(nextPlayer);

            var hashTable = new ExitGames.Client.Photon.Hashtable
            {
                { _nowPlayer,  nextPlayer }
                , { _passCnt , passCnt++ }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache,
            };

            var objectDatas = new object[]
            {
                nextPlayer
                , passCnt
                , deckCards.Count
                , isNotFinishedPlayerCnt
            };

            PhotonNetwork.RaiseEvent(_moveBtnPassNextPlayer, objectDatas, raiseEventOptions, SendOptions.SendReliable);

        }

        /// <summary>
        /// 「出す」ボタン押下時の処理
        /// </summary>
        public async void OnBtnSendClicked()
        {
            // カスタムプロパティが取得できるまで待機
            var customProperties = await WaitUntilGetCustomProperties();

            // 次のプレイヤーを決定
            var nextPlayer = GetNextPlayer(customProperties);

            // 場のカード情報の取得
            var fieldCards = JsonConvert.DeserializeObject<SortedList<string, List<CardDto>>>(customProperties[_fieldCards].ToString());

            // 場に出すカードの取得
            var sendCards = GetSendCards(customProperties);
            fieldCards.Add(nextPlayer, sendCards);

            // 場に出したカードのチェック
            if (!IsSendCards(sendCards, customProperties))
            {
                // インスタンス※MonoBehaviourを継承している場合は、new禁止
                var dialogService = gameObject.GetComponent<DialogService>();
                dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE, DialogMessage.ERR_MSG_SEND_CARD_FAILED);
                return;
            }

            // フレーム非表示＆ボタンの非活性
            _imgFirstPlayerFrame.enabled = false;
            _btnPass.interactable = false;
            _btnPull.interactable = false;
            _btnSend.interactable = false;

            // 場に出すカードの角度を取得
            (int angleX, int angleY) = GetCardAngle(fieldCards.Count);

            // 場に出すカードのゲームオブジェクトの作成
            var newGameObject = new GameObject("GameObject");
            var newGameObjectRectTransform = newGameObject.AddComponent<RectTransform>();
            var fieldRectTransform = _field.GetComponent<RectTransform>();
            newGameObjectRectTransform.sizeDelta = new Vector2(fieldRectTransform.sizeDelta.x, fieldRectTransform.sizeDelta.y);
            newGameObjectRectTransform.position = new Vector3(angleX, angleY, 0);
            var horizontalLayoutGroup = newGameObject.AddComponent<HorizontalLayoutGroup>();
            // TODO:定数化
            horizontalLayoutGroup.spacing = -204;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.childControlHeight = false;
            horizontalLayoutGroup.childControlWidth = false;
            horizontalLayoutGroup.childScaleHeight= false;
            horizontalLayoutGroup.childScaleWidth= false;
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childForceExpandWidth= false;
            newGameObject.transform.SetParent(_field.transform, false);

            foreach(var sendCard in sendCards)
            {
                // カードの複製
                var clone = Instantiate(_myFirstCard.transform.gameObject);
                clone.GetComponent<RawImage>().texture = Resources.Load<Texture2D>($"{Const.CARD_IMG_PASS}{sendCard.Name}");
                clone.GetComponent<RawImage>().name = sendCard.Name;
                clone.transform.SetParent(newGameObject.transform, false);

                // 出すカードを手札から消す
                Transform child = _firstPlayerHand.transform.Find(sendCard.Name);
                Destroy(child.gameObject);

            }

            // 枠の移動
            MoveFrame(nextPlayer);

            // プロパティ更新
            var hashTable = new ExitGames.Client.Photon.Hashtable
            {
                { _nowPlayer,  nextPlayer }
                ,{ _fieldCards, JsonConvert.SerializeObject(fieldCards)}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

            // 他のプレイヤーにイベントを送信
            //var raiseEventOptions = new RaiseEventOptions
            //{
            //    Receivers = ReceiverGroup.Others,
            //    CachingOption = EventCaching.AddToRoomCache,
            //};

            //var objectDatas = new object[]
            //{
            //    sendCards[0].Name
            //    ,photonView.ViewID
            //    ,photonView.transform.position
            //    ,photonView.transform.rotation
            //};
            //PhotonNetwork.RaiseEvent(_moveBtnSendNextPlayer, objectDatas, raiseEventOptions, SendOptions.SendReliable);

        }

        /// <summary>
        /// 次のプレイヤーを取得
        /// </summary>
        /// <param name="hashTable">カスタムプロパティ</param>
        /// <returns>次のプレイヤー</returns>
        private string GetNextPlayer(ExitGames.Client.Photon.Hashtable hashTable)
        {
            // 次のプレイヤーを決定
            var lblPlayerNames = new List<Text>()
            {
                _lblSecondPlayerName
                , _lblThirdPlayerName
                , _lblFourthPlayerName
            };
            var nextPlayer = String.Empty;
            foreach (var lblPlayerName in lblPlayerNames)
            {
                var isFinished = Convert.ToBoolean(hashTable[$"{_isFinished}{lblPlayerName.text}"].ToString());
                if (isFinished)
                {
                    continue;
                }
                nextPlayer = lblPlayerName.text;
                break;
            }

            return nextPlayer;

        }

        /// <summary>
        /// 上がっていないプレイヤー数を取得
        /// </summary>
        /// <param name="hashTable">カスタムプロパティ</param>
        /// <returns>上がっていないプレイヤー数</returns>
        private int GetIsNotFinishedCnt(ExitGames.Client.Photon.Hashtable hashTable)
        {
            // 次のプレイヤーを決定
            var lblPlayerNames = new List<Text>()
            {
                _lblSecondPlayerName
                , _lblThirdPlayerName
                , _lblFourthPlayerName
            };
            var nextPlayer = String.Empty;
            var playerCnt = 0;
            foreach (var lblPlayerName in lblPlayerNames)
            {
                var isFinished = Convert.ToBoolean(hashTable[$"{_isFinished}{lblPlayerName.text}"].ToString());
                if (!isFinished)
                {
                    playerCnt++;
                }
            }

            return playerCnt;

        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == _moveBtnPullNextPlayer)
            {
                // 「引く」ボタン押下時の相手側の処理
                OnBtnPullClickedOther((object[])photonEvent.CustomData);
            }
            else if (photonEvent.Code == _moveBtnPassNextPlayer)
            {
                // 「パス」ボタン押下時の相手側の処理
                OnBtnPassClickedOther((object[])photonEvent.CustomData);
            }
            else if(photonEvent.Code == _moveBtnSendNextPlayer)
            {
                // 「出す」ボタン押下時の相手側の処理
                OnBtnSendClickedOther((object[])photonEvent.CustomData);
            }

        }

        private void MyTurn()
        {
            var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            var nowPlayer = customProperties[_nowPlayer].ToString();
            if (PhotonNetwork.NickName == nowPlayer)
            {
                _btnPull.interactable = true;
            }
        }

        private new void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private new void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        /// <summary>
        /// カスタムプロパティが取得できるまで待機
        /// </summary>
        /// <returns></returns>
        private async UniTask<ExitGames.Client.Photon.Hashtable> WaitUntilGetCustomProperties()
        {
            var currentRoom = PhotonNetwork.CurrentRoom;
            var hashTable = new ExitGames.Client.Photon.Hashtable();
            if (currentRoom == null)
            {
                await new WaitUntil(() => PhotonNetwork.CurrentRoom != null); ;
            }
            return currentRoom.CustomProperties;

        }

        /// <summary>
        /// 「引く」ボタン押下時の相手側の処理
        /// </summary>
        private void OnBtnPullClickedOther(object[] customData)
        {
            _lblRemainingNumber.text = $"{Const.RESULT_LBL_REMAINING_NUMBER}{customData[(int)EnumBtnPullNextPlayer._deckCardCnt]}";
            var pullPlayerName = customData[(int)EnumBtnPullNextPlayer._playerName].ToString();
            var lblPlayerNames = new List<Text>()
            {
                _lblSecondPlayerName
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

            // カードの複製
            for(var i = 0;i < lblPlayerNames.Count(); i++)
            {
                if (lblPlayerNames[i].text == pullPlayerName)
                {
                    var clone = Instantiate(backCards[i].transform.gameObject);
                    clone.transform.SetParent(playerBackHands[i].transform, false);
                    break;
                }
            }

        }

        /// <summary>
        /// 「パス」ボタン押下時の相手側の処理
        /// </summary>
        private void OnBtnPassClickedOther(object[] customData)
        {
            var nextPlayer = customData[(int)EnumBtnPassNextPlayer._nextPlayer].ToString();
            var passCnt = Int32.Parse(customData[(int)EnumBtnPassNextPlayer._passCnt].ToString());
            var deckCardsCnt = Int32.Parse(customData[(int)EnumBtnPassNextPlayer._deckCardsCnt].ToString());
            var isNotFinishedPlayerCnt = Int32.Parse(customData[(int)EnumBtnPassNextPlayer._isNotFinished].ToString());

            // 赤枠の移動
            MoveFrame(nextPlayer);

            // 自分の場合
            if (nextPlayer == PhotonNetwork.NickName)
            {
                // イベント追加
                foreach (Transform childTransForm in _firstPlayerHand.transform)
                {
                    childTransForm.transform.gameObject.GetComponent<Button>().onClick.AddListener(() => { OnCardClicked(childTransForm.transform.gameObject.GetComponent<RectTransform>()); });
                }

                // 山札にカードが存在し、パスカウントが他のプレイヤーの数と等しくない
                if (deckCardsCnt != 0 && passCnt != isNotFinishedPlayerCnt)
                {
                    _btnPull.interactable = true;
                }

            }

        }

        /// <summary>
        /// 場に出すカードを取得
        /// </summary>
        /// <param name="customProperties">カスタムプロパティ</param>
        /// <returns>場に出すカード</returns>
        private List<CardDto> GetSendCards(ExitGames.Client.Photon.Hashtable customProperties)
        {
            var sendCards = new List<CardDto>();

            // 全てのカード
            var allCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[_allCards].ToString());

            foreach (Transform childTransForm in _firstPlayerHand.transform)
            {
                // カードの位置が変わっているものが対象
                if(childTransForm.transform.gameObject.GetComponent<RectTransform>().anchoredPosition.y != Const.MY_CARD_Y)
                {
                    sendCards.Add(allCards.First(x => x.Name == childTransForm.name));
                }
            }

            return sendCards;

        }


        /// <summary>
        /// カードを場に出せるかどうかのチェック(初手)
        /// </summary>
        /// <param name="fieldCards">場に出す予定のカード</param>
        /// <param name="customProperties">カスタムプロパティ</param>
        /// <returns>チェック結果</returns>
        private bool IsSendCards(List<CardDto> fieldCards, ExitGames.Client.Photon.Hashtable customProperties)
        {
            var orderByIdCards = fieldCards.OrderBy(x => x.Id).ToList();

            // 全てのカード
            var allCards = JsonConvert.DeserializeObject<List<CardDto>>(customProperties[_allCards].ToString());

            // 1枚の場合はチェック不要
            if(orderByIdCards.Count == 1) return true;
            if (orderByIdCards.Count == 2)
            {
                // 2枚の場合は同じ数字でないとダメ(JOKERは含んでおｋ)
                // 同じ数字、もしくは、JOKERが1枚でも入っていればおｋ
                if ((orderByIdCards[0].Number == orderByIdCards[1].Number) || orderByIdCards[0].IsJoker || orderByIdCards[1].IsJoker)
                {
                    return true;
                }

            }
            else if(orderByIdCards.Count == 3)
            {
                // JOKER2枚の場合はチェック不要
                if(orderByIdCards.Select(x => x.IsJoker).Count() == 2)
                {
                    return true;
                }
                else if(orderByIdCards.Select(x => x.IsJoker).Count() == 1)
                {
                    // JOKER1枚の場合は同じ数字もしくは数字が連続or1つ飛ばしであればおｋ
                    if (GetConvertedCardNumber(orderByIdCards[0].Number) == GetConvertedCardNumber(orderByIdCards[1].Number)
                        || GetConvertedCardNumber(orderByIdCards[0].Number) - GetConvertedCardNumber(orderByIdCards[1].Number) == 0
                        || GetConvertedCardNumber(orderByIdCards[0].Number) - GetConvertedCardNumber(orderByIdCards[1].Number) == 1)
                    {
                        return true;
                    }
                }

            }else if(orderByIdCards.Count >= 4)
            {
                var nowNumber = 0;
                var beforeNumber = 0;
                var sumBetweenNumberAndNumber = 0;
                // 自分の手札-1が数字と数字の間の合計と一致していればおｋ※JOKER1枚2枚同様
                for (var i = 0; i < orderByIdCards.Count; i++)
                {
                    if (i != 0)
                    {
                        nowNumber = orderByIdCards[i].Number;
                    }
                    sumBetweenNumberAndNumber += nowNumber - beforeNumber;
                    beforeNumber = orderByIdCards[i].Number;

                }
                if(orderByIdCards.Count - 1 == sumBetweenNumberAndNumber)
                {
                    return true;
                }
                // 全部の数字が同じであればおｋ
                if(orderByIdCards.Select(x => x.Number).Distinct().Count() == 1)
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// 1もしくは2は14と15として扱う
        /// </summary>
        /// <param name="number">1もしくは2</param>
        /// <returns>14もしくは15</returns>
        private int GetConvertedCardNumber(int number)
        {
            if(number == 1 || number == 2)
            {
                return 13 + number;
            }
            return number;
        }

        /// <summary>
        /// 「パス」ボタン押下時の相手側の処理
        /// </summary>
        private void OnBtnSendClickedOther(object[] customData)
        {
            var clone = Instantiate(_myFirstCard.transform.gameObject, (Vector3)customData[2], (Quaternion)customData[3]);
            clone.GetComponent<RawImage>().texture = Resources.Load<Texture2D>($"{Const.CARD_IMG_PASS}{customData[0].ToString()}");
            clone.GetComponent<RawImage>().name = customData[0].ToString();
            clone.transform.SetParent(_field.transform, false);
            var photonView = clone.AddComponent<PhotonView>();
            var photonTransformView = clone.AddComponent<PhotonTransformView>();
            photonTransformView.m_SynchronizeScale = true;
            photonTransformView.m_SynchronizeRotation = true;
            photonTransformView.m_SynchronizePosition = true;
            photonView.ObservedComponents = new List<Component> { photonTransformView };
            photonView.ViewID = Int32.Parse(customData[1].ToString());
        }

        /// <summary>
        /// 場に出すカードの角度を取得
        /// </summary>
        /// <param name="fieldCardBlockCnt">カードのブロック数</param>
        /// <returns>カードの角度</returns>
        private (int angleX,int angleY) GetCardAngle(int fieldCardBlockCnt)
        {
            // 求めた余りに応じて角度を設定
            var remainder = fieldCardBlockCnt % 4;
            if (remainder == 0)
            {
                // 左上に表示
                return (-30 , 0);
            }
            else if (remainder == 1)
            {
                // 右上に表示
                return (30, 30);
            }
            else if (remainder == 1)
            {
                // 左下に表示
                return (-50, 30);
            }
            else
            {
                // 右下に表示
                return (20, -15);
            }
        }

        /// <summary>
        /// 枠を移動
        /// </summary>
        /// <param name="nextPlayer">次のプレイヤー</param>
        private void MoveFrame(string nextPlayer)
        {
            var lblPlayerNames = new List<Text>()
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

            // 枠の移動
            for (var i = 0; i < lblPlayerNames.Count(); i++)
            {
                imgPlayerFrames[i].enabled = false;
                if (lblPlayerNames[i].text == nextPlayer)
                {
                    imgPlayerFrames[i].enabled = true;
                }
            }

        }

    }
}
