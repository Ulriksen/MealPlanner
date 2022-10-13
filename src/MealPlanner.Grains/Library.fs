namespace Mealplanner.Grains


open Mealplanner.GrainInterfaces
open System.Threading.Tasks

type PingGrain =
    interface IPingGrain with 
        member _.Ping name : Task<string> = 
            task {
                return $"Pong {name}" }