using Photon.Pun;

namespace Assets.Services
{
    public class LobbyService : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            // マスタークライアントのsceneと同じsceneを部屋に入室した人もロードする
            // マスタークライアントはルームを作ったクライアント
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// Photonサーバーに接続する
        /// </summary>
        public void ConnectToPhotonServer()
        {
            // Photonに接続されているかどうか
            if (!PhotonNetwork.IsConnected)
            {
                // PhotonServerSettingsに設定した内容を使って
                // マスターサーバーへ接続する
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        /// <summary>
        /// Photonに接続した時
        /// </summary>
        public override void OnConnected()
        {
            UnityEngine.Debug.Log("Photonに接続");
        }

        /// <summary>
        /// マスターサーバーへ接続した時
        /// </summary>
        public override void OnConnectedToMaster()
        {
            UnityEngine.Debug.Log("マスターサーバーへ接続");
        }

    }
}
