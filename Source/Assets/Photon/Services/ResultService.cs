using Photon.Commons;
using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using WDT;
using Assets.Photon.Dtos;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using Photon.Services;
using Photon.Messages;
using Assets.Photon.Argencies;
using UnityEngine.SceneManagement;

namespace Assets.Services
{
    public class ResultService : MonoBehaviour
    {
        // グローバル変数
        private string _userId;
        public WDataTable dataTable;
        public List<ResultDto> resultsTestDto = new ();
        [SerializeField] private GameObject firstRow;
        [SerializeField] private GameObject secondRow;
        [SerializeField] private GameObject thirdRow;
        [SerializeField] private GameObject fourthRow;

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public async UniTask Init(string userId)
        {
            try
            {
                _userId = userId;

                var resultsDto = new List<ResultDto> ();
                var dialogService = gameObject.GetComponent<DialogService>();

                var leaderboardRequest = new GetLeaderboardRequest
                {
                    StatisticName = Const.RANKING_NAME,
                    StartPosition = 0,
                    MaxResultsCount = Const.MAX_PLAYERES
                };

                GetLeaderboardResult leaderboardResult = null;
                PlayFabError error = null;

                PlayFabClientAPI.GetLeaderboard(leaderboardRequest, x => leaderboardResult = x, x => error = x);

                // ランキング情報が取得できるまで待機※結果がコールバックで帰ってくるため
                await new WaitUntil(() => leaderboardResult != null || error != null);

                // ランキング情報が取得できた場合
                if (leaderboardResult != null)
                {
                    // ランキングに登録されているIDを全取得
                    var tmpIds = GetPlayfabIds(leaderboardResult);

                    // 取得したIDから成績を全部取得
                    foreach (var tmpId in tmpIds)
                    {
                        resultsDto.Add(await GetPlayrerData(tmpId.PlayfabId,tmpId.DisplayId));
                    }

                    // 成績画面を作成
                    CreateResultObject(resultsDto);

                }
                else if (error != null)
                {
                    // ランキング情報取得に失敗
                    dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE,DialogMessage.ERR_MSG_RUNKING_INFO_GET_FAILED);
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Playfabに登録されているIDを取得
        /// </summary>
        /// <param name="leaderboardResult">リーダーボード</param>
        /// <returns>ID</returns>
        private List<ResultDto> GetPlayfabIds(GetLeaderboardResult leaderboardResult)
        {
            var tmpIds = new List<ResultDto>();
            foreach (var leaderboardEntry in leaderboardResult.Leaderboard)
            {
                var tmpId = new ResultDto()
                {
                    PlayfabId = leaderboardEntry.PlayFabId,
                    DisplayId = leaderboardEntry.DisplayName
                };
                tmpIds.Add(tmpId);
            }
            return tmpIds;
        }

        /// <summary>
        /// プレイヤー情報を取得
        /// </summary>
        /// <param name="playfabId">PlayfabId</param>
        /// <param name="displayId">表示名</param>
        /// <returns>プレイヤー情報</returns>
        private async Task<ResultDto> GetPlayrerData(string playfabId,string displayId)
        {
            PlayFabError error = null;
            var userDataRequest = new GetUserDataRequest()
            {
                PlayFabId = playfabId
            };
            var resultDto = new ResultDto();

            // IDに紐づく成績を取得
            var userDataResult = new GetUserDataResult();
            PlayFabClientAPI.GetUserData(userDataRequest, x => userDataResult = x, x => error = x);
            await new WaitUntil(() => userDataResult.Data != null || error != null);

            if (userDataResult.Data != null)
            {
                resultDto = new ResultDto()
                {
                    PlayfabId = playfabId,
                    DisplayId = displayId,
                    RankNum = userDataResult.Data[Const.COLUMN_NAME_RANK_NUM].Value,
                    ReverseFourNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_REVERSE_FOUR_NUM].Value),
                    SkipFiveNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_SKIP_FIVE_NUM].Value),
                    HandSevenNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_HAND_SEVEN_NUM].Value),
                    AmbulanceNineNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_AMBULANCE_NINE_NUM].Value),
                    ThrowTenNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_THROW_TEN_NUM].Value),
                    BackElevenNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_BACK_ELEVEN_NUM].Value),
                    StairsNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_STAIRS_NUM].Value),
                    RevolutionNum = int.Parse(userDataResult.Data[Const.COLUMN_NAME_RANK_REVOLUTION_NUM].Value)
                };

            }

            return resultDto;

        }

        /// <summary>
        /// プレイヤー情報のオブジェクト作成
        /// </summary>
        /// <param name="resultsDto">プレイヤー情報</param>
        private void CreateResultObject(List<ResultDto> resultsDto)
        {
            // 1位を先頭に持ってくる
            var cnvFirstTopResultsDto = resultsDto.OrderByDescending(x => x.RankNum.Split(",")[0]).ToList();

            // 1～4位をそれぞれのオブジェクトにバインド
            foreach(var cnvFirstTopResultDto in cnvFirstTopResultsDto)
            {
                var splitedCnvFirstTopResultDto = cnvFirstTopResultDto.RankNum.Split(",");
                cnvFirstTopResultDto.FirstRankNum = int.Parse(splitedCnvFirstTopResultDto[0]);
                cnvFirstTopResultDto.SecondRankNum = int.Parse(splitedCnvFirstTopResultDto[1]);
                cnvFirstTopResultDto.ThirdRankNum = int.Parse(splitedCnvFirstTopResultDto[2]);
                cnvFirstTopResultDto.FourthRankNum = int.Parse(splitedCnvFirstTopResultDto[3]);
            }

            // 1行目はオブジェクトを予め作成しているため、取得した値をそのままバインド
            SetValueRowChildObject(firstRow, cnvFirstTopResultsDto.First());

            var addRowGameObjects = new List<GameObject>
            {
                secondRow,
                thirdRow,
                fourthRow
            };

            // 2～4行目は1行目のオブジェクトを複製して取得した値をバインド
            for (var i = 0;i < cnvFirstTopResultsDto.Count; i++)
            {
                // 1行目は除外
                if (i == 0) continue;

                // 1行目のオブジェクトをすべて複製
                for(var j = 0;j < firstRow.transform.childCount; j++)
                {
                    // 子オブジェクトをゲームオブジェクトとして取得
                    GameObject childObject = firstRow.transform.GetChild(j).gameObject;

                    // 1行目の座標を取得し、1行目以降の行の高さを設定
                    var pos = firstRow.transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition;
                    pos.y -= Const.RESULT_BETWEEN_LINES_HEIGHT * i;

                    // 子オブジェクトのコピーをSampleの子オブジェクトとして生成(位置や大きさは上で設定した任意のオブジェクトに依存）
                    GameObject childObjectClone = Instantiate(childObject);
                    childObjectClone.GetComponent<RectTransform>().position = pos;
                    childObjectClone.transform.SetParent(addRowGameObjects[i - 1].transform,false);

                }

                SetValueRowChildObject(addRowGameObjects[i - 1], cnvFirstTopResultsDto[i]);

            }
        }

        /// <summary>
        /// 1行の子要素に値を設定
        /// </summary>
        /// <param name="gameObject">設定する行</param>
        /// <param name="resultDto">プレイヤー情報</param>
        private void SetValueRowChildObject(GameObject gameObject,ResultDto resultDto)
        {
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_DISPLAY_ID).GetComponent<Text>().text = resultDto.DisplayId;
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_FIRST_RANK_NUM).GetComponent<Text>().text = resultDto.FirstRankNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_SECOND_RANK_NUM).GetComponent<Text>().text = resultDto.SecondRankNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_THIRD_RANK_NUM).GetComponent<Text>().text = resultDto.ThirdRankNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_FOURTH_RANK_NUM).GetComponent<Text>().text = resultDto.FourthRankNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_REVERSE_FOUR_NUM).GetComponent<Text>().text = resultDto.ReverseFourNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_SKIP_FIVE_NUM).GetComponent<Text>().text = resultDto.SkipFiveNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_HAND_SEVEN_NUM).GetComponent<Text>().text = resultDto.HandSevenNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_AMBULANCE_NINE_NUM).GetComponent<Text>().text = resultDto.AmbulanceNineNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_THROW_TEN_NUM).GetComponent<Text>().text = resultDto.ThrowTenNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_BACK_ELEVEN_NUM).GetComponent<Text>().text = resultDto.BackElevenNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_STAIRS_NUM).GetComponent<Text>().text = resultDto.StairsNum.ToString();
            gameObject.transform.GetChild(Const.RESULT_COLUMN_INDEX_REVOLUTION_NUM).GetComponent<Text>().text = resultDto.RevolutionNum.ToString();
        }

        /// <summary>
        /// 「ロビー」画面に遷移
        /// </summary>
        public void LoadLobby()
        {
            try
            {
                // ロビー画面に遷移
                LobbyResultArgency.UserId = _userId;
                SceneManager.LoadScene(Const.SCENE_NAME_LOBBY);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
