//turns big numbers into short readable text = 1234567 -> "1.23M"
//static class = call it anywhere with NumberFormatter.FormatMoney(money)
public static class NumberFormatter
{
    //suffix for each 1000x step (Qa = quadrillion, Qi = quintillion, etc.)
    private static readonly string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

    //money version just puts a $ in front
    public static string FormatMoney(double value)
    {
        return "$" + Format(value);
    }

    //core formatter, shared by money and fish counts
    public static string Format(double value)
    {
        if (value < 0) return "-" + Format(-value); //handle negatives cleanly
        if (value < 1000) return value.ToString("0.##"); //small numbers stay as-is

        int suffixIndex = 0;
        while (value >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            value /= 1000;
            suffixIndex++;
        }
        //two decimals keeps it readable = "1.25M" not "1.254839M"
        return value.ToString("0.##") + suffixes[suffixIndex];
    }

    //fish/sec style readout = "12.5/sec"
    public static string FormatPerSecond(double value)
    {
        return Format(value) + "/sec";
    }
}
