using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Photon.Commons
{
    public class Const
    {
        // ������
        // ���[�U�[ID
        public static readonly int CONST_USER_ID_MIN_LENGTH = 3;
        public static readonly int CONST_USER_ID_MAX_LENGTH = 25;

        // �J������(PlarFab�̃��[�U�[���)
        public static readonly string COLUMN_NAME_RANK_NUM = "rank_num";
        public static readonly string COLUMN_NAME_RANK_REVERSE_FOUR_NUM = "reverse_four_num";
        public static readonly string COLUMN_NAME_RANK_SKIP_FIVE_NUM = "skip_five_num";
        public static readonly string COLUMN_NAME_RANK_HAND_SEVEN_NUM = "hand_seven_num";
        public static readonly string COLUMN_NAME_RANK_AMBULANCE_NINE_NUM = "ambulance_nine_num";
        public static readonly string COLUMN_NAME_RANK_THROW_TEN_NUM = "throw_ten_num";
        public static readonly string COLUMN_NAME_RANK_BACK_ELEVEN_NUM = "back_eleven_num";
        public static readonly string COLUMN_NAME_RANK_STAIRS_NUM = "stairs_num";
        public static readonly string COLUMN_NAME_RANK_REVOLUTION_NUM = "revolution_num";

        // �V�[��
        public static readonly string SCENE_NAME_LOBBY = "Lobby";
        public static readonly string SCENE_NAME_MATCHING = "Matching";
        public static readonly string SCENE_NAME_RESULT = "Result";
        public static readonly string SCENE_NAME_FIGHT = "Fight";

        // ������
        public static readonly string ROOM_NAME = "IT������ �ɐ���x��";

        // �ő�v���C���[��
        public static readonly int MAX_PLAYERES = 4;

        // �����L���O��
        public static readonly string RANKING_NAME = "���[�U�[ID�擾�p";

        // �u���ʁv��ʂ̍s��
        public static readonly int RESULT_BETWEEN_LINES_HEIGHT = 71;

        // �u���ʁv��ʂ̗�ԍ�
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

        // �g�����v�̃}�[�N
        public static readonly string CARD_MARK_SPADE = "Spade";
        public static readonly string CARD_MARK_CLUB = "Club";
        public static readonly string CARD_MARK_DIAMOND = "Diamond";
        public static readonly string CARD_MARK_HEART = "Heart";

        // ������D����
        public static readonly int FIRST_HAND_NUMBER = 7;

        // �W���[�J�[�̖���
        public static readonly int JOKER_NUMBER = 2;

        // �u���ʁv��ʂ̎c�薇��
        public static readonly string RESULT_LBL_REMAINING_NUMBER = "�c�薇���F";

        // �g�����v�̉摜�p�X
        public static readonly string CARD_IMG_PASS = "Images/Cards/";

        // JOKER�̉摜�t�@�C����
        public static readonly string CARD_JOKER_IMG_FILE_NAME_COLOR = "Joker_Color";
        public static readonly string CARD_JOKER_IMG_FILE_NAME_MONOCHROME = "Joker_Monochrome";
        public static readonly Dictionary<int, string> JOKER_DICTIONARY = new()
        {
            {53, CARD_JOKER_IMG_FILE_NAME_COLOR},
            {54, CARD_JOKER_IMG_FILE_NAME_MONOCHROME},
        };

        // �e�X�g�����ǂ���
        public static readonly bool IS_TEST = true;

    }

}

