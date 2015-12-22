using System;
using System.Collections;

namespace GoComics.Shared.Controls
{
    public interface ISuggestionProvider
    {
        IEnumerable GetSuggestions(string filter);
    }

    public class SuggestionProvider : ISuggestionProvider
    {
        public SuggestionProvider(Func<string, IEnumerable> method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            SuggestAction = method;
        }

        public Func<string, IEnumerable> SuggestAction { get; private set; }

        public IEnumerable GetSuggestions(string filter)
        {
            return SuggestAction(filter);
        }
    }
}