module ConsoleWriter

open Metrics

let write (p:Metrics) = 
    p.cpuinfo.usage |> (fun x -> printfn "cpu: %3.0f" (x*100f))
    p.cpufreq.freqs |> List.iter (fun f -> printfn "cpuFreq: %4d" f)
    p.cputemp.temps |> List.iter (fun f -> printfn "cpuTemp: %3d" f)
    p.meminfo.usage |> (fun x -> printfn "mem: %3.0f" (x*100f))
    p.dskinfo.disks |> List.iter (fun f -> 
        printfn "%s: %3d, %3d" f.dev f.riops f.wiops)
    p.pwrinfo |> (fun x -> printfn "%s: %s, %2d" x.psu x.status x.level)
    printfn "--"
