namespace Mealplanner.GrainInterfaces

open System.Threading.Tasks
open Orleans


type IPingGrain =
    inherit IGrainWithStringKey

    abstract Ping: string -> Task<string>
