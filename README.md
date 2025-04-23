# Asp_MVC
Aplikacja Asp_MVC, będą projektem aplikacji w architekturze MVC we frameworku Asp.Net Core oraz silniku szablonów Razor. 
Jest to przykład wykorzystania frameoworku Asp.Net Core jako frameworku tworzenia backendu aplikacji internetowych, napisany na potrzeby pracy dyplomowej.
Backend aplikacji napisany jest w Asp.Net Core, frontend napisany jest w silniku szablonów stron HTML - Razor, a wybrana baza danych to PostgreSQL.
Aplikacja tworzy serwis ogłoszeń, gdzie użytkownicy mogą zamieszczać ogłoszenia o chęci zakupu, sprzedaży lub darmowego oddania określonych przedmiotów lub usług. 
Zawiera dwie klasy - klasę User opisującą użytkowników aplikacji oraz klasę Advertisement opisującą ogłoszenia aplikacji w relacji  jeden-do-wielu (ang. One-to-Many), gdzie:
•	jeden użytkownik (User) może mieć wiele ogłoszeń (Advertisement);
•	każde ogłoszenie (Advertisement) należy do jednego użytkownika (User).
Klasa użytkowników przechowuje identyfikator użytkownika (ang. id), jego imię i nazwisko, numer telefonu, adres email oraz hasło logowania, przechowywane w postaci hasz. 
Każde ogłoszenie przechowuje jego identyfikator (id), tytuł, opis, koszt, czas trwania oraz termin jego utworzenia. 
Użytkownicy dzielą się na dwie role - zwykłych użytkowników o roli USER oraz administratorów o roli ADMIN. Zwykli użytkownicy mogą edytować dane tylko swojego konta oraz swoich ogłoszeń (oraz je usuwać). 
Administrator ma dostęp do edycji oraz usuwania wszystkich kont użytkowników oraz ogłoszeń. Nie może on jednak usunąć własnego konta. 
Aplikacja wczytuje domyślne wartości do bazy danych, zawierające trzech użytkowników administratorów, siedmiu zwykłych użytkowników oraz trzydzieści ogłoszeń. 
Aplikacja ustawiona jest, żeby sama usuwała i generowała od nowa strukturę bazy danych oraz jej dane przy każdym  uruchomieniu. 
W celu zarządzania pakietami projektów Asp.Net Core używa narzędzia NuGet.
Aplikacja zawiera także testy wydajnościowe napisane w technologii k6.
