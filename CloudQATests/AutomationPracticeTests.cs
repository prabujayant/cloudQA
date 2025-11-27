using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace CloudQAFormTests
{
    [TestFixture]
    public class AutomationPracticeTests
    {
        private IWebDriver driver;
        private const string Url = "https://app.cloudqa.io/home/AutomationPracticeForm";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(Url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        public void TestFirstNameInput()
        {
            // Locate the First Name input by its label text, not by attributes on the input itself.
            // This keeps the test stable even if the input's id/name/placeholder change or it moves within the form.
            By firstNameLocator = By.XPath("//form[@id='automationtestform']//label[normalize-space()='First Name']/following::input[1]");
            
            try
            {
                IWebElement firstNameField = driver.FindElement(firstNameLocator);
                string testName = "Jane Doe";

                firstNameField.SendKeys(testName);
                Assert.That(firstNameField.GetAttribute("value"), Is.EqualTo(testName));
                firstNameField.Clear();
                Assert.That(firstNameField.GetAttribute("value"), Is.Empty);
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The 'First Name' input field could not be found.");
            }
        }

        [Test]
        public void TestGenderSelection_Female()
        {
            // The visible text "Female" is used on a span next to the actual radio input.
            // We anchor the lookup within the main form so reordering or id/value changes do not break the locator.
            By femaleInputLocator = By.XPath("//form[@id='automationtestform']//span[normalize-space()='Female']/preceding-sibling::input[1]");
            
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement femaleInput = wait.Until(d => d.FindElement(femaleInputLocator));

                femaleInput.Click();

                Assert.That(femaleInput.Selected, Is.True, "The Female gender radio button was not selected.");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The 'Female' gender option could not be found.");
            }
        }
        
        [Test]
        public void TestEmailInput()
        {
            // Locate the Email input via its label text inside the main form.
            By emailLocator = By.XPath("//form[@id='automationtestform']//label[normalize-space()='Email']/following::input[1]");
            
            try
            {
                IWebElement emailField = driver.FindElement(emailLocator);
                string testEmail = "robust.test@example.com";

                emailField.SendKeys(testEmail);
                Assert.That(emailField.GetAttribute("value"), Is.EqualTo(testEmail));
                emailField.Clear();
                Assert.That(emailField.GetAttribute("value"), Is.Empty);
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The 'Email' input field could not be found.");
            }
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
