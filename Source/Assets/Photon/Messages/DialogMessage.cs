namespace Photon.Messages
{
    /// <summary>
    /// �_�C�A���O�̃��b�Z�[�W
    /// </summary>
    public class DialogMessage
    {
        // �����K��
        // ���x��(ERR(�G���[)�EWRN(�x��)�EINF(���))+ "MSG" + �I�u�W�F�N�g�� + ���(��F��A�������������X)
        public static readonly string SUCCESS_MSG_TITLE = "�������b�Z�[�W";

        public static readonly string INF_MSG_USER_DATA_SUCCESSED = "���[�U�[���̓o�^���������܂����B(*'��')";

        public static readonly string ERR_MSG_TITLE = "�G���[���b�Z�[�W";

        public static readonly string ERR_MSG_USER_ID_EMPTY = "���[�U�[ID����͂��Ă��������B�R(`�D�L)�������";

        public static readonly string ERR_MSG_USER_ID_LENGTH = "�o�^�ł��郆�[�U�[ID�̕�������3�����ȏ�25�����ȉ��ł��B�R(`�D�L)�������";

        public static readonly string ERR_MSG_USER_ID_REGISTERD = "���͂������[�U�[ID�͊��ɓo�^�ς݂ł��B�R(`�D�L)�������";

        public static readonly string ERR_MSG_JOIN_ROOM_FAILED = "���[�������Ɏ��s���܂����B���[�����쐬���Ă��������B�R(`�D�L)�������";

    }
}