/*
 * Copyright © 2017 mik14a <mik14a@gmail.com>
 * This work is free. You can redistribute it and/or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar. See the COPYING file for more details.
 */

using System.Collections;
using System.Collections.Generic;

namespace System.Text.Tokenization
{
    /// <summary>
    /// Tokenizer factory.
    /// </summary>
    /// <typeparam name="TokenType">Type of token.</typeparam>
    public static class Tokenizer
    {
        /// <summary>
        /// Create tokenizer.
        /// </summary>
        /// <typeparam name="TokenType">Type of token.</typeparam>
        /// <param name="definitions">Token definition for tokenize.</param>
        /// <param name="text">Tokenize source input.</param>
        /// <returns>New instance of tokenizer.</returns>
        public static IEnumerable<Token<TokenType>> Tokenize<TokenType>(TokenDefinition<TokenType>[] definitions, string text) {
            return new Tokenizer<TokenType>(definitions, text);
        }
    }

    /// <summary>
    /// Tokenizer.
    /// </summary>
    /// <typeparam name="TokenType">Type of token.</typeparam>
    public class Tokenizer<TokenType> : IEnumerable<Token<TokenType>>, IEnumerator<Token<TokenType>>
    {
        /// <summary>Current token.</summary>
        public Token<TokenType> Current { get; private set; }

        /// <summary>
        /// Construct tokenizer with token definition and target string.
        /// </summary>
        /// <param name="definitions">Token definition for tokenize.</param>
        /// <param name="text">Tokenize source input.</param>
        public Tokenizer(TokenDefinition<TokenType>[] definitions, string text) {
            if (definitions == null || definitions.Length == 0)
                throw new ArgumentException($"{nameof(definitions)} is null or empty.", nameof(definitions));
            if (text == null)
                throw new ArgumentNullException(nameof(text), $"{nameof(text)} is null.");

            this.definitions = definitions;
            this.text = text;
            index = 0;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Token<TokenType>> GetEnumerator() {
            return this;
        }

        /// <summary>
        /// Performs associated with freeing resources.
        /// </summary>
        public void Dispose() {
            definitions = null;
            text = null;
            index = -1;
            Current = null;
        }

        /// <summary>
        /// Advances the enumerator to the next token of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next token;
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext() {
            if (index == -1 || definitions == null || text == null)
                return false;

            foreach (var definition in definitions) {
                var value = definition.Match(text, index);
                if (value != null) {
                    Current = new Token<TokenType>(index, definition.Type, value);
                    index += value.Length;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position,
        /// which is before the first token in the collection.
        /// </summary>
        public void Reset() {
            index = 0;
            Current = null;
        }

        object IEnumerator.Current {
            get { return Current; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        TokenDefinition<TokenType>[] definitions;
        string text;
        int index;
    }
}
