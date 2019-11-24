using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace BinaryStringGenerator
{
    public class GeneratedListViewModel : UserControl
    {
        private readonly ObservableCollection<string> _generatedNumbers = new ObservableCollection<string>();

        public ObservableCollection<string> GeneratedNumbers => this._generatedNumbers;

        #region SingletonePattern
        /// <summary>
        /// Constructor.
        /// </summary>
        static GeneratedListViewModel() { }

        private static readonly GeneratedListViewModel _instance = new GeneratedListViewModel();
        public static GeneratedListViewModel Instance => _instance;
        public GeneratedListViewModel() { }
        #endregion

        public void Add(IEnumerable<string> values)
        {
            if (Instance.GeneratedNumbers.Count > 0)
                Instance.GeneratedNumbers.Clear();

            foreach (var item in values)
            {
                this._generatedNumbers.Add(item);
            }  
        }
    }
}
