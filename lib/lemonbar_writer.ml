open Printf
open Fseerrec

let weDays = ["Su"; "Mo"; "Tu"; "We"; "Th"; "Fr"; "Sa"]
let months = ["Jan"; "Feb"; "Mar"; "Apr"; "May"; "Jun";
              "Jul"; "Aug"; "Sep"; "Oct"; "Nov"; "Dec"]

let write (p:Metrics.metrics) = 
    let cpu = p.cpuinfo   and
        cpuf = p.cpufreq  and
        cput = p.cputemp  and
        mem = p.meminfo   and
        net = p.netinfo   and
        snd = p.sndinfo   and
        pwr = p.pwrinfo   in
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
    let n,_,_,rr,rx = List.nth netif 0 and 
        psu, pl, ps =
            if pwr.psu = "Psu"
            then (pwr.psu, 0, "")
            else (pwr.psu, pwr.level, pwr.status) and
        timetm = Unix.localtime (Unix.time ())
    in 
    printf 
        "%%{l} %s %s %s %%{c} %s | %s %02d, %02d | %02d:%02d:%02d %%{r}cpu:(%2.0f %4dMHz %3d C) mem:%2.0f %s:(%.1f %.1f) %s%02d %s %02d %s \n"
        "?" "?" "?"
        (List.nth weDays timetm.tm_wday)
        (List.nth months timetm.tm_mon)
        timetm.tm_mday
        (timetm.tm_year + 1900)
        timetm.tm_hour
        timetm.tm_min
        timetm.tm_sec
        (cpu.usage *. 100.0)
        cpuf.freq_avg
        cput.temp_max
        (mem.usage *. 100.0)
        n rr rx
        (if snd.muted then "muted: " else "vol: ")
        snd.volume
        psu pl ps
