global using FastEndpoints;
global using JobQueuesEfCoreDemo;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
   .AddFastEndpoints()
   .AddJobQueues<JobRecord, JobStorageProvider>()
   .SwaggerDocument();

var app = bld.Build();
app.UseFastEndpoints()
   .UseJobQueues()
   .UseSwaggerGen();
app.Run();