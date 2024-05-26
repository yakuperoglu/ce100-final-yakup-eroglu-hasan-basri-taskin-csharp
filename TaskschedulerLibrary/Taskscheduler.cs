/**
* @file LibrarysystemLibrary.cs
* @brief Contains the using directives and the definitions of the Book and User classes for the TaskschedulerLibrary library.
*/
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OtpNet;
/**
 * @namespace LibrarysystemLibrary
 * @brief Contains classes and methods for managing the library system functionality.
 */
namespace TaskschedulerLibrary
{
    /**
 * @class User
 * @brief Represents a user in the system.
 */
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        /**
         * @brief Gets or sets the email address of the user.
         */
        public string Email { get; set; }
        /**
         * @brief Gets or sets the password of the user.
         */
        public string Password { get; set; }
    }

    /**
     * @class Task
     * @brief Represents a task in the system.
     */
    [Serializable]
    public class Task
    {
        public int Id { get; set; }
        /**
         * @brief Gets or sets the name of the task.
         */
        public string TaskName { get; set; }
        /**
         * @brief Gets or sets the description of the task.
         */
        public string TaskDescription { get; set; }
        /**
         * @brief Gets or sets the deadline of the task.
         */
        public string Deadline { get; set; }
        /**
         * @brief Gets or sets the priority of the task.
         */
        public byte? Priority { get; set; }
        /**
         * @brief Gets or sets the cost associated with the task.
         */
        public double Cost { get; set; }
        /**
         * @brief Gets or sets the category of the task.
         */
        public Category? Category { get; set; }
        /**
         * @brief Gets or sets the owner of the task.
         */
        public User Owner { get; set; }
    }

    /**
     * @class Category
     * @brief Represents a category of tasks.
     */
    [Serializable]
    public class Category
    {
        public int Id { get; set; }
        /**
         * @brief Gets or sets the name of the category.
         */
        public string CategoryName { get; set; }
    }

    /**
     * @class Huffman
     * @brief Data structure for Huffman Tree Node.
     */
    public class Huffman
    {
        /**
         * @brief Gets or sets the character for the Huffman node.
         */
        public char Character { get; set; }
        /**
         * @brief Gets or sets the frequency of the character.
         */
        public int Frequency { get; set; }
        /**
         * @brief Gets or sets the left child node.
         */
        public Huffman Left { get; set; }
        /**
         * @brief Gets or sets the right child node.
         */
        public Huffman Right { get; set; }
    }

    /**
     * @class Edge
     * @brief Represents an edge in a graph.
     */
    public class Edge
    {
        /**
         * @brief Gets or sets the starting node of the edge.
         */
        public int From { get; set; }
        /**
         * @brief Gets or sets the ending node of the edge.
         */
        public int To { get; set; }
        /**
         * @brief Gets or sets the capacity of the edge.
         */
        public double Capacity { get; set; }
        /**
         * @brief Gets or sets the flow through the edge.
         */
        public double Flow { get; set; }
    }
    public class Taskscheduler
    {

        /**
         * @brief A secret key used for encryption or other secure operations.
         */
        private string secretKey = "1234567890123456";

        /**
         * @brief Indicates whether the system is in test mode.
         */
        public bool IsTestMode { get; set; } = false;

        /**
         * @brief Indicates whether to bypass certain checks or validations.
         */
        public bool Bypass { get; set; } = true;

        /**
         * @brief Gets or sets the currently logged-in user.
         */
        public User LoggedInUser { get; set; }
        /**
 * @brief Handles input errors by displaying an error message.
 * @return Always returns false to indicate an error.
 */
        public bool HandleInputError()
        {
            Console.WriteLine("Only enter numerical value");
            return false;
        }
        /**
  * @brief Clears the console screen, unless the library system is in test mode.
  */
        public void ClearScreen()
        {
            if (IsTestMode)
            {
                return; // Test modundayken konsolu temizleme
            }

            Console.Clear();
        }
        /**
 * @brief Prompts the user to press any key to continue.
 * @return Always returns true.
 */
        public bool EnterToContinue()
        {
            Console.Write("Press any key to continue... ");
            if (!IsTestMode)
            {
                Console.ReadKey();
            }
            return true;
        }
        /**
 * @brief Displays the main menu and handles user input.
 * @param pathFileUsers Path to the file containing user data.
 * @param pathFileTasks Path to the file containing task data.
 * @param pathFileCategories Path to the file containing category data.
 * @return Returns 0 to indicate program exit.
 */
        public int MainMenu(string pathFileUsers, string pathFileTasks, string pathFileCategories)
        {
            int choice;

            while (true)
            {
                ClearScreen();
                PrintMainMenu();
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ClearScreen();
                        if (LoginUserMenu(pathFileUsers))
                            UserOperationsMenu(pathFileTasks, pathFileCategories, pathFileUsers);
                        break;

                    case 2:
                        ClearScreen();
                        RegisterMenu(pathFileUsers);
                        break;

                    case 3:
                        ClearScreen();
                        GuestOperation(pathFileCategories);
                        break;

                    case 4:
                        Console.WriteLine("Exit Program");
                        return 0;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }

        /**
 * @brief Displays the login menu and handles user login.
 * @param pathFileUsers Path to the file containing user data.
 * @return Returns true if login is successful, false otherwise.
 */
        public bool LoginUserMenu(string pathFileUsers)
        {
            ClearScreen();
            User loginUser = new User();
            Console.Write("Enter email: ");
            loginUser.Email = Console.ReadLine();

            Console.Write("Enter password: ");
            loginUser.Password = Console.ReadLine();
            var totp = new Totp(Encoding.UTF8.GetBytes(loginUser.Password));
            string computedOtp = (IsTestMode || Bypass) ? "123456" : totp.ComputeTotp();
            Console.WriteLine("Current OTP: " + computedOtp);
            Console.Write("Enter OTP: ");
            string otp = Console.ReadLine();
            if (otp == computedOtp)
            {
                return LoginUser(loginUser, pathFileUsers);
            }
            else
            {
                Console.WriteLine("Invalid OTP. Please try again.");
                EnterToContinue();
                return false;
            }
        }
        /**
 * @brief Authenticates the user by comparing entered credentials with stored data.
 * @param user User object containing login credentials.
 * @param pathFile Path to the file containing user data.
 * @return Returns true if authentication is successful, false otherwise.
 */
        public bool LoginUser(User user, string pathFile)
        {
            if (File.Exists(pathFile))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(pathFile, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        User existingUser = new User
                        {
                            Id = reader.ReadInt32(),
                            Email = reader.ReadString(),
                            Password = reader.ReadString()
                        };
                        string hashString;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            hashString = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password)).ToString();
                        }
                        string encryptedBytes = AesEncrypt(hashString, secretKey).ToString();

                        if (existingUser.Email == user.Email && existingUser.Password == encryptedBytes)
                        {
                            LoggedInUser = existingUser;
                            Console.WriteLine("Login successful.");
                            EnterToContinue();
                            //UserOperationsMenu'u çağır
                            return true;
                        }
                    }
                }
            }
            Console.WriteLine("Invalid email or password. Please try again.");
            EnterToContinue();
            return false;
        }
        /**
 * @brief Displays the registration menu and handles user registration.
 * @param pathFileUsers Path to the file containing user data.
 * @return Returns true if registration is successful, false otherwise.
 */
        public bool RegisterMenu(string pathFileUsers)
        {
            ClearScreen();
            User newUser = new User();
            Console.Write("Enter email: ");
            newUser.Email = Console.ReadLine();

            Console.Write("Enter password: ");
            newUser.Password = Console.ReadLine();
            newUser.Id = GetNewUserId(pathFileUsers);
            return RegisterUser(newUser, pathFileUsers);
        }
        /**
   * @brief Registers a new user by storing their data in the file.
   * @param user User object containing registration details.
   * @param pathFileUsers Path to the file containing user data.
   * @return Returns true if registration is successful, false otherwise.
   */
        public bool RegisterUser(User user, string pathFileUsers)
        {
            string hashString;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashString = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password)).ToString();
            }
            string encryptedBytes = AesEncrypt(hashString, secretKey).ToString();

            using (BinaryWriter writer = new BinaryWriter(File.Open(pathFileUsers, FileMode.Append)))
            {
                writer.Write(user.Id);
                writer.Write(user.Email);
                writer.Write(encryptedBytes);
            }

            Console.WriteLine("User registered successfully.");
            EnterToContinue();
            return true;
        }

        /**
 * @brief Encrypts a plain text using AES encryption.
 * @param password The encryption key.
 * @param plainText The text to be encrypted.
 * @return The encrypted byte array.
 */
        public byte[] AesEncrypt(string password, string plainText)
        {
            byte[] encryptedBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                using (SharpAESCrypt.SharpAESCrypt aes =
                       new SharpAESCrypt.SharpAESCrypt(password, ms, SharpAESCrypt.OperationMode.Encrypt))
                {
                    using (StreamWriter sw = new StreamWriter(aes))
                    {
                        sw.Write(plainText);
                    }
                }
                encryptedBytes = ms.ToArray();
            }
            return encryptedBytes;
        }
        /**
 * @brief Handles guest operations by displaying the guest menu and processing user input.
 * @param pathFileCategories Path to the file containing category data.
 * @return Returns false to indicate the guest operation is completed.
 */
        public bool GuestOperation(string pathFileCategories)
        {
            int choice;

            while (true)
            {
                PrintGuestMenu();

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ClearScreen();
                        PrintCategoriesToConsole(pathFileCategories);
                        EnterToContinue();
                        break;

                    case 2:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }
        /**
 * @brief Handles user operations by displaying the user menu and processing user input.
 * @param pathFileTasks Path to the file containing task data.
 * @param pathFileCategories Path to the file containing category data.
 * @param pathFileUsers Path to the file containing user data.
 * @return Returns false to indicate the user operation is completed.
 */

        public bool UserOperationsMenu(string pathFileTasks, string pathFileCategories, string pathFileUsers)
        {
            int choice;

            while (true)
            {
                ClearScreen();
                PrintUserMenu();

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        TaskMenu(pathFileTasks, pathFileCategories, pathFileUsers);
                        break;
                    case 2:
                        DeadlineSettingMenu(pathFileTasks);
                        break;

                    case 3:
                        ReminderSystemMenu(pathFileTasks);
                        break;

                    case 4:
                        TaskPrioritizationMenu(pathFileTasks);
                        break;
                    case 5:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }
        /**
 * @brief Handles task-related operations by displaying the task menu and processing user input.
 * @param pathFileTasks Path to the file containing task data.
 * @param pathFileCategories Path to the file containing category data.
 * @param pathFileUsers Path to the file containing user data.
 * @return Returns false to indicate the task menu operation is completed.
 */

        public bool TaskMenu(string pathFileTasks, string pathFileCategories, string pathFileUsers)
        {
            int choice;

            while (true)
            {
                ClearScreen();
                PrintTaskMenu();

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        CreateTaskMenu(pathFileTasks);
                        break;

                    case 2:
                        CategorizeTaskMenu(pathFileTasks, pathFileCategories);
                        break;

                    case 3:
                        FindSimilarTasksMenu(pathFileTasks);
                        break;
                    case 4:
                        FindSimilarUsersByTaskCategoryMenu(pathFileUsers, pathFileTasks);
                        break;
                    case 5:
                        TheShortestPathBetweenTasks(pathFileTasks);
                        break;
                    case 6:
                        OptimizeBudget(pathFileTasks);
                        break;
                    case 7:
                        AllocateResourcesBasedOnBudgetAndDeadline(pathFileTasks);
                        break;
                    case 8:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }
        /**
 * @brief Displays the create task menu and processes user input for creating a task.
 * @param pathFileTasks Path to the file containing task data.
 * @return Returns true if the task is created successfully, false otherwise.
 */
        public bool CreateTaskMenu(string pathFileTasks)
        {
            ClearScreen();
            Console.WriteLine("Create Task \n ");
            Task newTask = new Task();

            Console.Write("Enter task name: ");
            newTask.TaskName = Console.ReadLine();

            Console.Write("Enter task description: ");
            newTask.TaskDescription = Console.ReadLine();

            Console.Write("Enter your cost: ");
            newTask.Cost = Convert.ToDouble(Console.ReadLine());

            return CreateTask(newTask, pathFileTasks);
        }
        /**
         * @brief Creates a new task and saves it to the specified file.
         * @param task The task to be created.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the task is created successfully, false otherwise.
         */
        public bool CreateTask(Task task, string pathFileTasks)
        {

            List<Task> tasks;

            if (File.Exists(pathFileTasks))
            {
                string jsonString = File.ReadAllText(pathFileTasks);
                tasks = JsonSerializer.Deserialize<List<Task>>(jsonString);
            }
            else
            {
                tasks = new List<Task>();
            }
            task.Id = GetNewTaskId(pathFileTasks);
            task.Owner = LoggedInUser;
            task.Cost = task.Cost;
            tasks.Add(task);

            string jsonStringUpdated = JsonSerializer.Serialize(tasks);
            File.WriteAllText(pathFileTasks, jsonStringUpdated);

            Console.WriteLine("Task created successfully.");
            EnterToContinue();
            return true;

        }
        /**
         * @brief Displays the categorize task menu and processes user input for categorizing tasks.
         * @param pathFileTasks Path to the file containing task data.
         * @param pathFileCategories Path to the file containing category data.
         * @return Returns true if the task is categorized successfully, false otherwise.
         */
        public bool CategorizeTaskMenu(string pathFileTasks, string pathFileCategories)
        {
            Task selectedTask = null;
            Category selectedCategory = null;
            List<Task> ownedTasks = LoadOwnedTasks(pathFileTasks);
            List<Category> categories = LoadAllCategories(pathFileCategories);

            do
            {
                ClearScreen();
                Console.WriteLine("Categorize Task \n");

                if (ownedTasks.Count == 0)
                {
                    Console.WriteLine("No tasks found.");
                    EnterToContinue();
                    return false;
                }

                PrintOwnedTasksToConsole(pathFileTasks);


                Console.Write("Select a task to categorize. You can just select uncategorized tasks. (Type \"exit\" to exit): ");
                int taskIndex;
                string taskIndexString = Console.ReadLine();

                if (taskIndexString.ToLower() == "exit")
                {
                    return false;
                }

                if (!int.TryParse(taskIndexString, out taskIndex))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                //Check if the task index is valid
                if (taskIndex < 0)
                {
                    Console.WriteLine("Invalid task index. Please try again.");
                    EnterToContinue();
                    continue;
                }

                //Check user has the task
                foreach (Task task in ownedTasks)
                {
                    if (task.Id == taskIndex && task.Category == null)
                    {
                        selectedTask = task;
                        break;
                    }
                }

                if (selectedTask == null)
                {
                    Console.WriteLine("You do not have the task with the given index. Please try again.");
                    EnterToContinue();
                    continue;
                }
                break;

            } while (true);

            do
            {
                //Write Selected Task to console

                ClearScreen();
                Console.WriteLine("Selected Task: ");
                Console.WriteLine("----------------------------------------------------------------------------------------");
                Console.WriteLine("| Task Id   | Task Name        | Task Description      | Deadline   | Category         |");
                Console.WriteLine("----------------------------------------------------------------------------------------");

                string taskId = selectedTask.Id.ToString().PadRight(9);
                string taskName = selectedTask.TaskName.PadRight(16);
                string taskDescription = selectedTask.TaskDescription.PadRight(21);
                string deadline = !string.IsNullOrEmpty(selectedTask.Deadline) ? selectedTask.Deadline.PadRight(10) : "-".PadRight(10);
                string taskCategory = selectedTask.Category != null ? selectedTask.Category.CategoryName.ToString().PadRight(16) : "-".PadRight(16);

                Console.WriteLine($"| {taskId} | {taskName} | {taskDescription} | {deadline} | {taskCategory} |");
                Console.WriteLine("----------------------------------------------------------------------------------------\n");
                PrintCategoriesToConsole(pathFileCategories);

                Console.Write("Select a category for the task (Type \"exit\" to exit): ");
                int categoryIndex;
                string categoryIndexString = Console.ReadLine();

                if (categoryIndexString.ToLower() == "exit")
                {
                    return false;
                }

                if (!int.TryParse(categoryIndexString, out categoryIndex))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                //Check if the category index is valid
                if (categoryIndex < 0)
                {
                    Console.WriteLine("Invalid category index. Please try again.");
                    EnterToContinue();
                    continue;
                }

                foreach (Category category in categories)
                {
                    if (category.Id == categoryIndex)
                    {
                        selectedCategory = category;
                        break;
                    }
                }

                //Check if the category index in the list
                if (selectedCategory == null)
                {
                    Console.WriteLine("Invalid category index. Please try again.");
                    EnterToContinue();
                    continue;
                }
                break;

            } while (true);
            return CategorizeTask(selectedTask, selectedCategory, pathFileTasks);
        }
        /**
         * @brief Categorizes a task by updating its category and saving it to the specified file.
         * @param task The task to be categorized.
         * @param category The category to assign to the task.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the task is categorized successfully, false otherwise.
         */
        public bool CategorizeTask(Task task, Category category, string pathFileTasks)
        {
            List<Task> tasks = LoadAllTasks(pathFileTasks);

            // foreach ile tasks listesindeki task'ları dönün ve seçilen task'ı bulun ve kategorisini güncelleyin
            foreach (Task taskItem in tasks)
            {
                if (taskItem.Id == task.Id)
                {
                    taskItem.Category = category;
                    break;
                }
            }

            string updatedJsonString = JsonSerializer.Serialize(tasks);
            File.WriteAllText(pathFileTasks, updatedJsonString);

            Console.WriteLine("Task categorized successfully.");
            EnterToContinue();
            return true;
        }


        /**
        // * @brief Prints tasks with similar names for a given user.
        // * @param pathFileTasks Path to the file containing task data.
        // * @return Returns true after displaying similar tasks.
        // */
        public bool FindSimilarTasksMenu(string pathFileTasks)
        {
            ClearScreen();
            double similarityThreshold = 0.5;
            // Get tasks owned by the user
            var userTasks = LoadOwnedTasks(pathFileTasks);

            Console.WriteLine("Similar Tasks\n");
            // Compare each task with each other
            int taskCount = 0;
            for (int i = 0; i < userTasks.Count; i++)
            {
                for (int j = i + 1; j < userTasks.Count; j++)
                {
                    int lcsLength = CalculateLongestCommonSubsequence(userTasks[i].TaskName, userTasks[j].TaskName);
                    int maxLength = Math.Max(userTasks[i].TaskName.Length, userTasks[j].TaskName.Length);
                    double similarity = (double)lcsLength / maxLength;

                    // If similarity is above threshold, print the tasks
                    if (similarity >= similarityThreshold)
                    {
                        taskCount++;
                        Console.WriteLine($"{taskCount}) {userTasks[i].TaskName} and {userTasks[j].TaskName}");
                    }
                }
            }

            EnterToContinue();
            return true;
        }
        /**
 * @brief Finds and prints similar users by task category using BFS or DFS.
 * @param pathFileUsers Path to the file containing user data.
 * @param pathFileTasks Path to the file containing task data.
 * @return Returns true after displaying similar users.
 */
        public bool FindSimilarUsersByTaskCategoryMenu(string pathFileUsers, string pathFileTasks)
        {
            ClearScreen();
            List<User> usersList = LoadAllUsers(pathFileUsers);
            List<Task> tasksList = LoadAllTasks(pathFileTasks);
            if (usersList.Count < 2 || tasksList.Count < 2)
            {
                Console.WriteLine("Not enough users or tasks to find similar users.");
                EnterToContinue();
                return false;
            }

            Dictionary<int, Task> tasks = new Dictionary<int, Task>();
            Dictionary<int, User> users = new Dictionary<int, User>();

            foreach (var user in usersList)
            {
                if (!users.ContainsKey(user.Id))
                {
                    users.Add(user.Id, user);
                }
            }

            foreach (var task in tasksList)
            {
                if (!tasks.ContainsKey(task.Id))
                {
                    tasks.Add(task.Id, task);
                }
            }

            Random random = new Random();
            int startTaskId = tasks.Keys.ElementAt(random.Next(tasks.Count));

            //Ask to user which algorithm will be used. BFS or DFS
            List<User> similarUsers = null;
            while (true)
            {
                Console.WriteLine("Choose an algorithm to find similar users:");
                Console.WriteLine("1. BFS");
                Console.WriteLine("2. DFS");
                Console.Write("Please enter a number to select: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        similarUsers = FindSimilarUsersByTaskCategoryBFS(tasks, users, startTaskId);
                        break;
                    case 2:
                        similarUsers = FindSimilarUsersByTaskCategoryDFS(tasks, users, startTaskId);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }

                if (similarUsers != null)
                {
                    break;
                }
            }

            //Write similar users to the console
            Console.WriteLine("Similar Users\n");
            Console.WriteLine("----------------------------");
            Console.WriteLine("| ID    | Email            |");
            Console.WriteLine("----------------------------");
            foreach (var similarUser in similarUsers)
            {
                string userId = similarUser.Id.ToString().PadRight(5);
                string userEmail = similarUser.Email.PadRight(16);

                Console.WriteLine($"| {userId} | {userEmail} |");
                Console.WriteLine("----------------------------");
            }

            EnterToContinue();
            return true;
        }
        /**
         * @brief Finds the shortest path between tasks using Prim's algorithm.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true after finding and displaying the shortest path.
         */
        //MST PRIM algorithm
        public bool TheShortestPathBetweenTasks(string pathFileTasks)
        {
            // Load owned tasks user
            List<Task> tasks = LoadOwnedTasks(pathFileTasks);
            if (tasks.Count < 2)
            {
                Console.WriteLine("Not enough tasks to find the shortest path.");
                EnterToContinue();
                return false;
            }

            // Form the edges (for example, random costs between each task)
            Dictionary<Tuple<int, int>, int> edges = new Dictionary<Tuple<int, int>, int>();
            Random rand = new Random();
            for (int i = 0; i < tasks.Count; i++)
            {
                for (int j = i + 1; j < tasks.Count; j++)
                {
                    int cost = rand.Next(1, 10); // Rastgele maliyetler üret
                    edges.Add(new Tuple<int, int>(tasks[i].Id, tasks[j].Id), cost);
                    edges.Add(new Tuple<int, int>(tasks[j].Id, tasks[i].Id), cost);
                }
            }

            // Define the necessary structures for Prim's Algorithm.
            int numTasks = tasks.Count;
            int[] key = new int[numTasks]; // Key values used to pick minimum weight edge in cut
            bool[] mstSet = new bool[numTasks]; // To represent set of vertices included in MST
            int[] parent = new int[numTasks]; // Array to store constructed MST

            for (int i = 0; i < numTasks; i++)
            {
                key[i] = int.MaxValue;
                mstSet[i] = false;
                parent[i] = -1;
            }

            key[0] = 0; // Start Point

            for (int count = 0; count < numTasks - 1; count++)
            {
                int u = MinKey(key, mstSet, numTasks);
                mstSet[u] = true;

                for (int v = 0; v < numTasks; v++)
                {
                    if (edges.ContainsKey(new Tuple<int, int>(tasks[u].Id, tasks[v].Id)) && !mstSet[v] && edges[new Tuple<int, int>(tasks[u].Id, tasks[v].Id)] < key[v])
                    {
                        parent[v] = u;
                        key[v] = edges[new Tuple<int, int>(tasks[u].Id, tasks[v].Id)];
                    }
                }
            }

            PrintMST(parent, numTasks, tasks, key);
            return true;
        }
        /**
         * @brief Finds the vertex with the minimum key value that is not yet included in the MST.
         * @param key Array of key values.
         * @param mstSet Array indicating whether a vertex is included in the MST.
         * @param numTasks Number of tasks.
         * @return Returns the index of the vertex with the minimum key value.
         */
        private int MinKey(int[] key, bool[] mstSet, int numTasks)
        {
            int min = int.MaxValue, minIndex = -1;
            for (int v = 0; v < numTasks; v++)
                if (!mstSet[v] && key[v] < min)
                {
                    min = key[v];
                    minIndex = v;
                }
            return minIndex;
        }
        /**
         * @brief Prints the Minimum Spanning Tree (MST).
         * @param parent Array storing the constructed MST.
         * @param numTasks Number of tasks.
         * @param tasks List of tasks.
         * @param key Array of key values.
         * @return Returns true after printing the MST.
         */
        private bool PrintMST(int[] parent, int numTasks, List<Task> tasks, int[] key)
        {
            Console.WriteLine("Edges in the MST:");
            for (int i = 1; i < numTasks; i++)
                if (parent[i] != -1) // Parent of the root node is -1
                    Console.WriteLine($"{tasks[parent[i]].TaskName} - {tasks[i].TaskName}, Cost: {key[i]}");
            EnterToContinue();
            return true;
        }
        /**
         * @brief Prints the Minimum Spanning Tree (MST).
         * @param parent Array storing the constructed MST.
         * @param numTasks Number of tasks.
         * @param tasks List of tasks.
         * @param key Array of key values.
         * @return Returns true after printing the MST.
         */
        //Ford Fulker Algorithm
        public void OptimizeBudget(string pathFileTasks)
        {
            ClearScreen();
            List<Task> tasks = LoadOwnedTasks(pathFileTasks);
            Console.Write("Please enter your budget: ");
            double budget = Convert.ToDouble(Console.ReadLine());

            List<Edge> edges = new List<Edge>();
            int source = 0, sink = tasks.Count + 1;
            int[] parent = new int[sink + 1];
            bool[] visited = new bool[sink + 1];
            int[] level = new int[sink + 1]; // For Dinic's algorithm

            //Ask user to which algorithm will be used. Ford-Fulkerson or Edmonds-Karp
            string algorithm;
            InitializeGraph(tasks, edges, source, sink);
            double maxFlow;
            while (true)
            {
                ClearScreen();
                Console.WriteLine("Choose an algorithm to optimize budget:");
                Console.WriteLine("1. Ford-Fulkerson");
                Console.WriteLine("2. Edmonds-Karp");
                Console.WriteLine("3. Dinic's Algorithm");
                Console.Write("Please enter a number to select: ");
                algorithm = Console.ReadLine();

                if (algorithm == "1")
                {
                    maxFlow = FordFulkerson(edges, parent, visited, source, sink);
                    break;
                }
                else if (algorithm == "2")
                {
                    maxFlow = EdmondsKarp(edges, parent, visited, source, sink);
                    break;
                }
                else if (algorithm == "3")
                {
                    maxFlow = DinicsAlgorithm(tasks, edges, level, parent, source, sink);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    EnterToContinue();
                }
            }


            if (maxFlow <= budget)
                Console.WriteLine($"Budget is sufficient. Maximum flow is {maxFlow}.");
            else
                Console.WriteLine($"Budget is insufficient. Maximum flow is {maxFlow}.");
            EnterToContinue();
        }


        /**
 * @brief Displays the deadline setting menu and processes user input for setting deadlines.
 * @param pathFileTasks Path to the file containing task data.
 * @return Returns true if the deadline setting process is completed, false otherwise.
 */
        public bool DeadlineSettingMenu(string pathFileTasks)
        {
            int choice;

            while (true)
            {
                ClearScreen();
                PrintDeadlineSettingMenu();

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        SetDeadlineForTaskMenu(pathFileTasks);
                        break;

                    case 2:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }
        /**
         * @brief Displays the menu for setting a deadline for a specific task and processes user input.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the deadline is set successfully, false otherwise.
         */
        public bool SetDeadlineForTaskMenu(string pathFileTasks)
        {
            Task selectedTask = null;
            List<Task> ownedTasks = LoadOwnedTasks(pathFileTasks);

            do
            {
                ClearScreen();
                Console.WriteLine("Set Deadline for a Task \n");

                if (ownedTasks.Count == 0)
                {
                    Console.WriteLine("No tasks found.");
                    EnterToContinue();
                    return false;
                }

                PrintOwnedTasksToConsole(pathFileTasks);

                Console.Write("Select a task to set deadline. You can choose tasks that do not have a due date. (Type \"exit\" to exit): ");
                int taskIndex;
                string taskIndexString = Console.ReadLine();

                if (taskIndexString.ToLower() == "exit")
                {
                    return false;
                }

                if (!int.TryParse(taskIndexString, out taskIndex))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                //Check if the task index is valid
                if (taskIndex < 0)
                {
                    Console.WriteLine("Invalid task index. Please try again.");
                    EnterToContinue();
                    continue;
                }

                //Check user has the task
                foreach (Task task in ownedTasks)
                {
                    if (task.Id == taskIndex && (task.Deadline == null || task.Deadline.Length == 0))
                    {
                        selectedTask = task;
                        break;
                    }
                }

                if (selectedTask == null)
                {
                    Console.WriteLine("You do not have the task with the given index. Please try again.");
                    EnterToContinue();
                    continue;
                }
                break;

            } while (true);
            //Ask the user to enter the deadline for the selected task. It must be step by step. First, ask the user day , then month, and finally year. Every question have do while 

            int day;
            int month;
            int year;
            do
            {
                //Day
                Console.Write("Enter the day of the deadline: ");
                string dayString = Console.ReadLine();
                if (!int.TryParse(dayString, out day))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                if (day < 1 || day > 31)
                {
                    Console.WriteLine("Invalid day. Please try again.");
                    EnterToContinue();
                    continue;
                }

                break;
            } while (day < 1 || day > 31);

            do
            {
                //Month
                Console.Write("Enter the month of the deadline: ");
                string monthString = Console.ReadLine();
                if (!int.TryParse(monthString, out month))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                if (month < 1 || month > 12)
                {
                    Console.WriteLine("Invalid month. Please try again.");
                    EnterToContinue();
                    continue;
                }

                break;
            } while (month < 1 || month > 12);

            do
            {
                //Year
                Console.Write("Enter the year of the deadline: ");
                string yearString = Console.ReadLine();
                if (!int.TryParse(yearString, out year))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                //İf the year is less than the current year, give an error message
                if (year < DateTime.Now.Year)
                {
                    Console.WriteLine("Invalid year. Please try again.");
                    EnterToContinue();
                    continue;
                }

                break;
            } while (year < DateTime.Now.Year);

            return SetDeadlineForTask(selectedTask, $"{day}/{month}/{year}", pathFileTasks);

        }
        /**
         * @brief Sets a deadline for a specific task and updates the task data file.
         * @param task The task for which the deadline is being set.
         * @param date The deadline date in string format.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the deadline is set successfully, false otherwise.
         */
        public bool SetDeadlineForTask(Task task, string date, string pathFileTasks)
        {
            List<Task> tasks = LoadAllTasks(pathFileTasks);

            foreach (Task taskItem in tasks)
            {
                if (taskItem.Id == task.Id)
                {
                    taskItem.Deadline = date;
                    break;
                }
            }

            string updatedJsonString = JsonSerializer.Serialize(tasks);
            File.WriteAllText(pathFileTasks, updatedJsonString);

            Console.WriteLine("Deadline set successfully.");
            EnterToContinue();
            return true;
        }

        /**
         * @brief Displays the menu for setting reminders for tasks and processes user input.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the reminder setting process is completed, false otherwise.
         */
        public bool SetRemindersMenu(string pathFileTasks)
        {
            Task selectedTask = null;
            do
            {
                ClearScreen();
                List<Task> ownedTasks = LoadOwnedTasks(pathFileTasks);

                if (ownedTasks.Count == 0)
                {
                    Console.WriteLine("No tasks found.");
                    EnterToContinue();
                    return false;
                }

                PrintOwnedTasksToConsole(pathFileTasks);
                //Get input from user. If the user enters "exit", return false
                Console.Write("Select a task to set reminder. You can choose tasks that have a due date. (Type \"exit\" to exit): ");
                int taskIndex;
                string taskIndexString = Console.ReadLine();

                if (taskIndexString.ToLower() == "exit")
                {
                    return false;
                }

                if (!int.TryParse(taskIndexString, out taskIndex))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;

                }

                //Check if the task index is valid
                if (taskIndex < 0)
                {
                    Console.WriteLine("Invalid task index. Please try again.");
                    EnterToContinue();
                    continue;
                }
                foreach (Task task in ownedTasks)
                {
                    if (task.Id == taskIndex && task.Deadline != null && task.Deadline.Length > 0)
                    {
                        selectedTask = task;
                        break;
                    }
                }

                if (selectedTask == null)
                {
                    Console.WriteLine("You do not have the task with the given index or the task does not have a due date. Please try again.");
                    EnterToContinue();
                    continue;
                }
                break;
            } while (true);
            return SetReminders(selectedTask, pathFileTasks);
        }
        /**
         * @brief Sets reminders for a specific task and updates the task data file.
         * @param selectedTask The task for which the reminders are being set.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the reminders are set successfully, false otherwise.
         */
        public bool SetReminders(Task selectedTask, string pathFileTasks)
        {
            int choice;
            do
            {
                ClearScreen();
                PrintTasksToConsole(new List<Task>() { selectedTask });
                PrintSetRemindersMenu();
                //Write task details to the console
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Thread.Sleep(30);
                        Console.WriteLine($"\nReminder for the task \"{selectedTask.TaskName}\"\n");
                        break;

                    case 2:
                        Thread.Sleep(50);
                        Console.WriteLine($"\nReminder for the task \"{selectedTask.TaskName}\"\n");
                        break;

                    case 3:
                        Thread.Sleep(100);
                        Console.WriteLine($"\nReminder for the task \"{selectedTask.TaskName}\"\n");
                        break;
                    case 4:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        continue;
                }

                EnterToContinue();
                return true;
            } while (true);
        }
        /**
         * @brief Displays the reminder system menu and processes user input for setting or managing reminders.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the reminder system process is completed, false otherwise.
         */
        public bool ReminderSystemMenu(string pathFileTasks)
        {
            int choice;

            while (true)
            {
                ClearScreen();
                PrintReminderSystemMenu();

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        SetRemindersMenu(pathFileTasks);
                        break;

                    case 2:
                        ReminderSettingsMenu(pathFileTasks);
                        break;

                    case 3:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }
        /**
         * @brief Displays the reminder settings menu and processes user input for configuring reminder settings.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the reminder settings are configured successfully, false otherwise.
         */
        public bool ReminderSettingsMenu(string pathFileTasks)
        {
            int choice;

            while (true)
            {
                PrintReminderSettingsMenu();
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Email reminder settings saved.");
                        EnterToContinue();
                        return true;

                    case 2:
                        Console.WriteLine("SMS reminder settings saved.");
                        EnterToContinue();
                        return true;


                    case 3:
                        Console.WriteLine("In-App Notification reminder settings saved.");
                        EnterToContinue();
                        return true;
                    case 4:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }

            }
        }
        /**
         * @brief Displays the task prioritization menu and processes user input for prioritizing tasks.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the task prioritization process is completed, false otherwise.
         */

        public bool TaskPrioritizationMenu(string pathFileTasks)
        {
            int choice;

            while (true)
            {
                ClearScreen();
                PrintTaskPrioritizationMenu();

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        PrioritizeTasks(pathFileTasks);
                        break;

                    case 2:
                        return false;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        EnterToContinue();
                        break;
                }
            }
        }
        /**
         * @brief Displays the menu for prioritizing a specific task and processes user input.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the task is prioritized successfully, false otherwise.
         */
        public bool PrioritizeTasks(string pathFileTasks)
        {
            ClearScreen();
            List<Task> tasks = LoadOwnedTasks(pathFileTasks);
            byte priority;

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found.");
                EnterToContinue();
                return false;
            }

            //Sort tasks by priority
            tasks = tasks.OrderBy(task => task.Priority).ToList();
            PrintTasksToConsole(tasks);

            Task selectedTask = null;
            do
            {
                //Type exit to exit
                Console.Write("Select a task to prioritize (Type \"exit\" to exit): ");

                int taskId;
                string taskIdString = Console.ReadLine();

                if (taskIdString.ToLower() == "exit")
                {
                    return false;
                }

                if (!int.TryParse(taskIdString, out taskId))
                {
                    HandleInputError();
                    EnterToContinue();
                    continue;
                }

                foreach (var task1 in tasks)
                {
                    if (task1.Id == taskId)
                    {
                        selectedTask = task1;
                        break;
                    }
                }

                if (selectedTask == null)
                {
                    Console.WriteLine("Invalid task id. Please try again.");
                    EnterToContinue();
                    continue;
                }

                do
                {
                    Console.Write("Enter the priority of the task: ");
                    string priorityString = Console.ReadLine();
                    if (!byte.TryParse(priorityString, out priority))
                    {
                        HandleInputError();
                        EnterToContinue();
                        continue;
                    }

                    if (priority < 1 || priority > 5)
                    {
                        Console.WriteLine("Invalid priority. Please try again (1-5).");
                        EnterToContinue();
                        continue;
                    }

                    break;
                } while (true);

                selectedTask.Priority = priority;
                break;
            } while (true);

            return SetPriorityToTask(selectedTask, pathFileTasks);
        }
        /**
         * @brief Sets the priority for a specific task and updates the task data file.
         * @param task The task for which the priority is being set.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if the priority is set successfully, false otherwise.
         */
        public bool SetPriorityToTask(Task task, string pathFileTasks)
        {
            List<Task> tasks = LoadAllTasks(pathFileTasks);

            foreach (Task taskItem in tasks)
            {
                if (taskItem.Id == task.Id)
                {
                    taskItem.Priority = task.Priority;
                    break;
                }
            }

            string updatedJsonString = JsonSerializer.Serialize(tasks);
            File.WriteAllText(pathFileTasks, updatedJsonString);

            Console.WriteLine("Task prioritized successfully.");
            EnterToContinue();
            return true;
        }


        /**
 * @brief Prints the categories to the console.
 * @param pathFileCategories Path to the file containing category data.
 * @return Returns true after printing the categories.
 */
        public bool PrintCategoriesToConsole(string pathFileCategories)
        {
            List<Category> categories = LoadAllCategories(pathFileCategories);

            Console.WriteLine("Categories\n");
            Console.WriteLine("----------------------------");
            Console.WriteLine("| ID    | Category Name    |");
            Console.WriteLine("----------------------------");

            foreach (Category category in categories)
            {
                string categoryId = category.Id.ToString().PadRight(5); // ID alanını 6 karakter genişliğine getir
                string categoryName = category.CategoryName.PadRight(16); // Category Name alanını 25 karakter genişliğine getir

                Console.WriteLine($"| {categoryId} | {categoryName} |");
                Console.WriteLine("----------------------------");
            }
            Console.WriteLine();

            return true;
        }
        /**
         * @brief Prints the tasks to the console.
         * @param tasks List of tasks to be printed.
         * @return Returns true if tasks are printed successfully, false otherwise.
         */
        public bool PrintTasksToConsole(List<Task> tasks)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found.");
                EnterToContinue();
                return false;
            }

            Console.WriteLine("Tasks\n");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Task Id   | Task Name        | Task Description      | Deadline   | Category         | Priority   | Cost  |");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------");

            foreach (Task task in tasks)
            {

                string taskId = task.Id.ToString().PadRight(9);
                string taskName = task.TaskName.PadRight(16);
                string taskDescription = task.TaskDescription.PadRight(21);
                string deadline = !string.IsNullOrEmpty(task.Deadline) ? task.Deadline.PadRight(10) : "-".PadRight(10);
                string category = task.Category != null ? task.Category.CategoryName.PadRight(16) : "-".PadRight(16);
                string priority = task.Priority != null ? task.Priority.ToString().PadRight(10) : "-".PadRight(10);
                string cost = task.Cost.ToString().PadRight(5);

                Console.WriteLine($"| {taskId} | {taskName} | {taskDescription} | {deadline} | {category} | {priority} | {cost} |");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            }
            Console.WriteLine();

            return true;
        }
        /**
         * @brief Prints the tasks owned by the logged-in user to the console.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns true if tasks are printed successfully, false otherwise.
         */
        public bool PrintOwnedTasksToConsole(string pathFileTasks)
        {
            List<Task> tasks = LoadOwnedTasks(pathFileTasks);
            return PrintTasksToConsole(tasks);
        }



        /**
         * @brief Loads all tasks from the specified file.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns a list of all tasks.
         */
        public List<Task> LoadAllTasks(string pathFileTasks)
        {
            List<Task> tasks = new List<Task>();

            if (File.Exists(pathFileTasks))
            {
                string jsonString = File.ReadAllText(pathFileTasks);
                tasks = JsonSerializer.Deserialize<List<Task>>(jsonString);
            }

            return tasks;
        }
        /**
         * @brief Loads all users from the specified file.
         * @param pathFileUsers Path to the file containing user data.
         * @return Returns a list of all users.
         */
        public List<User> LoadAllUsers(string pathFileUsers)
        {
            List<User> users = new List<User>();

            if (File.Exists(pathFileUsers))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(pathFileUsers, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        User user = new User
                        {
                            Id = reader.ReadInt32(),
                            Email = reader.ReadString(),
                            Password = reader.ReadString()
                        };

                        users.Add(user);
                    }
                }
            }

            return users;
        }
        /**
         * @brief Loads tasks owned by the logged-in user from the specified file.
         * @param pathFileTasks Path to the file containing task data.
         * @return Returns a list of tasks owned by the logged-in user.
         */
        public List<Task> LoadOwnedTasks(string pathFileTasks)
        {
            List<Task> tasks = LoadAllTasks(pathFileTasks);

            //Filter tasks according to the owner. Use for loop 
            List<Task> ownedTasks = new List<Task>();
            foreach (Task task in tasks)
            {
                if (task.Owner.Id == LoggedInUser.Id)
                {
                    ownedTasks.Add(task);
                }
            }

            return ownedTasks;
        }
        /**
         * @brief Loads all categories from the specified file.
         * @param pathFileCategories Path to the file containing category data.
         * @return Returns a list of all categories.
         */
        public List<Category> LoadAllCategories(string pathFileCategories)
        {
            List<Category> categories = new List<Category>();

            if (File.Exists(pathFileCategories))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(pathFileCategories, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        Category category = new Category
                        {
                            Id = reader.ReadInt32(),
                            CategoryName = reader.ReadString()
                        };

                        categories.Add(category);
                    }
                }
            }
            else
            {
                categories = new List<Category>
                {
                    new Category { Id = 1, CategoryName = "Work" },
                    new Category { Id = 2, CategoryName = "Personal" },
                    new Category { Id = 3, CategoryName = "Shopping" },
                    new Category { Id = 4, CategoryName = "Study" },
                    new Category { Id = 5, CategoryName = "Health" },
                    new Category { Id = 6, CategoryName = "Sport" },
                    new Category { Id = 7, CategoryName = "Diet" }
                };

                //Write categories to the file as category.bin
                using (BinaryWriter writer = new BinaryWriter(File.Open(pathFileCategories, FileMode.Create)))
                {
                    foreach (Category category in categories)
                    {
                        writer.Write(category.Id);
                        writer.Write(category.CategoryName);
                    }
                }
                string newPath = pathFileCategories.Replace(".bin", "");
                SaveCategoriesWithHuffman(newPath, categories);
            }

            // Encode and save categories with Huffman Coding
            return categories;
        }

        /**
       * @brief Gets a new task ID by incrementing the highest existing task ID.
       * @param pathFileTasks Path to the file containing task data.
       * @return Returns a new task ID.
       */
        public int GetNewTaskId(string pathFileTasks)
        {
            List<Task> tasks = LoadAllTasks(pathFileTasks);
            if (tasks.Count == 0)
            {
                return 1;
            }
            return tasks[tasks.Count - 1].Id + 1;
        }
        /**
         * @brief Gets a new user ID by incrementing the highest existing user ID.
         * @param pathFileUsers Path to the file containing user data.
         * @return Returns a new user ID.
         */
        public int GetNewUserId(string pathFileUsers)
        {
            List<User> users = LoadAllUsers(pathFileUsers);
            if (users.Count == 0)
            {
                return 1;
            }
            return users[users.Count - 1].Id + 1;
        }


        /**
         * @brief Prints the set reminders menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintSetRemindersMenu()
        {
            Console.WriteLine("Set Reminders\n\n");
            Console.WriteLine("1. 3 seconds");
            Console.WriteLine("2. 5 seconds");
            Console.WriteLine("3. 15 seconds");
            Console.WriteLine("4. Return to Reminder System Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the main menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintMainMenu()
        {
            Console.WriteLine("Welcome To Personal Library System\n\n");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Guest Mode");
            Console.WriteLine("4. Exit Program");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the task prioritization menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintTaskPrioritizationMenu()
        {
            ClearScreen();
            Console.WriteLine("Task Prioritization Menu\n\n");
            Console.WriteLine("1. Prioritize Tasks");
            Console.WriteLine("2. Return to User Operations Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the reminder settings menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintReminderSettingsMenu()
        {
            ClearScreen();
            Console.WriteLine("Reminder Settings Menu\n\n");
            Console.WriteLine("1. Email");
            Console.WriteLine("2. SMS");
            Console.WriteLine("3. In-App Notification");
            Console.WriteLine("4. Return to Reminder System Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the deadline setting menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintDeadlineSettingMenu()
        {
            ClearScreen();
            Console.WriteLine("Deadline Setting Menu\n\n");
            Console.WriteLine("1. Set Deadline for a Task");
            Console.WriteLine("2. Return to User Operations Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the task menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintTaskMenu()
        {
            ClearScreen();
            Console.WriteLine("Task  Menu\n\n");
            Console.WriteLine("1. Create a Task");
            Console.WriteLine("2. Categorize Task");
            Console.WriteLine("3. Similar Tasks");
            Console.WriteLine("4. Find Users add tasks like you");
            Console.WriteLine("5. The shortest path between Tasks");
            Console.WriteLine("6. Optimize Budget");
            Console.WriteLine("7 Shortest Path Between Tasks");
            Console.WriteLine("8. Return to User Operations Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the user menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintUserMenu()
        {
            ClearScreen();
            Console.WriteLine("Welcome to User Operations\n\n");
            Console.WriteLine("1. Task Creation");
            Console.WriteLine("2. Deadline Setting");
            Console.WriteLine("3. Reminder System");
            Console.WriteLine("4. Task Prioritization");
            Console.WriteLine("5. Return to Main Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the guest menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintGuestMenu()
        {
            ClearScreen();
            Console.WriteLine("Guest Operations\n\n");
            Console.WriteLine("1. View Categories");
            Console.WriteLine("2. Return to Main Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }
        /**
         * @brief Prints the reminder system menu to the console.
         * @return Returns true after printing the menu.
         */
        public bool PrintReminderSystemMenu()
        {
            ClearScreen();
            Console.WriteLine("Reminder System Menu\n\n");
            Console.WriteLine("1. Set Reminders");
            Console.WriteLine("2. Reminder Settings");
            Console.WriteLine("3. Return to User Operations Menu");
            Console.Write("Please enter a number to select: ");
            return true;
        }

        //Algorithms 
        //LCS
        // Method to find the longest common subsequence of two strings.
        /**
 * @brief Calculates the longest common subsequence (LCS) of two strings.
 * @param firstString The first string.
 * @param secondString The second string.
 * @return The length of the LCS.
 */
        private int CalculateLongestCommonSubsequence(string firstString, string secondString)
        {
            int[,] table = new int[firstString.Length + 1, secondString.Length + 1];

            // Fill the table for LCS calculation
            for (int i = 0; i <= firstString.Length; i++)
            {
                for (int j = 0; j <= secondString.Length; j++)
                {
                    if (i == 0 || j == 0)
                        table[i, j] = 0;
                    else if (firstString[i - 1] == secondString[j - 1])
                        table[i, j] = table[i - 1, j - 1] + 1;
                    else
                        table[i, j] = Math.Max(table[i - 1, j], table[i, j - 1]);
                }
            }

            return table[firstString.Length, secondString.Length];
        }

        // BFS
        /**
         * @brief Finds similar users by task category using BFS.
         * @param tasks Dictionary of tasks indexed by task ID.
         * @param users Dictionary of users indexed by user ID.
         * @param startTaskId The ID of the starting task.
         * @return A list of similar users.
         */
        public List<User> FindSimilarUsersByTaskCategoryBFS(Dictionary<int, Task> tasks, Dictionary<int, User> users, int startTaskId)
        {
            if (!tasks.ContainsKey(startTaskId))
            {
                Console.WriteLine("Task not found.");
                return new List<User>();
            }

            var startTask = tasks[startTaskId];
            var startCategory = startTask.Category;

            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            List<User> similarUsers = new List<User>();

            queue.Enqueue(startTaskId);
            visited.Add(startTaskId);

            while (queue.Count > 0)
            {
                int currentTaskId = queue.Dequeue();
                Task currentTask = tasks[currentTaskId];

                // Eğer bu görevi ekleyen kullanıcı daha önce listeye eklenmediyse, listeye ekle
                if (!similarUsers.Any(u => u.Id == currentTask.Owner.Id))
                {
                    similarUsers.Add(users[currentTask.Owner.Id]);
                }

                foreach (var taskPair in tasks)
                {
                    int taskId = taskPair.Key;
                    Task task = taskPair.Value;

                    if (!visited.Contains(taskId) && task.Category?.Id == startCategory?.Id)
                    {
                        visited.Add(taskId);
                        queue.Enqueue(taskId);
                    }
                }
            }

            return similarUsers;
        }
        //DFS
        /**
         * @brief Finds similar users by task category using DFS.
         * @param tasks Dictionary of tasks indexed by task ID.
         * @param users Dictionary of users indexed by user ID.
         * @param startTaskId The ID of the starting task.
         * @return A list of similar users.
         */
        public List<User> FindSimilarUsersByTaskCategoryDFS(Dictionary<int, Task> tasks, Dictionary<int, User> users, int startTaskId)
        {
            if (!tasks.ContainsKey(startTaskId))
            {
                Console.WriteLine("Task not found.");
                return new List<User>();
            }

            var startTask = tasks[startTaskId];
            var startCategory = startTask.Category;

            Stack<int> stack = new Stack<int>();
            HashSet<int> visited = new HashSet<int>();
            List<User> similarUsers = new List<User>();

            stack.Push(startTaskId);
            visited.Add(startTaskId);

            while (stack.Count > 0)
            {
                int currentTaskId = stack.Pop();
                Task currentTask = tasks[currentTaskId];

                // Eğer bu görevi ekleyen kullanıcı daha önce listeye eklenmediyse, listeye ekle
                if (!similarUsers.Any(u => u.Id == currentTask.Owner.Id))
                {
                    similarUsers.Add(users[currentTask.Owner.Id]);
                }

                foreach (var taskPair in tasks)
                {
                    int taskId = taskPair.Key;
                    Task task = taskPair.Value;

                    if (!visited.Contains(taskId) && task.Category?.Id == startCategory?.Id)
                    {
                        visited.Add(taskId);
                        stack.Push(taskId);
                    }
                }
            }

            return similarUsers;
        }
        //Hufman
        /**
         * @brief Saves categories to a file using Huffman encoding.
         * @param path The file path to save the encoded data.
         * @param categories The list of categories to encode.
         */
        public void SaveCategoriesWithHuffman(string path, List<Category> categories)
        {
            string data = string.Join("\n", categories.Select(c => $"{c.Id}:{c.CategoryName}"));
            var codes = Encode(data);
            string encoded = string.Join("", data.Select(c => codes[c]));
            File.WriteAllText(path + ".huff", encoded);
        }
        /**
         * @brief Encodes the given data using Huffman encoding.
         * @param data The data to encode.
         * @return A dictionary mapping characters to their Huffman codes.
         */
        public Dictionary<char, string> Encode(string data)
        {
            var frequencies = data.GroupBy(c => c)
                .Select(group => new Huffman { Character = group.Key, Frequency = group.Count() })
                .ToList();
            while (frequencies.Count > 1)
            {
                List<Huffman> orderedNodes = frequencies.OrderBy(node => node.Frequency).ToList();
                if (orderedNodes.Count >= 2)
                {
                    List<Huffman> taken = orderedNodes.Take(2).ToList();
                    Huffman parent = new Huffman
                    {
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };
                    frequencies.Remove(taken[0]);
                    frequencies.Remove(taken[1]);
                    frequencies.Add(parent);
                }
            }

            var rootNode = frequencies.FirstOrDefault();
            var codes = new Dictionary<char, string>();
            GenerateCodes(rootNode, "", codes);
            return codes;
        }
        /**
         * @brief Generates Huffman codes for the given node.
         * @param node The current Huffman node.
         * @param code The current code.
         * @param codes The dictionary to store the generated codes.
         */
        public void GenerateCodes(Huffman node, string code, Dictionary<char, string> codes)
        {
            if (node.Left == null && node.Right == null)
            {
                codes.Add(node.Character, code);
                return;
            }
            GenerateCodes(node.Left, code + "0", codes);
            GenerateCodes(node.Right, code + "1", codes);
        }
        /**
         * @brief Adds an edge to the list of edges for the Ford-Fulkerson algorithm.
         * @param edges The list of edges.
         * @param from The source vertex of the edge.
         * @param to The destination vertex of the edge.
         * @param capacity The capacity of the edge.
         */


        //Max-Flow and Min-Cut Ford-Fulkerson Algorithm
        private void AddEdge(List<Edge> edges, int from, int to, double capacity)
        {
            edges.Add(new Edge { From = from, To = to, Capacity = capacity, Flow = 0 });
            edges.Add(new Edge { From = to, To = from, Capacity = 0, Flow = 0 }); // reverse edge
        }
        /**
         * @brief Finds the maximum flow in a flow network using the Ford-Fulkerson algorithm.
         * @param edges The list of edges.
         * @param parent The parent array for BFS.
         * @param visited The visited array for BFS.
         * @param source The source vertex.
         * @param sink The sink vertex.
         * @return The maximum flow.
         */
        private double FordFulkerson(List<Edge> edges, int[] parent, bool[] visited, int source, int sink)
        {
            double maxFlow = 0;
            while (BFS(edges, parent, visited, source, sink))
            {
                double pathFlow = double.MaxValue;
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    Edge edge = edges.Find(e => e.From == u && e.To == v);
                    pathFlow = Math.Min(pathFlow, edge.Capacity - edge.Flow);
                }

                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    Edge edge = edges.Find(e => e.From == u && e.To == v);
                    edge.Flow += pathFlow;
                    Edge reverseEdge = edges.Find(e => e.From == v && e.To == u);
                    reverseEdge.Flow -= pathFlow;
                }

                maxFlow += pathFlow;
            }
            return maxFlow;
        }
        /**
         * @brief Performs BFS for the Ford-Fulkerson algorithm.
         * @param edges The list of edges.
         * @param parent The parent array for BFS.
         * @param visited The visited array for BFS.
         * @param source The source vertex.
         * @param sink The sink vertex.
         * @return True if there is a path from source to sink, false otherwise.
         */
        private bool BFS(List<Edge> edges, int[] parent, bool[] visited, int source, int sink)
        {
            Array.Fill(visited, false);
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(source);
            visited[source] = true;
            parent[source] = -1;

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();
                foreach (Edge edge in edges.Where(e => e.From == u && !visited[e.To] && e.Capacity > e.Flow))
                {
                    parent[edge.To] = u;
                    visited[edge.To] = true;
                    queue.Enqueue(edge.To);
                    if (edge.To == sink) return true;
                }
            }
            return false;
        }
        /**
         * @brief Initializes the graph for the max-flow algorithms.
         * @param tasks The list of tasks.
         * @param edges The list of edges.
         * @param source The source vertex.
         * @param sink The sink vertex.
         */
        private void InitializeGraph(List<Task> tasks, List<Edge> edges, int source, int sink)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                AddEdge(edges, source, i + 1, tasks[i].Cost);
                AddEdge(edges, i + 1, sink, tasks[i].Cost);
            }
        }
        /**
         * @brief Finds the maximum flow in a flow network using the Edmonds-Karp algorithm.
         * @param edges The list of edges.
         * @param parent The parent array for BFS.
         * @param visited The visited array for BFS.
         * @param source The source vertex.
         * @param sink The sink vertex.
         * @return The maximum flow.
         */
        //Edmonds-Karp Algorithm
        private double EdmondsKarp(List<Edge> edges, int[] parent, bool[] visited, int source, int sink)
        {
            double maxFlow = 0;
            while (BFS(edges, parent, visited, source, sink))
            {
                double pathFlow = double.MaxValue;
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    Edge edge = edges.FirstOrDefault(e => e.From == u && e.To == v);
                    if (edge != null)
                    {
                        pathFlow = Math.Min(pathFlow, edge.Capacity - edge.Flow);
                    }
                }

                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    Edge edge = edges.FirstOrDefault(e => e.From == u && e.To == v);
                    Edge reverseEdge = edges.FirstOrDefault(e => e.From == v && e.To == u);
                    if (edge != null && reverseEdge != null)
                    {
                        edge.Flow += pathFlow;
                        reverseEdge.Flow -= pathFlow;
                    }
                }

                maxFlow += pathFlow;
            }
            return maxFlow;
        }
        /**
         * @brief Finds the maximum flow in a flow network using Dinic's algorithm.
         * @param tasks The list of tasks.
         * @param edges The list of edges.
         * @param level The level array for the level graph.
         * @param parent The parent array for BFS.
         * @param source The source vertex.
         * @param sink The sink vertex.
         * @return The maximum flow.
         */
        //Dinic's Algorithm
        private double DinicsAlgorithm(List<Task> tasks, List<Edge> edges, int[] level, int[] parent, int source, int sink)
        {
            // Implement Dinic's algorithm using level graph and blocking flow
            double maxFlow = 0;
            while (BuildLevelGraph(edges, level, source, sink))
            {
                int flow;
                do
                {
                    flow = SendFlow(source, int.MaxValue, sink, edges, parent, level);
                    maxFlow += flow;
                } while (flow > 0);
            }
            return maxFlow;
        }
        /**
         * @brief Builds the level graph for Dinic's algorithm.
         * @param edges The list of edges.
         * @param level The level array for the level graph.
         * @param source The source vertex.
         * @param sink The sink vertex.
         * @return True if there is a level graph from source to sink, false otherwise.
         */




