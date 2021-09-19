using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace AutoPonto.Models
{
    public record UserPreferences
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Ponto DayEnter { get; set; }
        public Ponto LunchEnter { get; set; }
        public Ponto LunchBack { get; set; }
        public Ponto DayLeft { get; set; }

        public UserPreferences()
        {
            DateTime Now = DateTime.Now;

            JObject jsonNodes = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("./user-preferences.json"));

            this.Login = jsonNodes["IDENTIFICACAO"].ToObject<string>();

            this.Password = jsonNodes["PASSWORD"].ToObject<string>();

            #region DAY_ENTER
            int initialDayEnterHour = jsonNodes["ENTRADA DIA"]["HORA INICIAL"].ToObject<int>();
            int initialDayEnterMinute = jsonNodes["ENTRADA DIA"]["MINUTO INICIAL"].ToObject<int>();

            int finalDayEnterHour = jsonNodes["ENTRADA DIA"]["HORA FINAL"].ToObject<int>();
            int finalDayEnterMinute = jsonNodes["ENTRADA DIA"]["MINUTO FINAL"].ToObject<int>();

            this.DayEnter = new Ponto()
            {
                InitialInterval = new DateTime(Now.Year, Now.Month, Now.Day, initialDayEnterHour, initialDayEnterMinute, 0),
                FinalInterval = new DateTime(Now.Year, Now.Month, Now.Day, finalDayEnterHour, finalDayEnterMinute, 0),
                PontoDescription = jsonNodes["ENTRADA DIA"]["JUSTIFICATIVA"].ToObject<string>()
            };
            #endregion

            #region LUNCH_ENTER
            int initialLunchEnterHour = jsonNodes["ENTRADA ALMOCO"]["HORA INICIAL"].ToObject<int>();
            int initialLunchEnterMinute = jsonNodes["ENTRADA ALMOCO"]["MINUTO INICIAL"].ToObject<int>();

            int finalLunchEnterHour = jsonNodes["ENTRADA ALMOCO"]["HORA FINAL"].ToObject<int>();
            int finalLunchEnterMinute = jsonNodes["ENTRADA ALMOCO"]["MINUTO FINAL"].ToObject<int>();

            this.LunchEnter = new Ponto()
            {
                InitialInterval = new DateTime(Now.Year, Now.Month, Now.Day, initialLunchEnterHour, initialLunchEnterMinute, 0),
                FinalInterval = new DateTime(Now.Year, Now.Month, Now.Day, finalLunchEnterHour, finalLunchEnterMinute, 0),
                PontoDescription = jsonNodes["ENTRADA ALMOCO"]["JUSTIFICATIVA"].ToObject<string>()
            };
            #endregion

            #region LUNCH_BACK
            int initialLunchBackHour = jsonNodes["RETORNO ALMOCO"]["HORA INICIAL"].ToObject<int>();
            int initialLunchBackMinute = jsonNodes["RETORNO ALMOCO"]["MINUTO INICIAL"].ToObject<int>();

            int finalLunchBackHour = jsonNodes["RETORNO ALMOCO"]["HORA FINAL"].ToObject<int>();
            int finalLunchBackMinute = jsonNodes["RETORNO ALMOCO"]["MINUTO FINAL"].ToObject<int>();

            this.LunchBack = new Ponto()
            {
                InitialInterval = new DateTime(Now.Year, Now.Month, Now.Day, initialLunchBackHour, initialLunchBackMinute, 0),
                FinalInterval = new DateTime(Now.Year, Now.Month, Now.Day, finalLunchBackHour, finalLunchBackMinute, 0),
                PontoDescription = jsonNodes["RETORNO ALMOCO"]["JUSTIFICATIVA"].ToObject<string>()
            };
            #endregion

            #region DAY_LEFT
            int initialDayLeftHour = jsonNodes["SAIDA DIA"]["HORA INICIAL"].ToObject<int>();
            int initialDayLeftMinute = jsonNodes["SAIDA DIA"]["MINUTO INICIAL"].ToObject<int>();

            int finalDayLeftHour = jsonNodes["SAIDA DIA"]["HORA FINAL"].ToObject<int>();
            int finalDayLeftMinute = jsonNodes["SAIDA DIA"]["MINUTO FINAL"].ToObject<int>();

            this.DayLeft = new Ponto()
            {
                InitialInterval = new DateTime(Now.Year, Now.Month, Now.Day, initialDayLeftHour, initialDayLeftMinute, 0),
                FinalInterval = new DateTime(Now.Year, Now.Month, Now.Day, finalDayLeftHour, finalDayLeftMinute, 0),
                PontoDescription = jsonNodes["SAIDA DIA"]["JUSTIFICATIVA"].ToObject<string>()
            };
            #endregion
        }
    }
}
