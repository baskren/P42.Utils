﻿using System;

namespace P42.Utils.Uno
{
    /// <summary>
    /// P42.Utils.Uno FormattedString Span
    /// </summary>
    abstract class Span : P42.NotifiableObject.FieldBackedNotifiablePropertyObject, ICopiable<Span> , IEquatable<Span>
    {
        #region Fields
        internal string Key;

        int _start = -1;
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the span's start.
        /// </summary>
        /// <value>The start.</value>
        public int Start
        {
            get => _start;
            set
            {
                if (_start == value)
                    return;
                _start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        // use int.MaxValue to indicate that the span is unterminated (goes to the end of the string)
        int _end = -1;
        /// <summary>
        /// Gets or sets the span's end.
        /// </summary>
        /// <value>The end.</value>
        public int End
        {
            get => _end;
            set
            {
                if (_end == value)
                    return;
                _end = value;
                OnPropertyChanged(nameof(End));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.  int.MaxValue to indicate that the span is unterminated (goes to the end of the string)</value>
        public int Length
        {
            get
            {
                if (_end == int.MaxValue)
                    return int.MaxValue;

                return _end - _start + 1;
            }
            set
            {
                if ((_end - _start + 1) == value)
                    return;
                if (value == int.MaxValue)
                    _end = int.MaxValue;
                else
                    _end = _start + value - 1;
                OnPropertyChanged(nameof(End));
            }
        }
        #endregion


        #region Construction / Diposal
        /// <summary>
        /// Initializes a new instance of the <see cref="P42.Utils.Uno.Span"/> class.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        protected Span(int start, int end)
        {
            _start = start;
            _end = end;
        }
        #endregion


        #region
        public void PropertiesFrom(Span source)
        {
            Key = source.Key;
            Start = source.Start;
            End = source.End;
        }

        public virtual Span Copy()
        {
            throw new NotImplementedException();
        }
        #endregion

        public bool Equals(Span other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Key == other.Key && _start == other._start && _end == other._end;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Span)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, _start, _end);
        }
    }

}

