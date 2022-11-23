using Assets.Photon.Argencies;
using Assets.Services;
using Photon.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Photon.Controllers
{
    internal class ResultController : MonoBehaviour
    {
        // 表示フィールド
        [SerializeField] public Text _dspUserId;

        /// <summary>
        /// 初期処理
        /// </summary>
        void Start()
        {
            var userId = TitleLobbyArgency.UserId;

            // ログインユーザの表示
            _dspUserId.text = userId;

            // インスタンス※MonoBehaviourを継承している場合は、new禁止
            var resultService = gameObject.GetComponent<ResultService>();
            resultService.Init(userId);

        }
    }
}
