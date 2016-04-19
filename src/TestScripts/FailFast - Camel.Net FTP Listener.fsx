﻿//  ============================================================================================================
//
//  This script demonstrates an FTP listener, which retreives an XML file and sends its contents to
//  the route. The Processors demonstate the receival of the message.
//  The RouteEngine demonstrates simple start, stop and info commands.
//
//  Logs can be found under: ./src/TestScripts/logs/<scriptname>.log
//
//  Prerequisites:
//      1.  A running FTP Server
//      2.  A user account test/test
//
//  ============================================================================================================

#I __SOURCE_DIRECTORY__
#I ".." 
#I "../../packages" 
#r @"Camel.Core/bin/Debug/Camel.Core.dll"   // the order of #r to dll's is important
#r @"Camel.FTP/bin/Debug/Camel.FTP.dll"
#r @"NLog/lib/net45/NLog.dll"

open System
open System.IO
open NLog
open Camel.Core
open Camel.Core.Definitions
open Camel.Producers
open Camel.Consumers
open Camel.Core.General
open Camel.Core.RouteEngine
open Camel.FileTransfer

//  Configure Nlog, logfile can be found under: ./src/TestScripts/logs/<scriptname>.log
let nlogPath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "./nlog.config"))
let logfile = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "logs", (sprintf "%s.log" __SOURCE_FILE__)))
let xmlConfig = new NLog.Config.XmlLoggingConfiguration(nlogPath)
xmlConfig.Variables.Item("logpath") <- Layouts.SimpleLayout(logfile)
LogManager.Configuration <- xmlConfig


//  Try this at home with your own configuration, for example: VirtualBox with Linux and vsftpd
let connection = "TestRemoteVM"                     // hostname of the ftp server
let fileListenerPath = "inbox"                      // (relative) path on the ftp server
let credentials = Credentials.Create "test" "test"  // credentials


let maps = [("hi-message", "//root/message")] |> Map.ofList     // for xpath substitution

let Process1 = To.Process(fun (m:Message) -> printfn "message received: %A" m.Body)
let Process2 = To.Process(maps, fun mp m -> printfn "processing: %s" mp.["hi-message"])

let route = 
    From.Ftp(fileListenerPath, connection, [FtpOption.Credentials(credentials)])
    =>= Process1
    =>= Process2

let id = route.Id

RegisterRoute route

RouteInfo() |> List.iter(fun e -> printfn "%A\t%A" e.Id e.RunningState)
StartRoute id

//  At this point, you can put a file in /home/test/inbox on the FTP system, which will be picked up by the route.
//  As an example file, you can use: "./src/TestExamples/TestFiles/test-message2.xml"
//  When the file is processed, it will be moved to /home/test/inbox/.camel or /home/test/inbox/.error

RouteInfo() |> List.iter(fun e -> printfn "%A\t%A" e.Id e.RunningState)
StopRoute id
RouteInfo() |> List.iter(fun e -> printfn "%A\t%A" e.Id e.RunningState)