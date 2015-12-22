using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoComics.Shared.Controls
{
    internal class SuggestionsAdapter
    {

        #region "Fields"

        private AutoCompleteTextBox _actb;

        private string _filter;
        #endregion

        #region "Constructors"

        public SuggestionsAdapter(AutoCompleteTextBox actb)
        {
            _actb = actb;
        }

        #endregion

        #region "Methods"

        public void GetSuggestions(string searchText)
        {
            _filter = searchText;
            _actb.IsLoading = true;
            ParameterizedThreadStart thInfo = new ParameterizedThreadStart(GetSuggestionsAsync);
            Thread th = new Thread(thInfo);
            th.Start(new object[] {
                searchText,
                _actb.Provider
            });
        }

        private void DisplaySuggestions(IEnumerable suggestions, string filter)
        {
            if (_filter != filter)
            {
                return;
            }
            if (_actb.IsDropDownOpen)
            {
                _actb.IsLoading = false;
                _actb.ItemsSelector.ItemsSource = suggestions;
                _actb.IsDropDownOpen = _actb.ItemsSelector.HasItems;
            }

        }

        private void GetSuggestionsAsync(object param)
        {
            object[] args = param as object[];
            string searchText = Convert.ToString(args[0]);
            ISuggestionProvider provider = args[1] as ISuggestionProvider;
            IEnumerable list = provider.GetSuggestions(searchText);
            _actb.Dispatcher.BeginInvoke(new Action<IEnumerable, string>(DisplaySuggestions), DispatcherPriority.Background, new object[] {
                list,
                searchText
            });
        }

        #endregion

    }
}
