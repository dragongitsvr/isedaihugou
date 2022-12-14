using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Photon.Commons
{
    public class Const
    {
        // 文字数
        // ユーザーID
        public static readonly int CONST_USER_ID_MIN_LENGTH = 3;
        public static readonly int CONST_USER_ID_MAX_LENGTH = 25;

        // カラム名(PlarFabのユーザー情報)
        public static readonly string COLUMN_NAME_RANK_NUM = "rank_num";
        public static readonly string COLUMN_NAME_RANK_REVERSE_FOUR_NUM = "reverse_four_num";
        public static readonly string COLUMN_NAME_RANK_SKIP_FIVE_NUM = "skip_five_num";
        public static readonly string COLUMN_NAME_RANK_HAND_SEVEN_NUM = "hand_seven_num";
        public static readonly string COLUMN_NAME_RANK_AMBULANCE_NINE_NUM = "ambulance_nine_num";
        public static readonly string COLUMN_NAME_RANK_THROW_TEN_NUM = "throw_ten_num";
        public static readonly string COLUMN_NAME_RANK_BACK_ELEVEN_NUM = "back_eleven_num";
        public static readonly string COLUMN_NAME_RANK_STAIRS_NUM = "stairs_num";
        public static readonly string COLUMN_NAME_RANK_REVOLUTION_NUM = "revolution_num";

        // シーン
        public static readonly string SCENE_NAME_LOBBY = "Lobby";
        public static readonly string SCENE_NAME_MATCHING = "Matching";
        public static readonly string SCENE_NAME_RESULT = "Result";
        public static readonly string SCENE_NAME_FIGHT = "Fight";

        // 部屋名
        public static readonly string ROOM_NAME = "IT研究部 伊勢大富豪";

        // 最大プレイヤー数
        public static readonly int MAX_PLAYERES = 4;

        // ランキング名
        public static readonly string RANKING_NAME = "ユーザーID取得用";

        // 「結果」画面の行間
        public static readonly int RESULT_BETWEEN_LINES_HEIGHT = 71;

        // 「結果」画面の列番号
        public static readonly int RESULT_COLUMN_INDEX_DISPLAY_ID = 0;
        public static readonly int RESULT_COLUMN_INDEX_FIRST_RANK_NUM = 1;
        public static readonly int RESULT_COLUMN_INDEX_SECOND_RANK_NUM = 2;
        public static readonly int RESULT_COLUMN_INDEX_THIRD_RANK_NUM = 3;
        public static readonly int RESULT_COLUMN_INDEX_FOURTH_RANK_NUM = 4;
        public static readonly int RESULT_COLUMN_INDEX_REVERSE_FOUR_NUM = 5;
        public static readonly int RESULT_COLUMN_INDEX_SKIP_FIVE_NUM = 6;
        public static readonly int RESULT_COLUMN_INDEX_HAND_SEVEN_NUM = 7;
        public static readonly int RESULT_COLUMN_INDEX_AMBULANCE_NINE_NUM = 8;
        public static readonly int RESULT_COLUMN_INDEX_THROW_TEN_NUM = 9;
        public static readonly int RESULT_COLUMN_INDEX_BACK_ELEVEN_NUM = 10;
        public static readonly int RESULT_COLUMN_INDEX_STAIRS_NUM = 11;
        public static readonly int RESULT_COLUMN_INDEX_REVOLUTION_NUM = 12;

        // トランプのマーク
        public static readonly string CARD_MARK_SPADE = "Spade";
        public static readonly string CARD_MARK_CLUB = "Club";
        public static readonly string CARD_MARK_DIAMOND = "Diamond";
        public static readonly string CARD_MARK_HEART = "Heart";

        // 初期手札枚数
        public static readonly int FIRST_HAND_NUMBER = 7;

        // ジョーカーの枚数
        public static readonly int JOKER_NUMBER = 2;

        // 「結果」画面の残り枚数
        public static readonly string RESULT_LBL_REMAINING_NUMBER = "残";

        // トランプの画像パス
        public static readonly string CARD_IMG_PASS = "Images/Cards/";

        // JOKERの画像ファイル名
        public static readonly string CARD_JOKER_IMG_FILE_NAME_COLOR = "Joker_Color";
        public static readonly string CARD_JOKER_IMG_FILE_NAME_MONOCHROME = "Joker_Monochrome";
        public static readonly Dictionary<int, string> JOKER_DICTIONARY = new()
        {
            {53, CARD_JOKER_IMG_FILE_NAME_COLOR},
            {54, CARD_JOKER_IMG_FILE_NAME_MONOCHROME},
        };

        // テスト環境かどうか
        public static readonly bool IS_TEST = true;

        // 自分の手札のy座標
        public static readonly float MY_CARD_Y = -99.2592f;

        // 場に出たカードとカードの間隔
        public static readonly int FIELD_CARDS_SPACING = -204;

        // 場に出たカードの座標
        public static readonly int FIELD_CARDS_UPPER_LEFT_X_COORDINATE = -36;
        public static readonly int FIELD_CARDS_UPPER_LEFT_Y_COORDINATE = 15;
        public static readonly int FIELD_CARDS_UPPER_RIGHT_X_COORDINATE = 30;
        public static readonly int FIELD_CARDS_UPPER_RIGHT_Y_COORDINATE = 30;
        public static readonly int FIELD_CARDS_LOWER_LEFT_X_COORDINATE = -50;
        public static readonly int FIELD_CARDS_LOWER_LEFT_Y_COORDINATE = -30;
        public static readonly int FIELD_CARDS_LOWER_RIGHT_X_COORDINATE = 20;
        public static readonly int FIELD_CARDS_LOWER_RIGHT_Y_COORDINATE = -15;

        // 自分の番の色
        public static readonly string PLAYER_FRAME_COLOR_CD = "#FFD300";

    }

}

