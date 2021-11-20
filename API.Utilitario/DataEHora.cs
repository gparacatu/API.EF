using System;

namespace AppPromotora.Api.Utilitario
{
    public static class DataEHora
    {
        public static DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }
}
