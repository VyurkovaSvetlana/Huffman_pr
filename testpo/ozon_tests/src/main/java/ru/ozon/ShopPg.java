package ru.ozon;

import org.openqa.selenium.*;
import org.openqa.selenium.support.FindBy;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import java.util.List;

public class ShopPg {
    private WebDriver driver;
    private WebDriverWait wait;

    public ShopPg(WebDriver driver) {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, 240);
        wait.until(ExpectedConditions.visibilityOfElementLocated(searchResultsFormLocator));
    }

    private By searchRadiusButtonLocator = By.cssSelector("[style=\"border-radius: 16px;\"]");
    private By searchResultsFormLocator = By.cssSelector("[data-widget=\"megaPaginator\"]");
    private By totalPriceFormLocator = By.cssSelector("[data-widget=\"totalv2\"]");

    @FindBy(css = "[qa-id=\"range-from\"]")
    private List<WebElement> valuesFromForm;

    @FindBy(css = "[qa-id=\"range-to\"]")
    private List<WebElement> valuesToForm;

    @FindBy(css = "[style=\"border-radius: 16px;\"] span")
    private WebElement searchRadiusButton;

    @FindBy(css = "[theme=\"select\"] input[role=\"combobox\"]")
    private WebElement sortingMethodForm;

    @FindBy(css = "a[href=\"/cart\"] span")
    private WebElement cartButton;

    public void fillPriceFromFilter() throws InterruptedException {
        for (int i = 0; i < 6; i++) {
            valuesFromForm.get(0).sendKeys(Keys.BACK_SPACE);
            Thread.sleep(500);
        }
        valuesFromForm.get(0).sendKeys("3000");
        Thread.sleep(500);
        valuesFromForm.get(0).sendKeys(Keys.ENTER);
        wait.until(ExpectedConditions.visibilityOfElementLocated(searchRadiusButtonLocator));
    }

    public void fillPriceToFilter() throws InterruptedException {
        for (int i = 0; i < 6; i++) {
            valuesToForm.get(0).sendKeys(Keys.BACK_SPACE);
            Thread.sleep(500);
        }
        valuesToForm.get(0).sendKeys("4000", Keys.ENTER);
        wait.until(ExpectedConditions.textToBePresentInElement(searchRadiusButton, "Цена: от 3 000 до 4 000"));
    }

    public void fillPowerFromFilter() throws InterruptedException {
        for (int i = 0; i < 6; i++) {
            valuesFromForm.get(1).sendKeys(Keys.BACK_SPACE);
            Thread.sleep(500);
        }
        valuesFromForm.get(1).sendKeys("1100");
        Thread.sleep(500);
        valuesFromForm.get(1).sendKeys(Keys.ENTER);
        wait.until(ExpectedConditions.textToBePresentInElement(searchRadiusButton, "Мощность, Вт: от 1 100 до 1 700"));
    }

    public void changeFiltrationMethod() throws InterruptedException {
        sortingMethodForm.click();
        Thread.sleep(1000);
        for (int i = 0; i < 2; i++) {
            sortingMethodForm.sendKeys(Keys.ARROW_DOWN);
            Thread.sleep(500);
        }
        sortingMethodForm.sendKeys(Keys.ENTER);
        Thread.sleep(3000);
    }

    public void addSecondPositionToCart() {
        WebElement addButton = driver.findElement(By.cssSelector("[data-widget=\"megaPaginator\"] > div"))
                .findElement(By.cssSelector("div[data-widget=\"searchResultsV2\"]"))
                .findElements(By.cssSelector("div > div[style=\"grid-column-start: span 12;\"]")).get(1)
                .findElements(By.cssSelector("div > div > div[style=\"width: 25%; max-width: 25%; flex: 0 0 25%;\"]"))
                .get(1).findElements(By.cssSelector("div > button")).get(1);
        JavascriptExecutor executor = (JavascriptExecutor)driver;
        executor.executeScript("arguments[0].click();", addButton);
        wait.until(ExpectedConditions.textToBePresentInElement(cartButton, "1"));
    }

    public void checkCart() {
        cartButton.click();
        wait.until(ExpectedConditions.visibilityOfElementLocated(totalPriceFormLocator));
    }
}
