package ru.ozon;


import org.junit.Assert;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class HomePg {

    private WebDriver driver;
    private WebDriverWait wait;

    public HomePg(WebDriver driver) {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, 240);
    }

    private String title = "OZON — интернет-магазин. Миллионы товаров по выгодным ценам";
    private By profileAnonymousButtonLocator = By.cssSelector("[data-widget=\"profileMenuAnonymous\"]");
    private By phoneFormLocator = By.cssSelector("[name=\"phone\"]");
    private By currentCityButtonLocator = By.cssSelector("div[role=\"navigation\"] button span");
    private By changeCityFormLocator = By.cssSelector("[class=\"modal-container\"]");
    private By searchQueryFormLocator = By.cssSelector("[placeholder=\"Искать на Ozon\"]");

    @FindBy(css = "[data-widget=\"profileMenuAnonymous\"]")
    private WebElement profileAnonymousButton;

    @FindBy(css = "[name=\"phone\"]")
    private WebElement phoneForm;

    @FindBy(css = "div[role=\"navigation\"] button span")
    private WebElement currentCityButton;

    @FindBy(css = "div[class=\"modal-container\"] input")
    private WebElement cityForm;

    @FindBy(css = "[placeholder=\"Искать на Ozon\"]")
    private WebElement searchQueryForm;

    public void open() {
        driver.get("https://www.ozon.ru/");
        Assert.assertTrue(driver.getTitle().equals(title));
    }

    public void openLoginForm() {
        wait.until(ExpectedConditions.visibilityOfElementLocated(profileAnonymousButtonLocator));
        profileAnonymousButton.click();
    }

    public void fillPhone() {
        wait.until(ExpectedConditions.visibilityOfElementLocated(phoneFormLocator));
        phoneForm.sendKeys("9873346147", Keys.ENTER);
    }

    public void changeCity() {
        wait.until(ExpectedConditions.visibilityOfElementLocated(currentCityButtonLocator));
        currentCityButton.click();
    }

    public void fillCity(String city) throws InterruptedException {
        wait.until(ExpectedConditions.visibilityOfElementLocated(changeCityFormLocator));
        cityForm.sendKeys(city);
        Thread.sleep(1000);
        cityForm.sendKeys(Keys.ENTER);
    }

    public void checkCurrentCity(String city) {
        wait.until(ExpectedConditions.textToBePresentInElement(currentCityButton, city));
    }

    public void fillSearchQuery() {
        wait.until(ExpectedConditions.visibilityOfElementLocated(searchQueryFormLocator));
        searchQueryForm.sendKeys("Соковыжималка", Keys.ENTER);
    }
}
