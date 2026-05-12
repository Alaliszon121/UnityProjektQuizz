## Quiz App - Kreator i Solver Quizów

## Opis projektu

Quiz App to kompleksowa aplikacja stworzona w silniku Unity, służąca do tworzenia, zarządzania oraz rozwiązywania interaktywnych testów wiedzy. Projekt został podzielony na dwa niezależne środowiska: moduł twórcy (Quiz Creator) umożliwiający budowanie własnych baz pytań oraz moduł gracza (Quiz Solver) służący do ich rozwiązywania pod presją czasu. Aplikacja przechowuje dane lokalnie w uniwersalnym formacie JSON, co ułatwia ewentualne modyfikacje plików z zewnątrz.

<img width="1920" height="1109" alt="MenuGeneratoraQuizów" src="https://github.com/user-attachments/assets/28627740-8c73-4632-ab4d-66c1c5904a0a" />


## Główne funkcjonalności

### Moduł Kreatora (Quiz Creator)

* **Budowanie od podstaw:** Możliwość stworzenia nowego quizu, nadania mu unikalnej nazwy oraz zdefiniowania nieograniczonej liczby pytań.
* **Obsługa wielu typów pytań:** Obsługa pytań wielokrotnego wyboru (z możliwością zdefiniowania do 10 odpowiedzi) oraz pytań typu Prawda/Fałsz.
* **System wag:** Każde pytanie posiada własny modyfikator punktowy (mnożnik), co pozwala na różnicowanie wagi trudniejszych zagadnień.
* **Walidacja w czasie rzeczywistym:** System na bieżąco sprawdza, czy pytania i odpowiedzi nie są puste oraz czy wskazano poprawne warianty. Dopiero w pełni poprawny quiz może zostać zapisany.
* **Edycja istniejących plików:** Możliwość wczytania wcześniej zapisanego quizu z lokalnego repozytorium w celu wprowadzenia poprawek.


<img width="1920" height="1109" alt="MenuGeneratoraQuizów" src="https://github.com/user-attachments/assets/b9d017f4-43be-4fdb-9ff1-9774e1282aad" />
<img width="1920" height="1109" alt="EdycjaQuizu" src="https://github.com/user-attachments/assets/076d336c-e99d-4b89-8291-aeef09a3e656" />


### Moduł Rozwiązywania (Quiz Solver)

* **Rozwiązywanie testów:** Intuicyjny interfejs pozwalający na nawigację między pytaniami (możliwość cofania się do poprzednich pytań i zmiany odpowiedzi przed ostatecznym zakończeniem).
* **Presja czasu:** Zaimplementowany licznik czasu (10 minut na cały test). Na 10 sekund przed końcem odtwarzany jest dźwięk ostrzegawczy. Po upływie czasu test kończy się automatycznie.
* **Animowane podsumowanie:** Po zakończeniu testu, aplikacja generuje płynnie animowany raport. Pytania pojawiają się sekwencyjnie na liście wraz z informacją o zdobytych punktach, całkowitym czasie rozwiązywania oraz sygnałami dźwiękowymi informującymi o bezbłędnej (lub błędnej) odpowiedzi.
* **Szczegółowa analiza odpowiedzi:** Raport dokładnie wskazuje, które opcje zostały wybrane poprawnie, które pominięto, a gdzie popełniono błąd.


<img width="1920" height="1109" alt="MenuRozwiązywaniaQuizzów" src="https://github.com/user-attachments/assets/8f980335-3149-48e1-92e9-b5191e9ed3ea" />
<img width="1920" height="1109" alt="RozwiązywanieQuizu" src="https://github.com/user-attachments/assets/7a4ebad5-d876-421c-b192-c46a2ddcd0a3" />
<img width="1920" height="1109" alt="EkranPodsumowaniaQuizu" src="https://github.com/user-attachments/assets/06f9b318-54ab-4408-b81d-9826134dec76" />


## Aspekty techniczne

Projekt został napisany w języku C# z wykorzystaniem biblioteki Newtonsoft.Json. Architektura kodu opiera się na wzorcu Model-View-Presenter (MVP). Gwarantuje to całkowitą separację logiki biznesowej od warstwy wizualnej (UI), co znacząco ułatwia skalowanie aplikacji, wprowadzanie zmian w interfejsie oraz utrzymanie porządku w kodzie. UI zostało w pełni oparte o system TextMeshPro.
