using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winformCrawling
{
    public partial class Form1 : Form
    {
        private ChromeDriverService _driverService = null;
        private ChromeOptions _options = null;
        private ChromeDriver _driver = null;

        public Form1()
        {
            InitializeComponent();

            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = tboxID.Text;
            string pw = tboxPW.Text;

            _driver = new ChromeDriver(_driverService,_options);
            _driver.Navigate().GoToUrl("https://www.naver.com/"); //웹사이트에 접속합니다.
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var element = _driver.FindElementByXPath("//*[@id='account']/a");   // Main 로그인 버튼
            //*[@id="account"]/a
            element.Click();
            Thread.Sleep(3000);

            element = _driver.FindElementByXPath("//*[@id='id']"); // 아이디 입력창
            element.SendKeys(id);

            element = _driver.FindElementByXPath("//*[@id='pw']"); //PW 입력창
            element.SendKeys(pw);

            element = _driver.FindElementByXPath("//*[@id='log.login']"); //실제 로그인 버튼
            element.Click();
        }
    }
}
