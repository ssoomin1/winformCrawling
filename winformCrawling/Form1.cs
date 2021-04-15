using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        List<string> Lsrc = null; //이미지 URL (SRC)
        int i = 0; // 현재 배열 위치


        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strURL = "https://www.google.com/search?q=" + tboxSearch.Text + "&source=lnms&tbm=isch";

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl(strURL); // 웹 사이트에 접속합니다.
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _driver.ExecuteScript("window.scrollBy(0,10000)"); //사진을 더 많이 보여주려고 하는 역할 => 굳이 할 필요 없다.
            Lsrc = new List<string>();

            foreach (IWebElement item in _driver.FindElementsByClassName("rg_i")) //사진들 클래스 이름들 안에 rg_i라는 것을 포함하고 있다. 
            {
                if (item.GetAttribute("src") != null)  //이미지 주소 복사
                    Lsrc.Add(item.GetAttribute("src"));
            }
            lblTotal.Text = " / " + Lsrc.Count.ToString();

            //화면에 뿌려주기
            this.Invoke(new Action(delegate ()
            {
                try
                {
                    foreach (string strsrc in Lsrc)
                    {
                        i++;

                        GetMapImage(Lsrc[i]);
                        tboxNow.Text = i.ToString();
                        Refresh();
                        Thread.Sleep(50);
                    }
                }
                catch (Exception)
                {
                }
            }));
        }

        private void GetMapImage(string base64String)
        {
            try
            {
                var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value; //정규화
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    if (stream.Length == 0)
                    {
                        pboxMain.Load(base64String);
                        tboxNow.Text = i.ToString();
                        tboxUrl.Text = base64String;
                    }
                    else
                    {
                        //이걸 하지 않으면 20개정도까지만 뜨고 나머지는 안 뜰 수도 있다. 
                        var image = Image.FromStream(stream);
                        pboxMain.Image = image;
                        tboxUrl.Text = base64String;
                    }
                }
            }
            catch
            {

            }
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i--;

                GetMapImage(Lsrc[i]);
                tboxNow.Text = i.ToString();
            }));
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i = int.Parse(tboxNow.Text);

                GetMapImage(Lsrc[i]);
                tboxNow.Text = i.ToString();
            }));
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i++;

                GetMapImage(Lsrc[i]);
                tboxNow.Text = i.ToString();
            }));
        }
    }
}
