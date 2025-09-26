using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;

#nullable enable

namespace System
{
    /// <summary>
    /// Provides an initial configuration file handling class for the application.
    /// </summary>
    public static class EnvironmentSettings
    {
        private static Dictionary<string, string> storedValues = new Dictionary<string, string>();
        private static string file = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Environment.ini";
        private static bool flushed = false;

        /// <summary>
        /// Get or sets the name of the initial settings file.
        /// </summary>
        public static string EnvironmentFile
        {
            get => file;
            set
            {
                if (!ApplicationStorage.isPathNameValid(value)) throw new InvalidDataException("Environment file cannot contain invalid path chars.");
                file = value;
            }
        }

        /// <summary>
        /// Gets an object with the environment variables as properties.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static dynamic GetDynamicObject()
        {
            if (!flushed)
            {
                throw new Exception("INI Container not flushed or initialized.");
            }

            var newObject = new ExpandoObject() as IDictionary<string, object>;

            foreach (var keyValuePair in storedValues)
            {
                newObject.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return newObject as dynamic;
        }

        /// <summary>
        /// Reads environment variables from file and stores them in the application memory.
        /// </summary>
        public static void Flush()
        {
            storedValues = FlushData();
        }

        /// <summary>
        /// Reads environment variables from file and stores them in the application memory and creates the Environment file if it doens't exists.
        /// </summary>
        /// <param name="createEnvironmentFileIfDontExists">Creates the Environment file if it doens't exists.</param>
        public static void Flush(bool createEnvironmentFileIfDontExists)
        {
            storedValues = FlushData();
            if (createEnvironmentFileIfDontExists)
            {
                if (!System.IO.File.Exists(file))
                {
                    System.IO.File.Create(file).Close();
                }
            }
        }

        /// <summary>
        /// Gets an variable from the environment file.
        /// </summary>
        /// <param name="propertyName">The variable name.</param>
        /// <param name="defaultValue">The default value to return if the variable don't exists.</param>
        /// <returns></returns>
        public static string? Get(string propertyName, string? defaultValue = null)
        {
            if (!flushed)
            {
                throw new Exception("INI Container not flushed or initialized.");
            }
            string? v = storedValues.Where((i) => i.Key.ToLower() == propertyName.ToLower()).FirstOrDefault().Value;
            return v ?? defaultValue;
        }
        [ExcludeFromCodeCoverage]
        private static Dictionary<string, string> FlushData()
        {
            //read properties
            Dictionary<string, string> values = new Dictionary<string, string>();

            flushed = true;

            if (!System.IO.File.Exists(file)) return values;

            foreach (string line in System.IO.File.ReadLines(file))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue; // group
                }
                if (line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue; // comment
                }
                if (line.StartsWith("["))
                {
                    continue; // group
                }
                try
                {
                    string propertyName = line.Substring(0, line.IndexOf("=")).Trim();
                    string propertyValue = line.Substring(line.IndexOf("=") + 1).Trim();

                    if (propertyValue.StartsWith("\""))
                    {
                        if (!propertyValue.EndsWith("\""))
                        {
                            throw new Exception("Quoted variables must end with quotes.");
                        }
                        propertyValue = propertyValue.Substring(1, propertyValue.Length - 2);
                    }

                    if (values.ContainsKey(propertyName))
                    {
                        throw new Exception("A key with this name already exists in this INI collection: " + propertyName);
                    }

                    values.Add(propertyName, propertyValue);
                }
                catch (Exception ex)
                {
                    throw new Exception("Internal error on the parser: " + ex.Message);
                }
            }

            return values;
        }
    }

    /// <summary>
    /// Provides a solid file storage of variables and data.
    /// </summary>
    public class ApplicationStorage
    {
        private string _prefix = "";

        internal static bool isPathNameValid(string value)
        {
            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            foreach (char c in value)
            {
                if (invalidChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool isFileNameValid(string value)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in value)
            {
                if (invalidChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
        [ExcludeFromCodeCoverage]
        private static void initStorage(ApplicationStorage applicationStorage)
        {
            if (!Directory.Exists(applicationStorage.Prefix))
            {
                Directory.CreateDirectory(applicationStorage.Prefix);
            }
        }

        /// <summary>
        /// Gets or sets the directory where the data will be stored.
        /// </summary>
        public string Prefix
        {
            get => _prefix;
            set
            {
                if (!isPathNameValid(value)) throw new InvalidDataException("Storage prefixes cannot contain invalid path chars.");
                _prefix = value;
            }
        }

        /// <summary>
        /// Creates an new <see cref="System.ApplicationStorage"/> instance with the default storage prefix.
        /// </summary>
        public ApplicationStorage()
        {
            this.Prefix = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        }

        /// <summary>
        /// Creates an new <see cref="System.ApplicationStorage"/> instance with the provided storage prefix.
        /// </summary>
        /// <param name="storagePrefix">The directory where the data will be stored.</param>
        public ApplicationStorage(string storagePrefix)
        {
            this.Prefix = storagePrefix;
        }

        /// <summary>
        /// Creates an new <see cref="System.ApplicationStorage"/> instance with an new storage prefix inside this instance's prefix.
        /// </summary>
        /// <param name="subStoragePrefix">The subdirectory prefix for the new instance.</param>
        /// <returns></returns>
        public ApplicationStorage GetSubStorage(string subStoragePrefix)
        {
            return new ApplicationStorage(Path.Combine(Prefix, subStoragePrefix));
        }

        /// <summary>
        /// Gets an variable value inside this storage.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="defaultValue">Optional. If the variable is not found, this value is returned.</param>
        /// <returns></returns>
        public string? Get(string name, string? defaultValue = null)
        {
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            try
            {
                return System.IO.File.ReadAllText(System.IO.Path.Combine(Prefix, name));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public string[] GetVariables()
        {
            return System.IO.Directory.GetFiles(Prefix);
        }

        /// <summary>
        /// Gets the modification date of a variable. If it does not exist, nothing is returned.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <returns></returns>
        public DateTime? GetVarDate(string name)
        {
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            try
            {
                return System.IO.File.GetLastWriteTime(System.IO.Path.Combine(Prefix, name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Sets an variable and stores the value on it.
        /// </summary>
        /// <param name="name">The variable name. It must be an valid name and cannot have invalid path chars.</param>
        /// <param name="data">The string data to store in the variable. If nothing is provided, the variable is deleted.</param>
        public void Set(string name, string? data)
        {
            initStorage(this);
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            string path = System.IO.Path.Combine(Prefix, name);
            if (data == null)
            {
                System.IO.File.Delete(path);
                return;
            }
            System.IO.File.WriteAllText(path, data);
        }

        /// <summary>
        /// Appends values to an variable. If it doens't exist, it will be created.
        /// </summary>
        /// <param name="name">The variable name. It must be an valid name and cannot have invalid path chars.</param>
        /// <param name="data">The string data to store in the variable.</param>
        public void Append(string name, string data)
        {
            initStorage(this);
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            string path = System.IO.Path.Combine(Prefix, name);
            System.IO.File.AppendAllLines(path, new string[] { data.Trim() });
        }
    }
}