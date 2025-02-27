﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.PowerFx.Core.Lexer;
using Microsoft.PowerFx.Core.Types;
using Microsoft.PowerFx.Core.Utils;

namespace Microsoft.PowerFx.Core.Texl.Intellisense
{
    internal class AddSuggestionHelper
    {
        public bool AddSuggestion(IntellisenseData.IntellisenseData intellisenseData, string suggestion, SuggestionKind suggestionKind, SuggestionIconKind iconKind, DType type, bool requiresSuggestionEscaping, uint sortPriority = 0)
        {
            Contracts.AssertValue(intellisenseData);
            Contracts.AssertValue(suggestion);

            if (!intellisenseData.DetermineSuggestibility(suggestion, type))
                return false;

            IntellisenseSuggestionList suggestions = intellisenseData.Suggestions;
            IntellisenseSuggestionList substringSuggestions = intellisenseData.SubstringSuggestions;
            int matchingLength = intellisenseData.MatchingLength;
            string matchingStr = intellisenseData.MatchingStr;
            string boundTo = intellisenseData.BoundTo;

            var valueToSuggest = requiresSuggestionEscaping ? TexlLexer.EscapeName(suggestion) : suggestion;
            int highlightStart = suggestion.IndexOf(matchingStr, StringComparison.OrdinalIgnoreCase);
            // If the suggestion has special characters we need to find the highlightStart index by escaping the matching string as well.
            // Because, the suggestion could be something like 'Ident with Space' and the user might have typed Ident. In this case,
            // we want to highlight only Ident while displaying 'Ident with Space'.
            if (requiresSuggestionEscaping && !string.IsNullOrEmpty(matchingStr) && valueToSuggest != suggestion && highlightStart == 0)
                highlightStart++;
            else
                matchingLength--;

            int highlightEnd = highlightStart + matchingStr.Length;
            if (IntellisenseHelper.IsMatch(suggestion, matchingStr))
            {
                // In special circumstance where the user escapes an identifier where they don't have to, the matching length will
                // include the starting delimiter that user provided, where as the suggestion would not include any delimiters.
                // Hence we have to count for that fact.
                if (matchingLength > 0 & matchingLength > matchingStr.Length)
                    highlightEnd = matchingLength > valueToSuggest.Length ? valueToSuggest.Length : matchingLength;
                UIString UIsuggestion = ConstructUIString(suggestionKind, type, suggestions, valueToSuggest, highlightStart, highlightEnd);
                IntellisenseSuggestion candidate = new IntellisenseSuggestion(UIsuggestion,
                    suggestionKind,
                    iconKind,
                    type,
                    boundTo,
                    -1,
                    string.Empty,
                    null,
                    sortPriority);
                return CheckAndAddSuggestion(suggestions, candidate);
            }
            if (highlightStart > -1)
            {
                UIString UIsuggestion = ConstructUIString(suggestionKind, type, substringSuggestions, valueToSuggest, highlightStart, highlightEnd);
                IntellisenseSuggestion candidate = new IntellisenseSuggestion(UIsuggestion,
                    suggestionKind,
                    iconKind,
                    type,
                    boundTo,
                    -1,
                    string.Empty,
                    null,
                    sortPriority);

                return CheckAndAddSuggestion(substringSuggestions, candidate);
            }

            return false;
        }

        protected virtual bool CheckAndAddSuggestion(IntellisenseSuggestionList suggestions, IntellisenseSuggestion candidate)
        {
            return IntellisenseHelper.CheckAndAddSuggestion(candidate, suggestions);
        }

        protected virtual UIString ConstructUIString(SuggestionKind suggestionKind, DType type, IntellisenseSuggestionList suggestions, string valueToSuggest, int highlightStart, int highlightEnd)
        {
            return IntellisenseHelper.DisambiguateGlobals(suggestions,
                new UIString(valueToSuggest, highlightStart, highlightEnd),
                suggestionKind,
                type);
        }
    }
}
