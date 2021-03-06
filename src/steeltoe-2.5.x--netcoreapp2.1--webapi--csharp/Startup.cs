using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
{{#RequiresHttps}}
using Microsoft.AspNetCore.HttpsPolicy;
{{/RequiresHttps}}
using Microsoft.AspNetCore.Mvc;
{{#Auth}}
using Microsoft.AspNetCore.Authentication;
{{/Auth}}
{{#oauth}}
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
{{/oauth}}
{{#IndividualB2CAuth}}
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
{{/IndividualB2CAuth}}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
{{#actuator-or-cloud-foundry}}
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Hypermedia;
{{/actuator-or-cloud-foundry}}
{{#cloud-foundry}}
using Steeltoe.Extensions.Configuration.CloudFoundry;
{{/cloud-foundry}}
{{#circuit-breaker}}
using Steeltoe.CircuitBreaker.Hystrix;
{{/circuit-breaker}}
{{#mysql-or-mysql-efcore}}
using Steeltoe.CloudFoundry.Connector.MySql;
{{/mysql-or-mysql-efcore}}
{{#mysql-efcore}}
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
{{/mysql-efcore}}
{{#sqlserver}}
using Steeltoe.CloudFoundry.Connector.SqlServer;
{{/sqlserver}}
{{#eureka-client}}
using Steeltoe.Discovery.Client;
{{/eureka-client}}
{{#postgresql}}
using Steeltoe.CloudFoundry.Connector.PostgreSql;
{{/postgresql}}
{{#amqp}}
using Steeltoe.CloudFoundry.Connector.RabbitMQ;
{{/amqp}}
{{#data-redis}}
using Steeltoe.CloudFoundry.Connector.Redis;
{{/data-redis}}
{{#data-mongodb}}
using Steeltoe.CloudFoundry.Connector.MongoDb;
{{/data-mongodb}}
{{#oauth}}
using Steeltoe.CloudFoundry.Connector.OAuth;
{{/oauth}}
{{#postgresql-efcore}}
using Steeltoe.CloudFoundry.Connector.PostgreSql.EFCore;
{{/postgresql-efcore}}

namespace {{Namespace}}
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
{{#OrganizationalAuth}}
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
{{/OrganizationalAuth}}
{{#IndividualB2CAuth}}
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
{{/IndividualB2CAuth}}
{{#mysql}}
            services.AddMySqlConnection(Configuration);
{{/mysql}}
{{#actuator}}
{{#cloud-foundry}}
            services.ConfigureCloudFoundryOptions(Configuration);
            services.AddCloudFoundryActuators(Configuration, MediaTypeVersion.V2, ActuatorContext.ActuatorAndCloudFoundry);
{{/cloud-foundry}}
{{^cloud-foundry}}
            services.AddCloudFoundryActuators(Configuration);
{{/cloud-foundry}}
{{/actuator}}
{{#eureka-client}}
            services.AddDiscoveryClient(Configuration);
{{/eureka-client}}
{{#postgresql}}
            services.AddPostgresConnection(Configuration);
{{/postgresql}}
{{#amqp}}
            services.AddRabbitMQConnection(Configuration);
{{/amqp}}
{{#data-redis}}
            // Add the Redis distributed cache.

            // We are using the Steeltoe Redis Connector to pickup the CloudFoundry
            // Redis Service binding and use it to configure the underlying RedisCache
            // This adds a IDistributedCache to the container
            services.AddDistributedRedisCache(Configuration);

            // This works like the above, but adds a IConnectionMultiplexer to the container
            // services.AddRedisConnectionMultiplexer(Configuration);
{{/data-redis}}
{{#data-mongodb}}
            services.AddMongoClient(Configuration);
{{/data-mongodb}}

{{#oauth}}
            services.AddOAuthServiceOptions(Configuration);
{{/oauth}}
{{#mysql-efcore}}
            services.AddDbContext<TestContext>(options => options.UseMySql(Configuration));
{{/mysql-efcore}}
{{#postgresql-efcore}}
            services.AddDbContext<TestContext>(options => options.UseNpgsql(Configuration));
{{/postgresql-efcore}}
{{#sqlserver}}
            services.AddSqlServerConnection(Configuration);
{{/sqlserver}}
            services.AddMvc();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            {{#RequiresHttps}}
            else
            {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            {{/RequiresHttps}}
            {{#Auth}}
            app.UseAuthentication();
            {{/Auth}}

            {{#actuator}}
            {{#cloud-foundry}}
            app.UseCloudFoundryActuators(MediaTypeVersion.V2, ActuatorContext.ActuatorAndCloudFoundry);
            {{/cloud-foundry}}
            {{^cloud-foundry}}
            app.UseCloudFoundryActuators();
            {{/cloud-foundry}}
            {{/actuator}}

            {{#eureka-client}}
            app.UseDiscoveryClient();
            {{/eureka-client}}
            app.UseMvc();
        }
    }
}
