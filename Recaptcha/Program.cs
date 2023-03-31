namespace Recaptcha
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddStackExchangeRedisCache(o => 
            { 
                o.InstanceName = "id_";
                o.Configuration = "oregon-redis.render.com:6379,user=red-cgjjjgjhp8u4muavbfug,password=ayOVYqJqInoRhwVmYIJsF1UtUYdr72Uc,ssl=True";
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}