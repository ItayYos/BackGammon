using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.SignalRChat;
using DAL.SignalRChat;
using Hubs.SignalRChat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Server.BL;
using Server.Gameboard_Initializer;
using Server.Hubs;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IDB, DB>();
            services.AddTransient<IUserBL, UserBL>();
            services.AddTransient<IUserHubNotificationService, UserHubNotificationService>();
            services.AddTransient<IGameBL, BackgammonBL>();
            services.AddTransient<IGameBoardGenerator, GameBoardGenerator>();
            services.AddTransient<IGameNotificator, GameNotificator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSignalR(configure =>
            {
                configure.MapHub<BackgammonHub>("/Backgammon");
                configure.MapHub<ChatHub>("/Chat");
                configure.MapHub<UserHub>("/User");
            });
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
