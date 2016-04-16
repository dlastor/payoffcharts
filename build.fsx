// include Fake lib
#r @"tools\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile

RestorePackages()

// Directories
let buildDir  = @".\build\"
let testDir   = @".\test\"
let deployDir = @".\deploy\"
let packagesDir = @".\packages"
let packagingDir = @".\packaging"

let authors = ["Jan Fajfr"]
let projectName = "Pricer"
let projectSummary = "Library with several methods to price options and estimate historical volatility"
let projectDescription = "Pricer for options and other financial products"

// version info
//I will have to add AssemblyInfo fsharp style and upgrade version from it
let version = "0.15.0"
let nugetKey = getBuildParamOrDefault "nugetKey" ""

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; deployDir]
)

Target "CompileApp" (fun _ ->
    [@"FinanceAnalysis/FinanceAnalysis.fsproj"]
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "CompileTest" (fun _ ->
    [@"FinanceAnalysisTests\FinanceAnalysisTests.fsproj"]
      |> MSBuildDebug testDir "Build"
      |> Log "TestBuild-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + @"\FinanceAnalysisTests.dll")
      |> NUnit (fun p ->
                 {p with
                   DisableShadowCopy = true;
                   OutputFile = testDir + @"TestResults.xml"})
)


Target "Zip" (fun _ ->
    !! (buildDir + "\**\*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "Pricer." + version + ".zip")
)
// Dependenciesf

"Clean"
  ==> "CompileApp"
  ==> "CompileTest"
  ==> "Test"
  ==> "Zip"

"CompileApp"
  ==> "CompileTest"
  ==> "Test"

// start build
RunTargetOrDefault "CompileTest"
