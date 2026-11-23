using QA_Master.Services.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using QA_Master.Models.Core;
using QA_Master.Services.Extensions;
using QA_Master.Models.Enums;
using QA_Master.Models.Extensions;
using OpenQA.Selenium.Interactions;
using QA_Master.Services.Support;
using QA_Master.Models.Utilities;
using OpenQA.Selenium.DevTools;

namespace QA_Master.Services.Core
{
    public class SeleniumService
    {
        private Run? Run = null;
        public static SeleniumService? CurrentService { get; private set; }

        private static EnumBrowser previousBrowser;
        private EnumBrowser browser;
        public bool IsServiceStarted { get; private set; }
        private int GenericDelayInMilliSecods;

        public WebDriver? WebDriver { get; private set; }
        public IWebElement? IWebElement { get; private set; }
        private By? by = null;

        public SeleniumService() { CurrentService = this; }

        private const int windowWidth = 1600, windowHeight = 900;

        private const double pageLoadTimeoutSeconds = 180.0d;
        private const double implicitTimeoutMilliSeconds = 200d;
        private const double defaultWaitAfterClickSecond = 0.01d;

        public void Setup_SeleniumService(EnumBrowser enumBrowser, int genericDelayInMilliSecods = 300, Run? run = null)
        {
            Run = run;
            browser = enumBrowser;
            GenericDelayInMilliSecods = genericDelayInMilliSecods;
        }

        #region Start Selenum Service
        public void StartService()
        {
            if (previousBrowser != browser)
            {
                WebDriver?.Quit();
                previousBrowser = browser;
                IsServiceStarted = false;
            }
            
            if (IsServiceStarted && WebDriver != null && WebDriver.IsBrowserRunning()) return;

            DriverService? driverService = GetDriverService();
            if (driverService != null) driverService.HideCommandPromptWindow = true;

            DriverOptions? driverOptions = GetDriverOptions();
            if (driverOptions != null) driverOptions.AcceptInsecureCertificates = true;

            WebDriver = StartDriverService(driverService, driverOptions);

            if (WebDriver != null)
            {
                WebDriver.Manage().Window.Size = new System.Drawing.Size(windowWidth, windowHeight);
                WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(implicitTimeoutMilliSeconds);
                WebDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadTimeoutSeconds);

                IDevToolsSession? devTools = ((IDevTools)WebDriver).GetDevToolsSession(new DevToolsOptions() { WaitForDebuggerOnStart = true });
                devTools.DevToolsEventReceived += delegate (object? sender, DevToolsEventReceivedEventArgs e)
                {
                    System.Diagnostics.Debug.WriteLine(e.EventName);
                };
                
            }

            IsServiceStarted = true;
        }

        private DriverService? GetDriverService()
        {
            return browser switch
            {
                EnumBrowser.Chrome => ChromeDriverService.CreateDefaultService(),
                EnumBrowser.Firefox => FirefoxDriverService.CreateDefaultService(),
                _ => null,
            };
        }
        private DriverOptions? GetDriverOptions()
        {
            switch (browser)
            {
                case EnumBrowser.Chrome:
                    ChromeOptions chromeOptions = new();
                    //chromeOptions.AddArgument($"--window-size={windowWidth},{windowHeight}");
                    return chromeOptions;
                case EnumBrowser.Firefox:
                    FirefoxOptions firefoxOptions = new();
                    //firefoxOptions.AddArgument($"--window-size={windowWidth},{windowHeight}");
                    return firefoxOptions;
                default:
                    return null;
            }
        }
        private WebDriver? StartDriverService(DriverService? driverService, DriverOptions? driverOptions)
        {
            return browser switch
            {
                EnumBrowser.Chrome => new ChromeDriver((ChromeDriverService?)driverService, (ChromeOptions?)driverOptions),
                EnumBrowser.Firefox => new FirefoxDriver((FirefoxDriverService?)driverService, (FirefoxOptions?)driverOptions),
                _ => null,
            };
        }
        #endregion

        #region Selenium Automation
        public bool PerformNavigate(ActionNavigate actionNavigate)
        {
            WebDriver?.Navigate().GoToUrl(actionNavigate.URL);

            return false;
        }

