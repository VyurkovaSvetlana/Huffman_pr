package ru.ozon;

import org.junit.After;
import org.junit.Before;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.support.ui.WebDriverWait;

public class WebDriwerSettings {
    public ChromeDriver driver;
    public WebDriverWait wait;

    //перед каждым тестом
    @Before
    public void setUp() {
        System.setProperty("webdriver.chrome.driver", "C:/chromedriver.exe");
        driver = new ChromeDriver();
        wait = new WebDriverWait(driver, 240);
    }

    //по окончании теста
    @After
    public void close() {
        driver.quit();
    }
}
