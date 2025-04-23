import http from 'k6/http';
import { check, sleep } from 'k6';
import { parseHTML } from 'k6/html';

export default function () {
    const BASE_URL = 'https://localhost:7028';

    // 1. Pobranie strony logowania w celu uzyskania tokenu CSRF
    let loginPageResponse = http.get(`${BASE_URL}/Account/Login`);

    // Sprawdzenie czy strona logowania została poprawnie załadowana
    check(loginPageResponse, {
        'Pomyślne załadowanie strony logowania': (r) => r.status === 200
    });

    // Parsowanie tokenu anty-CSRF ze strony logowania
    let doc = parseHTML(loginPageResponse.body);
    let token = doc.find('input[name="__RequestVerificationToken"]').attr('value');
    console.log('Token anty-CSRF: ' + token);

    // Zachowanie ciasteczka AntiForgery z odpowiedzi
    let antiForgeryCookie = loginPageResponse.cookies.AntiForgery;
    console.log('Ciasteczko AntiForgery: ' + JSON.stringify(antiForgeryCookie, null, 2));

    // 2. Przygotowanie danych logowania
    let loginPayload = {
        'Email': 'admin1@pl',
        'Password': 'Hasloha11!',
        'login-submit': 'Log In',
        '__RequestVerificationToken': token 
    };

    // Przygotowanie nagłówków z tokenem CSRF
    let params = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-CSRF-TOKEN': token, 
        },
        redirects: 0 // Zapobiega automatycznemu przekierowaniu, aby móc przeanalizować nagłówki odpowiedzi
    };

    // 3. Wykonanie żądania logowania
    let loginResponse = http.post(`${BASE_URL}/Account/Login`, loginPayload, params);

    // Sprawdzenie odpowiedzi logowania
    check(loginResponse, {
        'Logowanie zakończone sukcesem (302 redirect)': (r) => r.status === 302,
        'Posiada ciasteczko uwierzytelnienia': (r) => r.cookies['.AspNetCore.Identity.Application'] !== undefined
    });

    // 4. Wydobycie i wyświetlenie ciasteczka uwierzytelniającego
    let authCookie = null;
    if (loginResponse.cookies['.AspNetCore.Identity.Application']) {
        console.log('Pobrano ciasteczko .AspNetCore.Identity.Application:');
        console.log(JSON.stringify(loginResponse.cookies['.AspNetCore.Identity.Application'], null, 2));

        // Zapisanie ciasteczka do użycia w innych testach
        authCookie = loginResponse.cookies['.AspNetCore.Identity.Application'];
    }

    // 5. Przygotowanie nagłówków dla kolejnych żądań
    let headers = {
        'X-CSRF-TOKEN': token
    };

    // Przygotowanie ciasteczek dla kolejnych żądań
    let cookies = {
        '.AspNetCore.Identity.Application': authCookie ? authCookie.value : '',
        'AntiForgery': antiForgeryCookie ? antiForgeryCookie.value : ''
    };

    // 6. Wykonanie żądania do stron z listą ogłoszeń oraz użytkowników z użyciem pobranego ciasteczka
    let advertisementPageResponse = http.get(`${BASE_URL}/Advertisement`, {
        headers: headers,
        cookies: cookies
    });

    check(advertisementPageResponse, {
        'Pomyślne załadowanie strony ogłoszeń': (r) => r.status === 200
    });
    //console.log('Advertisement Response:', JSON.stringify(advertisementPageResponse));

    let usersPageResponse = http.get(`${BASE_URL}/User`, {
        headers: headers,
        cookies: cookies
    });

    check(usersPageResponse, {
        'Pomyślne załadowanie strony użytkowników': (r) => r.status === 200
    });
    //console.log('Users Response:', JSON.stringify(usersPageResponse));

    // 7. Wykonanie żądania Update dla ogłoszenia o ID 15
    //Przejście na strone edycji ogłoszenia o id 15
    let editPageResponse = http.get(`${BASE_URL}/Advertisement`, {
        headers: headers,
        cookies: cookies
    });
    check(editPageResponse, {
        'Pomyślne załadowanie strony edycji ogłoszenia nr 15': (r) => r.status === 200
    });
    //console.log('editPage:', JSON.stringify(editPageResponse));

    // Parsowanie nowego tokenu anty-CSRF ze strony logowania
    doc = parseHTML(editPageResponse.body);
    token = doc.find('input[name="__RequestVerificationToken"]').attr('value');
    console.log('Nowy token anty-CSRF: ' + token);
    headers = {
        'X-CSRF-TOKEN': token
    };
    function generateRandomPrice() {
        let randomNumber = Math.floor(Math.random() * 9999) + 1; // Losowa liczba od 1 do 9999
        return `${randomNumber} zł`; // Łączenie liczby z "zł"
    }
    let formData = {
        'Title': 'Oddam regały na książki',
        'Description': 'Dwa takie same regały z IKEA, 5 półek, wymiary 200x80x30 cm',
        'Price': generateRandomPrice(),
        'Duration': 'Brak',
        '__RequestVerificationToken': token
    };
    // Wykonanie żądania post w celu edycji danych ogłoszenia o ID 15
    const editResponse = http.post(`${BASE_URL}/Advertisement/UpdateAdvertisement/15`, formData,{
        headers: headers,
        cookies: cookies
    });

    check(editResponse, {
        'Pomyślna edycja ogłoszenia nr 15': (r) => r.status === 200
    });
    //console.log('Advertisement Update Response Status:', editResponse.status);

    sleep(1);
}