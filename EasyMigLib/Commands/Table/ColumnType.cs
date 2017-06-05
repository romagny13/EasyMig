namespace EasyMigLib.Commands
{
    // Int

    public class IntColumnType : ColumnType
    {
        public bool Unsigned { get; }

        public IntColumnType(bool unsigned = false)
        {
            this.Unsigned = unsigned;
        }
    }

    // String

    public class StringColumnType : ColumnType
    {
        public int Length { get; }

        public StringColumnType(int length = 255)
        {
            this.Length = length;
        }
    }

    // Text

    public class TextColumnType : ColumnType
    { }

    // Boolean / Bit

    public class BooleanColumnType : ColumnType
    { }

    // Float

    public class FloatColumnType : ColumnType
    { }

    // DateTime

    public class DateTimeColumnType : ColumnType
    { }

    // Timestamp

    public class TimestampColumnType : ColumnType
    { }


    public class ColumnType
    {

        public bool CheckValue(object defaultValue)
        {
            if (defaultValue != null)
            {
                var type = this.GetType();
                if (type == typeof(IntColumnType))
                {
                    return int.TryParse(defaultValue.ToString(), out int result);
                }
                else if (type == typeof(FloatColumnType))
                {
                    return double.TryParse(defaultValue.ToString(), out double result);
                }
                else if (type == typeof(BooleanColumnType))
                {
                    return defaultValue.GetType() == typeof(bool);
                }
                else
                {
                    return defaultValue.GetType() == typeof(string);
                }
            }
            return true;
        }

        public static IntColumnType Int(bool unsigned = false)
        {
            // INT
            return new IntColumnType(unsigned);
        }

        public static StringColumnType String(int length = 255)
        {
            // VARCHAR(255)
            return new StringColumnType(length);
        }

        public static TextColumnType Text()
        {
            // NVARCHAR(max) or TEXT
            return new TextColumnType();
        }

        public static TimestampColumnType Timestamp()
        {
            // TIMESTAMP
            return new TimestampColumnType();
        }

        public static BooleanColumnType Boolean()
        {
            // BIT
            return new BooleanColumnType();
        }

        public static DateTimeColumnType DateTime()
        {
            // DATETIME
            return new DateTimeColumnType();
        }

        public static FloatColumnType Float()
        {
            // FLOAT
            return new FloatColumnType();
        }
    }
}
