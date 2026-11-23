using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace QA_Master.Services.Support
{
    public class ExpectedConditionsExtra
    {
        private static Func<IWebDriver, bool> PageReady(int timeoutSecond)
        {
            return (driver) =>
            {
                int blockSeconds = 1;

                WebDriverWait webDriverWait = new(driver, TimeSpan.FromSeconds(blockSeconds));

                int finallyLoaded = 0, loadThreshold = 3;
                while (finallyLoaded < loadThreshold && timeoutSecond >= 0)
                {
                    //if (finallyLoaded) break;

                    try
                    {
                        webDriverWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                        finallyLoaded++;
                        Thread.Sleep(TimeSpan.FromSeconds(0.05d));
                    }
                    catch
                    {
                        finallyLoaded = 0;
                        Thread.Sleep(TimeSpan.FromSeconds(blockSeconds));
                    }

                    timeoutSecond -= blockSeconds;
                }

                return finallyLoaded == loadThreshold;
            };
        }
    }
}
