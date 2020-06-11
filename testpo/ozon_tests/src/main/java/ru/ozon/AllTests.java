package ru.ozon;

import org.junit.Test;
import org.openqa.selenium.support.PageFactory;
import ru.ozon.HomePg;
import ru.ozon.UserPg;
import ru.ozon.ShopPg;

public class AllTests extends WebDriwerSettings{
    private String city = "Вольск";

    @Test
    public void login() throws InterruptedException {

        HomePg homePg = PageFactory.initElements(driver, HomePg.class);
        UserPg userPg = PageFactory.initElements(driver, UserPg.class);

        homePg.open();
        homePg.openLoginForm();
        homePg.fillPhone();
        userPg.checkAuthorization();
    }

    @Test
    public void changeCity() throws InterruptedException {

        HomePg homePg = PageFactory.initElements(driver, HomePg.class);
        UserPg userPg = PageFactory.initElements(driver, UserPg.class);

        homePg.open();
        homePg.changeCity();
        homePg.fillCity(city);
        homePg.checkCurrentCity(city);
        homePg.openLoginForm();
        homePg.fillPhone();
        userPg.open();
        homePg.checkCurrentCity(city);
    }

    @Test
    public void filtrationByPrice() throws InterruptedException {

        HomePg homePg = PageFactory.initElements(driver, HomePg.class);

        homePg.open();
        homePg.fillSearchQuery();

        ShoppPg shopPg = PageFactory.initElements(driver, ShopPg.class);

        shopPg.fillPriceFromFilter();
        shopPg.fillPriceToFilter();
        shopPg.changeFiltrationMethod();
        shopPg.addSecondPositionToCart();
        shopPg.checkCart();
    }

    @Test
    public void filtrationByPower() throws InterruptedException {
        HomePg homePg = PageFactory.initElements(driver, HomePg.class);

        homePg.open();
        homePg.fillSearchQuery();

        ShopPg shopPg = PageFactory.initElements(driver, ShopPg.class);

        shopPg.fillPowerFromFilter();
        shopPg.changeFiltrationMethod();
        shopPg.addSecondPositionToCart();
        shopPg.checkCart();
    }
}
