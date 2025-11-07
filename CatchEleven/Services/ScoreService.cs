using CatchEleven.Constants;
using CatchEleven.Helpers;
using CatchEleven.Models.Players;

namespace CatchEleven.Services
{
    public static class ScoreService
    {
        public static (int HumanScore, int RobotScore) CalculateRoundScores(IBasePlayer human, IBasePlayer robot)
        {
            int humanScore = human.RoundScore;
            int robotScore = robot.RoundScore;

            var humanCards = human.CollectedCards;
            var robotCards = robot.CollectedCards;

            // 1. Most Cards: Use Score.MostCardsBonus
            if (humanCards.Count > robotCards.Count)
            {
                humanScore += Score.MostCardsBonus;
            }
            else if (robotCards.Count > humanCards.Count)
            {
                robotScore += Score.MostCardsBonus;
            }

            // 2. Most Diamonds: +1 point
            // "If two players tie... nobody gets that bonus."
            int humanDiamonds = humanCards.CountCardsBySymbol(Symbol.Diamonds);
            int robotDiamonds = robotCards.CountCardsBySymbol(Symbol.Diamonds);

            if (humanDiamonds > robotDiamonds)
            {
                humanScore += Score.MostDiamondsBonus;
            }
            else if (robotDiamonds > humanDiamonds)
            {
                robotScore += Score.MostDiamondsBonus;
            }

            // 3. Two of Diamonds (2♦): +2 points
            if (humanCards.ContainsTwoOfDiamonds())
            {
                humanScore += Score.TwoOfDiamondsBonus;
            }
            else if (robotCards.ContainsTwoOfDiamonds())
            {
                robotScore += Score.TwoOfDiamondsBonus;
            }

            // 4. Jack of Diamonds (J♦): +1 point
            if (humanCards.ContainsJackOfDiamonds())
            {
                humanScore += Score.JackOfDiamondsBonus;
            }
            else if (robotCards.ContainsJackOfDiamonds())
            {
                robotScore += Score.JackOfDiamondsBonus;
            }

            return (humanScore, robotScore);
        }
    }
}
