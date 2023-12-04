using System.Globalization;

public static class CurrencyFormatter
{
    public static string FormatPrice(double price)
    {
        CultureInfo euroCulture = new CultureInfo("de-DE");
        NumberFormatInfo euroFormat = euroCulture.NumberFormat;
        euroFormat.CurrencySymbol = "€";
        euroFormat.CurrencyDecimalDigits = 2;
        return price.ToString("C", euroFormat);
    }
}
