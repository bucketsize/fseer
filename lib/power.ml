open Futil
open Fseerrec.Power

let info () = 
    let psu =
        if Sys.file_exists "/sys/class/power_supply/BAT0/status"
            then "Cell"
            else "Psu"
    in
    let status = 
        if psu = "Cell"
            then 
                match read_file_line "/sys/class/power_supply/BAT0/status" with
                | Some x -> x
                | None -> "Unknown"
            else "Unknown"
    and
        level = 
        if psu = "Cell"
            then 
                match read_file_line "/sys/class/power_supply/BAT0/capacity" with
                | Some x -> Int32.of_string x
                | None -> 0l
            else 0l
    in
    {psu=psu;status=status;level=Int32.to_int level}

    