        public async Task<bool> Start(Models.Core.Action? action)
        {
            if (action == null || Run == null) return false;

            IWebElement = null;
            EnumRunStatus enumRunStatus;

            if (action != null) Run?.SetRunStatus(action, EnumRunStatus.Running);

            //run action
            enumRunStatus = Run_Action(action) ? EnumRunStatus.Passed : EnumRunStatus.Failed;

            if (action != null) Run?.SetRunStatus(action, enumRunStatus);

            if (Run?.GetRunStatus(action) == EnumRunStatus.Failed) return false;

            await Task.Delay(GenericDelayInMilliSecods);

            if (action?.Perform != null)
            {
                action.Perform.Action = action;

                Run?.SetRunStatus(action.Perform, EnumRunStatus.Running);

                //run perform
                enumRunStatus = Run_Perform(action.Perform) ? EnumRunStatus.Passed : EnumRunStatus.Failed;

                Run?.SetRunStatus(action.Perform, enumRunStatus);

                if (Run?.GetRunStatus(action.Perform) == EnumRunStatus.Failed) return false;

                await Task.Delay(GenericDelayInMilliSecods);
            }

            return true;
        }
        private bool Run_Action(Models.Core.Action? action)
        {
            if (action == null) return true;

            by = null;
            try
            {
                switch (action)
                {
                    case ActionAdCloser actionAdCloser:
                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                        By adElemen = By.Id("dismiss-button");
                        WebDriver? webDriver = WebDriver;
                        IWebElement? iframeElement;

                        try
                        {
                            iframeElement = webDriver?.FindElement(By.XPath("//iframe[contains(@id, '_ads_iframe_')]"));
                            System.Diagnostics.Debug.WriteLine(iframeElement?.GetDomAttribute("id"));
                            webDriver?.SwitchTo().Frame(iframeElement);
                            webDriver?.FindElement(adElemen).Click();
                        }
                        catch (NoSuchElementException)
                        {
                            try
                            {
                                iframeElement = webDriver?.FindElement(By.XPath("//iframe[@id = 'ad_iframe']"));
                                System.Diagnostics.Debug.WriteLine(iframeElement?.GetDomAttribute("id"));
                                webDriver?.SwitchTo().Frame(iframeElement);
                                webDriver?.FindElement(adElemen).Click();
                            }
                            catch
                            {
                                //nothing to do
                            }
                        }
                        catch
                        {
                            //nothing to do
                        }
                        finally
                        {
                            webDriver?.SwitchTo().DefaultContent();
                        }

                        break;
                    case ActionManualInterrupt actionManualInterrupt:
                        Run?.ManualInterrupt();
                        break;
                    case ActionNavigate actionNavigate:
                        WebDriver?.Navigate().GoToUrl(actionNavigate.URL);
                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);
                        action.Screenshot = WebDriver?.GetScreenShotAsByteArray();
                        break;

                    case ActionMatchTitle actionMatchTitle:
                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);
                        break;

                    case ActionMatchURL actionMatchURL:
                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);
                        break;

                    case ActionFindHtmlElement actionFindHtmlElement:
                        if (actionFindHtmlElement.EnumActionFindHtmlElementBy == EnumActionFindHtmlElementBy.None)
                            return false;

