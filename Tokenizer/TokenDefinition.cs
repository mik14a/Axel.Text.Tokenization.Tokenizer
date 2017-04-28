/*
 * Copyright © 2017 mik14a <mik14a@gmail.com>
 * This work is free. You can redistribute it and/or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar. See the COPYING file for more details.
 */

using System.Text.RegularExpressions;

namespace System.Text.Tokenization
{
    /// <summary>
    /// TokenDefinition factories.
    /// </summary>
    public static class TokenDefinition
    {
        /// <summary>
        /// Create TokenDefinition.
        /// </summary>
        /// <typeparam name="TokenType">Type of token.</typeparam>
        /// <param name="type">Token type of definition.</param>
        /// <param name="pattern">Pattern to match.</param>
        /// <returns>New instance of TokenDefinition.</returns>
        public static TokenDefinition<TokenType> Create<TokenType>(TokenType type, string pattern) {
            return Create(type, pattern, RegexOptions.None);
        }

        /// <summary>
        /// Create TokenDefinition.
        /// </summary>
        /// <typeparam name="TokenType">Type of token.</typeparam>
        /// <param name="type">Token type of definition.</param>
        /// <param name="pattern">Pattern to match.</param>
        /// <param name="regexOptions">A bitwise options the regular expression.</param>
        /// <returns>New instance of TokenDefinition.</returns>
        public static TokenDefinition<TokenType> Create<TokenType>(TokenType type, string pattern, RegexOptions regexOptions) {
            return new TokenDefinition<TokenType>(type, pattern, regexOptions);
        }
    }

    /// <summary>
    /// Definition of token.
    /// </summary>
    /// <typeparam name="TokenType">Type of token.</typeparam>
    public class TokenDefinition<TokenType>
    {
        /// <summary>Definition type.</summary>
        public TokenType Type { get; }
        /// <summary>Definition pattern.</summary>
        public string Pattern { get; }

        /// <summary>
        /// Construct token definition with token type and regular expression pattern.
        /// </summary>
        /// <param name="type">Token type of definition.</param>
        /// <param name="pattern">Pattern to match.</param>
        /// <param name="regexOptions">A bitwise options the regular expression.</param>
        public TokenDefinition(TokenType type, string pattern, RegexOptions regexOptions) {
            if (type == null) throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null.");
            if (pattern == null) throw new ArgumentNullException(nameof(pattern), $"{nameof(pattern)} is null.");

            Type = type;
            Pattern = pattern;
            regex = new Regex(pattern, RegexOptions.Compiled | regexOptions);
        }

        /// <summary>
        /// Get matched value at the specified starting position in the string.
        /// </summary>
        /// <param name="input">The string to match.</param>
        /// <param name="startat">The zero-based character position at which to start the search.</param>
        /// <returns>Matched string at a start position.</returns>
        public string Match(string input, int startat) {
            if (input == null)
                throw new ArgumentNullException(nameof(input), $"{nameof(input)} is null.");

            var match = regex.Match(input, startat);
            var success = match.Success && match.Index == startat;
            return success ? match.Value : null;
        }

        readonly Regex regex;
    }
}
