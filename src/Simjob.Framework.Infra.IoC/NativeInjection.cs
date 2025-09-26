using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Bus;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Data.Repositories;
using Simjob.Framework.Infra.Identity.CommandHandlers;
using Simjob.Framework.Infra.Identity.Commands;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Helpers;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Repositories;
using Simjob.Framework.Infra.Identity.Services;
using Simjob.Framework.Infra.Schemas.Builders;
using Simjob.Framework.Infra.Schemas.CommandHandlers;
using Simjob.Framework.Infra.Schemas.Commands;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Commands.Views;
using Simjob.Framework.Infra.Schemas.Contexts;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Services;
using System.Reflection;

namespace Simjob.Framework.Infra.IoC
{
    public static class NativeInjection
    {
        public static void InjectDependecies(IServiceCollection services)
        {

            // ASP NET

            services.AddHttpContextAccessor();

            // MEDIATR

            services.AddMediatR(Assembly.Load("Simjob.Framework.Domain.Core"));
            services.AddMediatR(Assembly.Load("Simjob.Framework.Domain"));
            services.AddMediatR(Assembly.Load("Simjob.Framework.Infra.Identity"));
            services.AddMediatR(Assembly.Load("Simjob.Framework.Infra.Schemas"));

            // DOMAIN

            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // BUS 
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // INFRA DATA

            services.AddScoped<MongoDbContext>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            // EVENT Sourcing
            //services.AddScoped<EventStoreRepository<MongoDbContext>>();
            //services.AddScoped<EventStoreRepository<IdentityContext>>();

            // INFRA IDENTITY

            services.AddScoped<IdentityContext>();
            services.AddScoped<TwoFactorAuthContext>();
            services.AddScoped<GroupContext>();
            services.AddScoped<PermissionContext>();
            services.AddScoped<SearchdefsContext>();
            services.AddScoped<ViewContext>(); // maybe del
            services.AddScoped<ProfileContext>();
            services.AddScoped<UserAccessContext>();
            services.AddScoped<ExternalTokensContext>();
            services.AddScoped<GeneratorsContext>();
            services.AddScoped<SegmentationContext>();
            services.AddScoped<NotificationContext>();
            services.AddScoped<ApproveLogContext>();
            services.AddScoped<StatusFlowContext>();
            services.AddScoped<StatusFlowItemContext>();
            services.AddScoped<SharedSchemaRecordContext>();
            services.AddScoped<ModuleContext>();
            services.AddScoped<ModuleIdentityContext>();
            services.AddScoped<ModulePermissionContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserAdminContext>();
            services.AddScoped<IUserAdminService, UserAdminService>();

            services.AddScoped<SourceContext>();
            services.AddScoped<ISourceService, SourceService>();


            //SHAREDSCHEMARECORD
            services.AddScoped<ISharedSchemaRecordService, SharedSchemaRecordService>();
            services.AddScoped<ISharedSchemaRecordRepository, SharedSchemaRecordRepository>();

            // Searchdefs
            services.AddScoped<ISearchdefsService, SearchdefsService>();
            services.AddScoped<ISearchdefsRepository, SearchdefsRepository>();

            // Storage Service
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<IHuaweiStorageService, HuaweiStorageService>();
            services.AddScoped<IAWSStorageService, AWSStorageService>();

            services.AddScoped<StorageService>();


            //Profile
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProfileRepository, ProfileRepository>();

            // Two Factor Auth
            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
            services.AddScoped<ITwoFactorAuthRepository, TwoFactorAuthRepository>();


            // GRUPO DE ACESSO 

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupRepository, GroupRepository>();

            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();




            //UserAccess

            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IUserAccessRepository, UserAccessRepository>();


            //Generators
            services.AddScoped<IGeneratorsService, GeneratorsService>();
            services.AddScoped<IGeneratorsRepository, GeneratorsRepository>();


            // Segmentation
            services.AddScoped<ISegmentationService, SegmentationService>();
            services.AddScoped<ISegmentationRepository, SegmentationRepository>();

            // USER 

            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRequestHandler<SignInUserCommand, bool>, UserCommandHandler>();
            services.AddScoped<IRequestHandler<RegisterUserCommand, bool>, UserCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateUserCommand, bool>, UserCommandHandler>();

            // TOKENS

            services.AddScoped<IExternalTokensRepository, ExternalTokensRepository>();
            services.AddScoped<IExternalTokensService, ExternalTokensService>();

            // Notification
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<UpdateNotificationCommand>();
            services.AddScoped<IApproveLogRepository, ApproveLogRepository>();
            services.AddScoped<IApproveLogService, ApproveLogService>();

            // StatusFlow
            services.AddScoped<IStatusFlowRepository, StatusFlowRepository>();
            services.AddScoped<IStatusFlowService, StatusFlowService>();
            services.AddScoped<StatusFlowCommand>();

            // StatusFlowItem
            services.AddScoped<IStatusFlowItemRepository, StatusFlowItemRepository>();
            services.AddScoped<IStatusFlowItemService, StatusFlowItemService>();

            //Modules
            services.AddScoped<IModuleService, ModuleService>();

            //ModuleIdentity
            services.AddScoped<IModuleIdentityService, ModuleIdentityService>();
            //ModulePermission

            services.AddScoped<IModulePermissionService, ModulePermissionService>();
            // VIEWS 

            services.AddScoped<IRequestHandler<InsertViewCommand, bool>, ViewCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateViewCommand, bool>, ViewCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteViewCommand, bool>, ViewCommandHandler>();
            services.AddScoped<IViewService, ViewService>();

            // INFRA SCHEMA

            services.AddScoped<ISchemaBuilder, SchemaBuilder>();
            services.AddScoped<IEntityService, EntityService>();

            services.AddScoped<IRequestHandler<InsertEntityCommand, bool>, EntityCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateEntityCommand, bool>, EntityCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteEntityCommand, bool>, EntityCommandHandler>();

            services.AddScoped<IRequestHandler<InsertSchemaCommand, bool>, SchemaCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateSchemaCommand, bool>, SchemaCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteSchemaCommand, bool>, SchemaCommandHandler>();


        }
    }
}