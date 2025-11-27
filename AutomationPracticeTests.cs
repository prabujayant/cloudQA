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
            // The ChromeDriver package handles finding/downloading the driver executable.
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(Url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        public void TestFirstNameInput()
        {
            // Robust Locator: Find input by placeholder 'Name'
            By firstNameLocator = By.XPath("//input[@placeholder='Name']");
            
            try
            {
                IWebElement firstNameField = driver.FindElement(firstNameLocator);
                string testName = "Jane Doe";

                firstNameField.SendKeys(testName);
                Assert.AreEqual(testName, firstNameField.GetAttribute("value"));
                firstNameField.Clear();
                Assert.IsEmpty(firstNameField.GetAttribute("value"));
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The 'First Name' input field could not be found.");
            }
        }

        [Test]
        public void TestGenderSelection_Female()
        {
            // Robust Locator: Find the label with the text 'Female'
            By femaleLabelLocator = By.XPath("//label[normalize-space(text())='Female']");
            
            try
            {
                IWebElement femaleLabel = driver.FindElement(femaleLabelLocator);
                
                femaleLabel.Click();

                // Find the associated radio button to assert its state
                By femaleInputLocator = By.XPath("//label[normalize-space(text())='Female']/input[@type='radio'] | //input[@type='radio' and (@value='Female' or @name='gender') and following-sibling::text()[normalize-space()='Female']]");
                
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement femaleInput = wait.Until(d => d.FindElement(femaleInputLocator));

                Assert.IsTrue(femaleInput.Selected, "The Female gender radio button was not selected.");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The 'Female' gender option could not be found.");
            }
        }
        
        [Test]
        public void TestEmailInput()
        {
            // Robust Locator: Find input by placeholder 'Email'
            By emailLocator = By.XPath("//input[@placeholder='Email']");
            
            try
            {
                IWebElement emailField = driver.FindElement(emailLocator);
                string testEmail = "robust.test@example.com";

                emailField.SendKeys(testEmail);
                Assert.AreEqual(testEmail, emailField.GetAttribute("value"));
                emailField.Clear();
                Assert.IsEmpty(emailField.GetAttribute("value"));
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The 'Email' input field could not be found.");
            }
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}