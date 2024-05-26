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