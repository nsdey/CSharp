using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace QA_Master.Services.Support
{
    public static class WebDriverSupport
    {
        [Obsolete("Do not use", true)]
        public static bool EnsureClick(this IWebElement? webElement, IWebDriver? webDriver, WebDriverWait? webDriverWait)
        {
            int count = 1;
            bool isClicked = false;

            if (webElement == null) return false;

            while (count >= 0)
            {
                webDriver.ExecuteJavaScript("var ele = {0};ele.addEventListener('click', function() {ele.setAttribute('automationTrack','true');});", webElement);
                webElement?.Click();

                //# now check the onclick attribute
                if (webElement != null) isClicked = bool.Parse(webElement.GetAttribute("automationTrack"));

                if (isClicked == false)
                {
                    if (webDriverWait != null) Thread.Sleep(webDriverWait.Timeout);
                }

                count--;
            }

            return isClicked;
        }

        public static bool TryWaitForPageReady(this WebDriver? webDriver, double timeoutSeconds)
        {
            double blockSeconds = 0.3d;
            
            WebDriverWait webDriverWait = new(webDriver, TimeSpan.FromSeconds(blockSeconds));

            int finallyLoaded = 0, loadThreshold = 3;
            while (finallyLoaded < loadThreshold && timeoutSeconds >= 0)
            {
                try
                {
                    webDriverWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                    finallyLoaded++;

                    Thread.Sleep(TimeSpan.FromSeconds(blockSeconds / 10));
                }
                catch
                {
                    finallyLoaded = 0;
                    Thread.Sleep(TimeSpan.FromSeconds(blockSeconds));
                }

                timeoutSeconds -= blockSeconds;
            }

            return finallyLoaded == loadThreshold;
        }

        public static IWebElement? TryFindElement(this WebDriver? webDriver, By? by, double timeoutSeconds)
        {
            if (webDriver == null || by == null) return null;

            webDriver.TryWaitForPageReady(timeoutSeconds);

            IWebElement? webElement = null;
            try
            {
                webElement = webDriver?.FindElement(by);
            }
            catch (WebDriverTimeoutException) { }
            catch (NoSuchElementException) { }
            catch { throw; }

            return webElement;
        }

        public static IWebElement? TryFindElement(this WebDriver? webDriver, WebDriverWait? webDriverWait, By? by)
        {
            if (webDriver == null || webDriverWait == null || by == null) return null;

            IWebElement? webElement = null;
            try
            {
                webDriverWait?.Until(ExpectedConditions.ElementExists(by));
                webElement = webDriver?.FindElement(by);
            }
            catch (WebDriverTimeoutException) { }
            catch (NoSuchElementException) { }
            catch { throw; }

            return webElement;
        }

        public static bool TryScrollToElement(this WebDriver? webDriver, IWebElement? webElement, int defaultExtraScroll = 50)
        {
            if (webDriver == null || webElement == null) return false;
           
            try
            {
                int windowInnerHeight = webDriver.ExecuteJavaScript<int>("return window.innerHeight", "");
                int windowScrollY = webDriver.ExecuteJavaScript<int>("return window.scrollY", "");

                int scrollRequired = webElement.Location.Y + webElement.Size.Height + defaultExtraScroll - windowInnerHeight - windowScrollY;

                //System.Diagnostics.Debug.WriteLine($"webElement.Location.Y : {webElement.Location.Y}");
                //System.Diagnostics.Debug.WriteLine($"webElement.Size.Height : {webElement.Size.Height}");
                //System.Diagnostics.Debug.WriteLine($"windowInnerHeight : {windowInnerHeight}");
                //System.Diagnostics.Debug.WriteLine($"windowScrollY : {windowScrollY}");

                if (scrollRequired > 0)
                {
                    webDriver.ExecuteJavaScript($"window.scrollBy(0,{scrollRequired})", "");

                    Thread.Sleep(TimeSpan.FromSeconds(0.2));
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
