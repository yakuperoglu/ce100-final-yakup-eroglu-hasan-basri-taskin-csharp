using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace TaskschedulerLibrary.Tests
{
    public class TaskschedulerTests
    {
        private string testFilePathUsers = "test_users.bin";
        private string testFilePathTasks = "test_tasks.bin";
        private string testFilePathCategories = "test_categories.bin";
        //Craete a variable TaskScheduler
        private Taskscheduler taskScheduler;

        [Fact]
        public void MainMenu_InvalidInput_ShouldHandleInputError()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("invalid\n4\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            int result = taskScheduler.MainMenu(testFilePathUsers, testFilePathTasks, testFilePathCategories);

            // Assert
            var outputString = output.ToString();
            Assert.Contains("Only enter numerical value", outputString);
            Assert.Contains("Exit Program", outputString);
            Assert.Equal(0, result);
        }
        [Fact]
        public void MainMenu_InvalidChoice_ShouldDisplayInvalidChoiceMessage()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("5\n4\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            int result = taskScheduler.MainMenu("pathFileUsers.txt", "pathFileTasks.txt", "pathFileCategories.txt");

            // Assert
            var outputString = output.ToString();
            Assert.Contains("Invalid choice. Please try again.", outputString);
            Assert.Contains("Exit Program", outputString);
            Assert.Equal(0, result);
        }
        [Fact]
        public void MainMenu_ShouldEnterEveryCaseAndExit()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("2\nqwe\nqwe\n1\nqwe\nqwe\n12345\n1\nýnvalýd\nýnvalýd\n123456\n1\nqwe\nqwe\n123456\n5\n3\n2\n4\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            int result = taskScheduler.MainMenu("pathFileUsers.txt", "pathFileTasks.txt", "pathFileCategories.txt");

            // Assert
            Assert.Equal(0, result);
        }
        [Fact]
        public void GuestMenu_ShouldEnterEveryCaseAndExit()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("Invalýd\n1\n3\n2\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.GuestOperation(testFilePathCategories);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserOperationsMenu_InvalidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("Invalýd\n9\n5\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.UserOperationsMenu(testFilePathTasks, testFilePathCategories, testFilePathUsers);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void UserOperationsMenu_ShouldEnterEveryCaseAndExit()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("1\n8\n2\n2\n3\n3\n4\n2\n5\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.UserOperationsMenu(testFilePathTasks, testFilePathCategories, testFilePathUsers);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void TaskMenu_InvalidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("Invalýd\n9\n8\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.TaskMenu(testFilePathTasks, testFilePathCategories, testFilePathUsers);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void TaskMenu_ShouldEnterEveryCaseAndExit()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("1\nqwe\nqwe\n13\n2\n1\n1\n3\n4\n5\n6\n132\n1\n7\n132\n8\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.TaskMenu(testFilePathTasks, testFilePathCategories, testFilePathUsers);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void CreateTask_ValidInputsWithFileExist_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            Task newTask = new Task()
            {
                Id = 1,
                TaskDescription = "Deneme",
                TaskName = "Deneme",
                Category = new Category { Id = 1, CategoryName = "Work" },
                Cost = 123,
                Deadline = "21/11/2002",
                Owner = new User { Id = 1, Email = "test1", Password = "test1" },
            };

            bool result = taskScheduler.CreateTask(newTask, testFilePathTasks);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void CategorizeTaskMenu_NoOwnedTasks_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            bool result = taskScheduler.CategorizeTaskMenu(testFilePathTasks, testFilePathCategories);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CategorizeTaskMenu_InvalidInputAndTypeExit_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("Invalid\n-1\nexit\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.CategorizeTaskMenu(testFilePathTasks, testFilePathCategories);

            Assert.False(result);
        }
        [Fact]
        public void CategorizeTaskMenu_HighSelectedTaskIndex_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("1111\nexit\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.CategorizeTaskMenu(testFilePathTasks, testFilePathCategories);

            Assert.False(result);
        }
        [Fact]
        public void CategorizeTaskMenu_CategorySectionInvalidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("11\nInvalid\n-1\nexit\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.CategorizeTaskMenu(testFilePathTasks, testFilePathCategories);

            Assert.False(result);
        }
        [Fact]
        public void CategorizeTaskMenu_HighSelectedCategoryIndex_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("11\n823\nexit\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.CategorizeTaskMenu(testFilePathTasks, testFilePathCategories);

            Assert.False(result);
        }

        [Fact]
        public void FindSimilarTasksMenu_HighSelectedCategoryIndex_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("11\n823\nexit\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.FindSimilarTasksMenu(testFilePathTasks);

            Assert.True(result);
        }
        [Fact]
        public void FindSimilarUsersByTaskCategoryMenu_InvalidInputs_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("Invalid\n3\n1\n");
            Console.SetIn(input);
            // Act
            bool result = taskScheduler.FindSimilarUsersByTaskCategoryMenu(testFilePathUsers, testFilePathTasks);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void FindSimilarUsersByTaskCategoryMenu_InsufficientUsersOrTasks_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            // Act
            bool result = taskScheduler.FindSimilarUsersByTaskCategoryMenu(testFilePathUsers, testFilePathTasks);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void FindSimilarUsersByTaskCategoryMenu_DFS_ShouldReturnExpectedResults()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("2\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.FindSimilarUsersByTaskCategoryMenu(testFilePathUsers, testFilePathTasks);

            Assert.True(result);
        }
        [Fact]
        public void FindSimilarUsersByTaskCategoryMenu_BFS_ShouldReturnExpectedResults()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("1\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.FindSimilarUsersByTaskCategoryMenu(testFilePathUsers, testFilePathTasks);

            Assert.True(result);
        }

        [Fact]
        public void TheShortestPathBetweenTasks_EnoughTasks_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("1\n");
            Console.SetIn(input);

            // Act
            bool result = taskScheduler.TheShortestPathBetweenTasks(testFilePathTasks);

            Assert.True(result);
        }

        [Fact]
        public void OptimizeBudget_EdmondsKarp_ShouldEnter()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("2\n2\n");
            Console.SetIn(input);

            // Act
            taskScheduler.OptimizeBudget(testFilePathTasks);
        }
        [Fact]
        public void OptimizeBudget_DinicsAlgorithm_ShouldEnter()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("4\n4\n3\n");
            Console.SetIn(input);

            // Act
            taskScheduler.OptimizeBudget(testFilePathTasks);
        }
        [Fact]
        public void DeadlineSettingMenu_InvalidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("Invalýd\n9\n2\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.DeadlineSettingMenu(testFilePathTasks);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void DeadlineSettingMenu_SetDeadline_DoesntHaveEnoughTask_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("1\n2\n");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.DeadlineSettingMenu(testFilePathTasks);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SetDeadlineForTaskMenu_InvalidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("Invalid\n-1\nexit");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.SetDeadlineForTaskMenu(testFilePathTasks);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SetDeadlineForTaskMenu_ValidInputs_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("12\n10\n10\n2222");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.SetDeadlineForTaskMenu(testFilePathTasks);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void SetDeadlineForTaskMenu_InvalidInputs_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("12\nqwe\n122\n10\nqwe\n1222\n12\nqwe\n2000\n2222");
            Console.SetIn(input);
            var output = new StringWriter();
            Console.SetOut(output);

            bool result = taskScheduler.SetDeadlineForTaskMenu(testFilePathTasks);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void SetRemindersMenu_DoesntHaveEnoughTasks_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            bool result = taskScheduler.SetRemindersMenu(testFilePathTasks);


            // Assert
            Assert.False(result);
        }
        [Fact]
        public void SetRemindersMenu_InvalidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("Invalid\n-1\nExit\n");
            Console.SetIn(input);

            bool result = taskScheduler.SetRemindersMenu(testFilePathTasks);


            // Assert
            Assert.False(result);
        }
        [Fact]
        public void SetRemindersMenu_ValidInputs_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();
            var input = new StringReader("1\n4\n");
            Console.SetIn(input);

            bool result = taskScheduler.SetRemindersMenu(testFilePathTasks);


            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SetReminders_CaseOne_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var task = new Task
            {
                Id = 1,
                TaskName = "Task1",
                TaskDescription = "TaskDescription1",
                Category = new Category { Id = 1, CategoryName = "Work" },
                Owner = new User { Id = 1, Email = "test1", Password = "test1" },
                Deadline = DateTime.Now.AddDays(1).ToString(),
                Priority = 1,
                Cost = 5555
            };
            var input = new StringReader("Invalid\n5\n1\n");
            Console.SetIn(input);
            bool result = taskScheduler.SetReminders(task, testFilePathTasks);


            // Assert
            Assert.True(result);
        }
        [Fact]
        public void SetReminders_CaseTwo_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var task = new Task
            {
                Id = 1,
                TaskName = "Task1",
                TaskDescription = "TaskDescription1",
                Category = new Category { Id = 1, CategoryName = "Work" },
                Owner = new User { Id = 1, Email = "test1", Password = "test1" },
                Deadline = DateTime.Now.AddDays(1).ToString(),
                Priority = 1,
                Cost = 5555
            };
            var input = new StringReader("2\n");
            Console.SetIn(input);
            bool result = taskScheduler.SetReminders(task, testFilePathTasks);


            // Assert
            Assert.True(result);
        }
        [Fact]
        public void SetReminders_CaseThree_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var task = new Task
            {
                Id = 1,
                TaskName = "Task1",
                TaskDescription = "TaskDescription1",
                Category = new Category { Id = 1, CategoryName = "Work" },
                Owner = new User { Id = 1, Email = "test1", Password = "test1" },
                Deadline = DateTime.Now.AddDays(1).ToString(),
                Priority = 1,
                Cost = 5555
            };
            var input = new StringReader("3\n");
            Console.SetIn(input);
            bool result = taskScheduler.SetReminders(task, testFilePathTasks);


            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ReminderSystemMenu_EnterEveryCase_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("Invalid\n4\n2\n4\n1\nexit\n3\n");
            Console.SetIn(input);
            bool result = taskScheduler.ReminderSystemMenu(testFilePathTasks);


            // Assert
            Assert.False(result);
        }
        [Fact]
        public void ReminderSettingsMenu_CaseOne_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("Invalid\n6\n1\n");
            Console.SetIn(input);
            bool result = taskScheduler.ReminderSettingsMenu(testFilePathTasks);


            // Assert
            Assert.True(result);
        }
        [Fact]
        public void ReminderSettingsMenu_CaseTwo_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("2\n");
            Console.SetIn(input);
            bool result = taskScheduler.ReminderSettingsMenu(testFilePathTasks);


            // Assert
            Assert.True(result);
        }
        [Fact]
        public void ReminderSettingsMenu_CaseThree_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("3\n");
            Console.SetIn(input);
            bool result = taskScheduler.ReminderSettingsMenu(testFilePathTasks);


            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TaskPrioritizationMenu_Invalid_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();

            var input = new StringReader("Invalid\n4\n1\n2\n");
            Console.SetIn(input);
            bool result = taskScheduler.TaskPrioritizationMenu(testFilePathTasks);


            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PrioritizeTasks_Invalid_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("Invalid\nexit\n");
            Console.SetIn(input);
            bool result = taskScheduler.PrioritizeTasks(testFilePathTasks);


            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PrioritizeTasks_DoesntExistTask_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("13\nexit\n");
            Console.SetIn(input);
            bool result = taskScheduler.PrioritizeTasks(testFilePathTasks);


            // Assert
            Assert.False(result);
        }

        [Fact]
        public void PrioritizeTasks_ValidPriority_ShouldReturnTrue()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("1\nInvalid\n6\n4\n");
            Console.SetIn(input);
            bool result = taskScheduler.PrioritizeTasks(testFilePathTasks);


            // Assert
            Assert.True(result);
        }

        [Fact]
        public void PrintTasksToConsole_NoTasks_ShouldReturnFalse()
        {
            // Arrange
            SetStartup();
            List<Task> tasks = new List<Task>();

            bool result = taskScheduler.PrintTasksToConsole(tasks);


            // Assert
            Assert.False(result);
        }

        [Fact]
        public void FindSimilarUsersByTaskCategoryBFS_NoStartTaskId_ShouldReturnEmptyArray()
        {
            // Arrange
            SetStartup();
            Dictionary<int, Task> taskDictionary = new Dictionary<int, Task>();
            Dictionary<int, User> dictionary = new Dictionary<int, User>();

            var result = taskScheduler.FindSimilarUsersByTaskCategoryBFS(taskDictionary, dictionary, 123);


            // Assert
            Assert.Equal(result.Count, 0);
        }
        [Fact]
        public void FindSimilarUsersByTaskCategoryDFS_NoStartTaskId_ShouldReturnEmptyArray()
        {
            // Arrange
            SetStartup();
            Dictionary<int, Task> taskDictionary = new Dictionary<int, Task>();
            Dictionary<int, User> dictionary = new Dictionary<int, User>();

            var result = taskScheduler.FindSimilarUsersByTaskCategoryDFS(taskDictionary, dictionary, 123);


            // Assert
            Assert.Equal(result.Count, 0);
        }


        [Fact]
        public void AllocateResourcesBasedOnBudgetAndDeadline_ShouldEnterEveryCase()
        {
            // Arrange
            SetStartup();
            CreateTestData();

            var input = new StringReader("Invalid\n64\n");
            Console.SetIn(input);
            taskScheduler.AllocateResourcesBasedOnBudgetAndDeadline(testFilePathTasks);
        }


















        private void CreateTestUsers()
        {
            //Users
            var testUsers = new List<User>
            {
                new User { Id = 1, Email = "test1", Password = "test1" },
                new User { Id = 2, Email = "test2", Password = "test2" },
                new User { Id = 3, Email = "test3", Password = "test3" },
                new User { Id = 4, Email = "test4", Password = "test4" },
            };

            // Create a test file with users
            using (BinaryWriter writer = new BinaryWriter(File.Open(testFilePathUsers, FileMode.Create)))
            {
                foreach (var user in testUsers)
                {
                    writer.Write(user.Id);
                    writer.Write(user.Email);
                    writer.Write(user.Password);
                }
            }
        }
        private void CreateTestCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Work" },
                new Category { Id = 2, CategoryName = "Personal" },
                new Category { Id = 3, CategoryName = "Shopping" },
                new Category { Id = 4, CategoryName = "Study" },
                new Category { Id = 5, CategoryName = "Health" },
                new Category { Id = 6, CategoryName = "Sport" },
                new Category { Id = 7, CategoryName = "Diet" }
            };

            // Create a test file with categories
            using (BinaryWriter writer = new BinaryWriter(File.Open(testFilePathCategories, FileMode.Create)))
            {
                foreach (var category in categories)
                {
                    writer.Write(category.Id);
                    writer.Write(category.CategoryName);
                }
            }
        }
        private void CreateTestTasks()
        {
            //Load users
            var users = new List<User>();
            using (BinaryReader reader = new BinaryReader(File.Open(testFilePathUsers, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var user = new User
                    {
                        Id = reader.ReadInt32(),
                        Email = reader.ReadString(),
                        Password = reader.ReadString(),
                    };
                    users.Add(user);
                }
            }

            //Load Categories
            var categories = new List<Category>();
            using (BinaryReader reader = new BinaryReader(File.Open(testFilePathCategories, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var category = new Category
                    {
                        Id = reader.ReadInt32(),
                        CategoryName = reader.ReadString(),
                    };
                    categories.Add(category);
                }
            }

            //Tasks
            var tasks = new List<Task>
            {
                new Task
                {
                    Id = 1, TaskName = "Task1", TaskDescription = "TaskDescription1", Category = categories[0],
                    Owner = users[0], Deadline = DateTime.Now.AddDays(1).ToString(), Priority = 1, Cost = 5555
                },
                new Task
                {
                    Id = 2, TaskName = "Task2", TaskDescription = "TaskDescription2", Category = categories[1],
                    Owner = users[1], Deadline = DateTime.Now.AddDays(2).ToString(), Priority = 3, Cost = 12
                },
                new Task
                {
                    Id = 3, TaskName = "Task3", TaskDescription = "TaskDescription3", Category = categories[2],
                    Owner = users[2], Deadline = DateTime.Now.AddDays(3).ToString(), Priority = 2 ,Cost = 123
                },
                new Task
                {
                    Id = 4, TaskName = "Task4", TaskDescription = "TaskDescription4", Category = categories[3],
                    Owner = users[3], Deadline = DateTime.Now.AddDays(4).ToString(), Priority = 5,Cost = 11
                },
                new Task
                {
                    Id = 5, TaskName = "Task5", TaskDescription = "TaskDescription5", Category = categories[4],
                    Owner = users[0], Deadline = DateTime.Now.AddDays(5).ToString(), Priority = 1,Cost = 12
                },
                new Task
                {
                    Id = 6, TaskName = "Task6", TaskDescription = "TaskDescription6", Category = categories[5],
                    Owner = users[1], Deadline = DateTime.Now.AddDays(6).ToString(), Priority = 1,Cost = 12
                },
                new Task
                {
                    Id = 7, TaskName = "Task7", TaskDescription = "TaskDescription7", Category = categories[6],
                    Owner = users[2], Deadline = DateTime.Now.AddDays(7).ToString(), Priority = 4,Cost = 1231
                },
                new Task
                {
                    Id = 8, TaskName = "Task8", TaskDescription = "TaskDescription8", Category = categories[0],
                    Owner = users[3], Deadline = DateTime.Now.AddDays(8).ToString(), Priority = 3,Cost = 41
                },
                new Task
                {
                    Id = 9, TaskName = "Task9", TaskDescription = "TaskDescription9", Category = categories[1],
                    Owner = users[0], Deadline = DateTime.Now.AddDays(9).ToString(), Priority = 2,Cost = 61
                },
                new Task
                {
                    Id = 10, TaskName = "Task10", TaskDescription = "TaskDescription10", Category = categories[2],
                    Owner = users[1], Deadline = DateTime.Now.AddDays(10).ToString(), Priority = 1,Cost = 12
                },
                new Task
                {
                    Id = 11, TaskName = "Task11", TaskDescription = "TaskDescription11", Category = null,
                    Owner = users[0], Deadline = DateTime.Now.AddDays(10).ToString(), Priority = 1 ,Cost = 1231
                },
                new Task
                {
                    Id = 12, TaskName = "Task12", TaskDescription = "TaskDescription12", Category = null,
                    Owner = users[0], Deadline = null, Priority = 1 ,Cost = 1231
                },
            };
            ;
            // Create a test file with tasks
            string jsonStringUpdated = JsonSerializer.Serialize(tasks);
            File.WriteAllText(testFilePathTasks, jsonStringUpdated);


        }
        private void CreateTestData()
        {
            CreateTestUsers();
            CreateTestCategories();
            CreateTestTasks();
        }
        private void CleanupTestDataUser()
        {
            if (File.Exists(testFilePathUsers))
            {
                File.Delete(testFilePathUsers);
            }
        }
        private void CleanupTestDataTasks()
        {
            if (File.Exists(testFilePathTasks))
            {
                File.Delete(testFilePathTasks);
            }
        }
        private void CleanupTestDataCategories()
        {
            if (File.Exists(testFilePathCategories))
            {
                File.Delete(testFilePathCategories);
            }
        }
        private void CleanupTestData()
        {
            CleanupTestDataUser();
            CleanupTestDataTasks();
            CleanupTestDataCategories();
        }
        private void SetStartup()
        {
            CleanupTestData();
            taskScheduler = new Taskscheduler();
            taskScheduler.IsTestMode = true;
            taskScheduler.LoggedInUser = new User
            { Id = 1, Email = "test1", Password = "test1" };
        }
    }
}
