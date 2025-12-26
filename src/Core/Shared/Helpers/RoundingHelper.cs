namespace Shared.Helpers;

public static class RoundingHelper
{
    /// <summary>
    /// Rounds an integer based on its magnitude.
    /// Rules:
    /// - Less than 100:      round to the nearest 10.
    /// - 100 to 5,000:       round to the nearest 100.
    /// - Greater than 5,000: round to the nearest 1,000.
    /// </summary>
    public static int RoundCustom(int number)
    {
        if (number < 100)
        {
            return RoundToNearest(number, 10);
        }
        if (number <= 5000)
        {
            return RoundToNearest(number, 100);
        }
        return RoundToNearest(number, 1000);
    }

    /// <summary>
    /// Helper to round a number to the nearest value, rounding .5 cases up.
    /// </summary>
    private static int RoundToNearest(int number, int toNearest)
    {
        if (number == 0)
            return 0;

        // Cast to double to perform the division for rounding
        double dNumber = (double)number / toNearest;

        // MidpointRounding.AwayFromZero ensures that values like 2.5 are rounded to 3
        double rounded = Math.Round(dNumber, MidpointRounding.AwayFromZero);

        if (rounded == 0)
            return number;

        // Cast the final result back to int
        return (int)(rounded * toNearest);
    }
    
    public static decimal RoundCustom(decimal number)
    {
        if (number < 100m)
        {
            return RoundToNearest(number, 10);
        }
        if (number <= 5000m)
        {
            return RoundToNearest(number, 100);
        }
        return RoundToNearest(number, 1000);
    }

    private static decimal RoundToNearest(decimal number, int toNearest)
    {
        if (number == 0)
            return 0;

        // MidpointRounding.AwayFromZero ensures that values like 2.5 are rounded to 3.
        decimal rounded = Math.Round(number / toNearest, MidpointRounding.AwayFromZero);

        if (rounded == 0)
            return number;

        return rounded * toNearest;
    }
}