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



