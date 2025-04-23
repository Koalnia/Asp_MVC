import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { parseHTML } from 'k6/html';
import { SharedArray } from 'k6/data';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

// Konfiguracja testu obciążeniowego
export const options = {
    stages: [
        { duration: '5s', target: 4 }, // Rozgrzewka: 4 użytkowników w 5 sekund
        { duration: '20s', target: 10 }, // Obciążenie: 10 użytkowników przez 20 sekund
        { duration: '5s', target: 0 }, // Redukcja: do 0 użytkowników w 5 sekund
    ],
    thresholds: {
        http_req_duration: ['p(95)<3000'], // 95% requestów musi być poniżej 3s
        http_req_failed: ['rate<0.1'], // Poziom błędów poniżej 10%
    },
};

// Dane testowe - różni użytkownicy
const users = new SharedArray('users', function () {
    return [
        { email: 'admin1@pl', password: 'Hasloha11!' },
        { email: 'anna.kowalska@gmail.com', password: 'Password123!' },
        { email: 'jan.nowak@gmail.com', password: 'Qwerty456!' },
        { email: 't.zielinski@gmail.com', password: 'Pass1234!' }
    ];
});

// Lista ID ogłoszeń do testów
const advertisementIds = [12, 13, 14, 15];

// Funkcja generująca losową cenę
function generateRandomPrice() {
    let randomNumber = Math.floor(Math.random() * 9999) + 1; // Losowa liczba od 1 do 9999
    return `${randomNumber} zł`; // Łączenie liczby z "zł"
}

export default function () {
    const BASE_URL = 'https://localhost:7028';
    const selectedUser = users[randomIntBetween(0, users.length - 1)];
    let authCookie = null;
    let antiForgeryCookie = null;
    let token = null;

    group('Logowanie', function () {
        // 1. Pobranie strony logowania w celu uzyskania tokenu CSRF
        let loginPageResponse = http.get(`${BASE_URL}/Account/Login`);

        // Sprawdzenie czy strona logowania została poprawnie załadowana
        check(loginPageResponse, {
            'Pomyślne załadowanie strony logowania': (r) => r.status === 200
        });

        // Parsowanie tokenu anty-CSRF ze strony logowania
        let doc = parseHTML(loginPageResponse.body);
        token = doc.find('input[name="__RequestVerificationToken"]').attr('value');

        // Zachowanie ciasteczka AntiForgery z odpowiedzi
        antiForgeryCookie = loginPageResponse.cookies.AntiForgery;

        // 2. Przygotowanie danych logowania z losowo wybranym użytkownikiem
        let loginPayload = {
            'Email': selectedUser.email,
            'Password': selectedUser.password,
            'login-submit': 'Log In',
            '__RequestVerificationToken': token
        };

        // Przygotowanie nagłówków z tokenem CSRF
        let params = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-CSRF-TOKEN': token,
            },
            redirects: 0 // Zapobiega automatycznemu przekierowaniu
        };

        // 3. Wykonanie żądania logowania
        let loginResponse = http.post(`${BASE_URL}/Account/Login`, loginPayload, params);

        // Sprawdzenie odpowiedzi logowania
        check(loginResponse, {
            'Logowanie zakończone sukcesem (302 redirect)': (r) => r.status === 302,
            'Posiada ciasteczko uwierzytelnienia': (r) => r.cookies['.AspNetCore.Identity.Application'] !== undefined
        });

        // 4. Wydobycie ciasteczka uwierzytelniającego
        if (loginResponse.cookies['.AspNetCore.Identity.Application']) {
            authCookie = loginResponse.cookies['.AspNetCore.Identity.Application'];
        }

        // Symulacja czasu, jaki użytkownik spędza po zalogowaniu
        sleep(randomIntBetween(1, 2));
    });

    // 5. Przygotowanie nagłówków dla kolejnych żądań
    let headers = {
        'X-CSRF-TOKEN': token
    };

    // Przygotowanie ciasteczek dla kolejnych żądań
    let cookies = {
        '.AspNetCore.Identity.Application': authCookie ? authCookie.value : '',
        'AntiForgery': antiForgeryCookie ? antiForgeryCookie.value : ''
    };

    group('Przeglądanie', function () {
        // 6. Wykonanie żądania do stron z listą ogłoszeń
        let advertisementPageResponse = http.get(`${BASE_URL}/Advertisement`, {
            headers: headers,
            cookies: cookies
        });

        check(advertisementPageResponse, {
            'Pomyślne załadowanie strony ogłoszeń': (r) => r.status === 200
        });

        // Symulacja przeglądania strony
        sleep(randomIntBetween(1, 3));

        // Wykonanie żądania do strony użytkowników
        let usersPageResponse = http.get(`${BASE_URL}/User`, {
            headers: headers,
            cookies: cookies
        });

        check(usersPageResponse, {
            'Pomyślne załadowanie strony użytkowników': (r) => r.status === 200
        });

        // Symulacja przeglądania strony
        sleep(randomIntBetween(1, 2));
    });

    group('Edycja ogłoszenia', function () {
        // Wybór losowego ID ogłoszenia z listy
        const adId = advertisementIds[randomIntBetween(0, advertisementIds.length - 1)];

        // 7. Przejście na stronę edycji losowego ogłoszenia
        let editPageResponse = http.get(`${BASE_URL}/Advertisement`, {
            headers: headers,
            cookies: cookies
        });

        check(editPageResponse, {
            [`Pomyślne załadowanie strony edycji ogłoszenia nr ${adId}`]: (r) => r.status === 200
        });

        // Parsowanie nowego tokenu anty-CSRF ze strony
        let doc = parseHTML(editPageResponse.body);
        let newToken = doc.find('input[name="__RequestVerificationToken"]').attr('value');

        // Aktualizacja nagłówków z nowym tokenem
        headers = {
            'X-CSRF-TOKEN': newToken
        };

        // Przygotowanie losowych danych do aktualizacji
        let formData = {
            'Title': `Zaktualizowane ogłoszenie ${new Date().toISOString()}`,
            'Description': `Opis zaktualizowany podczas testu obciążeniowego, ID: ${adId}`,
            'Price': generateRandomPrice(),
            'Duration': 'Brak',
            '__RequestVerificationToken': newToken
        };

        // Wykonanie żądania post w celu edycji danych ogłoszenia
        const editResponse = http.post(`${BASE_URL}/Advertisement/UpdateAdvertisement/${adId}`, formData, {
            headers: headers,
            cookies: cookies
        });

        check(editResponse, {
            [`Pomyślna edycja ogłoszenia nr ${adId}`]: (r) => r.status === 200
        });

        // Symulacja czasu spędzonego po edycji
        sleep(randomIntBetween(2, 4));
    });
}