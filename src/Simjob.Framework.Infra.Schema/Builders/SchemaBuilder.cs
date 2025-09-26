using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema.CodeGeneration.CSharp;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Commands;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Extensions;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Builders
{
    [ExcludeFromCodeCoverage]
    public class SchemaBuilder : ISchemaBuilder
    {

        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;
        private readonly IUserHelper _userHelper;
        public string _internSchemasInMemoryName
        {
            get
            {
                return $"{_userHelper.GetTenanty()}-intern-schemas";
            }
        }
        private string _schemaInMemoryName
        {
            get
            {
                return $"{_userHelper.GetTenanty()}-schemas";
            }
        }
        private string _schemaFolderName
        {
            get
            {

                return $"{_userHelper.GetTenanty()}-Schemas";
            }
        }
        private string _schemanameSpace
        {
            get
            {
                return $"{_userHelper.GetTenanty()}.Schemas";
            }
        }
        private string _objectValueNameSpace
        {
            get
            {
                return $"{_userHelper.GetTenanty()}.ObjectValue";
            }
        }
        private string AssemblyLocation
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }
        private string _schemasSystemLocation
        {
            get
            {
                return Path.Combine(Path.Combine(AssemblyLocation, "SchemasJson"), _userHelper.GetTenanty());
            }
        }
        public string SchemaPath
        {
            get
            {
                return Path.Combine(AssemblyLocation, _schemaFolderName);
            }
        }

        public SchemaBuilder(IRepository<MongoDbContext, Schema> schemaRepository, IMemoryCache memoryCache,
                                                    INotificationHandler<DomainNotification> notifications, IMediatorHandler bus, IUserHelper userHelper)
        {
            _schemaRepository = schemaRepository;
            _memoryCache = memoryCache;
            _notifications = (DomainNotificationHandler)notifications;
            _bus = bus;
            _userHelper = userHelper;
        }

        public async Task TryBuildSystemSchema(string schemaName)
        {
            var schema = GetSchemaByName(schemaName);
            if (schema != null) return;

            string path = Path.Combine(_schemasSystemLocation, $"{schemaName}.json");
            string json = System.IO.File.ReadAllText(path);

            var insertSchemaCommand = new InsertSchemaCommand() { Name = schemaName, StrongEntity = true, JsonValue = json };
            await _bus.SendCommand(insertSchemaCommand);
            await GetSchemaType(schemaName);
        }
        public bool IsInternSchema(string schemaName)
        {
            var schemas = GetInternSchemasInMemory();
            return schemas.Exists(schema => schema.Name.ToLower() == schemaName.ToLower());
        }
        public List<InsertSchemaCommand> GetInternSchemasInMemory()
        {
            var systemSchemas = _internSchemasInMemoryName != null ? _memoryCache.Get<List<InsertSchemaCommand>>(_internSchemasInMemoryName) : new List<InsertSchemaCommand>();
            if (systemSchemas.IsNullOrEmpty()) return new List<InsertSchemaCommand>();
            return systemSchemas;
        }
        public async Task InsertInternSchemas()
        {
            var internSchemas = SetInternSchemasInMemory();
            foreach (var internSchema in internSchemas)
                await InsertInternSchemaIfNotExists(internSchema);
        }
        private async Task InsertInternSchemaIfNotExists(InsertSchemaCommand schema)
        {
            var schemaExists = GetSchemaByName(schema.Name);
            if (schemaExists != null) return;

            await _bus.SendCommand(schema);
        }
        private async Task InsertInternSchemaIfNotExists(string schemaName)
        {
            var internSchemas = GetInternSchemasInMemory();
            var insertSchemaCommand = internSchemas.FirstOrDefault(schema => schema.Name == schemaName);
            if (insertSchemaCommand == null) return;

            await InsertInternSchemaIfNotExists(insertSchemaCommand);
        }
        private List<InsertSchemaCommand> SetInternSchemasInMemory()
        {

            var internSchemas = GetInternSchemasInMemory();
            if (internSchemas.Any()) return internSchemas;

            List<InsertSchemaCommand> schemasCommandList = new();

            string[] files = { };

            if (Directory.Exists(_schemasSystemLocation))
            {
                files = Directory.GetFiles(_schemasSystemLocation);
            }
            else
            {
                Directory.CreateDirectory(_schemasSystemLocation);
            }

            if (!files.Any())
            {
                _memoryCache.Set(_internSchemasInMemoryName, schemasCommandList);
                return internSchemas;
            }

            foreach (var path in files)
            {
                string name = Path.GetFileName(path).Replace(".json", "");
                string json = System.IO.File.ReadAllText(path);

                var insertSchemaCommand = new InsertSchemaCommand() { Name = name, StrongEntity = true, JsonValue = json };
                schemasCommandList.Add(insertSchemaCommand);
            }

            _memoryCache.Set(_internSchemasInMemoryName, schemasCommandList);
            return schemasCommandList;
        }
        public async Task BuildSystemSchemas()
        {
            var files = Directory.GetFiles(_schemasSystemLocation);
            if (!files.Any()) return;

            foreach (var file in files)
            {
                string name = file.Replace(".json", "");
                var schema = GetSchemaByName(name);
                if (schema != null) continue;

                string path = Path.Combine(_schemasSystemLocation, file);
                string json = System.IO.File.ReadAllText(path);

                var insertSchemaCommand = new InsertSchemaCommand() { Name = name, StrongEntity = true, JsonValue = json };
                await _bus.SendCommand(insertSchemaCommand);
                await GetSchemaType(name);
            }
        }
        public Schema GetSchemaByName(string name)
        {
            return _schemaRepository.GetByField("name", name);
        }
        [ExcludeFromCodeCoverage]
        private bool ExistsRefInMemory(string schemaName)
        {
            var schemasCompiled = _memoryCache.Get<List<string>>(_schemaInMemoryName);
            return schemasCompiled.Exists(schema => schema.ToLower() == schemaName.ToLower());
        }
        [ExcludeFromCodeCoverage]
        private void RemoveRefInMemory(string schemaName)
        {

            var schemasCompiled = _memoryCache.Get<List<string>>(_schemaInMemoryName);
            if (schemasCompiled is not null)
            {
                if (schemasCompiled.Exists(schema => schema.ToLower() == schemaName.ToLower()))
                    schemasCompiled.Remove(schemaName.ToLower());
                else return;
            }
            else return;
            _memoryCache.Set("schemas", schemasCompiled);
        }
        [ExcludeFromCodeCoverage]
        private void AddRefInMemoryCache(string schemaName)
        {

            var schemasCompiled = _memoryCache.Get<List<string>>(_schemaInMemoryName);

            if (schemasCompiled == null) schemasCompiled = new List<string>();

            if (schemasCompiled.Exists(schema => schema.ToLower() == schemaName.ToLower())) return;

            schemasCompiled.Add(schemaName.ToLower());

            _memoryCache.Set(_schemaInMemoryName, schemasCompiled);
        }
        [ExcludeFromCodeCoverage]
        private bool IsCompiled(string schemaName)
        {
            var schemasCompiled = _schemaInMemoryName != null ?  _memoryCache.Get<List<string>>(_schemaInMemoryName) : new List<string>();
            if (schemasCompiled.IsNullOrEmpty()) return false;
            return ExistsRefInMemory(schemaName);
        }
        public void RemoveDll(string schemaName)
        {

            if (File.Exists(Path.Combine(AssemblyLocation, $"{_schemaFolderName}_{schemaName}.dll")))
            {
                // delete your file.
                File.SetAttributes(Path.Combine(AssemblyLocation, $"{_schemaFolderName}_{schemaName}.dll"), FileAttributes.Normal);

                File.Delete(Path.Combine(AssemblyLocation, $"{_schemaFolderName}_{schemaName}.dll"));
            }
        }
        [ExcludeFromCodeCoverage]
        private CSharpCompilation GetCompilation(string className, SyntaxTree syntaxTree)
        {
            string objectAssemblyLocation = Path.GetDirectoryName(typeof(object).Assembly.Location);

            CSharpCompilation compilation = CSharpCompilation.Create(
              className + _userHelper.GetTenanty().FirstCharToUpper(),
              new[] { syntaxTree },
              new[] {
                  MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Enum).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(EnumMemberAttribute).Assembly.Location),
                        MetadataReference.CreateFromFile(Path.Combine(objectAssemblyLocation,"System.ComponentModel.DataAnnotations.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(objectAssemblyLocation,"System.ComponentModel.Annotations.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(objectAssemblyLocation,"netstandard.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(objectAssemblyLocation,"System.Runtime.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(AssemblyLocation,"Newtonsoft.Json.dll")),
              },
              new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation;
        }
        public Schema GetSchema(string name)
        {
            var schema = _schemaRepository.GetSchemaByField("name", name);
            schema.JsonValue = AddDefinitionsInSchema(schema.JsonValue);
            return schema;
        }
        public async Task<bool> IsValid(string name, string jsonValue)
        {
            var schema = GetSchema(name);
            var jsonSchema = await NJsonSchema.JsonSchema.FromJsonAsync(schema.JsonValue);
            var res = jsonSchema.Validate(jsonValue);

            if (res.Any())
                foreach (var error in res)
                    await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, error.Path));

            return !res.Any();
        }
        [ExcludeFromCodeCoverage]
        private async Task<string> SchemaToCSharp(string name, string jsonSchema, bool strongEntity = true)
        {
            name = name.FirstCharToUpper();

            name = name + _userHelper.GetTenanty().FirstCharToUpper();

            string schemaOnDefinitions = AddDefinitionsInSchema(jsonSchema);

            var schema = await NJsonSchema.JsonSchema.FromJsonAsync(schemaOnDefinitions);

            schema.AllowAdditionalItems = false;
            schema.AllowAdditionalProperties = false;

            var generator = new CSharpGenerator(schema);
            generator.Settings.Namespace = strongEntity ? _schemanameSpace : _objectValueNameSpace;

            var ccharp = generator.GenerateFile();

            if (!Directory.Exists(SchemaPath))
                Directory.CreateDirectory(SchemaPath);

            File.WriteAllText(Path.Combine(SchemaPath, $"{name}.cs"), ccharp);
            return ccharp;
        }
        public async Task<IEnumerable<ClassDeclarationSyntax>> GetClassDeclarationSyntaxesList()
        {
            try
            {
                var schemas = _schemaRepository.GetAll().Data.Where(s => !s.StrongEntity);
                List<ClassDeclarationSyntax> classDeclarationSyntaxList = new();
                foreach (var schema in schemas)
                {

                    string className = schema.Name;

                    className.ToArray()[0] = className[0].ToString().ToUpper()[0];
                    string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string path = Path.Combine(currentPath, "Schemas", $"{className}.cs");

                    await SchemaToCSharp(schema.Name, schema.JsonValue, false);

                    string ccharp = File.ReadAllText(path);

                    var syntaxTree = CSharpSyntaxTree.ParseText(ccharp);

                    var root = syntaxTree.GetCompilationUnitRoot();
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)root.Members[0];
                    var classDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members[namespaceDeclaration.Members.Count - 1];

                    classDeclarationSyntaxList.Add(classDeclaration);

                }

                return classDeclarationSyntaxList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new List<ClassDeclarationSyntax>();
            }

        }
        public async Task<Assembly> CompileSchema(string schemaName)
        {
            var schema = GetSchema(schemaName);


            if (schema == null)
            {
                var type = Type.GetType(schemaName);
                if (type != null)
                    return Assembly.GetAssembly(type);

                await _bus.RaiseEvent(new DomainNotification(this.GetType().Name, "Schema or Type not found"));
                return null;
            }
            string quote = "\"";
            schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");

            //schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[{quote},]", $"{quote}enum{quote}:[]");
            //RemoveRefInMemory(schemaName);
            if (IsCompiled(schemaName))
            {
                return Assembly.LoadFrom(Path.Combine(AssemblyLocation, $"{_schemaFolderName}_{schemaName}.dll"));
            }

            //schema.StrongEntity = false;
            string csharp = await SchemaToCSharp(schema.Name, schema.JsonValue, schema.StrongEntity);

            var syntaxTree = CSharpSyntaxTree.ParseText(csharp);

            var root = syntaxTree.GetCompilationUnitRoot();
            var namespaceDeclaration = (NamespaceDeclarationSyntax)root.Members[0];
            var classDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members.FirstOrDefault(); //[namespaceDeclaration.Members.Count - 1];

            var entityExtend = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(typeof(Entity).Name));

            classDeclaration = classDeclaration.AddBaseListTypes(entityExtend);

            var newNameSpace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_schemanameSpace));

            newNameSpace = newNameSpace.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(Entity).Namespace)));
            newNameSpace = newNameSpace.AddMembers(classDeclaration);

            foreach (var member in namespaceDeclaration.Members)
            {
                var index = namespaceDeclaration.Members.IndexOf(member);
                if (index == 0) continue;
                newNameSpace = newNameSpace.AddMembers(member);

            }

            var code = newNameSpace
               .NormalizeWhitespace()
               .ToFullString();

            syntaxTree = CSharpSyntaxTree.ParseText(code);

            var compilation = GetCompilation(schemaName, syntaxTree);

          
            try
            {
                var compilationResult = compilation.Emit(Path.Combine(AssemblyLocation, $"{_schemaFolderName}_{schemaName}.dll"));
                if (!compilationResult.Success)
                {
                    foreach (var diagnostic in compilationResult.Diagnostics)
                        await _bus.RaiseEvent(new DomainNotification(compilation.GetType().Name, diagnostic.GetMessage()));

                    return null;
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                AddRefInMemoryCache(schemaName);
            }
            return Assembly.LoadFrom(Path.Combine(AssemblyLocation, $"{_schemaFolderName}_{schemaName}.dll"));
        }
        public async Task CompileObjectValues()
        {
            var schemas = _schemaRepository.GetAll().Data.Where(s => !s.StrongEntity);

            foreach (var schema in schemas)
            {
                string schemaName = schema.Name;
                await CompileSchema(schemaName);
            }
        }
        public string AddDefinitionsInSchema(string schema)
        {
            var schemaModel = JsonConvert.DeserializeObject<SchemaModel>(schema);

            if (schemaModel.Definitions == null || !schemaModel.Definitions.Any()) return schema;

            var names = schemaModel.Definitions.Select(s => s.Key);

            JObject jObject = JObject.Parse(schema);
            var jObjectDefinitions = jObject["definitions"];
            foreach (var name in names)
            {
                string className = name.ToString();
                className = className.FirstCharToUpper() + _userHelper.GetTenanty().FirstCharToUpper();
                var schemaObjectValue = _schemaRepository.GetSchemaByField("name", className);
                if (schemaObjectValue != null)
                    jObjectDefinitions[name] = JObject.Parse(schemaObjectValue.JsonValue);

            }

            jObject["definitions"] = jObjectDefinitions;
            return jObject.ToString();
        }
        public async Task<Type> GetSchemaType(string schemaName, bool reloadSchema = false)
        {
            if (IsInternSchema(schemaName))
                await InsertInternSchemaIfNotExists(schemaName);

            //if (reloadSchema) RemoveRefInMemory(schemaName);

            var assembly = await CompileSchema(schemaName);
            return assembly.GetType($"{_schemanameSpace}.{schemaName}");
        }
        public async Task CompileAllSchemas()
        {
            var schemas = _schemaRepository.GetAll();
            var schemaNames = schemas.Data.Select(s => s.Name).ToList();
            foreach (var schemaName in schemaNames)
                await CompileSchema(schemaName);
        }
    }
}