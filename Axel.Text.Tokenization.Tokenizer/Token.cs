/*
 * Copyright © 2017 mik14a <mik14a@gmail.com>
 * This work is free. You can redistribute it and/or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar. See the COPYING file for more details.
 */

using System.Diagnostics;

namespace System.Text.Tokenization
{
    /// <summary>
    /// Token.
    /// </summary>
    /// <typeparam name="TokenType"></typeparam>
    [DebuggerDisplay("Index = {Index}, Type = {Type}, Value = {Value}")]
    public class Token<TokenType>
    {
        /// <summary>Index of token.</summary>
        public int Index { get; }
        /// <summary>Type of token.</summary>
        public TokenType Type { get; }
        /// <summary>Value of token.</summary>
        public string Value { get; }

        /// <summary>
        /// Construct token.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="type">Type</param>
        /// <param name="value">Value.</param>
        public Token(int index, TokenType type, string value) {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{nameof(value)} is null or empty.", nameof(value));

            Index = index;
            Type = type;
            Value = value;
        }
    }
}
