namespace Streetcode.BLL.Extensions;

public static class DateTimeExtensions
{
    public static string CreateDateString(this (DateTime start, DateTime? end) tuple)
    {
        var (start, end) = tuple;
        var startStr = $"{start.Day} {GetMonthNounInGenitiveCase(start)} {start.Year}";
        var endStr = end is DateTime e ? $"{e.Day} {GetMonthNounInGenitiveCase(e)} {e.Year}" : null;

        return endStr is not null ? $"{startStr} - {endStr}" : startStr;
    }

    public static string GetMonthNounInGenitiveCase(this DateTime date) => date.Month switch
    {
        1 => "січня",
        2 => "лютого",
        3 => "березня",
        4 => "квітня",
        5 => "травня",
        6 => "червня",
        7 => "липня",
        8 => "серпня",
        9 => "вересня",
        10 => "жовтня",
        11 => "листопада",
        12 => "грудня",
        _ => throw new InvalidOperationException("No such month"),
    };
}