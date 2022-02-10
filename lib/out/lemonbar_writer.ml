open Printf
open Fseerrec
open Fseer

let hume x =  
    if x > Consts.kb && x < Consts.mb then
        sprintf "%.1fM" (x /. Consts.kb) 
    else
        if x > Consts.mb && x < Consts.gb then 
            sprintf "%.1fK" (x /. Consts.mb) 
        else
            if x > Consts.gb then
                sprintf "%.1fK" (x /. Consts.gb)
            else
                sprintf "%.0f" x 


let action = [
    ("cpu", Consts.xterm_w ^ "'cat /proc/cpuinfo | less'");
    ("mem", Consts.xterm_w ^ "'cat /proc/meminfo | less'");
    ("net", "connman-gtk");
    ("sound", Consts.xterm_s ^ "alsamixer");
    ("date", Consts.xterm_w ^ "calcurse")
]

let wrap_action a s =
    let oa = Futil.get_item action a in
    match oa with
    | Some (k, v) ->
        sprintf "%%{A:%s:}%s%%{A}" v s
    | None -> s

let cpu label (p:Cpu.cpu_info) = 
    sprintf "%s%02.0f" label (p.usage *. 100.)

let cpu_freq label (p:Cpu_freq.cpu_freq) =
    sprintf "%s%s" label (hume (float p.freq_avg))

let cpu_temp label (p:Cpu_temp.cpu_temp) =
    let maxt = p.temps
        |> (List.sort (fun x y -> x - y))
        |> (fun l -> List.nth l 0)
    in
    sprintf "%s%3d" label maxt

let mem label (p:Mem.mem_info) = 
    sprintf "%s%02.0f" label (p.usage *. 100.0)

let net label (p:Net.net_info) = 
    let netif = p.intfs
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
            | None -> ("?",0L,0L,0.0,0.0)
    in
    sprintf "%s%s %s" label (hume rx) (hume rr)

let power l_psu l_bat (p:Power.power_info) = 
    if p.psu = "Psu" then
        l_psu
    else 
        sprintf "%s%02d %s" l_bat p.level p.status


let sound label (p:Sound.sound_info) = 
    sprintf "%s%02d%s" label p.volume        
        (if p.muted then "M" else "")

let date label = 
    let 
        timetm = Unix.localtime (Unix.time ()) 
    in
    sprintf "%s%s, %s %02d, %4d %02d:%02d:%02d" label
        (List.nth Consts.weDays timetm.tm_wday)
        (List.nth Consts.months timetm.tm_mon)
        timetm.tm_mday
        (timetm.tm_year + 1900)
        timetm.tm_hour
        timetm.tm_min
        timetm.tm_sec

let write (p:Metrics.metrics) = 
    [ "%{l}"
    ; wrap_action "date" (date "")
    ; "%{c} . . ."
    ; "%{r}"
    ; wrap_action "cpu" (cpu " Cpu " p.cpu_info)
    ; cpu_temp " T" p.cpu_temp
    ; wrap_action "mem" (mem " Mem " p.mem_info)
    ; wrap_action "net" (net " Net " p.net_info)
    ; wrap_action "sound" (sound " Vol " p.snd_info)
    ; power " Psu" " Bat " p.pwr_info
    ] |> List.iter (fun s -> print_string s);
    print_newline ()
