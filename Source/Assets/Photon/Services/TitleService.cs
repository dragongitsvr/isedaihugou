using Photon.Commons;
using Photon.Services;
using Photon.Messages;
using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;
using Assets.Photon.Argencies;

namespace Assets.Services
{
    public class TitleService
    {
        // グローバル変数
        private string _userId;

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public bool ChkVal(string userId)
        {
            try
            {
                // 変数
                bool result = false;

                // インスタンス
                DialogService dialogService = new();

                // 空チェック
                if (string.IsNullOrWhiteSpace(userId))
                {
                    dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE, DialogMessage.ERR_MSG_USER_ID_EMPTY);
                    return result;
                }

                // 文字数チェック
                if (userId.Length <= Const.CONST_USER_ID_MIN_LENGTH && userId.Length >= Const.CONST_USER_ID_MAX_LENGTH)
                {
                    dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE, DialogMessage.ERR_MSG_USER_ID_LENGTH);
                    return result;
                }

                return !result;

            }
            catch(Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// PlayFabにユーザー情報を登録
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void RegisterPlayFabData(string userId)
        {
            try
            {
                // ユーザー情報作成
                // ログイン成功時のコールバック関数の中でユーザー情報の更新が可能
                var request = new LoginWithCustomIDRequest() { CustomId = userId, CreateAccount = true };
                _userId = userId;
                PlayFabClientAPI.LoginWithCustomID(request, OnRegisterLoginSuccess, OnRegisterLoginFailure);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// PlayFabログイン時の成功処理
        /// </summary>
        /// <param name="result">ログイン結果</param>
        private void OnRegisterLoginSuccess(LoginResult result)
        {
            try
            {
                UnityEngine.Debug.Log("ログイン完了");

                // インスタンス
                DialogService dialogService = new();

                // 既に登録済みの場合
                if (!result.NewlyCreated)
                {
                    dialogService.OpenOkDialog(DialogMessage.ERR_MSG_TITLE, DialogMessage.ERR_MSG_USER_ID_REGISTERD);
                    return;
                }

                // 表示名の更新
                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = _userId
                    }
                    , result => { UnityEngine.Debug.Log("表示名の更新完了"); }
                    , error => { UnityEngine.Debug.Log("表示名の更新失敗"); }
                );

                // 登録情報
                var userRequest = new UpdateUserDataRequest()
                {
                    Data = new Dictionary<string, string>
                    {
                        { Const.COLUMN_NAME_RANK_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_REVERSE_FOUR_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_SKIP_FIVE_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_HAND_SEVEN_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_AMBULANCE_NINE_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_THROW_TEN_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_BACK_ELEVEN_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_STAIRS_NUM, "0" },
                        { Const.COLUMN_NAME_RANK_REVOLUTION_NUM, "0" }
                    }
                };

                // ユーザー情報の更新
                PlayFabClientAPI.UpdateUserData(userRequest, OnRegisteSuccessPlayFabInfo, OnRegisteFailedPlayFabInfo);

            }
            catch(Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// PlayFabログイン時の失敗処理
        /// </summary>
        /// <param name="error">エラー内容</param>
        private void OnRegisterLoginFailure(PlayFabError error)
        {
            try
            {
                UnityEngine.Debug.LogError($"ログイン失敗\n{error.GenerateErrorReport()}");
            }
            catch(Exception e) 
            {
                throw e;
            }

        }

        /// <summary>
        /// PlayFabユーザー登録時の成功処理
        /// </summary>
        /// <param name="error">更新結果</param>
        private void OnRegisteSuccessPlayFabInfo(UpdateUserDataResult result)
        {
            try
            {
                UnityEngine.Debug.Log("ユーザー情報登録完了");

                // インスタンス
                DialogService dialogService = new();

                dialogService.OpenOkDialog(DialogMessage.SUCCESS_MSG_TITLE, DialogMessage.INF_MSG_USER_DATA_SUCCESSED);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// PlayFabユーザー登録時の失敗処理
        /// </summary>
        /// <param name="error">エラー内容</param>
        private void OnRegisteFailedPlayFabInfo(PlayFabError error)
        {
            try
            {
                UnityEngine.Debug.LogError($"ユーザー情報登録失敗\n{error.GenerateErrorReport()}");
            }
            catch (Exception e)
            {
                throw e;
            }     

        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void LoginUser(string userId)
        {
            try
            {
                var request = new LoginWithCustomIDRequest() { CustomId = userId, CreateAccount = false };
                _userId = userId;
                PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// ログイン成功時の処理
        /// </summary>
        /// <param name="result">ログイン結果</param>
        private void OnLoginSuccess(LoginResult result)
        {
            try
            {
                UnityEngine.Debug.Log("ログイン完了");

                // TODO:対戦中の場合は再接続

                // ロビー画面に遷移
                TitleLobbyArgency.UserId = _userId;
                SceneManager.LoadScene(Const.SCENE_NAME_LOBBY);


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// ログイン失敗時の処理
        /// </summary>
        /// <param name="error">エラー内容</param>
        private void OnLoginFailure(PlayFabError error)
        {
            try
            {
                UnityEngine.Debug.LogError($"ログイン失敗\n{error.GenerateErrorReport()}");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
