
# Dokumentacja Techniczna Projektu "Quiz"
## 1. Architektura Systemu: Wzorzec MVP (Model-View-Presenter)

  

Projekt opiera się na wzorcu MVP. Dzieli to aplikację w Unity na trzy główne warstwy o jasno określonych zadaniach:

  

### Model (Logika Domenowa i Dane)

  

Przechowuje dane i zasady działania aplikacji. Ta warstwa nie ma pojęcia o wyglądzie interfejsu ani o technologii, w jakiej jest wyświetlana.

  

### View (Widok / Interfejs Użytkownika)

  

Widoki to implementacje interfejsów takie jak `IQuizCreatorMainView`. Ich rola ogranicza się do dwóch zadań:

  

* Przekazywanie akcji użytkownika wyżej do Prezentera.

* Aktualizowanie elementów na ekranie na wyraźne polecenie Prezentera.

Widoki to skrypty podpięte pod obiekty w Unity. Same nie podejmują żadnych decyzji biznesowych.

  

### Presenter (Prezenter)

  

To mózg operacji i główny punkt sterowania aplikacją. Skrypty takie jak `QuizCreatorPresenter` to czyste klasy w języku C#.

Prezenter łączy Widok z Modelem. Nasłuchuje akcji użytkownika z poziomu interfejsu, aktualizuje odpowiednio dane w Modelu, a następnie wysyła polecenia do Widoku, aby zaktualizować to, co widać na ekranie.
  

---

  

## 2. Realizacja Wymagań Projektowych

  

Projekt dzieli się na dwa niezależne moduły. Każdy z nich ma własnego Prezentera oraz dedykowane Widoki.

  

### 2.1. Model Obiektowy i Logika Domenowa

  

Warstwa modelu to fundament danych i zasad punktacji. Zbudowaliśmy go z wykorzystaniem podstawowych zasad programowania obiektowego. Klasy z tej warstwy to czysty kod C#, bez żadnych powiązań z silnikiem Unity.

  

*  **Podstawa pytań:** Stworzyliśmy bazową klasę `Question`. Definiuje ona treść pytania, modyfikator punktowy oraz listę odpowiedzi. Narzuca też każdej dziedziczącej po niej klasie konieczność posiadania własnego mechanizmu liczenia punktów.

*  **Pytania wielokrotnego wyboru:** Klasa `MultipleChoiceQuestion` sprawdza, czy zaznaczone przez użytkownika odpowiedzi zgadzają się z kluczem. Za bezbłędne rozwiązanie przyznaje 100% punktów. Jeśli użytkownik pomyli się dokładnie raz, otrzymuje połowę punktów. Każdy kolejny błąd sprawia, że wynik za dane pytanie wynosi zero.

*  **Pytania Prawda/Fałsz:** Klasa `TrueFalseQuestion` zawsze ma tylko dwie opcje odpowiedzi. Wymaga dokładnego zaznaczenia prawidłowej opcji, aby przyznać punkty.

*  **Odpowiedzi:** Klasa `Answer` to bardzo prosta struktura. Przechowuje jedynie tekst odpowiedzi i informację, czy jest ona poprawna.

*  **Struktura całego testu:** Klasa `Quiz` łączy nazwę testu z całą listą utworzonych pytań.

``` c#
using  System;
using  System.Collections.Generic;

[Serializable]
public  class  MultipleChoiceQuestion : Question
{
	public  override  float  CalculateScore(List<bool> userSelections)
	{
		if (userSelections == null || userSelections.Count != Answers.Count)
			return  0f;
			
		int  errors = 0;
		
		for (int  i = 0; i  <  Answers.Count; i++)
		{
			if (userSelections[i] != Answers[i].IsCorrect)
			{
				errors++;
			}
		}
		
		if (errors == 0) return  1.0f * Multiplier;
		if (errors == 1) return  0.5f * Multiplier;
		
		return  0.0f;
	}
} 
```
*[metoda licząca punkty z klasy MultipleChoiceQuestion, pokazująca logikę weryfikacji błędów]*

  

### 2.2. Zarządzanie Danymi: Zapis i Odczyt

  

Zapisem oraz wczytywaniem plików zajmuje się specjalna klasa `QuizRepository`.

  

* Użyliśmy zewnętrznej biblioteki Newtonsoft.Json, ponieważ jest bardziej zaawansowana niż standardowe narzędzia wbudowane w Unity.

* Dzięki odpowiednim ustawieniom tej biblioteki, program zapisuje w pliku informację o tym, jakiego konkretnie typu jest dane pytanie. Kiedy wczytujemy plik z powrotem, aplikacja od razu wie, czy ma odtworzyć pytanie wielokrotnego wyboru, czy pytanie prawda/fałsz.

* Quizy zapisują się w bezpiecznym miejscu na dysku użytkownika, do którego aplikacja ma zawsze pełny dostęp. Program automatycznie usuwa z nazwy pliku wszelkie niedozwolone znaki. Zapobiega to błędom podczas zapisywania danych.

