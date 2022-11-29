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
    public void Start()
    {
        try
        {
            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var fightService = gameObject.GetComponent<FightService>();
            fightService.Init();
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

}
