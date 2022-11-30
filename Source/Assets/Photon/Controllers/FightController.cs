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

}
