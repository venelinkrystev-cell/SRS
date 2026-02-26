namespace SRS.Progression
{
    public static class RewardCalculator
    {
        public static RewardBreakdown Calculate(RewardContext context, EventResult result)
        {
            if (result.wasCancelled || !result.finished || result.wasDisqualified)
            {
                return RewardBreakdown.Zero;
            }

            var breakdown = new RewardBreakdown
            {
                baseCash = context.baseCashReward,
                baseRespect = context.baseRespectReward,
                placementBonusCash = CalculatePlacementCashBonus(context.baseCashReward, result.placement),
                placementBonusRespect = CalculatePlacementRespectBonus(context.baseRespectReward, result.placement),
                cleanBonusCash = context.cleanFinish ? 0L : 0L,
                penaltiesCash = 0L,
                penaltiesRespect = 0
            };

            breakdown.totalCash = breakdown.baseCash + breakdown.placementBonusCash + breakdown.cleanBonusCash - breakdown.penaltiesCash;
            breakdown.totalRespect = breakdown.baseRespect + breakdown.placementBonusRespect - breakdown.penaltiesRespect;
            return breakdown;
        }

        private static long CalculatePlacementCashBonus(long baseCash, int placement)
        {
            return placement switch
            {
                1 => Percentage(baseCash, 50),
                2 => Percentage(baseCash, 25),
                3 => Percentage(baseCash, 10),
                _ => 0L
            };
        }

        private static int CalculatePlacementRespectBonus(int baseRespect, int placement)
        {
            return placement switch
            {
                1 => (int)Percentage(baseRespect, 50),
                2 => (int)Percentage(baseRespect, 25),
                3 => (int)Percentage(baseRespect, 10),
                _ => 0
            };
        }

        private static long Percentage(long value, int percent)
        {
            return (value * percent) / 100L;
        }

        private static long Percentage(int value, int percent)
        {
            return (value * percent) / 100L;
        }
    }
}
