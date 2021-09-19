using AutoPonto.Data;
using AutoPonto.Models;
using AutoPonto.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace AutoPonto.Watcher
{
    public class TimeWatch
    {
        //Timer que simula um relógio, ativando evento de verificação a cada segundo
        private Timer stopwatch = new Timer(1000);

        //Timer que verifica se um dia se passou, atribuindo novos intervalos de horas/minutos aos pontos
        private Timer dailyTimer = new Timer(86_400_000);

        //Lista que armazena quais pontos já foram batidos, para controle e evitar repetição
        private List<Ponto> pontoControl = new List<Ponto>();

        private readonly IWebRepository _context;

        private readonly UserPreferences _userPreferences;        

        private DateTime RandomDayEnter;

        private DateTime RandomLunchEnter;

        private DateTime RandomLunchBack;

        private DateTime RandomDayLeft;

        public TimeWatch(IWebRepository webContext, UserPreferences userPreferences)
        {            
            this._userPreferences = userPreferences;
            this._context = webContext;
            this.SetupRandomTimes();
            this.stopwatch.Elapsed += OneSecondEvent;
            this.stopwatch.Start();
            this.dailyTimer.Elapsed += DailyTimer;
            this.dailyTimer.Start();
        }

        private void SetupRandomTimes()
        {
            this.RandomDayEnter = SortDate(_userPreferences.DayEnter.InitialInterval, _userPreferences.DayEnter.FinalInterval);
            this.RandomLunchEnter = SortDate(_userPreferences.LunchEnter.InitialInterval, _userPreferences.LunchEnter.FinalInterval);
            this.RandomLunchBack = SortDate(_userPreferences.LunchBack.InitialInterval, _userPreferences.LunchBack.FinalInterval);
            this.RandomDayLeft = SortDate(_userPreferences.DayLeft.InitialInterval, _userPreferences.DayLeft.FinalInterval);

            pontoControl.Clear();
        }

        private void DailyTimer(object sender, ElapsedEventArgs e)
        {
            SetupRandomTimes();
        }

        private async void OneSecondEvent(object sender, ElapsedEventArgs e)
        {
            await CommandForwaded(_userPreferences.DayEnter);            

            Task forwardCommand = DateTime.Now switch
            {
                //Entrada do dia
                DateTime nowDate when RandomDayEnter.Equals(nowDate) && !AlreadyRegistered(_userPreferences.DayEnter, nowDate) =>
                    CommandForwaded(_userPreferences.DayEnter),

                //Entrada Almoço
                DateTime nowDate when RandomLunchEnter.Equals(nowDate) && !AlreadyRegistered(_userPreferences.LunchEnter, nowDate) =>
                    CommandForwaded(_userPreferences.LunchEnter),

                //Retorno Almoço
                DateTime nowDate when RandomLunchBack.Equals(nowDate) && !AlreadyRegistered(_userPreferences.LunchBack, nowDate) =>
                    CommandForwaded(_userPreferences.LunchBack),

                //Saída Dia
                DateTime nowDate when RandomDayLeft.Equals(nowDate) && !AlreadyRegistered(_userPreferences.DayLeft, nowDate) =>
                    CommandForwaded(_userPreferences.DayLeft),

                _ => Task.Run(() => { })
            };

            await forwardCommand;
        }

        private DateTime SortDate(DateTime startDate, DateTime finalDate)
        {
            DateTime currentDate = DateTime.Now;
            Random rand = new();

            return new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, rand.Next(startDate.Hour, finalDate.Hour), rand.Next(startDate.Minute, finalDate.Minute), rand.Next(0, 60));
        }

        private bool AlreadyRegistered(Ponto curPonto, DateTime curDatetime)
        {
            if (CheckDateInterval(curPonto.InitialInterval, curDatetime, curPonto.FinalInterval))
            {
                var wasRegistered = pontoControl.Where(reg => reg.PontoDescription.Equals(curPonto.PontoDescription)).Count() > 0;

                return wasRegistered;
            }

            return false;
        }

        private async Task CommandForwaded(Ponto curPonto)
        {
            try
            {
                using (new StopwatchControl(ref stopwatch))
                {
                    //Navega à página de login
                    await _context.Navigate("https://pontosecullum4-01.secullum.com.br/ponto4web/1005100225#login");

                    //Preenche a textbox de login
                    await _context.FillLoginTextBox("195");

                    //Preenche a textbox de "senha"
                    await _context.FillPasswordTextBox("1234");

                    //Clica para fazer login
                    await _context.ClickLoginButton();

                    //Navega à página de bater ponto
                    await _context.Navigate("https://pontosecullum4-01.secullum.com.br/ponto4web/1005100225#batidas-manuais");

                    //Clica para bater ponto
                    await _context.RegisterButtonClick();

                    //Preenche a textbox de justificativa
                    await _context.FillJustifyTextBox(curPonto.PontoDescription);

                    //Clica no botão de registrar ponto
                    await _context.FinishButtonClick();

                    pontoControl.Add(curPonto);
                }                
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex.ToString());
            }
        }

        /// <summary>
        /// Verifica se uma data está contida dentro de um intervalo de datas
        /// </summary>
        /// <param name="minDate">Limite mínimo do intervalo</param>
        /// <param name="verifyThisDate">Data sendo verificada</param>
        /// <param name="maxDate">Limite máximo do intervalo</param>
        /// <returns></returns>
        private bool CheckDateInterval(DateTime minDate, DateTime verifyThisDate, DateTime maxDate) => verifyThisDate >= minDate && verifyThisDate <= maxDate ? true : false;
    }
}