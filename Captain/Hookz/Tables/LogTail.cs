
namespace Captain.Hookz.Tables
{
	/// <summary>
	/// Represents a lexicographically sortable descending timestamp key for Azure Table Storage.
	/// </summary>
	public readonly struct LogTailKey : IComparable<LogTailKey>
	{
		/// <summary>
		/// The string value of the key (inverted ticks, padded to 19 digits).
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Initializes a new <see cref="LogTailKey"/> from a UTC timestamp.
		/// </summary>
		/// <param name="utc">The UTC <see cref="DateTime"/> to generate a descending key from.</param>
		public LogTailKey(DateTime utc)
		{
			if (utc.Kind != DateTimeKind.Utc)
				throw new ArgumentException("DateTime must be in UTC", nameof(utc));

			long invertedTicks = DateTime.MaxValue.Ticks - utc.Ticks;
			Value = invertedTicks.ToString("D19"); // Always 19 digits
		}

		/// <summary>
		/// Initializes a new <see cref="LogTailKey"/> from an existing key string.
		/// </summary>
		/// <param name="value">The raw RowKey string (must be 19 digits).</param>
		public LogTailKey(string value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));

			if (value.Length != 19 || !long.TryParse(value, out _))
				throw new ArgumentException("Value must be a 19-digit string representing inverted ticks.", nameof(value));

			Value = value;
		}

		/// <summary>
		/// Converts this key back into the original UTC timestamp.
		/// </summary>
		public DateTime ToUtc()
		{
			long inverted = long.Parse(Value);
			long originalTicks = DateTime.MaxValue.Ticks - inverted;
			return new DateTime(originalTicks, DateTimeKind.Utc);
		}
		
		public static LogTailKey Now => new(DateTime.UtcNow);
		public override string ToString() => Value;

		public int CompareTo(LogTailKey other) =>
			string.Compare(Value, other.Value, StringComparison.Ordinal);

		public override bool Equals(object? obj) =>
			obj is LogTailKey other && Value == other.Value;

		public override int GetHashCode() => Value.GetHashCode();

		public static implicit operator string(LogTailKey key) => key.Value;

		public static implicit operator LogTailKey(string value) => new(value);
		public static bool operator ==(LogTailKey left, LogTailKey right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(LogTailKey left, LogTailKey right)
		{
			return !(left == right);
		}

		public static bool operator <(LogTailKey left, LogTailKey right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(LogTailKey left, LogTailKey right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(LogTailKey left, LogTailKey right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(LogTailKey left, LogTailKey right)
		{
			return left.CompareTo(right) >= 0;
		}
	}
	public static class Extensions 
	{
		/// <summary>
		/// Converts a UTC timestamp to a descending <see cref="LogTailKey"/>.
		/// </summary>
		/// <param name="utc">The UTC timestamp.</param>
		public static LogTailKey ToLogTailKey(this DateTime utc) => new(utc);
		

	}
}
