using UnityEngine;
using UnityEngine.UI;
using Assets.Services;
using System;
using Photon.Services;
using Unity.Collections;
using Assets.Photon.Services;
using Cysharp.Threading.Tasks;

/// <summary>
/// タイトル画面のコントローラー
/// </summary>
public class TitleController : MonoBehaviour
{
    // 入力フィールド
    [SerializeField] private Text _inpUserId;

    public void Start()
    {
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
    }

    /// <summary>
    /// 「登録」ボタン押下時の処理
    /// </summary>
    public void BtnRegister_Clicked()
    {
        // ローディング画面を表示
        var loadingService = gameObject.GetComponent<LoadingService>();
        loadingService.ShowLoading();

        // インスタンス※MonoBehaviourを継承している場合は、new禁止
        var titleService = gameObject.GetComponent<TitleService>();

        try
        {
            string userId = _inpUserId.text;

            // 入力チェック
            if (!titleService.ChkVal(userId))
            {
                loadingService.CloseLoading();
                return;
            }

            // PlayFabの登録
            titleService.RegisterPlayFabData(userId);

            loadingService.CloseLoading();

        }
        catch(Exception e)
        {
            loadingService.CloseLoading();
            Debug.LogError(e.StackTrace);
        }

    }

    /// <summary>
    /// 「ログイン」ボタン押下時の処理
    /// </summary>
    public async void BtnLogin_Clicked()
    {
        // ローディング画面を表示
        var loadingService = gameObject.GetComponent<LoadingService>();

        try
        {
            loadingService.ShowLoading();

            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var titleService = gameObject.AddComponent<TitleService>();

            await titleService.LoginUser(_inpUserId.text);
            loadingService.CloseLoading();

        }
        catch (Exception e)
        {
            loadingService.CloseLoading();
            Debug.LogError(e.StackTrace);

        }

    }

}
