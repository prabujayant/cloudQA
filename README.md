# CloudQA Automation Practice Form Tests (C# + Selenium + NUnit)

This project contains automated UI tests for the **CloudQA Automation Practice Form**:

> https://app.cloudqa.io/home/AutomationPracticeForm

The tests are written in **C#**, use **Selenium WebDriver** to drive **Google Chrome**, and are organized as **NUnit** tests.

The primary goal is to demonstrate **robust, maintainable locators** that keep working even if:

- Elements move around on the page (layout changes), or  
- Most HTML attributes on the fields (such as `id`, `name`, `class`, `placeholder`) change.

The tests rely mainly on **human-visible text** (labels) and **document structure** instead of fragile attributes.

---

## Project Structure

Repository root (your `automation` folder):

- `CloudQATests/`
  - `CloudQATests.csproj` – .NET test project file (NUnit + Selenium)
  - `AutomationPracticeTests.cs` – main test class containing the three Selenium tests
  - `bin/`, `obj/` – build output (auto-generated)
- `AutomationPracticeTests.cs` (optional copy, not wired into the `CloudQATests` project)
- `README.md` – this document

All **executable tests** live under `CloudQATests` and are run by `CloudQATests.csproj`.

---

## Prerequisites

Ensure you have the following installed on your machine:

- **.NET SDK** (8.0 or later is sufficient; the project targets `net10.0` which is preview, but the CLI you already used to run `dotnet test` is fine)
- **Google Chrome** (current stable version)
- Internet access (the tests open the live CloudQA site)

The project already references these NuGet packages:

- `Selenium.WebDriver`
- `Selenium.WebDriver.ChromeDriver`
- `NUnit`
- `NUnit3TestAdapter`
- `Microsoft.NET.Test.Sdk`

No manual download of `chromedriver.exe` is required; it is managed via the NuGet package.

---

## How to Run the Tests

### 1. From the Command Line (PowerShell)

1. Open a PowerShell window.
2. Change directory to the test project:

   ```powershell
   cd "C:\Users\salag\OneDrive\Desktop\automation\CloudQATests"
   ```

3. Run the tests:

   ```powershell
   dotnet test
   ```

What you should see:

- .NET will build the project and download any missing NuGet packages.
- Chrome will open several times (once per test, depending on parallelization).
- The command will finally display a summary like:

```text
Passed!  - Failed: 0, Passed: 3, Skipped: 0, Total: 3
```

### 2. From Visual Studio / Rider / VS Code

1. Open the solution or the project `CloudQATests\CloudQATests.csproj` in your IDE.
2. Build the project once to restore packages.
3. Open the **Test Explorer** (or equivalent test view).
4. Run the test class **`AutomationPracticeTests`** or individual tests under it.

---

## Test Class Overview

File: `CloudQATests/AutomationPracticeTests.cs`
Namespace: `CloudQAFormTests`

This class uses **NUnit** attributes:

- `[TestFixture]` – marks the class as a test suite.
- `[SetUp]` – runs **before each test**, initializes the Chrome WebDriver and navigates to the form.
- `[Test]` – marks each individual test.
- `[TearDown]` – runs **after each test**, closing and disposing the browser.

### Setup / Teardown

In `Setup()`:

- A new `ChromeDriver` instance is created.
- The browser window is maximized.
- The driver navigates to `https://app.cloudqa.io/home/AutomationPracticeForm`.
- A short implicit wait is configured.

In `Teardown()`:

- The driver is properly closed and disposed with:
  - `driver?.Quit();`
  - `driver?.Dispose();`

This guarantees that each test runs with a clean browser instance and that system resources are released.

---

## What Exactly Is Tested?

The project contains **three main tests**, each targeting a different field on the form.

### 1. First Name Field – `TestFirstNameInput`

**Goal:** Verify that the **First Name** input accepts text and can be cleared.

**Locator strategy (robustness):**

```csharp
By firstNameLocator = By.XPath(
    "//form[@id='automationtestform']//label[normalize-space()='First Name']/following::input[1]"
);
```

Key points:

- The locator starts at the main form with `id='automationtestform'`, so it does not accidentally hit similar elements in other demos on the page (like Shadow DOM versions).
- It looks for the **label text** `"First Name"` (what the user sees).
- It selects the **first input that follows this label**, regardless of the input’s `id`, `name`, `class`, or `placeholder`.

**Behavior tested:**

1. Type a sample name (e.g., `"Jane Doe"`) into the field.
2. Assert that the field’s value equals the entered text.
3. Clear the field.
4. Assert that the field’s value is now empty.

If the input’s id, name, placeholder, or CSS class changes—but the label text “First Name” and the basic label→input structure remain—the locator will still work.

---

### 2. Gender – Female Radio Button – `TestGenderSelection_Female`

**Goal:** Verify that selecting the **Female** gender option correctly checks its radio button.

**Locator strategy (robustness):**

```csharp
By femaleInputLocator = By.XPath(
    "//form[@id='automationtestform']//span[normalize-space()='Female']/preceding-sibling::input[1]"
);
```

