namespace FinanceAnalysis.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Web.Http
open OptionsPricing
open FinanceAnalysis.Model

[<RoutePrefix("api/pricing")>]
type PricingController() =
    inherit ApiController()

    //given complete strategy  (stock and legs, returns the payoff chart data)
    member x.Put([<FromBody>] strategy:Strategy) : IHttpActionResult = 
        let strategyData, legsData = Options.getStrategyData strategy

        let buildLines (data:(Leg*(float*float) list) seq)= 
            data |> Seq.map (fun (leg,linedata) -> 
                {
                    Linename = leg.Definition.Name
                    Values = linedata
                })

        let payoff = {
            Legs = legsData |> Seq.map (fun (leg,_) -> leg)
            LegPayoffs = buildLines legsData
            StrategyPayoff = 
            {    
                Linename = "Strategy"
                Values = strategyData
            }
        }
        x.Ok(payoff) :> _

    //given stock and strategy name returns the example strategy
    member x.Post([<FromBody>] query:StrategyQuery) : IHttpActionResult = 
        let strategy = StrategiesExamples.getStrategy query.Name query.Stock
        x.Ok(strategy) :> _