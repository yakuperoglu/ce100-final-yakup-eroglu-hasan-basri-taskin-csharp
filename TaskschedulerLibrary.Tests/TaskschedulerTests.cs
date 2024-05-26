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
    }
}
