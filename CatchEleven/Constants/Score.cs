namespace CatchEleven.Constants
{
    /// <summary>
    /// Defines the constant point values for game rules,
    /// based on the README.md.
    /// </summary>
    public static class Score
    {
        /// <summary>
        /// Points for clearing the table (not with a Jack).
        /// </summary>
        public const int Basaat = 10;

        /// <summary>
        /// Points for capturing the most cards.
        /// </summary>
        public const int MostCardsBonus = 3;

        /// <summary>
        /// Points for capturing the most Diamonds.
        /// </summary>
        public const int MostDiamondsBonus = 1;

        /// <summary>
        /// Points for capturing the Two of Diamonds (2♦).
        /// </summary>
        public const int TwoOfDiamondsBonus = 2;

        /// <summary>
        /// Points for capturing the Jack of Diamonds (J♦).
        /// </summary>
        public const int JackOfDiamondsBonus = 1;

        /// <summary>
        /// The total score needed to win the game.
        /// </summary>
        public const int TargetWinScore = 62;
    }
}
