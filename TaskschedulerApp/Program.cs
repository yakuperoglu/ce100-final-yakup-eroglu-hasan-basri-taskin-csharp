/**
 * @class Program
 * @brief The main entry point for the application.
 */
internal class Program
{
    /**
     * @brief The main method of the application.
     * @param args The command line arguments.
     */
    private static void Main(string[] args)
    {
        string pathFileUsers = "users.bin";
        string pathFileTasks = "tasks.bin";
        string pathFileCategories = "categories.bin";
        var TaskschedulerLibrary = new TaskschedulerLibrary.Taskscheduler();
        TaskschedulerLibrary.MainMenu(pathFileUsers, pathFileTasks, pathFileCategories);
    }
}