                        by = actionFindHtmlElement.EnumActionFindHtmlElementBy switch
                        {
                            EnumActionFindHtmlElementBy.Id => By.Id(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.PartialId => By.XPath($"//*[@id = '{actionFindHtmlElement.FindHtmlElementValue}']"),
                            EnumActionFindHtmlElementBy.XPath => By.XPath(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.ClassName => By.ClassName(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.Name => By.Name(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.TagName => By.TagName(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.LinkText => By.LinkText(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.PartialLinkText => By.PartialLinkText(actionFindHtmlElement.FindHtmlElementValue),
                            EnumActionFindHtmlElementBy.UIText => By.XPath($"//*[text() = '{actionFindHtmlElement.FindHtmlElementValue}']"),
                            EnumActionFindHtmlElementBy.PartialUIText => By.XPath($"//*[contains(text(), '{actionFindHtmlElement.FindHtmlElementValue}')]"),
                            EnumActionFindHtmlElementBy.CSSSelector => By.CssSelector(actionFindHtmlElement.FindHtmlElementValue),
                            _ => null
                        };

                        IWebElement = WebDriver.TryFindElement(by, pageLoadTimeoutSeconds);

                        if (action.Perform != null && action.Perform is PerformElementDoesnotExists && IWebElement != null)
                        {
                            IWebElement = null;
                            throw new Exception(
                                $"Element find by " +
                                $"{actionFindHtmlElement.EnumActionFindHtmlElementBy.GetDescription()} " +
                                $"[{actionFindHtmlElement.FindHtmlElementValue.GetShorter(true)}] exists");
                        }

                        break;

                    case ActionWait actionWait:
                        Thread.Sleep(TimeSpan.FromSeconds(actionWait.Seconds.ConvertToInt(1)));
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
        }
        private bool Run_Perform(Perform perform)
        {
            if (perform == null) return true;

            try
            {
                switch (perform)
                {
                    //Action Find HTML Element > Perform
                    case PerformClick performClick:

                        if (performClick.CheckElementExists == true && IWebElement == null) return true;
                        
                        WebDriver.TryScrollToElement(IWebElement);

                        if (IWebElement == null) return false;

                        switch (performClick)
                        {
                            case PerformCheckBoxOn performCheckBoxOn:

                                if (IWebElement.TagName == "input" && IWebElement.GetAttribute("type") == "checkbox")
                                {
                                    if (IWebElement.Selected == false) IWebElement?.Click();
                                }
                                else
                                {
                                    IWebElement inputElement = IWebElement.FindElement(By.XPath(".//input[@type='checkbox']"));
                                    if (inputElement.Selected == false) IWebElement?.Click();
                                }

                                break;

                            case PerformCheckBoxOff performCheckBoxOff:

                                if (IWebElement.TagName == "input" && IWebElement.GetAttribute("type") == "checkbox")
                                {
                                    if (IWebElement.Selected == true) IWebElement?.Click();
                                }
                                else
                                {
                                    IWebElement inputElement = IWebElement.FindElement(By.XPath(".//input[@type='checkbox']"));
                                    if (inputElement.Selected == true) IWebElement?.Click();
                                }

                                break;

                            case PerformCheckBoxToggle performCheckBoxToggle:

                                IWebElement?.Click();

                                break;

                            default:

                                IWebElement?.Click();

                                break;
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(performClick.Seconds.ConvertToDouble(defaultWaitAfterClickSecond)));

                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                        break;

                    case PerformClickOffset performClickOffset:
                        if (IWebElement == null) return false;

                        WebDriver.TryScrollToElement(IWebElement);

                        if (string.IsNullOrWhiteSpace(performClickOffset.Offset) == true)
                        {
                            IWebElement?.Click();
                        }
                        else
                        {
                            int x, y = 0;
                            string[] arr = (performClickOffset.Offset + ",0,0").Split(',');
                            x = arr[0].ConvertToInt();
                            y = arr[1].ConvertToInt();
                            Actions moveAction = new Actions(WebDriver).MoveToElement(IWebElement).MoveByOffset(x, y);
                            if (performClickOffset.Action != null) performClickOffset.Action.Screenshot = WebDriver?.GetScreenShotAsByteArray();
                            moveAction?.Click().Perform();
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(defaultWaitAfterClickSecond));

                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                        break;

                    case PerformEnterText performEnterText:
                        if (IWebElement == null) return false;
                        
                        WebDriver.TryScrollToElement(IWebElement);

                        IWebElement.Clear();
                        string? expressionText = ExpressionEvaluator.Evaluate(performEnterText.Text);
                        IWebElement?.SendKeys(expressionText);

                        break;

                    case PerformMatchUIText performMatchUIText:
                        if (IWebElement == null) return false;

                        return IWebElement.Text == performMatchUIText.UIText;

                    case PerformChooseDropdownItem performChooseDropdownItem:
                        if (IWebElement == null) return false;

                        WebDriver.TryScrollToElement(IWebElement);

                        //IWebElement.Click();
                        //System.Threading.Thread.Sleep(360);

                        SelectElement selectElement = new (IWebElement);
                        if (string.IsNullOrWhiteSpace(performChooseDropdownItem.ByText) == false)
                        {
                            selectElement?.SelectByText(performChooseDropdownItem.ByText);
                        }
                        else
                        {
                            List<IWebElement> dropdownItems = 
                                selectElement.Options.Where(X => X?.GetDomProperty("value") != "").ToList();

                            if (dropdownItems.Count == 0) return false;

                            int index = new Random(Environment.TickCount).Next(0, dropdownItems.Count - 1);

                            selectElement?.SelectByText(dropdownItems[index].Text);
                        }

                        //IWebElement.Click();

                        break;

                    case PerformUploadFile performUploadFile:
                        if (IWebElement == null) return false;

                        WebDriver.TryScrollToElement(IWebElement);

                        if (IWebElement.TagName == "input" && IWebElement.GetAttribute("type") == "file")
                        {
                            IWebElement?.SendKeys(performUploadFile.FilePathAbsolute);
                        }
                        else
                        {
                            IWebElement inputElement = IWebElement.FindElement(By.XPath(".//input[@type='file']"));
                            IWebElement?.SendKeys(performUploadFile.FilePathAbsolute);
                        }

                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                        break;

                    //Other Perform
                    case PerformMatchTitle performMatchTitle:
                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                        return WebDriver?.Title == performMatchTitle.Title;

                    case PerformMatchURL performMatchURL:
                        WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                        perform.Screenshot = WebDriver?.GetScreenShotAsByteArray();

                        return WebDriver?.Url == performMatchURL.FullyQualifiedURL;
                }

                WebDriver?.TryWaitForPageReady(pageLoadTimeoutSeconds);

                perform.Screenshot = WebDriver?.GetScreenShotAsByteArray();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
        }
        #endregion

        public void DevTools()
        {
        }
    }
}
