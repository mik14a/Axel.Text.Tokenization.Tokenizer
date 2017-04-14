Axel.Text.Tokenization.Tokenizer
================================

Simple tokenizer.

## Usage

Use factory method.

```cs
var message = "The quick brown fox jumps over the lazy dog";
var word = TokenDefinition.Create("word", @"\w+");
var space = TokenDefinition.Create("space", @"\s+");
var definitions = new[] { word, space };
var tokens = Tokenizer.Tokenize(definitions, message);
var words = tokens.Where(t => t.Type == "word");
```

Or instance of `TokenizerFactory`.

```cs
var message = "The quick brown fox jumps over the lazy dog";
var factory = new TokenizerFactory<int>()
    .With(0, @"[a-z]+", RegexOptions.IgnoreCase)
    .With(1, @"\s+");
var tokens = factory.Tokenize(message);
var words = tokens.Where(t => t.Type == 0);
```

## Licence

[WTFPL](http://www.wtfpl.net/)

## Author

[mik14a](https://github.com/mik14a)
