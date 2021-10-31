module LemonbarWriter

open System
open System.IO
open Metrics

let lemonFifo = "/tmp/fseer.lemon.fifo"
Util.mkfifo lemonFifo Util.DEFFILEMODE |> ignore
let lemonFifoWriter = new StreamWriter(lemonFifo)
   
type Async with
    static member AwaitPlainTask (task : Task) =
        task.ContinueWith(ignore) |> Async.AwaitTask


let write (m:Metrics) = 
    let cpufreqAvg = 
        let fsum = m.cpufreq.freqs |> (List.fold (fun a x -> (a + x)) 0) in
            fsum / List.length m.cpufreq.freqs
    let lemonBar =
        sprintf "%%{l}en %%{c}%s %%{r}Cpu %3.0f %4dHz %3dC | Mem %3.0f | %s\n"
            (DateTime.Now.ToString("ddd | MMM dd, yyyy | HH:mm:ss"))
            (m.cpuinfo.usage * 100f)
            cpufreqAvg
            m.cputemp.temps.Head
            (m.meminfo.usage * 100f)
            (if m.pwrinfo.psu = "AC" then "AC" else "Bat")
    (* try *)
    (*     lemonFifoWriter.Write(lemonBar) *)
    (*     lemonFifoWriter.Flush() *)
    (* with *)
    (*     | ex -> (printfn "Exception: %s" (ex.ToString())) *) 
    async {
        do! lemonFifoWriter.WriteAsync(lemonBar) |> Async.AwaitTask 
        do! lemonFifoWriter.FlushAsync() |> Async.AwaitTask
    }

