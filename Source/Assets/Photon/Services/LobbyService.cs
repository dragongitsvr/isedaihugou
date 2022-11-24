using Assets.Photon.Argencies;
using Photon.Commons;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Assets.Services
{
    public class LobbyService : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// 「マッチング」画面をロード
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="needCreateRoomFlg">部屋を作成するかどうか</param>
        public void LoadMatching(string userId,bool needCreateRoomFlg)
        {
            // 「マッチング」画面に遷移
            LobbyMatchingArgency.UserId = userId;
            LobbyMatchingArgency.NeedCreateRoomFlg = needCreateRoomFlg;
            SceneManager.LoadScene(Const.SCENE_NAME_MATCHING);

        }

        /// <summary>
        /// 「成績」画面をロード
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void LoadResult(string userId)
        {
            // 「成績」画面に遷移
            LobbyResultArgency.UserId = userId;
            SceneManager.LoadScene(Const.SCENE_NAME_RESULT);

        }

    }
}
