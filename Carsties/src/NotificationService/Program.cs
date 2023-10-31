using MassTransit;
using NotificationService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        /*
        cfg.UseRetry(r =>
        {
            r.Handle<RabbitMqConnectionException>();
            r.Interval(5, TimeSpan.FromSeconds(10));
        });*/

        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddSignalR();

app.MapHub<NotificationHub>("/notifications");

app.Run();
