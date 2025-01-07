using System;
using System.Diagnostics;
using System.Windows;
using CommandLine;

namespace Spectralyzer.Updater.Host;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Parser.Default
              .ParseArguments<AppOptions>(e.Args)
              .WithParsed(options =>
              {
                  if (options.Debug)
                  {
                      Debugger.Launch();
                  }

                  AppHost? host = null;
                  try
                  {
                      var builder = new AppHostBuilder();
                      host = builder.Build();
                      host.Start();
                  }
                  catch (Exception ex)
                  {
                      Debug.WriteLine($"Fatal exception: {ex}");
                      host?.Stop();
                      throw;
                  }

                  base.OnStartup(e);
              });
    }
}