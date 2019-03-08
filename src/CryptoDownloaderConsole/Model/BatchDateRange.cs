using System;

namespace CryptoDownloader
{
    public interface IRange<T>
    {
        T Start { get; }
        T End { get; }
        IRange<T> GetNext();
        bool Includes(T value);
        bool Includes(IRange<T> range);
    }

    public class BatchDateRange : IRange<NodaTime.Instant>         
    {
        public BatchDateRange(NodaTime.Instant start, NodaTime.Instant end)
        {
            Start = start;
            End = end;
        }

        public NodaTime.Instant Start { get; private set; }
        public NodaTime.Instant End { get; private set; }

        public IRange<NodaTime.Instant> GetNext()
        {
            var newStart = Start.Plus(NodaTime.Duration.FromDays(7));
            var newEnd = End.Plus(NodaTime.Duration.FromDays(7));
            var range = new BatchDateRange(newStart, newEnd);
            range.Verify();
            return range;
        }

        public override string ToString()
        {
            return String.Format("Start: {0:yyyy-MM-dd HH:mm:ss} End: {1:yyyy-MM-dd HH:mm:ss}", this.Start, this.End);
        }
        public bool Includes(NodaTime.Instant value)
        {
            return (Start <= value) && (value <= End);
        }

        public bool Includes(IRange<NodaTime.Instant> range)
        {
            return (Start <= range.Start) && (range.End <= End);
        }

        public void Verify()
        {
            DateTime startAsDateTime = DateTimeExtensions.ToDateTime(this.Start);
            DateTime endAsDateTime = DateTimeExtensions.ToDateTime(this.End);

            if (startAsDateTime.DayOfWeek != DayOfWeek.Sunday)
            {
                throw new ArgumentException(
                    String.Format("Expected start dateTime to be Sunday, was: {0}", startAsDateTime.DayOfWeek));
            }
            if (startAsDateTime.Hour + startAsDateTime.Minute != 0)
            {
                throw new ArgumentException(
                    String.Format("Expected start dateTime to be HH:MM:SS 00:00:00, was: {0}", startAsDateTime.TimeOfDay));
            }
             if (endAsDateTime.DayOfWeek != DayOfWeek.Saturday)
            {
                throw new ArgumentException(
                    String.Format("Expected end dateTime to be Saturday, was: {0}", endAsDateTime.DayOfWeek));
            }
            if (endAsDateTime.Hour != 23 || endAsDateTime.Minute != 59 || endAsDateTime.Second != 59)
            {
                throw new ArgumentException(
                    String.Format("Expected end dateTime to be HH:MM:SS 23:59:59, was: {0}", endAsDateTime.TimeOfDay));
            }
            var diff = Math.Round((this.End - this.Start).TotalHours,0);
            if (diff != 168)
            {
                throw new ArgumentException(
                    String.Format("Expected end datetime - start datetime to be 1 week - 1 hour, was: {0}", diff));
            }
        }
    }
}