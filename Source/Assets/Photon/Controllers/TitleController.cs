using UnityEngine;
using UnityEngine.UI;
using Assets.Services;
using System;
using Photon.Services;

/// <summary>
/// タイトル画面のコントローラー
/// </summary>
public class TitleController : MonoBehaviour
{
    // 入力フィールド
    [SerializeField] private Text _inpUserId;

    /// <summary>
    /// 「登録」ボタン押下時の処理
    /// </summary>
    public void BtnRegister_Clicked()
    {
        // インスタンス※MonoBehaviourを継承している場合は、new禁止
        var titleService = gameObject.AddComponent<TitleService>();

        try
        {
            string userId = _inpUserId.text;

            // 入力チェック
            if (!titleService.ChkVal(userId))
            {
                return;
            }

            // PlayFabの登録
            titleService.RegisterPlayFabData(userId);

        }
        catch(Exception e)
        {
            Debug.LogError(e.StackTrace);
        }

    }

    /// <summary>
    /// 「ログイン」ボタン押下時の処理
    /// </summary>
    public void BtnLogin_Clicked()
    {
        try
        {
            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var titleService = gameObject.AddComponent<TitleService>();

            titleService.LoginUser(_inpUserId.text);

        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }

    }

}
