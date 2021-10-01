module Writer

let Cpu(p:float32) = 
    printfn "cpu: %3.0f" p |> ignore

let CpuFreq(p:int) = 
    printfn "cpuFreq: %4d" p |> ignore

let CpuTemp(p:int) = 
    printfn "cpuTemp: %3d" p |> ignore

let Mem(p:int) = 
    printfn "mem: %3d" p |> ignore

let Disks(p:string, r:int, w:int) =
    printfn "part: %s %d %d" p r w |> ignore

let Power(psu:string, status:string, level:int) =
    printfn "psu: %s" psu |> ignore
    printfn "psuStatus: %s" status |> ignore
    printfn "psuLevel: %d" level |> ignore

        