``` c#
{
	"QuizName": "Test Wiedzy IT",
	"Questions": [
		{
		"$type": "MultipleChoiceQuestion, Assembly-CSharp",
		"QuestionText": "Które z wymienionych języków programowania wspierają paradygmat zorientowany obiektowo? (Zaznacz wszystkie poprawne)",
		"Multiplier": 2.5,
		"Answers": [
		{
			"Text": "C#",
			"IsCorrect": true
		},
		{
			"Text": "C",
			"IsCorrect": false
		},
		{
			"Text": "Java",
			"IsCorrect": true
		},
		{
			"Text": "HTML",
			"IsCorrect": false
		}
	]
}, [...]
```
*[Fragment pliku JSON wygenerowany przez aplikację]*

  

### 2.3. Moduł Kreatora Quizów

  

#### Dynamiczny interfejs

  

Prezenter nie pracuje na elementach ułożonych na sztywno na scenie. Zamiast tego każe Widokowi tworzyć nowe elementy z szablonów w trakcie działania programu.

Każde nowe pytanie i każda nowa odpowiedź to niezależny element na liście. Użytkownik może dodawać do dziesięciu odpowiedzi w jednym pytaniu. Usunięcie elementu z interfejsu powoduje natychmiastowe usunięcie go z danych i odwrotnie.

  

#### Ochrona przed błędami

  

Aplikacja na bieżąco sprawdza, co wpisuje użytkownik. Przycisk zapisu staje się aktywny dopiero wtedy, gdy quiz ma nazwę, a w każdym pytaniu zaznaczono co najmniej jedną poprawną odpowiedź. Dodatkowo, przed samym zapisem, program upewnia się, że nie ma nigdzie pustych pól tekstowych. W razie wykrycia problemu proces jest przerywany, a na ekranie pojawia się odpowiedni komunikat.


<img width="2597" height="1440" alt="image" src="https://github.com/user-attachments/assets/420ae87a-eefa-4ac9-8133-afa94ac271cc" />

<img width="2589" height="1440" alt="image" src="https://github.com/user-attachments/assets/cd469b0f-afd7-4340-8c07-037a27f4ac0f" />

*[Interfejs Kreatora Quizów z pokazanym komunikatem błędu i celowo zablokowanym przyciskiem zapisu]*

  

#### Automatyczne odświeżanie

  

Po udanym zapisaniu pliku program sam wraca do ekranu startowego. Oczyszcza tam poprzednią listę quizów i ładuje ją na nowo z dysku, dzięki czemu nowo stworzony quiz jest od razu widoczny i gotowy do edycji lub rozwiązania.

  

### 2.4. Wymóg 4 odpowiedzi na pytanie

  

Interfejs tworzenia pytań wielokrotnego wyboru startuje z minimalną liczbą opcji, ale pozwala w prosty sposób dodawać kolejne. Rozwiązanie to swobodnie pozwala na spełnienie wymogu budowy pytań z dokładnie czterema wariantami odpowiedzi.

  

### 2.5. Moduł Rozwiązywania Quizu

  

*  **Wczytywanie z pliku:** Po uruchomieniu tego modułu program skanuje folder zapisu i tworzy listę dostępnych testów na ekranie głównym.

*  **Nawigacja:** Po wybraniu testu i jego rozpoczęciu, użytkownik może swobodnie przechodzić między pytaniami za pomocą przycisków w przód i w tył. Prezenter dba o to, aby przyciski blokowały się w odpowiednich momentach.

*  **Zegar:** Zaimplementowaliśmy odliczanie czasu ze sztywnym limitem 10 minut. Aktualny czas wyświetla się na ekranie. Na 10 sekund przed końcem odtwarza się dźwięk ostrzegawczy. Gdy czas upłynie, test kończy się automatycznie, niezależnie od postępów.

*  **Liczenie punktów:** Po kliknięciu przycisku zakończenia, Prezenter porównuje zaznaczone przez gracza odpowiedzi z kluczem w Modelu, a następnie sumuje ostateczny wynik.

  
<img width="2593" height="1440" alt="image" src="https://github.com/user-attachments/assets/67290456-74a1-4a0e-ac79-b522eb5b1257" />

*[Ekran rozwiązywania quizu z licznikiem czasu]*

  

*  **Podsumowanie:** Ekran wyników to nie jest zwykła ściana tekstu. Wyniki pojawiają się w formie animowanej listy. Kolejne pytania wjeżdżają na ekran jedno po drugim. Raport dokładnie pokazuje, które opcje zaznaczono poprawnie, czego zabrakło, a gdzie popełniono błąd. Prezentacji towarzyszą dźwięki sukcesu lub porażki, a na górze ekranu wyświetla się całkowity czas rozwiązania testu.

  
<img width="2600" height="1440" alt="image" src="https://github.com/user-attachments/assets/ddedf61b-0b4b-4a23-a073-261e0b0f4e09" />

*[Animowany ekran podsumowania po zakończonym quizie]*

  

---

  

## 3. Podsumowanie Techniczne

  

Wykorzystanie wzorca MVP pozwoliło nam uniknąć bałaganu w kodzie, co jest powszechnym problemem w projektach tworzonych w środowiskach graficznych, a oddzielenie logiki od interfejsu ułatwia wprowadzanie zmian.
