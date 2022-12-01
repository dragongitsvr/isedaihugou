using UnityEngine;
using UnityEngine.UI;
using Assets.Services;
using System;
using Unity.Collections;
using Assets.Photon.Services;
using Assets.Photon.Argencies;

/// <summary>
/// 「対戦」画面のコントローラー
/// </summary>
public class FightController : MonoBehaviour
{
    /// <summary>
    /// 初期処理
    /// </summary>
    async void Start()
    {
        var loadingService = gameObject.GetComponent<LoadingService>();

        try
        {
            loadingService.ShowLoading();

            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var fightService = gameObject.GetComponent<FightService>();
            await fightService.Init();

            loadingService.CloseLoading();

        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    /// <summary>
    /// 「引く」ボタン押下時の処理
    /// </summary>
    public void BtnPull_Clicked()
    {
        try
        {
            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var fightService = gameObject.GetComponent<FightService>();
            fightService.OnBtnPullClicked();

        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }

    }

    /// <summary>
    /// 「パス」ボタン押下時の処理
    /// </summary>
    public void BtnPass_Clicked()
    {
        try
        {
            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var fightService = gameObject.GetComponent<FightService>();
            fightService.OnBtnPassClicked();

        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }

    }

}
