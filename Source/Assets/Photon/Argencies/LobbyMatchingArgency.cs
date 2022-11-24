namespace Assets.Photon.Argencies
{
    /// <summary>
    /// 「ロビー」画面と「マッチング」画面の仲介クラス
    /// </summary>
    public class LobbyMatchingArgency
    {
        // ユーザーID
        public static string UserId { get; set; }

        // 部屋を作成するか
        public static bool NeedCreateRoomFlg { get; set; }

    }
}
