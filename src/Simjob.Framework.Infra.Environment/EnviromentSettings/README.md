
# DotNet Simple Environment and Storage

Provides an simple environment file handling and application storage based on application files.

## Using the `EnvironmentSettings` class

- Bootstrap the application environment variables.
   ```c#
   using System;

   private static EnvironmentSettings envVariables;
   ...
   static void Main(string[] args)
   {
	   string filename = ".env";
	   envVariables = new EnvironmenSettings(filename);
	   envVariables.Flush(); // read contents from the file
   }
   ```

- Get an variable from instance:
	```C#
	string variableName    = "APP_KEY";
	string defaultValue    = "abc123"; // optional, default null
	string variableContent = envVariables.Get(variableName, defaultValue);
	```
	
- Set an variable value:
	```C#
	string variableName    = "APP_NAME";
	string variableValue   = "Hello World";
	string variableContent = envVariables.Set(variableName, variableValue);

	// outputs:
	// APP_NAME=Hello World
	```
	
## Using the `ApplicationStorage` class

- Bootstrap the application storage instance.
   ```c#
   using System;

   private static ApplicationStorage storageInstance;
   ...
   static void Main(string[] args)
   {
	   storageInstance = new ApplicationStorage();
   }
   ```
   
- Get an variable from storage:
	```C#
	string variableName    = "UsedNoodles";
	string variableContent = storageInstance.Get(variableName);
	// if the variable ins't found, "" is returned
	```
- Get an variable last edit time from storage:
	```C#
	string variableName    = "NoodlesToUse";
	DateTime variableContent = storageInstance.GetVarDate(variableName);
	// if the variable ins't found, null is returned
	```
- Store an variable in storage:
	```C#
	string variableName    = "Noodles";
	string variableContent = storageInstance.Set(variableName, "Noodle!");
	// if the variable ins't found, it will be created
	```
- Append value to variable in storage:
	```C#
	string variableName    = "Log";
	string variableContent = storageInstance.Append(variableName, "Goodbye!");
	// if the variable ins't found, it will be created
	```