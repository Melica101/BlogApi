using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi
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
            var jwtSettings = Configuration.GetSection("JwtSettings");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])
                        ),
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                );
            });
            services.AddDbContext<BlogContext>(options => options.UseSqlite("Data Source=blog.db"));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // This will ignore circular references
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                        .Json
                        .ReferenceLoopHandling
                        .Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
