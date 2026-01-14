using ForumApp.App.Utilities;
using ForumApp.Data;
using ForumApp.Data.Services;
using ForumApp.Data.DTO;

namespace ForumApp.App
{
    internal class Program
    {
        private static UserSessionDTO currentSession = new UserSessionDTO { Id = 0, Username = "Guest" };
        private static ForumDbContext context = new ForumDbContext();
        private static UserService userService = new UserService(context);

        public static void Main(string[] args)
        {
            context.Database.EnsureCreated();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Добре дошли във Форума на играта!");
            RunInitialMenu();
        }

        private static void RunInitialMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("           НАЧАЛЕН ЕКРАН");
                Console.WriteLine($"Текущ потребител: {currentSession.Username} ({GetRoleString(currentSession)})");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("1. Вход (Login)");
                Console.WriteLine("2. Регистрация (Register)");
                Console.WriteLine("3. Продължи като Гост (Continue as Guest)");
                Console.WriteLine("4. Изход");
                Console.WriteLine("---------------------------------------------");

                int choice = InputUtility.ReadInt("Изберете опция: ");

                switch (choice)
                {
                    case 1:
                        Login();
                        if (currentSession.IsRegistered) RunMainMenu();
                        break;
                    case 2:
                        Register();
                        break;
                    case 3:
                        currentSession = new UserSessionDTO { Id = 0, Username = "Guest" };
                        RunMainMenu();
                        break;
                    case 4:
                        Console.WriteLine("Приложението се затваря. До нови срещи!");
                        return;
                    default:
                        Console.WriteLine("Невалидна опция. Моля, опитайте отново.");
                        InputUtility.ReadString("Натиснете Enter за продължаване...");
                        break;
                }
            }
        }

        private static void Register()
        {
            Console.Clear();
            Console.WriteLine("--- РЕГИСТРАЦИЯ НА НОВ ПОТРЕБИТЕЛ ---");

            var dto = new UserRegisterDTO();
            dto.Username = InputUtility.ReadString("Въведете потребителско име: ");

            string pass;
            string confirmPass;

            do
            {
                pass = InputUtility.ReadString("Въведете парола: ");
                confirmPass = InputUtility.ReadString("Потвърдете паролата: ");

                if (pass != confirmPass)
                {
                    Console.WriteLine("--> Паролите не съвпадат. Моля, опитайте отново.");
                }
            } while (pass != confirmPass);

            dto.Password = pass;
            dto.ConfirmPassword = confirmPass;

            if (userService.RegisterUser(dto))
            {
                Console.WriteLine($"\n[УСПЕХ] Потребител {dto.Username} е регистриран! Можете да влезете.");
            }

            InputUtility.ReadString("Натиснете Enter за продължаване...");
        }

        private static void Login()
        {
            Console.Clear();
            Console.WriteLine("--- ВХОД В СИСТЕМАТА ---");

            var dto = new UserLoginDTO
            {
                Username = InputUtility.ReadString("Потребителско име: "),
                Password = InputUtility.ReadString("Парола: ")
            };

            var session = userService.LoginUser(dto);

            if (session != null)
            {
                currentSession = session;
                Console.WriteLine($"\n[УСПЕХ] Добре дошли, {currentSession.Username}! Влязохте като {GetRoleString(currentSession)}.");
            }
            else
            {
                Console.WriteLine("\n[ГРЕШКА] Невалидно потребителско име или парола.");
            }

            InputUtility.ReadString("Натиснете Enter за продължаване...");
        }

        private static void RunMainMenu()
        {
            Console.WriteLine("\n*** ГЛАВНО МЕНЮ (Функционалност предстои...) ***");

            InputUtility.ReadString("Натиснете Enter за връщане към началния екран...");
        }

        private static string GetRoleString(UserSessionDTO session)
        {
            if (session.Id == 0) return "Гост";
            return session.Role.ToString();
        }
    }
}
