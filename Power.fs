module Power

open System
open System.IO

let psuinfo() = 
    let psu =
        if File.Exists("/sys/class/power_supply/BAT0/status")
            then "Cell"
            else "AC"
    let status = 
        if psu = "Cell"
            then Util.readLine("/sys/class/power_supply/BAT0/status")
            else "Unknown"
    let level = 
        if psu = "Cell"
            then Int32.Parse(Util.readLine("/sys/class/power_supply/BAT0/capacity"))
            else 0
    (psu,status,level)

let status() =
    let psu,status,level = psuinfo()
    printfn "psu: %s" psu
    printfn "psuStatus: %s" status
    printfn "psuLevel: %d" level
