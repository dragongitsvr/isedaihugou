using System;

namespace Assets.Photon.Dtos
{
    /// <summary>
    /// カード情報のDtoクラス
    /// </summary>
    [Serializable]
    public class CardDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id;

        /// <summary>
        /// マーク
        /// </summary>
        public string Mark;

        /// <summary>
        /// 数字
        /// </summary>
        public int Number;

        /// <summary>
        /// ジョーカーかどうか
        /// </summary>
        public bool IsJoker;

    }
}
