In the section where you register your DI Container, add following in exact order:

    services.AddSingleton<IUserLoggerInfoModel, UserLoggerInfoModel>();
    services.AddSingleton<ILoggerService, LoggerService>();

