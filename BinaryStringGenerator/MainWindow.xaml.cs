using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BinaryStringGenerator
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        /// <summary>
        /// Flaga wskazująca czy okno jest zmaksymalizowane.
        /// </summary>
        public bool IsMaximized { get; private set; }
        /// <summary>
        /// Instancja klasy mierząca upłynięcie czasu.
        /// </summary>
        private Stopwatch _watch;
        /// <summary>
        /// Instancja generatora liczb binarnych.
        /// </summary>
        private readonly Generator _generator;
        /// <summary>
        /// Czas w którym zostało zrealizowane wygnerowanie ciągów.
        /// </summary>
        private readonly double[] _timeInWhichTheTaskWasCompleted = new double[1];
        /// <summary>
        /// Cyfra podana przez użytkownika, wskazująca maksymalną ilość bajtów w ciągu.
        /// </summary>
        private int _valueProvidedByUser;
        #endregion

        #region Basic constructor
        public MainWindow()
        {
            InitializeComponent();

            this._generator = new Generator();
            this.DataContext = this._generator;
   
        }
        #endregion

        #region GenerateButton_Click
        /// <summary>
        /// Akcja po wciśnięciu przycisku generuj.
        /// Czyszczę inforamcję dla użytkownika, czyszczę listę z wcześniej wygenerowanymi liczbami.
        /// Weryfikuje podaną wartość przez użytkownika. Generuję cyfry i mierzę czas.
        /// </summary>
        /// <param name="sender">Przycisk generowania.</param>
        /// <param name="e">Zdarzenie - wciśnięcie przycisku.</param>
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            ResetValues();

            //Konwertujemy cyfrę podaną przez użytkownika i sprawdzamy czy spełnia wygmagania
            if (int.TryParse(this.UserNumber.Text, out _valueProvidedByUser) && _valueProvidedByUser > 0 && _valueProvidedByUser <= Generator.MAX_NUM_OF_BYTES)
            {
                //Pobieram cyfrę która może zostać maksymalnie wygenerowana w ciągu który che uzyskać użytkownik.
                if (this._generator.MaxValues.TryGetValue(_valueProvidedByUser, out ulong maxSizeOfGivenValue))
                {
                    StartGenerateBinaryStrings(maxSizeOfGivenValue);
                    GenResults();
                }
                else //Informacja dla użytkownika o tym że wystąpił błąd.
                    Information("Nie ma takiej cyfry w słowniku.", true);
                
            }
            else //Informacja dla użytkownika o tym że podał błędną wartość.
                 Information("Błędna liczba.", true);
         
        }
        #endregion
        #region Information
        /// <summary>
        /// Wypisuje informacje na dolnym pasku aplikacji.
        /// </summary>
        /// <param name="text">Wiadomość dla użytkownika</param>
        /// <param name="warrning">Jeżeli true to koloruje tło wiadomości na czerwono.</param>
        private void Information(string text, bool warrning = false)
        {
            this.InforamtionForUser.Text = text;
            if(warrning)
                this.InforamtionForUser.Background = Brushes.Red;
            else
                this.InforamtionForUser.Background = Brushes.Transparent;
        }
        #endregion
        #region StartGenerateBinaryStrings
        /// <summary>
        /// Zainicjowanie generowania ciągów binarnych.
        /// </summary>
        /// <param name="maxSizeOfGivenValue">Maksymalna ilość bajtów w ciągach.</param>
        private void StartGenerateBinaryStrings(ulong maxSizeOfGivenValue)
        {
            //Rozpoczynam genereowanie.
            this._watch = Stopwatch.StartNew();
            for (ulong i = 0; i <= maxSizeOfGivenValue; i++)
            {
                this._generator.GenBinary(i);
            }
            //Zapisuje czas wygnerowania
            this._timeInWhichTheTaskWasCompleted[0] = this._watch.ElapsedMilliseconds;
            this._watch.Stop();
        }
        #endregion
        #region GenResults
        /// <summary>
        /// Generowanie wyników.
        /// </summary>
        private void GenResults()
        {
            //Podawanie czasu
            if (this._timeInWhichTheTaskWasCompleted[0] > 1000)
                this.TotalTime.Text = $"{TimeSpan.FromMilliseconds(this._timeInWhichTheTaskWasCompleted[0]).TotalSeconds.ToString()}s";
            else
                this.TotalTime.Text = $"{this._timeInWhichTheTaskWasCompleted[0]}ms";

            //Wyświetlanie ilości elementów
            this.NumberOfElements.Text = "" + this._generator.GeneratedNumbers.Count;

            if (this.ShowResults.IsChecked == true)
            {
                GeneratedListViewModel.Instance.Add(this._generator.GeneratedNumbers);
                this.GeneratedNumbersList.ItemsSource = GeneratedListViewModel.Instance.GeneratedNumbers;
            }
            else SaveResultsToFile();
        }
        #endregion
        #region ResetValues
        /// <summary>
        /// Resetowanie wszystkich pól i właściwości potrzebnych do przeprowadzenia ponownego generowania ciągów.
        /// </summary>
        private void ResetValues()
        {
            this.TotalTime.Text = string.Empty;
            this.NumberOfElements.Text = string.Empty;
            this.GeneratedNumbersList.ItemsSource = null;
            this.InformationAboutResult.Visibility = Visibility.Collapsed;
            this._timeInWhichTheTaskWasCompleted[0] = 0;

            //Czyszcze pole z informacją dla użytkownika w przypadku jeżeli został wcześniej wywołany jakiś błąd.
            if (this.InforamtionForUser.Text != string.Empty)
            {
                this.InforamtionForUser.Text = string.Empty;
                this.InforamtionForUser.Background = Brushes.Transparent;
            }

            //Czyścimy listę z wygnerowanymi ciągmi jeżeli jest już jakaś w liście.
            if (this._generator.GeneratedNumbers.Count > 0)
                this._generator.GeneratedNumbers.Clear();
        }
        #endregion
        #region SaveResultsToFile
        /// <summary>
        /// Zapisywanie/wyświetlanie wyników w zależności od ustawień które wprowadzi użytkownik.
        /// </summary>
        private void SaveResultsToFile()
        {
            // Miejsce w którym będzie zapisany plik
            string docPath = string.Empty;
            //Pełna ścieżka dostępu z nazwą pliku
            string fullFilePath = string.Empty;

            try
            {
                docPath = Directory.GetCurrentDirectory();
                // Nazwa pliku
                string fileName = $"results {DateTime.Now.ToString("yyyy-dd-M HH-mm-ss")}.txt";
                fullFilePath = $"{docPath}{fileName}";

                // This text is added only once to the file.
                if (!File.Exists(fullFilePath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(fullFilePath))
                    {
                        if (this._timeInWhichTheTaskWasCompleted[0] > 1000)
                            sw.WriteLine($"Wygenerowanie ciągu zajęło łącznie {TimeSpan.FromMilliseconds(this._timeInWhichTheTaskWasCompleted[0]).TotalSeconds.ToString()}s.");
                        else
                            sw.WriteLine($"Wygenerowanie ciągu zajęło łącznie {this._timeInWhichTheTaskWasCompleted[0]}ms.");

                        sw.WriteLine($"Dla maksymalnie {this._valueProvidedByUser} bajtowej cyfry zostało wygnerowane {this._generator.GeneratedNumbers.Count} ciągów.");

                        foreach (string line in this._generator.GeneratedNumbers)
                            sw.WriteLine(line);

                        sw.Close();
                    }
                }

                this.InformationAboutResult.Visibility = Visibility.Visible;
                this.InformationAboutResult.Text = $"Wyniki zostały zapisane w pliku {docPath}{fileName}.";
            }
            catch (UnauthorizedAccessException) { Information($"Dostęp do ścieżki: {docPath} jest zablokowany przez system.", true); }
            catch (NotSupportedException) { Information("Wystąpił błąd podczas zapisu pliku lub wyszukania ścieżki dostępu.", true); }
            catch (ArgumentNullException) { Information("Ścieżka dostępu do pliku jest pusta.", true); }
            catch (EncoderFallbackException) { Information("Wystąpił błąd podczas zamykania strumenia zapisującego.", true); }
            catch (ArgumentException) { Information("Ścieżka dostępu do pliku jest błędna.", true); }
            catch (PathTooLongException) { Information("Ścieżka dostępu do pliku jest za długa.", true); }
            catch (DirectoryNotFoundException) { Information($"Nie znaleziono ścieżki dostępu: {fullFilePath}.", true); }
            catch (ObjectDisposedException) { Information($"Plik {fullFilePath}, został usunięty, nie można kontynuować.", true); }
            catch (IOException) { Information($"Wystąpił błąd z treścią pliku.", true); }
            catch (OverflowException) { Information($"Stos został przepełniony.", true); }
        }
        #endregion
        #region Window Functions Buttons
        #region Maximize_Click
        /// <summary>
        /// Maximizing size of the application when the application size is smaller then max size. 
        /// Setting standard size of the application when the application is maximized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (!IsMaximized)
            {
                //Property IsMaximized must by first to current count of list in AdministratorPage-UsersList!!!!
                IsMaximized = true;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                //Property IsMaximized must by first to current count of list in AdministratorPage-UsersList!!!!
                IsMaximized = false;
                this.WindowState = WindowState.Normal;
            }
        }

        #endregion
        #region Minimize_Click
        /// <summary>
        /// Minimalizing size of the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
        #region Exit_Click
        /// <summary>
        /// Action after click in top bar right corner button.
        /// Closing application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e) => this.Close();
        #endregion
        #endregion
    }

}
