module CpuTemp 

open System
open System.IO

type CpuTemp = {
    temps: List<Int32>;
}

let cpuTempFiles =
    [for i = 0 to 15 do
        for j = 0 to 3 do
            yield (sprintf "/sys/class/hwmon/hwmon%d/temp%d_label" i j)
     yield "/sys/class/thermal/thermal_zone0/temp"
    ]
    |> List.filter (File.Exists)

let info () = 
    { temps = cpuTempFiles
    |> List.map (Util.readLine)
    |> List.map (fun x -> (Int32.Parse(x)/1000)) }

