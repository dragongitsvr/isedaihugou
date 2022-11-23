namespace Assets.Photon.Dtos
{
    /// <summary>
    /// 「成績」クラスのDto
    /// </summary>
    public class ResultDto
    {
        /// <summary>
        /// PlayFabID
        /// </summary>
        public string PlayfabId { get; set; }

        /// <summary>
        /// 順位(カンマ区切り)
        /// </summary>
        public string RankNum { get; set; }

        /// <summary>
        /// ディスプレイID
        /// </summary>
        public string DisplayId { get; set; }

        /// <summary>
        /// 1位
        /// </summary>
        public int FirstRankNum { get; set; }

        /// <summary>
        /// 2位
        /// </summary>
        public int SecondRankNum { get; set; }

        /// <summary>
        /// 3位
        /// </summary>
        public int ThirdRankNum { get; set; }

        /// <summary>
        /// 4位
        /// </summary>
        public int FourthRankNum { get; set; }

        /// <summary>
        /// 4リバ
        /// </summary>
        public int ReverseFourNum { get; set; }

        /// <summary>
        /// 5飛ばし
        /// </summary>
        public int SkipFiveNum { get; set; }

        /// <summary>
        /// 7渡し
        /// </summary>
        public int HandSevenNum { get; set; }

        /// <summary>
        /// 99車
        /// </summary>
        public int AmbulanceNineNum { get; set; }

        /// <summary>
        /// 10捨て
        /// </summary>
        public int ThrowTenNum { get; set; }

        /// <summary>
        /// Jバック
        /// </summary>
        public int BackElevenNum { get; set; }

        /// <summary>
        /// 階段
        /// </summary>
        public int StairsNum { get; set; }

        /// <summary>
        /// 革命
        /// </summary>
        public int RevolutionNum { get; set; }

    }
}
