package ru.ozon;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class UserPg {
    private WebDriver driver;
    private WebDriverWait wait;

    public UserPg(WebDriver driver) {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, 240);
    }

    private By profileUserButtonLocator = By.cssSelector("[href=\"/my/settings\"]");
    private By wallpaperFormLocator = By.cssSelector("[data-widget=\"wallpaper\"]");

    @FindBy(css = "[href=\"/my/settings\"]")
    private WebElement profileUserButton;

    public void open() {
        wait.until(ExpectedConditions.visibilityOfElementLocated(profileUserButtonLocator));
        profileUserButton.click();
        wait.until(ExpectedConditions.visibilityOfElementLocated(wallpaperFormLocator));
    }

    public void checkAuthorization() {
        wait.until(ExpectedConditions.visibilityOfElementLocated(profileUserButtonLocator));
    }
}
