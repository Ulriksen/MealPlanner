open System

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging;

open Giraffe

open Orleans
open Orleans.Hosting

open Mealplanner.GrainInterfaces
open Mealplanner.Grains

let pingpong (name: string) = 
    fun (next: HttpFunc) (ctx: HttpContext) -> 
        task {
          let grainFactory = ctx.GetService<IGrainFactory>()
          let pingGrain = grainFactory.GetGrain<IPingGrain>("ping")
          let! hello = pingGrain.Ping name
          return! json hello next ctx
        }

let webApp =
    choose [
        routef "/ping/%s" pingpong
        route "/ping"   >=> text "pong"
        route "/"       >=> htmlFile "/pages/index.html" ]

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore


[<EntryPoint>]
let main _ =

    let a = typedefof<PingGrain>

    Host.CreateDefaultBuilder()
        .ConfigureLogging(fun loggingBuilder ->
            loggingBuilder
              .AddFilter("Microsoft", LogLevel.Warning)
              .AddFilter("System", LogLevel.Warning)
              .AddFilter("Orleans", LogLevel.Debug)
              .AddConsole() |> ignore)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .UseOrleans(Action<HostBuilderContext, ISiloBuilder>(fun _ builder -> 
                   builder.UseLocalhostClustering () |> ignore ))
        .Build()
        .Run()
    0

