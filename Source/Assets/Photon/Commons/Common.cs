using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Photon.Commons
{
    /// <summary>
    /// 共通処理
    /// </summary>
    public class Common
    {
        /// <summary>
        /// カスタムプロパティが取得できるまで待機
        /// </summary>
        /// <returns></returns>
        public static async UniTask<ExitGames.Client.Photon.Hashtable> WaitUntilGetCustomProperties()
        {
            var currentRoom = PhotonNetwork.CurrentRoom;
            var hashTable = new ExitGames.Client.Photon.Hashtable();
            if (currentRoom == null)
            {
                await new WaitUntil(() => PhotonNetwork.CurrentRoom != null); ;
            }
            return currentRoom.CustomProperties;

        }
    }
}
