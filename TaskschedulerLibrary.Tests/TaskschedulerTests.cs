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

    }
}
