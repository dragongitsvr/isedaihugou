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

    }

}

