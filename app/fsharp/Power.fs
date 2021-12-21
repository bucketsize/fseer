module Power

open System
open System.IO

type PowerInfo = {
    psu: string;
    status: string;
    level: Int32;
}

let info () = 
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
    {psu=psu;status=status;level=level}

    
