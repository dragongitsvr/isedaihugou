namespace Photon.Messages
{
    /// <summary>
    /// ダイアログのメッセージ
    /// </summary>
    public class DialogMessage
    {
        // 命名規則
        // レベル(ERR(エラー)・WRN(警告)・INF(情報))+ "MSG" + オブジェクト名 + 状態(例：空、文字数制限等々)
        public static readonly string SUCCESS_MSG_TITLE = "成功メッセージ";

        public static readonly string INF_MSG_USER_DATA_SUCCESSED = "ユーザー情報の登録が完了しました。(*'▽')";

        public static readonly string INF_MSG_GAME_START = "ゲームを開始します。(*'▽')";

        public static readonly string ERR_MSG_TITLE = "エラーメッセージ";

        public static readonly string ERR_MSG_USER_ID_EMPTY = "ユーザーIDを入力してください。ヽ(`Д´)ﾉﾌﾟﾝﾌﾟﾝ";

        public static readonly string ERR_MSG_USER_ID_LENGTH = "登録できるユーザーIDの文字数は3文字以上25文字以下です。ヽ(`Д´)ﾉﾌﾟﾝﾌﾟﾝ";

        public static readonly string ERR_MSG_USER_ID_REGISTERD = "入力したユーザーIDは既に登録済みです。ヽ(`Д´)ﾉﾌﾟﾝﾌﾟﾝ";

        public static readonly string ERR_MSG_JOIN_ROOM_FAILED = "ルーム入室に失敗しました。ルームを作成してください。ヽ(`Д´)ﾉﾌﾟﾝﾌﾟﾝ";

        public static readonly string ERR_MSG_RUNKING_INFO_GET_FAILED = "ランキング情報を取得できませんでした。(*´Д｀)";

        public static readonly string ERR_MSG_LOGIN_FAILED = "ログインに失敗しました。(*´Д｀)";

        public static readonly string ERR_MSG_CREATE_ROOM_FAILED = "既にルームが作成されています。「ルーム入室」ボタンより入室してください。ヽ(`Д´)ﾉﾌﾟﾝﾌﾟﾝ";

        public static readonly string ERR_MSG_SEND_CARD_FAILED = "その組み合わせは場に出せません。ヽ(`Д´)ﾉﾌﾟﾝﾌﾟﾝ";

    }
}