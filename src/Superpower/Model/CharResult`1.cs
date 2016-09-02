﻿using System;
using System.Linq;

namespace Superpower.Model
{
    public struct CharResult<T>
    {
        readonly T _value;

        public StringSpan Location { get; }
        public StringSpan Remainder { get; }
        public bool HasValue { get; }
        public string[] Expectations { get; }

        public bool IsPartial(StringSpan @from) => @from != Remainder;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Result has no value.");
                return _value;
            }
        }

        internal CharResult(T value, StringSpan location, StringSpan remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            Expectations = null;
        }

        internal CharResult(StringSpan remainder, string[] expectations)
        {
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
            Expectations = expectations;
        }

        public override string ToString()
        {
            if (Remainder == StringSpan.None)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Remainder.IsAtEnd)
            {
                location = $" (line {Remainder.Position.Line}, column {Remainder.Position.Column})";
            }

            return $"Parsing failure{location}: {message}.";
        }

        public string FormatErrorMessageFragment()
        {
            string message;
            if (Remainder.IsAtEnd)
            {
                message = "unexpected end of input";
            }
            else
            {
                var next = Remainder.NextChar().Value;
                message = $"unexpected `{next}`";
            }

            if (Expectations != null)
            {
                var expected = Expectations.Last();
                if (Expectations.Length > 1)
                    expected = $"{string.Join(", ", Expectations.Take(Expectations.Length - 1))} or {expected}";
                message += $", expected {expected}";
            }

            return message;
        }
    }
}