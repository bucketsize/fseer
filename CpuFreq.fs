module CpuFreq 

open System
open System.IO

let cpuFreqFiles =
    [0 .. 15]
    |> List.map(sprintf "/sys/devices/system/cpu/cpu%d/cpufreq/scaling_cur_freq")
    |> List.filter(fun x -> File.Exists(x))


let cpuFreq(ff:string) = 
    let stream = new StreamReader(ff)
    let line = stream.ReadLine()
    stream.Close()
    Int32.Parse(line)/1000

let cpuFreqs _ = 
    cpuFreqFiles 
    |> List.map cpuFreq
    |> List.iter (printfn "cpufreq: %d")
