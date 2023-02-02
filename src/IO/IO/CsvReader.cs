using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.IO
{
    public class CsvReader
    {
        public string Delimiter { get; private set; }
        private readonly int _delimLength;
        public int BufferSize { get; set; } = 32768;
        public bool TrimFields { get; set; } = true;
        private readonly TextReader _textReader;
        private readonly List<string?> _headers = new();

        public CsvReader(TextReader textReader) 
            : this(textReader, ",")
        { }

        public CsvReader(TextReader rdr, string delimiter)
        {
            _textReader = rdr;
            Delimiter = delimiter;
            _delimLength = delimiter.Length;

            if (_delimLength == 0)
                throw new ArgumentException("Delimiter cannot be empty.");

            LoadHeaders();
        }

        char[] _buffer = null;
        int _bufferLength;
        int _bufferLoadThreshold;
        int _lineStartPos = 0;
        private int _actualBufferLen = 0;
        private List<Field> _fields = null;
        private int _fieldsCount = 0;
        private int _linesRead = 0;

        private int ReadBlockAndCheckEof(char[] buffer, int start, int len, ref bool eof)
        {
            if (len == 0)
                return 0;
            var read = _textReader.ReadBlock(buffer, start, len);
            if (read < len)
                eof = true;
            return read;
        }

        private bool FillBuffer()
        {
            var eof = false;
            var toRead = _bufferLength - _actualBufferLen;
            if (toRead >= _bufferLoadThreshold)
            {
                int freeStart = (_lineStartPos + _actualBufferLen) % _buffer.Length;
                if (freeStart >= _lineStartPos)
                {
                    _actualBufferLen += ReadBlockAndCheckEof(_buffer, freeStart, _buffer.Length - freeStart, ref eof);
                    if (_lineStartPos > 0)
                        _actualBufferLen += ReadBlockAndCheckEof(_buffer, 0, _lineStartPos, ref eof);
                }
                else
                {
                    _actualBufferLen += ReadBlockAndCheckEof(_buffer, freeStart, toRead, ref eof);
                }
            }

            return eof;
        }

        private string GetLineTooLongMsg()
        {
            return String.Format("CSV line #{1} length exceedes buffer size ({0})", BufferSize, _linesRead);
        }

        private int ReadQuotedFieldToEnd(int start, int maxPos, bool eof, ref int escapedQuotesCount)
        {
            int pos = start;
            int chIdx;
            char ch;
            for (; pos < maxPos; pos++)
            {
                chIdx = pos < _bufferLength ? pos : pos % _bufferLength;
                ch = _buffer[chIdx];
                if (ch == '\"')
                {
                    bool hasNextCh = (pos + 1) < maxPos;
                    if (hasNextCh && _buffer[(pos + 1) % _bufferLength] == '\"')
                    {
                        // double quote inside quote = just a content
                        pos++;
                        escapedQuotesCount++;
                    }
                    else
                    {
                        return pos;
                    }
                }
            }

            if (eof)
            {
                // this is incorrect CSV as quote is not closed
                // but in case of EOF lets ignore that
                return pos - 1;
            }

            throw new InvalidDataException(GetLineTooLongMsg());
        }

        private bool ReadDelimTail(int start, int maxPos, ref int end)
        {
            int pos;
            int idx;
            int offset = 1;
            for (; offset < _delimLength; offset++)
            {
                pos = start + offset;
                idx = pos < _bufferLength ? pos : pos % _bufferLength;
                if (pos >= maxPos || _buffer[idx] != Delimiter[offset])
                    return false;
            }

            end = start + offset - 1;
            return true;
        }

        private Field GetOrAddField(int startIdx)
        {
            _fieldsCount++;
            while (_fieldsCount > _fields.Count)
                _fields.Add(new Field());
            var f = _fields[_fieldsCount - 1];
            f.Reset(startIdx);
            return f;
        }

        public int FieldsCount => _fieldsCount;

        public string? this[int idx] => idx < _fieldsCount ? _fields[idx].GetValue(_buffer) : null;

        public int GetValueLength(int idx)
        {
            if (idx < _fieldsCount)
            {
                var f = _fields[idx];
                return f.Quoted ? f.Length - f.EscapedQuotesCount : f.Length;
            }

            return -1;
        }

        public void ProcessValueInBuffer(int idx, Action<char[], int, int> handler)
        {
            if (idx < _fieldsCount)
            {
                var f = _fields[idx];
                if ((f.Quoted && f.EscapedQuotesCount > 0) || f.End >= _bufferLength)
                {
                    var chArr = f.GetValue(_buffer).ToCharArray();
                    handler(chArr, 0, chArr.Length);
                }
                else if (f.Quoted)
                {
                    handler(_buffer, f.Start + 1, f.Length - 2);
                }
                else
                {
                    handler(_buffer, f.Start, f.Length);
                }
            }
        }


        private void LoadHeaders()
        {
            Read();
            for (var i = 0; i < FieldsCount; i++)
            {
                _headers.Add(this[i]);
            }
        }


        public JObject RowAsJObject()
        {
            var jObject = new JObject();
            for (var i = 0; i < FieldsCount; i++)
            {
                jObject.Add(_headers[i] ?? throw new InvalidOperationException(), this[i]);
            }
            return jObject;
        }

        public bool Read()
        {
            Start:
            if (_fields == null)
            {
                _fields = new List<Field>();
                _fieldsCount = 0;
            }

            if (_buffer == null)
            {
                _bufferLoadThreshold = Math.Min(BufferSize, 8192);
                _bufferLength = BufferSize + _bufferLoadThreshold;
                _buffer = new char[_bufferLength];
                _lineStartPos = 0;
                _actualBufferLen = 0;
            }

            var eof = FillBuffer();

            _fieldsCount = 0;
            if (_actualBufferLen <= 0)
            {
                return false; // no more data
            }

            _linesRead++;

            int maxPos = _lineStartPos + _actualBufferLen;
            int charPos = _lineStartPos;

            var currentField = GetOrAddField(charPos);
            bool ignoreQuote = false;
            char delimFirstChar = Delimiter[0];
            bool trimFields = TrimFields;

            int charBufIdx;
            char ch;
            for (; charPos < maxPos; charPos++)
            {
                charBufIdx = charPos < _bufferLength ? charPos : charPos % _bufferLength;
                ch = _buffer[charBufIdx];
                switch (ch)
                {
                    case '\"':
                        if (ignoreQuote)
                        {
                            currentField.End = charPos;
                        }
                        else if (currentField.Quoted || currentField.Length > 0)
                        {
                            // current field already is quoted = lets treat quotes as usual chars
                            currentField.End = charPos;
                            currentField.Quoted = false;
                            ignoreQuote = true;
                        }
                        else
                        {
                            var endQuotePos = ReadQuotedFieldToEnd(charPos + 1, maxPos, eof,
                                ref currentField.EscapedQuotesCount);
                            currentField.Start = charPos;
                            currentField.End = endQuotePos;
                            currentField.Quoted = true;
                            charPos = endQuotePos;
                        }

                        break;
                    case '\r':
                        if ((charPos + 1) < maxPos && _buffer[(charPos + 1) % _bufferLength] == '\n')
                        {
                            // \r\n handling
                            charPos++;
                        }

                        // in some files only \r used as line separator - lets allow that
                        charPos++;
                        goto LineEnded;
                    case '\n':
                        charPos++;
                        goto LineEnded;
                    default:
                        if (ch == delimFirstChar && (_delimLength == 1 || ReadDelimTail(charPos, maxPos, ref charPos)))
                        {
                            currentField = GetOrAddField(charPos + 1);
                            ignoreQuote = false;
                            continue;
                        }

                        // space
                        if (ch == ' ' && trimFields)
                        {
                            continue; // do nothing
                        }

                        // content char
                        if (currentField.Length == 0)
                        {
                            currentField.Start = charPos;
                        }

                        if (currentField.Quoted)
                        {
                            // non-space content after quote = treat quotes as part of content
                            currentField.Quoted = false;
                            ignoreQuote = true;
                        }

                        currentField.End = charPos;
                        break;
                }
            }

            if (!eof)
            {
                // line is not finished, but whole buffer was processed and not EOF
                throw new InvalidDataException(GetLineTooLongMsg());
            }

            LineEnded:
            _actualBufferLen -= charPos - _lineStartPos;
            _lineStartPos = charPos % _bufferLength;

            if (_fieldsCount == 1 && _fields[0].Length == 0)
            {
                // skip empty lines
                goto Start;
            }

            return true;
        }


        internal sealed class Field
        {
            internal int Start;
            internal int End;

            internal int Length
            {
                get { return End - Start + 1; }
            }

            internal bool Quoted;
            internal int EscapedQuotesCount;
            string? _cachedValue = null;

            internal Field Reset(int start)
            {
                Start = start;
                End = start - 1;
                Quoted = false;
                EscapedQuotesCount = 0;
                _cachedValue = null;
                return this;
            }

            internal string? GetValue(char[] buf)
            {
                if (_cachedValue == null)
                {
                    _cachedValue = GetValueInternal(buf);
                }

                return _cachedValue;
            }

            string? GetValueInternal(char[] buf)
            {
                if (Quoted)
                {
                    var s = Start + 1;
                    var lenWithoutQuotes = Length - 2;
                    var val = lenWithoutQuotes > 0 ? GetString(buf, s, lenWithoutQuotes) : String.Empty;
                    if (EscapedQuotesCount > 0)
                        val = val.Replace("\"\"", "\"");
                    return val;
                }

                var len = Length;
                return len > 0 ? GetString(buf, Start, len) : String.Empty;
            }

            private string? GetString(char[] buf, int start, int len)
            {
                var bufLen = buf.Length;
                start = start < bufLen ? start : start % bufLen;
                var endIdx = start + len - 1;
                if (endIdx >= bufLen)
                {
                    var prefixLen = buf.Length - start;
                    var prefix = new string(buf, start, prefixLen);
                    var suffix = new string(buf, 0, len - prefixLen);
                    return prefix + suffix;
                }

                return new string(buf, start, len);
            }
        }
    }
}