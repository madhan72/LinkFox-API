using System.Text;

namespace LinkFox.Application.Utils
{
    /// <summary>
    /// Base62 encoder.Used to generate short codes
    /// </summary>
    public class Base62
    {
        private const string AlphaNumeric = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Encode(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non negatice");
            }

            if (value == 0) return "0";

            var stringBuilder = new StringBuilder();

            while (value > 0)
            {
                var rem = (int)(value % 62);
                stringBuilder.Insert(0, AlphaNumeric[rem]);
                value /= 62;
            }

            return stringBuilder.ToString();
        }
    }
}
