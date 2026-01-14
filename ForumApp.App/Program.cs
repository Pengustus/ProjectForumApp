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
        private static PostService postService = new PostService(context);
        private static CommentService commentService = new CommentService(context);
        private static AdminService adminService = new AdminService(context);

        public static void Main(string[] args)
        {
            context.Database.EnsureCreated();
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            DbSeeder.SeedCategories(context);
            DbSeeder.SeedAdmin(context);

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
                        InputUtility.PressEnterToContinue();
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

            InputUtility.PressEnterToContinue();
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

            InputUtility.PressEnterToContinue();
        }

        private static void RunMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== ГЛАВНО МЕНЮ | Потребител: {currentSession.Username} ===");
                Console.WriteLine("1. Виж всички публикации");
                Console.WriteLine("2. Създай нова публикация");

                if (currentSession.IsAdmin)
                {
                    Console.WriteLine("A. Админ Панел");
                }

                Console.WriteLine("3. Изход (Logout)");
                Console.WriteLine("---------------------------------------------");

                Console.Write("Изберете опция: ");
                string choice = Console.ReadLine()?.ToUpper();

                if (choice == "1") ShowAllPosts();
                else if (choice == "2") CreatePost();
                else if (choice == "A" && currentSession.IsAdmin) RunAdminMenu();
                else if (choice == "3") { currentSession = new UserSessionDTO { Id = 0, Username = "Guest" }; break; }
            }
        }

        private static void RunAdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- АДМИНИСТРАТОРСКИ ПАНЕЛ ---");
                Console.WriteLine("1. Списък на всички потребители");
                Console.WriteLine("2. Изтриване на публикация (Модерация)");
                Console.WriteLine("3. Назад");

                int choice = InputUtility.ReadInt("Избор: ");

                if (choice == 1)
                {
                    var users = adminService.GetAllUsers();
                    Console.WriteLine("\nРЕГИСТРИРАНИ ПОТРЕБИТЕЛИ:");
                    foreach (var u in users) Console.WriteLine($"- ID: {u.Id} | Име: {u.Username} | Роля: {u.Role}");
                    InputUtility.PressEnterToContinue();
                }
                else if (choice == 2)
                {
                    int postId = InputUtility.ReadInt("Въведете ID на публикацията за изтриване: ");
                    if (adminService.DeletePost(postId)) Console.WriteLine("Публикацията е изтрита успешно!");
                    else Console.WriteLine("Грешка: Публикацията не съществува.");
                    InputUtility.PressEnterToContinue();
                }
                else if (choice == 3) break;
            }
        }

        private static void ShowAllPosts()
        {
            Console.Clear();
            var posts = postService.GetAllPosts();

            Console.WriteLine("--- ВСИЧКИ ПУБЛИКАЦИИ ---");
            foreach (var p in posts)
            {
                Console.WriteLine($"[{p.Id}] {p.Title} (от {p.AuthorName})");
            }

            Console.WriteLine("\nВъведете ID на публикация, за да я прочетете, или 0 за връщане:");
            int postId = InputUtility.ReadInt("Избор: ");

            if (postId != 0)
            {
                ViewPostDetails(postId);
            }
        }

        private static void CreatePost()
        {
            if (currentSession.Id == 0)
            {
                Console.WriteLine("\n[ГРЕШКА] Гостите не могат да публикуват. Моля, регистрирайте се.");
                InputUtility.PressEnterToContinue();
                return;
            }

            var categories = postService.GetCategories();
            if (!categories.Any())
            {
                Console.WriteLine("\n[ГРЕШКА] Все още няма категории. Админът трябва да добави такива.");
                InputUtility.PressEnterToContinue();
                return;
            }

            Console.Clear();
            Console.WriteLine("--- НОВА ПУБЛИКАЦИЯ ---");
            string title = InputUtility.ReadString("Заглавие: ");
            string content = InputUtility.ReadString("Съдържание: ");

            Console.WriteLine("\nНалични категории:");
            foreach (var cat in categories) Console.WriteLine($"{cat.Key}: {cat.Value}");
            int catId = InputUtility.ReadInt("Изберете ID на категория: ");

            postService.CreatePost(new PostCreateDTO
            {
                Title = title,
                Content = content,
                AuthorId = currentSession.Id,
                CategoryId = catId
            });

            Console.WriteLine("\n[УСПЕХ] Темата е публикувана!");
            InputUtility.PressEnterToContinue();
        }

        private static void ViewPostDetails(int postId)
        {

            var posts = postService.GetAllPosts();
            var post = posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                Console.WriteLine("Публикацията не е намерена.");
                InputUtility.PressEnterToContinue();
                return;
            }

            Console.Clear();
            Console.WriteLine($"ЗАГЛАВИЕ: {post.Title}");
            Console.WriteLine($"АВТОР: {post.AuthorName} | КАТЕГОРИЯ: {post.CategoryName}");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine(post.Content);
            Console.WriteLine(new string('-', 40));

            var comments = commentService.GetCommentsByPostId(postId);
            Console.WriteLine($"КОМЕНТАРИ ({comments.Count}):");

            foreach (var c in comments)
            {
                Console.WriteLine($" >> [{c.AuthorName}]: {c.Content} ({c.CreatedOn:dd/MM HH:mm})");
            }

            Console.WriteLine("\nОпции: 1. Добави коментар | 2. Назад");
            int choice = InputUtility.ReadInt("Избор: ");

            if (choice == 1)
            {
                AddComment(postId);
                ViewPostDetails(postId); 
            }
        }

        private static void AddComment(int postId)
        {
            if (currentSession.Id == 0)
            {
                Console.WriteLine("Трябва да сте влезли в профила си, за да коментирате.");
                InputUtility.PressEnterToContinue();
                return;
            }

            string content = InputUtility.ReadString("Вашият коментар: ");

            commentService.AddComment(new CommentCreateDTO
            {
                Content = content,
                PostId = postId,
                AuthorId = currentSession.Id
            });

            Console.WriteLine("Коментарът е добавен!");
        }

        private static void ImportJson()
        {
            Console.Clear();
            Console.WriteLine("--- ИМПОРТ НА ДАННИ (JSON) ---");
            Console.Write("Въведете ПЪЛЕН ПЪТ до файла (напр. C:\\data\\tags.json): ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                try
                {
                    string jsonContent = File.ReadAllText(path);
                    string message = postService.ImportTags(jsonContent);
                    Console.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Грешка при четене на JSON: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Грешка: Файлът не е намерен на посочения път!");
            }
        }

        private static void ExportJson()
        {
            Console.Clear();
            Console.WriteLine("--- ЕКСПОРТ НА ДАННИ (JSON) ---");
            Console.Write("Въведете име за новия файл (напр. MyExport.json): ");
            string fileName = Console.ReadLine();

            try
            {
                postService.ExportPosts(fileName);
                Console.WriteLine($"Данните бяха успешно експортирани в {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Грешка при експорта: " + ex.Message);
            }
        }

        private static string GetRoleString(UserSessionDTO session)
        {
            if (session.Id == 0) return "Гост";
            return session.Role.ToString();
        }
    }
}
