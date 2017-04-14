/*
 * Copyright © 2017 mik14a <mik14a@gmail.com>
 * This work is free. You can redistribute it and/or modify it under the
 * terms of the Do What The Fuck You Want To Public License, Version 2,
 * as published by Sam Hocevar. See the COPYING file for more details.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Tokenization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axel.Text.Tokenization.TokenizerTest
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void RpnTokenizerTest() {
            Console.WriteLine($"Rpn Calculator");
            var calculator = new Rpn();
            foreach (var definition in calculator.Definitions) {
                Console.WriteLine($"Definition Type = {definition.Type}, Pattern = '{definition.Pattern}'");
            }
            Assert.IsNotNull(calculator);
            var expressions = new Dictionary<string, int>() {
                { string.Empty, 0 },
                { "1", 1 },
                { "1 + 2 + 3 + 4", 10 },
                { "1 * 2 + 3 * 4", 14 },
                { "1 + 2 * 3 + 4", 11 },
                { "1 - 2 + 3 - 4", -2 },
                { "1 / 2 * 3 / 4", 0 }
            };
            foreach (var expression in expressions) {
                Console.WriteLine($"Expression {expression.Key}, Expect {expression.Value}");
                Console.WriteLine($"Expression {expression.Key}, Rpn {string.Join(" ", calculator.Parse(expression.Key).Select(t => t.Value))}");
                Console.WriteLine($"Expression {expression.Key}, Expect {expression.Value}, Result {calculator.Calc(expression.Key)}");
                Assert.AreEqual(expression.Value, calculator.Calc(expression.Key));
            }
        }

        [TestMethod]
        public void TokenizerTest() {
            var message = "The quick brown fox jumps over the lazy dog";
            var separator = " ".ToCharArray();
            var word = TokenDefinition.Create("word", @"\w+");
            var space = TokenDefinition.Create("space", @"\s+");
            var definitions = new[] { word, space };
            var tokens = Tokenizer.Tokenize(definitions, message);
            var words = tokens.Where(t => t.Type == "word").ToList();
            CollectionAssert.AreEqual(words.Select(t => t.Value).ToArray(), message.Split(separator));
            words.ForEach(t => Console.WriteLine($"Index = {t.Index}, Type = {t.Type}, Value = {t.Value}"));
        }

        [TestMethod]
        public void TokenizerTestUseEnumerator() {
            var message = "The quick brown fox jumps over the lazy dog";
            var separator = " ".ToCharArray();
            var word = TokenDefinition.Create("word", @"\w+");
            var space = TokenDefinition.Create("space", @"\s+");
            var definitions = new[] { word, space };
            using (var tokens = Tokenizer.Tokenize(definitions, message).GetEnumerator()) {
                var words = message.Split(separator).GetEnumerator();
                while (tokens.MoveNext()) {
                    var token = tokens.Current;
                    if (token.Type == "word") {
                        Console.Write($"[{token.Value}]");
                        words.MoveNext();
                        Assert.AreEqual(token.Value, words.Current);
                    }
                }
                Console.WriteLine();
                tokens.MoveNext();
                tokens.Reset();
            }
        }

        [TestMethod]
        public void TokenizerTestWithFactory() {
            var message = "The quick brown fox jumps over the lazy dog";
            var separator = " ".ToCharArray();
            var factory = new TokenizerFactory<int>()
                .With(0, @"[a-z]+", RegexOptions.IgnoreCase)
                .With(1, @"\s+");
            var tokens = factory.Tokenize(message);
            var words = tokens.Where(t => t.Type == 0).ToList();
            CollectionAssert.AreEqual(words.Select(t => t.Value).ToArray(), message.Split(separator));
            words.ForEach(t => Console.WriteLine($"Index = {t.Index}, Type = {t.Type}, Value = {t.Value}"));
        }

        class Rpn
        {
            public enum TokenType
            {
                Num, Add, Sub, Mul, Div, Sp
            }

            public IReadOnlyList<TokenDefinition<TokenType>> Definitions {
                get { return Factory.TokenDefinitions; }
            }

            public int Calc(string expression) {
                return Calc(Parse(expression));
            }

            public int Calc(IEnumerable<Token<TokenType>> notation) {
                var stack = new Stack<int>();
                foreach (var token in notation) {
                    if (token.Type == TokenType.Num) {
                        stack.Push(Convert.ToInt32(token.Value));
                    } else {
                        stack.Push(Operator[token.Type](stack.Pop(), stack.Pop()));
                    }
                }
                Debug.Assert(stack.Count == 0 || stack.Count == 1);
                return 0 < stack.Count ? stack.Pop() : 0;
            }

            public IEnumerable<Token<TokenType>> Parse(string expression) {
                var stack = new Stack<Token<TokenType>>();
                var tokens = Factory.Tokenize(expression).Where(t => t.Type != TokenType.Sp);
                foreach (var token in tokens) {
                    if (token.Type == TokenType.Num) {
                        yield return token;
                    } else if (stack.Count == 0) {
                        stack.Push(token);
                    } else if (token.Type > stack.Peek().Type) {
                        stack.Push(token);
                    } else {
                        while (0 < stack.Count && token.Type < stack.Peek().Type) {
                            yield return stack.Pop();
                        }
                        stack.Push(token);
                    }
                }
                while (0 < stack.Count) {
                    yield return stack.Pop();
                }
            }

            static readonly TokenizerFactory<TokenType> Factory = new TokenizerFactory<TokenType>(RegexOptions.None)
                .With(TokenType.Num, @"[1-9][0-9]*")
                .With(TokenType.Add, @"[+]")
                .With(TokenType.Sub, @"[-]")
                .With(TokenType.Mul, @"[*]")
                .With(TokenType.Div, @"[/]")
                .With(TokenType.Sp, @"\s+");

            static readonly Dictionary<TokenType, Func<int, int, int>> Operator =
                new Dictionary<TokenType, Func<int, int, int>>() {
                    { TokenType.Add, (m, n) => n + m },
                    { TokenType.Sub, (m, n) => n - m },
                    { TokenType.Mul, (m, n) => n * m },
                    { TokenType.Div, (m, n) => n / m },
                };

        }

    }
}