Key points:

- The radio buttons are associated with visible text rendered in `span` elements (“Male”, “Female”, “Transgender”).
- This XPath:
  - Limits itself to the main form.
  - Finds the span whose visible text is `"Female"`.
  - Then takes the **immediately preceding input** sibling, which is the actual radio input.
- The locator does **not** depend on the radio button’s `id`, `name`, `value`, `class`, or on its exact position in a row of options.

**Behavior tested:**

1. Find the `Female` radio input using the robust XPath above.
2. Click the radio input.
3. Assert that `femaleInput.Selected` is `true`.

The test will keep working as long as the visible text “Female” and the input–span relationship remain, even if attributes like `id="female"` or `value="Female"` are renamed or reordered.

---

### 3. Email Field – `TestEmailInput`

**Goal:** Verify that the **Email** field accepts text and can be cleared.

**Locator strategy (robustness):**

```csharp
By emailLocator = By.XPath(
    "//form[@id='automationtestform']//label[normalize-space()='Email']/following::input[1]"
);
```

Key points:

- Similar pattern to the First Name test:
  - Start from the main form.
  - Find the **label with text `"Email"`**.
  - Select the first input that follows that label.
- This is resilient to changes in `id`, `name`, `type`, `class`, or `placeholder` attributes on the input itself.

**Behavior tested:**

1. Enter a sample email (e.g., `"robust.test@example.com"`).
2. Assert that the field’s value equals the entered text.
3. Clear the field.
4. Assert that the field’s value is empty.

---

## How the Tests Stay Robust Against HTML Changes

To satisfy the requirement _“Design your tests so that they keep working even if the elements’ position or any HTML attributes for those three fields change”_, the tests apply these principles:

- **Anchor to stable visual text, not technical attributes**
  - Use `normalize-space()='First Name'`, `'Email'`, and `'Female'` instead of `id="fname"`, `name="Email"`, etc.

- **Constrain searches to the main form**
  - All XPath expressions begin with `//form[@id='automationtestform']` to avoid matching elements in other examples on the same page, such as Shadow DOM demo forms.

- **Use relative relationships**
  - `label` → `following::input[1]` (for First Name and Email).
  - `span` text → `preceding-sibling::input[1]` (for gender radios).
  - This makes them robust against layout reordering or additional classes being added.

As long as the visible label text remains the same and the basic “label next to input” relationship is preserved, the tests will continue to locate the correct elements.

---

## Troubleshooting

**Chrome does not open or tests fail to start**

- Make sure Google Chrome is installed.
- Run `dotnet restore` in `CloudQATests` to ensure all packages are downloaded.

**`WebDriverException` or version mismatch errors**

- Sometimes Chrome is newer than the packaged ChromeDriver.
- Update the NuGet package `Selenium.WebDriver.ChromeDriver` in `CloudQATests.csproj` using your IDE’s NuGet manager or via:

  ```powershell
  cd "C:\Users\salag\OneDrive\Desktop\automation\CloudQATests"
  dotnet add package Selenium.WebDriver.ChromeDriver
  ```

**Tests suddenly start failing with `NoSuchElementException`**

- The site markup might have changed.
- Inspect the page in the browser (F12 → Elements) and verify:
  - The form still has `id="automationtestform"`.
  - The labels still show the same visible text (“First Name”, “Email”, “Female”).
  - The label and input (or span and radio input) are still directly related in the DOM.

If the visible texts change (e.g., “First Name” → “Given Name”), update the text inside the XPath expressions accordingly.

---

## Extending the Tests

If you want to add more robust tests for other fields on the page, follow the same pattern:

1. Identify a **stable, user-visible label or text** near the element.
2. Write an XPath that:
   - Starts from `//form[@id='automationtestform']`
   - Uses the label/text to locate the nearby element (`following::`, `preceding-sibling::`, etc.).
3. Interact with the element (type, click, select) and make the appropriate NUnit assertions.

Example idea:

- Country autocomplete field – anchored to the label text “Country”.
- Hobbies checkboxes – anchored to the displayed texts “Dance”, “Reading”, “Cricket” and their nearby checkboxes.

This keeps your tests expressive and resilient to most HTML refactors.

---

## Quick Summary

- **Technology stack:** C#, .NET, Selenium WebDriver (Chrome), NUnit.
- **Target page:** CloudQA Automation Practice Form.
- **Tests implemented:**
  - First Name text input
  - Gender – Female radio button
  - Email text input
- **Resilience techniques:**
  - Locators based on form id + label/visible text.
  - Minimal reliance on volatile attributes (`id`, `name`, `class`, `placeholder`).
  - Use of NUnit 4 assertion syntax and proper WebDriver disposal.

Run everything with:

```powershell
cd "C:\Users\salag\OneDrive\Desktop\automation\CloudQATests"
dotnet test
```

You now have a maintainable, example automation suite that demonstrates how to write robust UI tests against a real public demo form.

