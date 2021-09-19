using AutoPonto.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading.Tasks;

namespace AutoPonto.Repository
{
    public interface IWebRepository
    {
        Task Navigate(string URL);

        Task RegisterButtonClick();

        Task FillJustifyTextBox(string description);

        Task FinishButtonClick();

        Task FillLoginTextBox(string user);

        Task FillPasswordTextBox(string password);

        Task ClickLoginButton();

        Task<bool> UserIsLogged();
    }

    public class WebRepository : IWebRepository
    {
        private readonly ChromeDriver _driver;

        private readonly WebDriverWait _wait;

        public WebRepository(ChromeDriver driver)
        {
            this._driver = driver;

            this._wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
        }

        /// <summary>
        /// Navegar à página especificada
        /// </summary>
        /// /// <param name="URL">URL destino</param>
        public async Task Navigate(string URL)
        {
            _driver.Navigate().GoToUrl(URL);

            await Task.Delay(1000);

            LogWriter.Write($"Navegado à página {URL}");
        }

        /// <summary>
        /// Preenche o textbox de login na página inicial
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task FillLoginTextBox(string user)
        {
            IWebElement tbLogin = WaitUntilElementExists(By.XPath("/html/body/div[2]/div/form/div/div[2]/div[2]/p[2]/input"));

            if (tbLogin != default)
            {
                tbLogin.SendKeys(user);

                LogWriter.Write($"Textbox de login preenchida com o texto {user}");

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Preenche o textbox de senha na página inicial
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task FillPasswordTextBox(string password)
        {
            IWebElement tbSenha = WaitUntilElementExists(By.XPath("/html/body/div[2]/div/form/div/div[2]/div[2]/p[3]/input"));

            if (tbSenha != default)
            {
                tbSenha.SendKeys(password);

                LogWriter.Write($"Textbox de senha preenchida com o texto {password}");

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Pressiona o botão de realizar login
        /// </summary>
        /// <returns></returns>
        public async Task ClickLoginButton()
        {
            IWebElement btnEntrar = WaitUntilElementExists(By.Id("login"));

            if (btnEntrar != default)
            {
                btnEntrar.Click();

                LogWriter.Write($"Botão de login pressionado");

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Clica no botão Registrar da página 
        /// </summary>
        public async Task RegisterButtonClick()
        {
            IWebElement btnRegister = WaitUntilElementExists(By.XPath(@"//*[@id=""registrar""]"));

            btnRegister.Click();

            await Task.Delay(1000);

            LogWriter.Write("Botão 'Incluir Ponto' pressionado");
        }

        /// <summary>
        /// Preenche o texto de justificativa
        /// </summary>
        /// <param name="description">Justificativa a ser inserida no campo</param>
        public async Task FillJustifyTextBox(string description)
        {
            IWebElement textBox = WaitUntilElementExists(By.Id("justificativa"));
            
            if (textBox != default)
            {
                textBox.SendKeys(description);

                await Task.Delay(1000);

                LogWriter.Write($"Justificativa preenchida com o texto {description}");
            }            
        }

        /// <summary>
        /// Clica no botão Concluir
        /// </summary>
        public async Task FinishButtonClick()
        {
            IWebElement btnConcluir = WaitUntilElementExists(By.XPath("/html/body/div[6]/div[11]/div/button[1]/span"));

            if (btnConcluir != default)
            {
                btnConcluir.Click();

                await Task.Delay(1000);

                LogWriter.Write("Botão de registrar pressionado");

                LogWriter.Write("PONTO BATIDO!");
            }
        }
        
        /// <summary>
        /// Espera por até 60 segundos para o elemento carregar, cobrindo más conexões
        /// </summary>
        /// <param name="elementLocator"></param>
        /// <returns></returns>
        private IWebElement WaitUntilElementExists(By elementLocator)
        {
            try
            {
                MoveToPopUp();

                return _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(elementLocator));
            }
            catch (NoSuchElementException e)
            {
                LogWriter.Write(e.ToString());
            }

            return default;
        }

        /// <summary>
        /// Verifica se o usuário conseguiu realizar login visualizando se há label com nome do funcionário no canto superior direito
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UserIsLogged()
        {
            bool response = false;

            IWebElement userName = WaitUntilElementExists(By.XPath("/html/body/div[1]/div[2]/div/div/div[2]/p[1]"));

            if (userName != default)
            {
                LogWriter.Write($"Verificado se o usuário {userName.Text} realizou login");

                await Task.Delay(1000);

                response = userName.Text.Length > 0 ? true : false;
            }

            return response;
        }

        private void MoveToPopUp()
        {
            _driver.SwitchTo().ActiveElement();
        }
    }
}