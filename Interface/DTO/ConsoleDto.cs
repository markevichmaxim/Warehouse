namespace Interface.DTO
{
    public class ConsoleDto
    {
        public static T InputValidation<T>(string message, Func<string, T> convert, Predicate<T> validate)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine()!;
                T? value = default;
                try
                {
                    value = convert(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    continue;
                }
                if (!validate(value))
                {
                    Console.WriteLine("Invalid value. Please try again.");
                    continue;
                }
                return value;
            }
        }
    }
}
