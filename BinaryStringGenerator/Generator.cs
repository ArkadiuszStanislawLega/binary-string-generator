using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BinaryStringGenerator
{
    /// <summary>
    /// Generator ciągów binarnych.
    /// </summary>
    public class Generator
    {
        #region Properties
        /// <summary>
        /// Maksymalna ilośc bitów to wygenerowania.
        /// </summary>
        public const int MAX_NUM_OF_BYTES = 32;
        /// <summary>
        /// Przechowuje tymczasowo każdą cyfre binarną.
        /// </summary>
        private string _binaryNumber = string.Empty;
        /// <summary>
        /// Słownik z wszystkimi maksymalnymi wynikami jakie mogą utworzyć ciągi do 32 bitów.
        /// </summary>
        private readonly Dictionary<int, ulong> _maxValues = new Dictionary<int, ulong>();
        /// <summary>
        /// Wygenerowane ciągi bitowy.
        /// </summary>
        private readonly List<string> _generatedNumbers = new List<string>();
        /// <summary>
        /// Konwertuje ciąg bitów w cyfrę dziesiętną.
        /// </summary>
        private readonly Regex _binary = new Regex("^[01]{1,32}$", RegexOptions.Compiled);
        /// <summary>
        /// Wygenerowane ciągi bitowy.
        /// </summary>
        public List<string> GeneratedNumbers => this._generatedNumbers;
        /// <summary>
        /// Słownik z wszystkimi maksymalnymi wynikami jakie mogą utworzyć ciągi do 32 bitów.
        /// </summary>
        public Dictionary<int, ulong> MaxValues => this._maxValues;
        #endregion
        #region Basci Constructor
        public Generator()
        {
            GenMaxValues();
        }
        #endregion
        #region GenMaxValues
        /// <summary>
        /// Przygotowuje liczby do porówniania.
        /// </summary>
        private void GenMaxValues()
        {
            for (int i = 0; i < MAX_NUM_OF_BYTES; i++)
            {
                this._binaryNumber += 1;

                //Przerabiam string z liczbą binarną na liczbę dziesiętną i dodaję ją do słownika
                if (this._binary.IsMatch(this._binaryNumber))
                {
                    this._maxValues.Add(i + 1, Convert.ToUInt64(this._binaryNumber, 2));
                }
            }
            this._binaryNumber = string.Empty;
        }
        #endregion
        #region GenBinary
        /// <summary>
        /// Klasyczny sposób przekształcania liczby dziesiętnej na binarną.
        /// </summary>
        /// <param name="number">Liczba do przekształcenia</param>
        public void GenBinary(ulong number)
        {
            while (number != 0) //dopóki liczba będzie różna od zera
            {
                //bierzemy resztę z dzielenia przez 2 podanej cyfry i dodajemy do reszty bitów
                this._binaryNumber += number % 2;
                number /= 2;
            }

            if(this._binaryNumber != string.Empty)  
                this._generatedNumbers.Add(this._binaryNumber);

            this._binaryNumber = string.Empty;
        }
        #endregion
    }
}
