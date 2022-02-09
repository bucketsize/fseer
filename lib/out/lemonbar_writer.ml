open Printf
open Fseerrec

let weDays = ["Su"; "Mo"; "Tu"; "We"; "Th"; "Fr"; "Sa"]
let months = ["Jan"; "Feb"; "Mar"; "Apr"; "May"; "Jun";
              "Jul"; "Aug"; "Sep"; "Oct"; "Nov"; "Dec"]

let kb = 1024.
let mb = 1024. *. 1024.

let hume x =  
    if x > kb && x < mb then
        sprintf "%.1fM" (x /. kb) 
    else
        if x > mb then 
            sprintf "%.1fK" (x /. mb) 
        else
            sprintf "%.0f" x 

let write (p:Metrics.metrics) = 
    let cpu = p.cpu_info   and
        _   = p.cpu_freq  and
        cput = p.cpu_temp  and
        mem = p.mem_info   and
        net = p.net_info   and
        snd = p.snd_info   and
        pwr = p.pwr_info   in
    let netif = net.intfs
                |> List.filter
                    (fun (y:(string*Net.net_if)) -> 
                        let _,x = y in
                        (x.tx > 0L) && (x.name <> "lo"))
                |> List.map
                    (fun (y:(string*Net.net_if)) -> 
                        let _,x = y in
                        (x.name,
                            (Int64.div x.rx 1024L), 
                            (Int64.div x.tx 1024L), x.dr, x.dt))
    in
    let n,_,_,rr,rx = 
            match List.nth_opt netif 0 with
            | Some ni -> ni
            | None -> ("?",0L,0L,0.0,0.0) and 
        psu, pl, ps =
            if pwr.psu = "Psu"
            then (pwr.psu, 0, "")
            else (pwr.psu, pwr.level, pwr.status) and
        timetm = Unix.localtime (Unix.time ()) in
    printf
        "%%{l} %s, %s %02d, %02d | %02d:%02d:%02d\
         %%{c} %s %s %s\
         %%{r} Cpu %02.0f %3dC | Mem %2.0f | Net:%s %s %s | Vol %02d %s | %s %02d %s"
        (List.nth weDays timetm.tm_wday)
        (List.nth months timetm.tm_mon)
        timetm.tm_mday
        (timetm.tm_year + 1900)
        timetm.tm_hour
        timetm.tm_min
        timetm.tm_sec
        "." "." "."
        (cpu.usage *. 100.0)
        cput.temp_max
        (mem.usage *. 100.0)
        n (hume rr) (hume rx)
        snd.volume
        (if snd.muted then "muted" else "")
        psu pl ps;
    print_newline ()
