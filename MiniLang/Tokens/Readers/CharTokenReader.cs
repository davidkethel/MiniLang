namespace MiniLang.Tokens.Readers
{
    public class CharTokenReader : ITokenReader
    {
        public Token Read(char start, TextReader reader)
        {
            if (start != '\'') return null;

            var character = (char)reader.Read();

            // EOF reached
            if (character < 0) return new Token(TokenType.Invalid, "Unexpected end of file while reading string value");

            // Check closing single quote is supplied
            var finalSingleQuote = reader.Read();
            if (finalSingleQuote != '\'') throw new InvalidOperationException($"Expected ', got {finalSingleQuote} instead.");

            return new Token(TokenType.Char, character.ToString());
        }
    }
}
