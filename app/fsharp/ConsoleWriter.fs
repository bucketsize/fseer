module ConsoleWriter

open Metrics

let write (p:Metrics) = 
    printf "{"
    p.cpuinfo.usage |> (fun x -> printf "cpu:%3.0f " (x*100f))
    printf ",cpuFreq: "
    p.cpufreq.freqs |> List.iter (fun f -> printf "%4d " f)
    printf ",cputemp: "
    p.cputemp.temps |> List.iter (fun f -> printf "%3d " f)
    p.meminfo.usage |> (fun x -> printf ",mem: %3.0f " (x*100f))
    p.dskinfo.disks |> List.iter (fun f -> 
        printf ",part %s: %3d, %3d " f.dev f.riops f.wiops)
    p.pwrinfo |> (fun x -> printf "%s: %s, %2d " x.psu x.status x.level)
    p.netinfo.intfs |> List.iter (fun f -> 
        printf ",If %s: %d / %d " f.intf f.rx f.tx)
    printfn "}"
