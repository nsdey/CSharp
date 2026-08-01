using OpenQA.Selenium;

namespace QA_Master.Services.Extensions
{
    public static class Extension
    {
        public static bool IsBrowserRunning(this WebDriver webDriver)
        {
            if (webDriver == null) return false;

            try
            {
                string browserTitle = webDriver.Title;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] GetScreenShotAsByteArray(this WebDriver webDriver)
        {
            return webDriver.GetScreenshot().AsByteArray;
        }
    }
}
